using UnityEngine;

public class MinigameController : MonoBehaviour
{
    [SerializeField] float _minigameDuration = 7f;
    [SerializeField] float _maxPressure = 1f;
    [SerializeField] float _pressureIncreaseRate = 0.5f;
    [SerializeField] Vector2 _pressureDecayRateRange = new Vector2(0.1f, 0.3f);
    [SerializeField] float _pressureDecayRateChangeInterval = 1f;

    FishingUIController _fishingUIController;

    bool _isMinigameActive = false;
    bool _wasLastSuccess = false;
    float _timer;
    float _decayRate;
    float _decayTimer;
    float _currentPressure = 0;

    public bool IsMinigameActive { get { return _isMinigameActive; } }
    public bool WasLastSuccess { get { return _wasLastSuccess; } }

    private void Start()
    {
        _fishingUIController = FindAnyObjectByType<FishingUIController>();
        if (_fishingUIController == null)
        {
            Debug.LogError("FishingUIController not found in the scene.");
        }

        InputManager.Instance.OnReelIn += OnReelIn;
        _decayRate = Random.Range(_pressureDecayRateRange.x, _pressureDecayRateRange.y);
    }

    private void Update()
    {
        if (_isMinigameActive)
        {
            _decayTimer += Time.deltaTime;
            if(_decayTimer >= _pressureDecayRateChangeInterval)
            {
                _decayTimer = 0;
                _decayRate = Random.Range(_pressureDecayRateRange.x, _pressureDecayRateRange.y);
            }

            _currentPressure += (- _decayRate) * Time.deltaTime;

            if(_currentPressure <= 0 || _currentPressure >= _maxPressure)
            {
                EndMinigame(false);
            }

            _timer += Time.deltaTime;
            _fishingUIController.UpdateMinigame(_minigameDuration - _timer, _currentPressure / _maxPressure);
            if (_timer >= _minigameDuration)
            {
                EndMinigame(true);
            }
        }
    }

    void OnReelIn()
    {
        if (_isMinigameActive)
        {
            _currentPressure += _pressureIncreaseRate * Time.deltaTime;
        }
    }

    public void BeginMinigame(FishingDrop fishingDrop)
    {
        _fishingUIController.ShowMinigame();
        _isMinigameActive = true;
        _timer = 0;
        _currentPressure = _maxPressure / 2;
    }

    public void EndMinigame(bool isSuccess)
    {
        _fishingUIController.HideMinigame();
        _isMinigameActive = false;
        _wasLastSuccess = isSuccess;
    }
}
