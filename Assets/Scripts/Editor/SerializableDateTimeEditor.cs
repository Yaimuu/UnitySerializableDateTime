using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace SerializableDateTime
{
    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class SerializableDateTimeEditor : PropertyDrawer
    {
        private DateTime lastValidDateTime;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            // Locate the 'dateInput' property
            var dateInputProp = property.FindPropertyRelative("dateInput");

            var textField = new TextField("Date (ISO 8601)")
            {
                value = dateInputProp.stringValue
            };

            textField.RegisterValueChangedCallback(evt =>
            {
                // Update serialized property
                dateInputProp.stringValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            textField.RegisterCallback<ClickEvent>((evt) =>
            {
                if(!DateTimePickerWindow.IsOpen)
                    EditorApplication.ExecuteMenuItem("Window/UI Toolkit/DateTimePicker");
            });
            
            textField.RegisterCallback<FocusOutEvent>((evt) =>
            {
                // Optional: try to parse and log result
                if (DateTime.TryParse(textField.value, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
                {
                    lastValidDateTime = new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, 0);
                }
                else
                {
                    // Invalid date format
                    textField.value = lastValidDateTime.ToString("o");
                }
            });

            root.Add(textField);
            return root;
        }
    }
}

