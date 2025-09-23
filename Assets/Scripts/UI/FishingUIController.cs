using UnityEngine;
using UnityEngine.UIElements;

public class FishingUIController : MonoBehaviour
{
    [SerializeField] UIDocument _rootDocument;
    [SerializeField] float _showCatchDuration;

    FishingUIToolkit _fishingUI;

    bool _isShowing = false;
    float _timer = 0f;

    private void Awake()
    {
        _fishingUI = _rootDocument.rootVisualElement.Q<FishingUIToolkit>();
        if(_fishingUI == null)
        {
            Debug.LogError("FishingUIToolkit not found in the UI Document.");
        }
    }

    private void Start()
    {
        FindAnyObjectByType<PlayerFishing>().OnFishCaught += ShowFish;
    }

    private void Update()
    {
        if (_isShowing)
        {
            _timer += Time.deltaTime;
            if (_timer >= _showCatchDuration)
            {
                HideFish();
            }
        }
    }

    public void ShowFish(FishingDrop fish)
    {
        if (_fishingUI != null)
        {
            _fishingUI.ShowFish(fish);
        }
        _isShowing = true;
        _timer = 0f;
    }

    void HideFish()
    {
        if (_fishingUI != null)
        {
            _fishingUI.HideFish();
        }
        _isShowing = false;
    }

    public void ShowMinigame()
    {
        if (_fishingUI != null)
        {
            _fishingUI.ShowMinigame();
        }
    }

    public void HideMinigame()
    {
        if (_fishingUI != null)
        {
            _fishingUI.HideMinigame();
        }
    }

    public void UpdateMinigame(float time, float pressure)
    {
        if (_fishingUI != null)
        {
            _fishingUI.UpdateMinigame(time, pressure);
        }
    }
}
