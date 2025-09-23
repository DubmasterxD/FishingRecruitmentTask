using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Transform _topPoint;
    [SerializeField] Vector2 _waitingTimeRange;
    [SerializeField] float _maxHookedTime;

    [SerializeField][HideInInspector] public List<FishingDrop> DropsList = new List<FishingDrop>();
    [SerializeField][HideInInspector] public List<float> DropChances = new List<float>();
    Dictionary<FishingDrop, float> _drops;
    FishingDrop _fishOnHook;

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

        _drops = new Dictionary<FishingDrop, float>();
        for (int i = 0; i < DropsList.Count; i++)
        {
            if (DropsList[i] == null || DropChances[i] <= 0)
                continue;
            _drops.Add(DropsList[i], DropChances[i]);
        }

        _fishOnHook = null;
        _casted = false;
        _state = FishingState.None;
    }

    private void OnValidate()
    {
        if (DropsList.Count < DropChances.Count)
        {
            DropChances.RemoveRange(DropsList.Count, DropChances.Count - DropsList.Count);
        }
        else if (DropsList.Count > DropChances.Count)
        {
            for (int i = DropChances.Count; i < DropsList.Count; i++)
            {
                DropChances.Add(0f);
            }
        }
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
        transform.SetParent(null);
        transform.position = position;
        transform.rotation = Quaternion.identity;
        //DOTween throw
        _animator.SetTrigger("Cast");
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

    public FishingDrop PrepareMinigame()
    {
        _fishOnHook = GetRandomDrop();
        return _fishOnHook;
    }

    public FishingDrop GetRandomDrop()
    {
        float total = 0f;
        foreach (var chance in _drops.Values)
        {
            total += chance;
        }
        float randomPoint = Random.value * total;
        foreach (var pair in _drops)
        {
            if (randomPoint < pair.Value)
            {
                return pair.Key;
            }
            else
            {
                randomPoint -= pair.Value;
            }
        }
        return null;
    }

    public FishingDrop GetFishOnHook()
    {
        return _fishOnHook;
    }

    public void ReelIn()
    {
        _state = FishingState.ReelIn;
        _animator.SetTrigger("Finish");
    }

    public void Reeled()
    {
        _state = FishingState.None;
    }

    public Vector3 GetTopPoint()
    {
        return _topPoint.position;
    }
}
