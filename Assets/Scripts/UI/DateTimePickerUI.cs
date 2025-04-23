using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SerializableDateTime.UI
{
    public class DateTimePickerUI
    {
        private const string DaysPickerId = "days-picker";
        private const string YearsPickerId = "years-picker";
        private const string DayCellId = "day-cell";
        private const string NavTitleId = "nav-title";
        private const string NavLeftArrowId = "nav-left-arrow";
        private const string NavRightArrowId = "nav-right-arrow";
        private const string TextFieldInputId = "serialized-date-input";
        private const string CalendarContainerId = "calendar-container";
        
        private TextField _dateInput;

        private MultiColumnListView _daysPicker;
        private MultiColumnListView _yearsPicker;

        private Button _nextButton;
        private Button _previousButton;
        
        private VisualElement _calendarContainer;
        
        private readonly DateTimePickerData _dateTimePickerData;
        private readonly SerializedProperty _serializedProperty;

        public VisualElement Root { get; private set; }
        
        private DateTime _lastValidDateTime;

        public DateTime LastValidDateTime
        {
            get => _lastValidDateTime;
            private set
            {
                _lastValidDateTime = value;
                UpdateMonthDaysList(LastValidDateTime);
                _dateInput.value = LastValidDateTime.ToString(CultureInfo.InvariantCulture);
            }
        }
        
        public event UnityAction<DateTime> DateChanged;

        public DateTimePickerUI(TemplateContainer template)
        {
            Root = template;
            _dateTimePickerData = ScriptableObject.CreateInstance<DateTimePickerData>();
        }

        public DateTimePickerUI(TemplateContainer template, DateTime initDate = new()) : this(template)
        {
            Init();
            UpdateMonthDaysList(initDate);
        }

        public DateTimePickerUI(TemplateContainer template, SerializedProperty property) :
            this(template)
        {
            Init();
            
            _serializedProperty = property;
            
            SerializedProperty dateInputProp = _serializedProperty.FindPropertyRelative("dateInput");
            
            _dateInput.label = _serializedProperty.displayName;
            _dateInput.value = dateInputProp.stringValue;

            InitInputEvents();
            
            if(dateInputProp.stringValue == string.Empty)
                _dateInput.value = DateTime.Today.ToString(CultureInfo.InvariantCulture);

            if (DateTime.TryParse(_dateInput.value, null, DateTimeStyles.None, out var parsed))
            {
                LastValidDateTime = parsed == DateTime.MinValue || parsed == DateTime.MaxValue ? LastValidDateTime : parsed;
                UpdateMonthDaysList(LastValidDateTime);
            }
        }

        void Init()
        {
            _yearsPicker = Root.Q<MultiColumnListView>(YearsPickerId);
            
            _nextButton = Root.Q<Button>(NavRightArrowId);
            _previousButton = Root.Q<Button>(NavLeftArrowId);
            
            _dateInput = Root.Q<TextField>(TextFieldInputId);
            
            _calendarContainer = Root.Q<VisualElement>(CalendarContainerId);
            _calendarContainer.style.display = DisplayStyle.None;
            _calendarContainer.BringToFront();
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
                _calendarContainer.style.display = DisplayStyle.Flex;
            });
            
            // Text field has been overriden
            _dateInput.RegisterCallback<FocusOutEvent>((_) =>
            {
                // Optional: try to parse and log result
                if (DateTime.TryParse(_dateInput.value, null, DateTimeStyles.AssumeLocal, out var parsed))
                {
                    LastValidDateTime = new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, 0);
                }
                else
                {
                    // Invalid date format
                    _dateInput.value = LastValidDateTime.ToString(CultureInfo.InvariantCulture);
                }
            });
            
            Root.RegisterCallback<FocusOutEvent>((_) =>
            {
                _calendarContainer.style.display = DisplayStyle.None;
            });
            
            Root.RegisterCallback<FocusInEvent>((_) =>
            {
                _calendarContainer.style.display = DisplayStyle.Flex;
            });
            
            _nextButton.clickable = new Clickable(() =>
            {
                LastValidDateTime = LastValidDateTime.AddMonths(1);
                DateChanged?.Invoke(LastValidDateTime);
            });
            
            _previousButton.clickable = new Clickable(() =>
            {
                LastValidDateTime = LastValidDateTime.AddMonths(-1);
                DateChanged?.Invoke(LastValidDateTime);
            });
        }

        void UpdateMonthDaysList(DateTime newDate)
        {
            _dateTimePickerData.Init(newDate);
            
            _daysPicker = Root.Q<MultiColumnListView>(DaysPickerId);
            _daysPicker.bindingPath = "values";
            _daysPicker.columns.Clear();
            
            // Get day names starting from Sunday (default in most cultures)
            List<string> dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
            
            List<string> reordered = new List<string>(dayNames.Skip(1).Concat(dayNames.Take(1)));

            foreach (var weekDay in reordered)
            {
                int i = reordered.IndexOf(weekDay);
                Column column = new Column
                {
                    name = $"day-{i}",
                    bindingPath = "dateValue",
                    title = weekDay.Substring(0, 2),
                    width = Length.Auto(),
                    stretchable = true,
                    sortable = false,
                    optional = false,
                    resizable = false,
                    headerTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/DaysPicker/DayHeaderCellTemplate.uxml"),
                    cellTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/DaysPicker/DayTemplate.uxml"),
                    bindCell = (element, rowIndex) =>
                    {
                        element.Q<Label>(DayCellId).text = _dateTimePickerData.values[rowIndex].cells[i].dateValue;
                    }
                };
                column.makeCell = () =>
                {
                    if (column.cellTemplate == null)
                    {
                        Debug.LogWarning($"Could not find cell template in column : {column.name}");
                        Label fallbackCell = new Label
                        {
                            name = DayCellId
                        };
                        return fallbackCell;
                    }
                    VisualElement newCell = column.cellTemplate.Instantiate();
                    return newCell;
                };
                
                _daysPicker.columns.Add(column);
            }
            
            var so = new SerializedObject(_dateTimePickerData);
            _daysPicker.Bind(so);
            Root.Q<Button>(NavTitleId).text = _dateTimePickerData.title;
        }
    }
}