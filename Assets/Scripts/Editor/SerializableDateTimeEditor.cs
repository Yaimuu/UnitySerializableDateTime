using System;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SerializableDateTime
{
    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class SerializableDateTimeEditor : PropertyDrawer
    {
        public static event UnityAction<DateTime> OnValueChanged;
        private DateTime _lastValidDateTime;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            // Locate the 'dateInput' property
            SerializedProperty dateInputProp = property.FindPropertyRelative("dateInput");

            var textField = new TextField(property.displayName)
            {
                value = dateInputProp.stringValue
            };

            textField.RegisterValueChangedCallback(evt =>
            {
                // Update serialized property
                dateInputProp.stringValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            textField.RegisterCallback<ClickEvent>((_) =>
            {
                if(!DateTimePickerWindow.IsOpen)
                    EditorApplication.ExecuteMenuItem("Window/UI Toolkit/DateTimePicker");
            });
            
            textField.RegisterCallback<FocusOutEvent>((_) =>
            {
                // Optional: try to parse and log result
                if (DateTime.TryParse(textField.value, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
                {
                    _lastValidDateTime = new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, 0);
                    OnValueChanged?.Invoke(_lastValidDateTime);
                }
                else
                {
                    // Invalid date format
                    textField.value = _lastValidDateTime.ToString("o");
                }
            });

            root.Add(textField);
            return root;
        }
    }
}

