using System.Collections;
using UnityEngine;

public class PlayerFishing : MonoBehaviour
{
    [SerializeField] string _fishingTag;
    [SerializeField][Min(0)] float _maxCastRange = 10f;
    [SerializeField][Min(0)] float _maxCastHeight = 5f;
    [SerializeField][Min(0)] float _chargingSpeed = 10f;
    [SerializeField][Min(0)] float _maxCharge = 5f;

    [SerializeField] Bobber _bobberPrefab;
    Bobber _bobber;

    Camera _camera;
    bool _isCharging;
    float _currentCharge;

    private void Awake()
    {
        _camera = Camera.main;
        _isCharging = false;
        _currentCharge = 0;
    }

    private void Start()
    {
        InputManager.Instance.OnBeginCharge += ()=> _isCharging = true;
        InputManager.Instance.OnReleaseCharge += TryCasting;
        InputManager.Instance.OnReelIn += () => StartCoroutine(ReelIn());
        InputManager.Instance.OnHookFish += TryCatch;
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
        }
    }

    void TryCasting()
    {
        _isCharging = false;
        float castRange = Mathf.Lerp(0, _maxCastRange, _currentCharge / _maxCharge);
        _currentCharge = 0;

        Vector3 castDirection = new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z).normalized;
        Ray ray = new Ray(_camera.transform.position, castDirection);
        if(Physics.Raycast(ray, out RaycastHit obstacleHit, castRange))
        {
            //Fail cast
            Debug.Log("Failed to cast, something is in the way: " + obstacleHit.collider.name);
            return;
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
            Debug.Log("Failed to cast, no water found");
            return;
        }
    }

    IEnumerator Cast(Vector3 position)
    {
        InputManager.Instance.ToggleMovement(false);

        if (_bobber == null)
        {
            _bobber = Instantiate(_bobberPrefab, position, Quaternion.identity);
        }
        yield return _bobber.Cast(position);

        InputManager.Instance.ToggleFishing(true);
    }

    void TryCatch()
    {
        if(_bobber != null && _bobber.IsFishOnHook())
        {
            Debug.Log("Caught a fish!");
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
        yield return _bobber.ReelIn();
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
