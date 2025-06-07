using SerializedCalendar.UI;
using UnityEditor;
using UnityEngine.UIElements;

namespace SerializedCalendar
{
    [CustomPropertyDrawer(typeof(SerializedDateTime))]
    public class SerializedDateTimePropertyDrawer : PropertyDrawer
    {
        private VisualTreeAsset _visualTreeAsset;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            SerializedCalendarUI serializedCalendarUI = new SerializedCalendarUI(_visualTreeAsset.Instantiate(), property);
            root.Add(serializedCalendarUI.Root);
            
            return root;
        }
    }
}
