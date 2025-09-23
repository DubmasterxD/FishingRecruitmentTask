using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [SerializeField] Transform endPoint;
    [SerializeField] float _castSpeed = 2f;
    [SerializeField] LineRenderer _line;

    Animator _animator;
    float _currentPercentage;
    bool _stoppedCharging;
    bool _isReeling;
    Bobber _bobber;

    public event Action onBobberRequest;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
            _animator = gameObject.AddComponent<Animator>();

        _currentPercentage = 0;
        _isReeling = false;
    }

    private void Update()
    {
        if(_stoppedCharging && _currentPercentage >= 0)
        {
            _currentPercentage -= Time.deltaTime * _castSpeed;
            if (_currentPercentage < 0)
            {
                _stoppedCharging = false;
            }
            _animator.SetFloat("Charge", _currentPercentage);
        }

        CalculateLinePoints();
    }

    void CalculateLinePoints()
    {
        if (_line == null || endPoint == null)
            return;
        Vector3 startPos = endPoint.position;
        Vector3 endPos = _bobber.GetTopPoint();
        Vector3 middlePos = (startPos + endPos) / 2;
        float minY = Mathf.Min(startPos.y, endPos.y);
        float maxY = Mathf.Max(startPos.y, endPos.y);
        middlePos.y = Mathf.Lerp(minY, maxY, 0.25f);

        int segmentCount = 20;
        _line.positionCount = segmentCount + 1;
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 pointOnCurve = Mathf.Pow(1 - t, 2) * startPos + 2 * (1 - t) * t * middlePos + Mathf.Pow(t, 2) * endPos;
            _line.SetPosition(i, pointOnCurve);
        }
    }

    public void AttachBobber(Bobber bobber)
    {
        _bobber = bobber;
        bobber.transform.position = endPoint.position;
        bobber.transform.SetParent(endPoint);
        bobber.transform.localRotation = Quaternion.Euler(-60, 0, 0);
    }

    public void Charge(float percentage)
    {
        _animator.SetFloat("Charge", percentage);
        _currentPercentage = percentage;
    }

    public void StopCharging()
    {
        _stoppedCharging = true;
    }

    public bool IsCasting()
    {
        return _currentPercentage >= 0;
    }

    public IEnumerator ReelIn()
    {
        _animator.SetTrigger("Reel");
        _isReeling = true;
        while (_isReeling)
        {
            yield return null;
        }
    }

    public void StopReeling()
    {
        _isReeling = false;
    }

    public void RequestBobber()
    {
        onBobberRequest?.Invoke();
    }
}
