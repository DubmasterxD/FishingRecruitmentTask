using UnityEngine.UIElements;

public class FishShowcase : VisualElement
{
    Image _fishIcon;
    Label _fishName;
    Label _info;

    public FishShowcase()
    {
        DrawShowcase();
    }

    void DrawShowcase()
    {
        Image background = VisualElementCreator.Create<Image>("Background", "fish-showcase__background");
        Add(background);

        _fishIcon = VisualElementCreator.Create<Image>("FishIcon", "fish-showcase__icon");
        Add(_fishIcon);

        _fishName = VisualElementCreator.Create<Label>("FishName", "fish-showcase__name");
        _fishName.text = "Fish Name";
        Add(_fishName);
        
        _info = VisualElementCreator.Create<Label>("Info", "fish-showcase__info");
        _info.text = "Fish Info";
        Add(_info);
    }

    public void ShowFish(FishingDrop fish)
    {
        _fishIcon.style.backgroundImage = new StyleBackground(fish.ItemIcon);
        _fishName.text = fish.DisplayName;
        _info.text = fish.CatchInfo;
    }
}
