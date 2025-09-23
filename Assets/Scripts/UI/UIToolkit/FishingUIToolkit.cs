using UnityEngine.UIElements;

[UxmlElement("FishingUI")]
public partial class FishingUIToolkit : VisualElement
{
    FishShowcase _fishShowcase;

    public FishingUIToolkit()
    {
        AddToClassList("fishing-ui__container");

        _fishShowcase = VisualElementCreator.Create<FishShowcase>("FishShowcase", "fish-showcase");
        Add(_fishShowcase);
        _fishShowcase.style.display = DisplayStyle.None;
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
}
