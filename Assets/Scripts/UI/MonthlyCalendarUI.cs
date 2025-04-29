using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace SerializedCalendar.UI
{
    public class MonthlyCalendarUI : BaseDateTimePickerUI
    {
        private MultiColumnListView _daysPicker;

        public MultiColumnListView DaysPicker => _daysPicker;

        public event UnityAction<DateTime> DaySelected;

        public override void Show()
        {
            Root.Q<MultiColumnListView>(UIConstants.CalendarWeekdaysId).style.display = DisplayStyle.Flex;
        }

        public override void Hide()
        {
            Root.Q<MultiColumnListView>(UIConstants.CalendarWeekdaysId).style.display = DisplayStyle.None;
        }

        public MonthlyCalendarUI(TemplateContainer template, DateTime date) : base(template)
        {
            Init(date);
        }

        public void Init(DateTime date)
        {
            DateTimePickerData.UpdateCalendar(date);
            
            _daysPicker = Root.Q<MultiColumnListView>(UIConstants.CalendarWeekdaysId);
            _daysPicker.bindingPath = "values";
            _daysPicker.columns.Clear();
            
            var headerTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/UI/Calendars/HeaderCellTemplate.uxml");
            var cellTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Calendars/CellTemplate.uxml");

            // Get day names starting from Sunday (default in most cultures)
            List<string> dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
            // dayNames = new List<string>(dayNames.Skip(1).Concat(dayNames.Take(1)));

            foreach (var weekDay in dayNames)
            {
                int i = dayNames.IndexOf(weekDay);
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
                    headerTemplate = headerTemplate,
                    cellTemplate = cellTemplate,
                    bindCell = (element, rowIndex) =>
                    {
                        var cellButton = element.Q<Button>(UIConstants.DayCellId);
                        cellButton.text = DateTimePickerData.values[rowIndex].cells[i].dateValue;
                        
                        cellButton.RemoveFromClassList("disabled");
                        cellButton.RemoveFromClassList("selected");

                        int cellDay = short.Parse(cellButton.text);

                        bool isPreviousMonth = (rowIndex == 0 && cellDay > 7);
                        bool isNextMonth = (rowIndex is 4 or 5 && cellDay <= 14);
                        if (isPreviousMonth || isNextMonth)
                        {
                            cellButton.AddToClassList("disabled");
                        } else if (cellDay == date.Day)
                        {
                            cellButton.AddToClassList("selected");
                        }

                        cellButton.clickable = new Clickable(() =>
                        {
                            var isSelected = cellButton.ClassListContains("selected");
                            if (isSelected) return;

                            _daysPicker.Query<Button>().ForEach(b => b.RemoveFromClassList("selected"));
                            
                            int daySelected = short.Parse(cellButton.text);

                            var next = rowIndex == 0 && cellDay > 7;
                            var previous = rowIndex is 4 or 5 && cellDay <= 14;
                            if (next)
                                date = date.AddMonths(-1);
                            else if(previous)
                                date = date.AddMonths(1);
                            
                            date = date.AddDays(daySelected - date.Day);

                            cellButton.AddToClassList("selected");
                            
                            if(next || previous)
                                Init(date);

                            DaySelected?.Invoke(date);
                        });
                        
                        // Debug.Log("Cell bound");
                    }
                };
                column.makeCell = () =>
                {
                    if (column.cellTemplate == null)
                    {
                        Debug.LogWarning($"Could not find cell template in column : {column.name}");
                        Label fallbackCell = new Label
                        {
                            name = UIConstants.DayCellId
                        };
                        return fallbackCell;
                    }

                    VisualElement newCell = column.cellTemplate.Instantiate();
                    return newCell;
                };

                _daysPicker.columns.Add(column);
            }
            
            var so = new SerializedObject(DateTimePickerData);
            _daysPicker.Bind(so);
        }
        
        public void UpdateMonthlyCalendar(DateTime newDate)
        {
            if(_daysPicker == null) return;
            
            DateTimePickerData.UpdateCalendar(newDate);
        }
    }
}