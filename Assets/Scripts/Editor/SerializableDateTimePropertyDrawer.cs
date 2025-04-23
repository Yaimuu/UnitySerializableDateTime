using System;
using SerializableDateTime.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SerializableDateTime
{
    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class SerializableDateTimePropertyDrawer : PropertyDrawer
    {
        public static event UnityAction<DateTime> OnValueChanged;
        private DateTime _lastValidDateTime;

        public DateTime LastValidDateTime
        {
            get => _lastValidDateTime;
            private set
            {
                _lastValidDateTime = value;
                OnValueChanged?.Invoke(_lastValidDateTime);
            }
        }
        
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;
        
        private DateTimePickerUI _dateTimePickerUI;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            _dateTimePickerUI = new DateTimePickerUI(_visualTreeAsset.Instantiate(), property);
            root.Add(_dateTimePickerUI.Root);
            
            return root;
        }
    }
}

