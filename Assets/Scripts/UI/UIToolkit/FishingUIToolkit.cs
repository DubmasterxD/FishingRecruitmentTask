using UnityEngine.UIElements;

[UxmlElement("FishingUI")]
public partial class FishingUIToolkit : VisualElement
{
    FishShowcase _fishShowcase;
    VisualElement _minigame;
    Label _timer;
    Slider _pressure;

    public FishingUIToolkit()
    {
        AddToClassList("fishing-ui__container");

        _fishShowcase = VisualElementCreator.Create<FishShowcase>("FishShowcase", "fish-showcase");
        Add(_fishShowcase);
        _fishShowcase.style.display = DisplayStyle.None;

        _minigame = VisualElementCreator.Create<VisualElement>("Minigame", "fishing-minigame");
        Add(_minigame);
        _minigame.style.display = DisplayStyle.None;

        _timer = VisualElementCreator.Create<Label>("Timer", "fishing-minigame__timer");
        _timer.text = "99.9s";
        _minigame.Add(_timer);

        _pressure = VisualElementCreator.Create<Slider>("Pressure", "fishing-minigame__pressure");
        _pressure.lowValue = 0;
        _pressure.highValue = 1;
        _minigame.Add(_pressure);
    }

    public void ShowFish(FishingDrop fish)
    {
        _fishShowcase.style.display = DisplayStyle.Flex;
        _fishShowcase.ShowFish(fish);
    }

    public void HideFish()
    {
        _fishShowcase.style.display = DisplayStyle.None;
    }

    public void ShowMinigame()
    {
        _minigame.style.display = DisplayStyle.Flex;
    }

    public void HideMinigame()
    {
        _minigame.style.display = DisplayStyle.None;
    }

    public void UpdateMinigame(float time, float pressure)
    {
        _timer.text = $"{time:0.0}s";
        _pressure.value = pressure;
    }
}
