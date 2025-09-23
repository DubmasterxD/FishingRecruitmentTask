
public static class VisualElementCreator
{
    public static T Create<T>(string elementName, params string[] classNames) where T : UnityEngine.UIElements.VisualElement, new()
    {
        var ele = new T
        {
            name = elementName
        };
        foreach (string className in classNames)
        {
            if (!string.IsNullOrEmpty(className))
                ele.AddToClassList(className);
        }
        return ele;
    }
}
