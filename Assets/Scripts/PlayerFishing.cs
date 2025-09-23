using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFishing : MonoBehaviour
{
    [SerializeField] string _fishingTag;
    [SerializeField][Min(0)] float _maxCastRange = 10f;
    [SerializeField][Min(0)] float _maxCastHeight = 5f;
    [SerializeField][Min(0)] float _chargingSpeed = 10f;
    [SerializeField][Min(0)] float _maxCharge = 5f;

    [SerializeField] FishingRod _fishingRod;
    [SerializeField] Bobber _bobberPrefab;
    Bobber _bobber;

    PlayerMovement _movement;
    Camera _camera;
    bool _isCharging;
    float _currentCharge;

    public event Action<FishingDrop> OnFishCaught;

    private void Awake()
    {
        _camera = Camera.main;
        _movement = GetComponent<PlayerMovement>();
        _isCharging = false;
        _currentCharge = 0;
    }

    private void Start()
    {
        _bobber = Instantiate(_bobberPrefab);
        _fishingRod.AttachBobber(_bobber);

        InputManager.Instance.OnBeginCharge += ()=> _isCharging = true;
        InputManager.Instance.OnReleaseCharge += ReleaseCharge;
        InputManager.Instance.OnHookFish += ()=> StartCoroutine(TryCatch());
    }

    private void Update()
    {
        Charge();
    }

    private void Charge()
    {
        if (_isCharging && _currentCharge < _maxCharge)
        {
            _currentCharge += Time.deltaTime * _chargingSpeed;
            if (_currentCharge >= _maxCharge)
            {
                _currentCharge = _maxCharge;
            }
            _fishingRod.Charge(_currentCharge / _maxCharge);
        }
    }

    void ReleaseCharge()
    {
        _isCharging = false;
        _fishingRod.StopCharging();
        StartCoroutine(TryCasting());
    }

    IEnumerator TryCasting()
    {
        InputManager.Instance.ToggleMovement(false);
        InputManager.Instance.ToggleLooking(false);
        while (_fishingRod.IsCasting())
        {
            yield return null;
        }
        InputManager.Instance.ToggleLooking(true);

        float castRange = Mathf.Lerp(0, _maxCastRange, _currentCharge / _maxCharge);
        _currentCharge = 0;

        Vector3 castDirection = new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z).normalized;
        Ray ray = new Ray(_camera.transform.position, castDirection);
        if(Physics.Raycast(ray, out RaycastHit obstacleHit, castRange))
        {
            //Fail cast
            InputManager.Instance.ToggleMovement(true);
            Debug.Log("Failed to cast, something is in the way: " + obstacleHit.collider.name);
            yield break;
        }

        Vector3 castPosition = _camera.transform.position + castDirection * castRange;
        ray = new Ray(castPosition, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit surfaceHit, _maxCastHeight) && surfaceHit.collider.gameObject.CompareTag(_fishingTag))
        {
            StartCoroutine(Cast(surfaceHit.point));
        }
        else
        {
            //Fail cast
            InputManager.Instance.ToggleMovement(true);
            Debug.Log("Failed to cast, no water found");
        }
    }

    IEnumerator Cast(Vector3 position)
    {
        InputManager.Instance.ToggleMovement(false);
        if (_movement != null)
            _movement.LockLookDirection();

        yield return _bobber.Cast(position);

        InputManager.Instance.ToggleFishing(true);
    }

    IEnumerator TryCatch()
    {
        if(_bobber != null && _bobber.IsFishOnHook())
        {
            FishingDrop fishCaught = _bobber.PrepareMinigame();
            MinigameController minigameController = FindAnyObjectByType<MinigameController>();
            minigameController.BeginMinigame(fishCaught); //potential difficulties
            while (minigameController.IsMinigameActive)
            {
                yield return null;
            }
            if (minigameController.WasLastSuccess)
            {
                OnFishCaught?.Invoke(fishCaught);
            }
            else
            {
                Debug.Log("Fish got away!");
            }
        }
        else
        {
            Debug.Log("No fish on hook to catch.");
        }
        StartCoroutine(ReelIn());
    }

    IEnumerator ReelIn()
    {
        InputManager.Instance.ToggleFishing(false);
        _bobber.ReelIn();
        _fishingRod.onBobberRequest += () => _fishingRod.AttachBobber(_bobber);
        yield return _fishingRod.ReelIn();
        _fishingRod.onBobberRequest -= () => _fishingRod.AttachBobber(_bobber);

        if (_movement != null)
            _movement.UnlockLookDirection();
        InputManager.Instance.ToggleMovement(true);
    }

    private void OnDrawGizmos()
    {
        if (_camera == null)
            return;

        float castRange = Mathf.Lerp(0, _maxCastRange, _currentCharge / _maxCharge);
        Vector3 castDirection = new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z).normalized;
        Vector3 castPosition = _camera.transform.position + castDirection * castRange;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_camera.transform.position, new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z).normalized * _maxCastRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(_camera.transform.position, new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z).normalized * castRange);
        Gizmos.DrawRay(castPosition, Vector3.down * _maxCastHeight);
    }
}
