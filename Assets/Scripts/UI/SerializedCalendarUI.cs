using System;
using System.Globalization;
using SerializedCalendar.Utils;
using UnityEngine.Events;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace SerializedCalendar.UI
{
    public class SerializedCalendarUI : BaseCalendarUI
    {
        #region Variables

        private CalendarScope _scope = CalendarScope.Month;
        private CalendarScope Scope
        {
            get => _scope;
            set
            {
                _scope = value;
                
                switch (_scope)
                {
                    case CalendarScope.Month:
                        _monthlyCalendarUI.Show();
                        _yearlyCalendarUI.Hide();
                        CalendarData.title = _monthlyCalendarUI.Title;
                        break;
                    case CalendarScope.Year:
                    case CalendarScope.Decade:
                    case CalendarScope.Century:
                        _yearlyCalendarUI.Show();
                        _monthlyCalendarUI.Hide();
                        CalendarData.title = _yearlyCalendarUI.Title;
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
#if UNITY_EDITOR
        private readonly SerializedProperty _serializedProperty;
#endif
        private DateTime _lastValidDateTime;

        public DateTime LastValidDateTime
        {
            get => _lastValidDateTime;
            private set
            {
                _lastValidDateTime = value;
                CalendarData.UpdateCalendar(_lastValidDateTime, Scope);
                _dateInput.value = LastValidDateTime.ToString(CultureInfo.CurrentCulture);
            }
        }

        #endregion
        
        public event UnityAction<DateTime> DateChanged;

        private SerializedCalendarUI(TemplateContainer template) : base(template) { }

#if UNITY_EDITOR
        public SerializedCalendarUI(TemplateContainer template, SerializedProperty property) :
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

            if (DateTime.TryParse(_dateInput.value, null, DateTimeStyles.AssumeUniversal, out DateTime parsed))
            {
                LastValidDateTime = parsed;
            }

            var monthlyCalendarContainer = template.Q<TemplateContainer>(UIConstants.DaysPickerId);
            var yearlyCalendarContainer = template.Q<TemplateContainer>(UIConstants.YearsPickerId);
            _monthlyCalendarUI = new MonthlyCalendarUI(monthlyCalendarContainer, LastValidDateTime);
            _yearlyCalendarUI = new YearlyCalendarUI(yearlyCalendarContainer, LastValidDateTime);

            _monthlyCalendarUI.DaySelected += OnDaySelected;
            _yearlyCalendarUI.ScopeChanged += OnYearScopeDescends;
            
            _monthlyCalendarUI.Hide();
            _yearlyCalendarUI.Hide();
        }
#endif

        /// <summary>
        /// Triggered when scope is descending.
        /// </summary>
        /// <param name="newScope"></param>
        /// <param name="dateString"></param>
        private void OnYearScopeDescends(CalendarScope newScope, string dateString)
        {
            DateTime newDate;
            Scope = newScope;
            switch (newScope)
            {
                case CalendarScope.Month:
                    newDate = DateTime.Parse(dateString);
                    _monthlyCalendarUI.UpdateFromYear(newDate);
                    CalendarData.UpdateCalendar(newDate);
                    break;
                case CalendarScope.Year:
                case CalendarScope.Decade:
                case CalendarScope.Century:
                    newDate = DateTime.Parse(dateString);
                    _yearlyCalendarUI.UpdateCalendarScope(newDate, newScope);
                    break;
                case CalendarScope.Time:
                default:
                    break;
            }
        }

        private void Init()
        {
            Root.BringToFront();
            
            _nextButton = Root.Q<Button>(UIConstants.NavRightArrowId);
            _previousButton = Root.Q<Button>(UIConstants.NavLeftArrowId);
            _scopeButton = Root.Q<Button>(UIConstants.NavTitleId);
            _scopeButton.bindingPath = "title";
#if UNITY_EDITOR
            _scopeButton.Bind(new SerializedObject(CalendarData));
#endif
            _dateInput = Root.Q<TextField>(UIConstants.TextFieldInputId);
            
            _calendarContainer = Root.Q<VisualElement>(UIConstants.CalendarContainerId);
            HideCalendar();
        }

        private void InitInputEvents()
        {
#if UNITY_EDITOR
            _dateInput.RegisterValueChangedCallback(evt =>
            {
                SerializedProperty dateInputProp = _serializedProperty.FindPropertyRelative("dateInput");
                // Update serialized property
                dateInputProp.stringValue = evt.newValue;
                _serializedProperty.serializedObject.ApplyModifiedProperties();
            });
#endif

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
                switch (Scope)
                {
                    case CalendarScope.Month:
                        LastValidDateTime = _monthlyCalendarUI.Next(LastValidDateTime);
                        break;
                    case CalendarScope.Year:
                    case CalendarScope.Decade:
                    case CalendarScope.Century:
                        LastValidDateTime = _yearlyCalendarUI.Next(LastValidDateTime);
                        break;
                    case CalendarScope.Time:
                    default:
                        break;
                }
                
                DateChanged?.Invoke(LastValidDateTime);
            });
            
            _previousButton.clickable = new Clickable(() =>
            {
                switch (Scope)
                {
                    case CalendarScope.Month:
                        LastValidDateTime = _monthlyCalendarUI.Previous(LastValidDateTime);
                        break;
                    case CalendarScope.Year:
                    case CalendarScope.Decade:
                    case CalendarScope.Century:
                        LastValidDateTime = _yearlyCalendarUI.Previous(LastValidDateTime);
                        break;
                    case CalendarScope.Time:
                    default:
                        break;
                }
                
                DateChanged?.Invoke(LastValidDateTime);
            });

            _scopeButton.clickable = new Clickable((() =>
            {
                CalendarScope nextScope = Scope switch
                {
                    CalendarScope.Time => CalendarScope.Month,
                    CalendarScope.Month => CalendarScope.Year,
                    CalendarScope.Year => CalendarScope.Decade,
                    CalendarScope.Decade => CalendarScope.Century,
                    _ => Scope
                };

                _yearlyCalendarUI.UpdateCalendarScope(LastValidDateTime, nextScope);
                
                Scope = nextScope;
            }));
        }

        private void ShowCalendar()
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

        private void HideCalendar()
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