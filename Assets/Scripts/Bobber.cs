using System.Collections;
using UnityEngine;

enum FishingState
{
    None,
    Waiting,
    FishOnHook,
    MiniGame,
    ReelIn
}

public class Bobber : MonoBehaviour
{
    [SerializeField] Vector2 _waitingTimeRange;
    [SerializeField] float _maxHookedTime;

    Animator _animator;
    bool _casted;

    FishingState _state;
    float _timer;
    float _waitingTime;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if(_animator == null)
            _animator = gameObject.AddComponent<Animator>();

        _casted = false;
        _state = FishingState.None;
    }

    private void Update()
    {
        if(_state != FishingState.None)
        {
            UpdateFishing();
        }
    }

    void UpdateFishing()
    {
        _timer += Time.deltaTime;
        switch (_state)
        {
            case FishingState.Waiting:
                if (_timer >= _waitingTime)
                {
                    HookFish();
                }
                break;
            case FishingState.FishOnHook:
                if (_timer >= _maxHookedTime)
                {
                    StartWaiting();
                }
                break;
        }
    }

    public IEnumerator Cast(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.position = position;
        _animator.Play("Cast");
        while (!_casted)
            yield return null;

        StartWaiting();

        _casted = false;
    }

    void StartWaiting()
    {
        _timer = 0;
        _waitingTime = Random.Range(_waitingTimeRange.x, _waitingTimeRange.y);
        _state = FishingState.Waiting;
        _animator.SetBool("OnHook", false);
    }

    public void Casted()
    {
        _casted = true;
    }

    void HookFish()
    {
        _timer = 0f;
        _state = FishingState.FishOnHook;
        _animator.SetBool("OnHook", true);
    }

    public bool IsFishOnHook()
    {
        return _state == FishingState.FishOnHook;
    }

    public IEnumerator ReelIn()
    {
        _state = FishingState.ReelIn;
        _animator.SetTrigger("Finish");
        while(_state != FishingState.None)
            yield return null;

        gameObject.SetActive(false);
    }

    public void Reeled()
    {
        _state = FishingState.None;
    }
}
