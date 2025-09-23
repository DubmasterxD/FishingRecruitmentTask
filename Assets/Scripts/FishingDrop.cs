using UnityEngine;

[CreateAssetMenu(fileName = "FishingDrop", menuName = "Fishing/Drop", order = 1)]
public class FishingDrop : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite ItemIcon { get; private set; }
    [field: SerializeField] public string CatchInfo { get; private set; }
}
