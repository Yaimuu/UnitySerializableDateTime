using System;
using SerializableDateTime.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableDateTime
{
    public class DateTimePickerWindow : EditorWindow
    {
        private static DateTimePickerWindow Instance { get; set; }

        public static bool IsOpen => Instance != null;

        private DateTimePickerUI _dateTimePickerUI;

        [SerializeField]
        private VisualTreeAsset visualTreeAsset;

        [MenuItem("Window/UI Toolkit/DateTimePicker")]
        public static void ShowExample()
        {
            DateTimePickerWindow wnd = GetWindow<DateTimePickerWindow>();
            wnd.titleContent = new GUIContent("DateTimePicker");
            SerializableDateTimeEditor.OnValueChanged += Instance.OnNewDate;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            
            _dateTimePickerUI = new DateTimePickerUI(visualTreeAsset.Instantiate());

            root.Add(_dateTimePickerUI.Root);
        }

        void OnNewDate(DateTime newDate)
        {
            _dateTimePickerUI.UpdateMonthDaysList(newDate);
        }
        
        void OnEnable() {
            Instance = this;
        }

        void OnDisable()
        {
            SerializableDateTimeEditor.OnValueChanged -= OnNewDate;
        }
    }
}
