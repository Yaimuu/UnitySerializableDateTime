using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace SerializedCalendar.UI
{
    public enum CalendarScope
    {
        Time,
        Month,
        Year,
        Decade,
        Century
    }
    public class SerializedDateTimePickerUI : BaseDateTimePickerUI
    {
        private CalendarScope _scope = CalendarScope.Month;
        private CalendarScope Scope
        {
            get => _scope;
            set
            {
                _scope = value;
                Debug.Log(_scope);
                if(_scope == CalendarScope.Century) return;
                switch (_scope)
                {
                    case CalendarScope.Month:
                        _monthlyCalendarUI.Show();
                        _yearlyCalendarUI.Hide();
                        break;
                    case CalendarScope.Year:
                    case CalendarScope.Decade:
                    case CalendarScope.Century:
                        _yearlyCalendarUI.Show();
                        _monthlyCalendarUI.Hide();
                        break;
                    case CalendarScope.Time:
                    default:
                        break;
                }
            }
        }
        
        private TextField _dateInput;

        private readonly MonthlyCalendarUI _monthlyCalendarUI;
        private readonly YearlyCalendarUI _yearlyCalendarUI;

        private Button _scopeButton;
        private Button _nextButton;
        private Button _previousButton;
        
        private VisualElement _calendarContainer;
        
        private readonly SerializedProperty _serializedProperty;
        
        private DateTime _lastValidDateTime;

        public DateTime LastValidDateTime
        {
            get => _lastValidDateTime;
            private set
            {
                _lastValidDateTime = value;
                DateTimePickerData.UpdateCalendar(_lastValidDateTime);
                _dateInput.value = LastValidDateTime.ToString(CultureInfo.CurrentCulture);
            }
        }
        
        public event UnityAction<DateTime> DateChanged;

        private SerializedDateTimePickerUI(TemplateContainer template) : base(template) { }

        public SerializedDateTimePickerUI(TemplateContainer template, SerializedProperty property) :
            this(template)
        {
            Init();
            
            _serializedProperty = property;
            
            SerializedProperty dateInputProp = _serializedProperty.FindPropertyRelative("dateInput");
            
            _dateInput.label = _serializedProperty.displayName;
            _dateInput.value = dateInputProp.stringValue;

            InitInputEvents();
            
            if(dateInputProp.stringValue == string.Empty)
                _dateInput.value = DateTime.Today.ToString(CultureInfo.CurrentCulture);

            if (DateTime.TryParse(_dateInput.value, null, DateTimeStyles.AssumeUniversal, out var parsed))
            {
                LastValidDateTime = parsed;
            }

            var monthlyCalendarContainer = template.Q<TemplateContainer>(UIConstants.DaysPickerId);
            var yearlyCalendarContainer = template.Q<TemplateContainer>(UIConstants.YearsPickerId);
            _monthlyCalendarUI = new MonthlyCalendarUI(monthlyCalendarContainer, LastValidDateTime);
            _yearlyCalendarUI = new YearlyCalendarUI(yearlyCalendarContainer, LastValidDateTime);

            _monthlyCalendarUI.DaySelected += OnDaySelected;
            _yearlyCalendarUI.ScopeChanged += OnYearScopeChanged;
            
            _monthlyCalendarUI.Hide();
            _yearlyCalendarUI.Hide();
        }

        private void OnYearScopeChanged(CalendarScope newScope, string dateString)
        {
            Scope = newScope;
            switch (Scope)
            {
                case CalendarScope.Month:
                    _monthlyCalendarUI.UpdateMonthlyCalendar(DateTime.Parse($"{dateString} {LastValidDateTime:yyyy}"));
                    break;
            }
        }

        void Init()
        {
            Root.BringToFront();
            
            _nextButton = Root.Q<Button>(UIConstants.NavRightArrowId);
            _previousButton = Root.Q<Button>(UIConstants.NavLeftArrowId);
            _scopeButton = Root.Q<Button>(UIConstants.NavTitleId);
            _scopeButton.bindingPath = "title";
            _scopeButton.Bind(new SerializedObject(DateTimePickerData));
            
            _dateInput = Root.Q<TextField>(UIConstants.TextFieldInputId);
            
            _calendarContainer = Root.Q<VisualElement>(UIConstants.CalendarContainerId);
            HideCalendar();
        }

        void InitInputEvents()
        {
            _dateInput.RegisterValueChangedCallback(evt =>
            {
                SerializedProperty dateInputProp = _serializedProperty.FindPropertyRelative("dateInput");
                // Update serialized property
                dateInputProp.stringValue = evt.newValue;
                _serializedProperty.serializedObject.ApplyModifiedProperties();
            });

            _dateInput.RegisterCallback<ClickEvent>((_) =>
            {
                ShowCalendar();
            });
            
            // Text field has been overriden
            _dateInput.RegisterCallback<FocusOutEvent>((_) =>
            {
                // Optional: try to parse and log result
                if (DateTime.TryParse(_dateInput.value, null, DateTimeStyles.AssumeLocal, out var parsed))
                {
                    LastValidDateTime = new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, 0);
                    _monthlyCalendarUI.UpdateMonthlyCalendar(LastValidDateTime);
                }
                else
                {
                    // Invalid date format
                    _dateInput.value = LastValidDateTime.ToString(CultureInfo.CurrentCulture);
                }
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
            
            _nextButton.clickable = new Clickable(() =>
            {
                LastValidDateTime = LastValidDateTime.AddMonths(1);
                _monthlyCalendarUI.Init(LastValidDateTime);
                DateChanged?.Invoke(LastValidDateTime);
            });
            
            _previousButton.clickable = new Clickable(() =>
            {
                LastValidDateTime = LastValidDateTime.AddMonths(-1);
                _monthlyCalendarUI.Init(LastValidDateTime);
                DateChanged?.Invoke(LastValidDateTime);
            });

            _scopeButton.clickable = new Clickable((() =>
            {
                Scope = Scope switch
                {
                    CalendarScope.Time => CalendarScope.Month,
                    CalendarScope.Month => CalendarScope.Year,
                    CalendarScope.Year => CalendarScope.Decade,
                    CalendarScope.Decade => CalendarScope.Century,
                    _ => Scope
                };

                _yearlyCalendarUI.UpdateCalendarScope(LastValidDateTime, Scope);
            }));
        }

        void ShowCalendar()
        {
            _calendarContainer.style.display = DisplayStyle.Flex;
            if (Scope == CalendarScope.Month)
            {
                _monthlyCalendarUI?.Show();
                _yearlyCalendarUI?.Hide();
            }
            else
            {
                _yearlyCalendarUI?.Show();
                _monthlyCalendarUI?.Hide();
            }
        }

        void HideCalendar()
        {
            _calendarContainer.style.display = DisplayStyle.None;
            _monthlyCalendarUI?.Show();
            _yearlyCalendarUI?.Hide();
        }
        
        private void OnDaySelected(DateTime newDate)
        {
            LastValidDateTime = newDate;
            
        }
    }
}