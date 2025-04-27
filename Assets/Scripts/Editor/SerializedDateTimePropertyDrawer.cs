using SerializedDateTime.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializedDateTime
{
    [CustomPropertyDrawer(typeof(SerializedDateTime))]
    public class SerializedDateTimePropertyDrawer : PropertyDrawer
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            SerializedDateTimePickerUI serializedDateTimePickerUI = new SerializedDateTimePickerUI(_visualTreeAsset.Instantiate(), property);
            root.Add(serializedDateTimePickerUI.Root);
            
            return root;
        }
    }
}
