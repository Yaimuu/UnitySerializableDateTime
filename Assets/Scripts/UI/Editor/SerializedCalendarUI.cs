using System;
using System.Globalization;
using UnityEngine.UIElements;
using UnityEditor;

namespace SerializedCalendar.UI
{
    public class SerializedCalendarUI
    {
        #region Variables
        public VisualElement Root { get; private set; }
        
        private TextField _dateInput;
        
        private VisualElement _calendarContainer;

        private readonly SerializedProperty _serializedProperty;

        private readonly CalendarUI _calendar;

        private DateTime _lastValidDateTime;

        private DateTime LastValidDateTime
        {
            get => _lastValidDateTime;
            set
            {
                _lastValidDateTime = value;
                _dateInput.value = LastValidDateTime.ToString(CultureInfo.CurrentCulture);
            }
        }

        #endregion
        
        public SerializedCalendarUI(TemplateContainer template, SerializedProperty property)
        {
            Root = template;
            
            Init();
            
            _serializedProperty = property;
            
            SerializedProperty dateInputProp = _serializedProperty.FindPropertyRelative("dateInput");
            
            _dateInput.label = _serializedProperty.displayName;
            _dateInput.value = dateInputProp.stringValue;

            InitInputEvents();
            
            if(dateInputProp.stringValue == string.Empty)
                _dateInput.value = DateTime.Today.ToString(CultureInfo.CurrentCulture);

            if (DateTime.TryParse(_dateInput.value, null, DateTimeStyles.AssumeUniversal, out DateTime parsed))
            {
                LastValidDateTime = parsed;
            }
            
            _calendar = new CalendarUI(template, LastValidDateTime);
            _calendar.DateChanged += newDate => LastValidDateTime = newDate;
            
            HideCalendar();
        }
        
        private void Init()
        {
            Root.style.display = DisplayStyle.Flex;
            Root.BringToFront();
            
            _dateInput = Root.Q<TextField>(UIConstants.TextFieldInputId);
            
            _calendarContainer = Root.Q<VisualElement>(UIConstants.CalendarContainerId);
        }

        private void InitInputEvents()
        {
            _dateInput.RegisterValueChangedCallback(evt =>
            {
                SerializedProperty dateInputProp = _serializedProperty.FindPropertyRelative("dateInput");
                // Update serialized property
                dateInputProp.stringValue = evt.newValue;
                _serializedProperty.serializedObject.ApplyModifiedProperties();
                
                if (DateTime.TryParse(_dateInput.value, null, DateTimeStyles.AssumeLocal, out DateTime parsed))
                {
                    LastValidDateTime = new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, 0);
                    _calendar.Monthly.Update(LastValidDateTime);
                }
            });

            _dateInput.RegisterCallback<ClickEvent>((_) =>
            {
                ShowCalendar();
            });
            
            // Text field has been overriden
            _dateInput.RegisterCallback<FocusOutEvent>((_) =>
            {
                // Optional: try to parse and log result
                if (DateTime.TryParse(_dateInput.value, null, DateTimeStyles.AssumeLocal, out DateTime parsed))
                {
                    // LastValidDateTime = new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, 0);
                    // _calendar.Monthly.Update(LastValidDateTime);
                    return;
                }
                
                // Invalid date format
                _dateInput.value = LastValidDateTime.ToString(CultureInfo.CurrentCulture);
            });
            
            _calendarContainer.RegisterCallback<ClickEvent>((_) =>
            {
                _calendarContainer.Focus();
            });
            
            _calendarContainer.RegisterCallback<FocusOutEvent>((_) =>
            {
                HideCalendar();
            });
            
            _calendarContainer.RegisterCallback<FocusInEvent>((_) =>
            {
                ShowCalendar();
            });
        }

        private void ShowCalendar()
        {
            _calendarContainer.style.display = DisplayStyle.Flex;
            _calendar.Show();
        }

        private void HideCalendar()
        {
            _calendarContainer.style.display = DisplayStyle.None;
            _calendar.Hide();
        }
    }
}