using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(Bobber))]
public class BobberEditor : Editor
{
    Bobber _bobber;
    Foldout _drops;
    VisualElement _dropsButtons;

    public override VisualElement CreateInspectorGUI()
    {
        return DrawGUI();
    }

    private VisualElement DrawGUI()
    {
        if (_bobber == null) 
            _bobber = (Bobber)target;

        serializedObject.Update();

        VisualElement root = new VisualElement();
        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        _drops = new Foldout();
        _drops.viewDataKey = nameof(_drops);
        _drops.text = "Drops";

        PopulateList();

        _dropsButtons = new VisualElement();
        _dropsButtons.style.flexDirection = FlexDirection.Row;

        Button removeButton = new Button();
        removeButton.text = "Remove Last";
        removeButton.clicked += () => RemoveRow();

        Button addButton = new Button();
        addButton.text = "Add New";
        addButton.clicked += () => AddRow();

        _dropsButtons.Add(removeButton);
        _dropsButtons.Add(addButton);
        _drops.Add(_dropsButtons);

        root.Add(_drops);
        return root;
    }

    private void PopulateList()
    {
        for (int i = 0; i < _bobber.DropsList.Count; i++)
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;

            ObjectField dropsListField = new ObjectField();
            dropsListField.style.flexGrow = 1;
            dropsListField.style.flexBasis = 0;
            dropsListField.objectType = typeof(FishingDrop);
            dropsListField.value = _bobber.DropsList[i];

            FloatField dropsChancesField = new FloatField();
            dropsChancesField.style.minWidth = new StyleLength((Length)120);
            dropsChancesField.style.width = new StyleLength(Length.Percent(30));
            dropsChancesField.value = _bobber.DropChances[i];

            int index = i;

            dropsListField.RegisterValueChangedCallback((evt) =>
            {
                serializedObject.Update();
                _bobber.DropsList[index] = (FishingDrop)evt.newValue;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_bobber);
            });

            dropsChancesField.RegisterValueChangedCallback((evt) =>
            {
                serializedObject.Update();
                _bobber.DropChances[index] = evt.newValue;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_bobber);
            });

            row.Add(dropsListField);
            row.Add(dropsChancesField);
            _drops.Add(row);
        }
    }

    void RemoveRow()
    {
        serializedObject.Update();
        if (_bobber.DropsList.Count > 0)
        {
            _bobber.DropsList.RemoveAt(_bobber.DropsList.Count - 1);
        }
        if (_bobber.DropChances.Count > 0)
        {
            _bobber.DropChances.RemoveAt(_bobber.DropChances.Count - 1);
        }
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(_bobber);
        RefreshList();
    }

    void AddRow()
    {
        serializedObject.Update();
        _bobber.DropsList.Add(null);
        _bobber.DropChances.Add(0f);
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(_bobber);
        RefreshList();
    }

    void RefreshList()
    {
        _drops.Remove(_dropsButtons);
        _drops.Clear();
        PopulateList();
        _drops.Add(_dropsButtons);
    }
}
