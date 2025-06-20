using System;
using System.Globalization;
using SerializedCalendar.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SerializedCalendar.UI
{
    public class YearlyCalendarUI : BaseCalendarUI
    {
        private MultiColumnListView _yearsPicker;
        
        private CalendarScope _scope = CalendarScope.Year;
        
        public event UnityAction<CalendarScope, string> ScopeChanged;
        
        public YearlyCalendarUI(TemplateContainer template, DateTime date) : base(template)
        {
            Init(date);
        }

        private void Init(DateTime date)
        {
            _yearsPicker = Root.Q<MultiColumnListView>(UIConstants.YearlyCalendarId);
            _yearsPicker.showFoldoutHeader = false;
            _yearsPicker.columns.Clear();

            // TODO : Update this using settings
            var headerTemplate = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Calendars/HeaderCellTemplate.uxml");
            var cellTemplate = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Calendars/CellTemplate.uxml");

            for (int i = 0; i < CalendarDataSource.ColumnCount; i++)
            {
                int id = i;
                Column column = new Column
                {
                    name = $"year-{i}",
                    bindingPath = "dateValue",
                    width = Length.Auto(),
                    stretchable = true,
                    sortable = false,
                    optional = false,
                    resizable = false,
                    makeHeader = () =>
                    {
                        TemplateContainer root = headerTemplate.Instantiate();
                        root.style.display = DisplayStyle.None;
                        return root;
                    },
                    headerTemplate = headerTemplate,
                    cellTemplate = cellTemplate,
                    bindCell = (element, rowIndex) =>
                    {
                        var cellButton = element.Q<Button>(UIConstants.DayCellId);
                        cellButton.text = CalendarDataSource.values[rowIndex].cells[id].dateValue;
                        
                        cellButton.RemoveFromClassList("disabled");
                        cellButton.RemoveFromClassList("selected");

                        switch (_scope)
                        {
                            case CalendarScope.Year:
                                if (cellButton.text.Equals(date.ToString("MMMM")))
                                {
                                    cellButton.AddToClassList("selected");
                                }
                                break;
                            case CalendarScope.Decade:
                                bool parsed = short.TryParse(cellButton.text, out short year);
                                if (parsed && year == date.Year)
                                {
                                    cellButton.AddToClassList("selected");
                                }
                                break;
                            case CalendarScope.Century:
                                if (cellButton.text.Contains(date.Year.ToString().Substring(0, 3)))
                                {
                                    cellButton.AddToClassList("selected");
                                }
                                break;
                            case CalendarScope.Time:
                            case CalendarScope.Month:
                            default:
                                break;
                        }

                        cellButton.clickable = new Clickable((() =>
                        {
                            string dateString = date.ToString(CultureInfo.CurrentCulture);
                            switch (_scope)
                            {
                                case CalendarScope.Year:
                                    dateString = $"{cellButton.text} {date:yyyy}";
                                    _scope = CalendarScope.Month;
                                    break;
                                case CalendarScope.Decade:
                                    dateString = $"{date:MMMM} {cellButton.text}";
                                    _scope = CalendarScope.Year;
                                    break;
                                case CalendarScope.Century:
                                    dateString = $"{date:MMMM} {cellButton.text}";
                                    _scope = CalendarScope.Decade;
                                    break;
                            }
                            ScopeChanged?.Invoke(_scope, dateString);
                        }));
                    },
                };

                column.makeCell = () =>
                {
                    VisualElement newCell = column.cellTemplate.Instantiate();
                    return newCell;
                };
                
                _yearsPicker.columns.Add(column);
            }

            Update(date);
        }

        public void UpdateCalendarScope(DateTime date, CalendarScope newScope)
        {
            if(newScope is CalendarScope.Month or CalendarScope.Time) return;
            
            _scope = newScope;

            Init(date);
        }

        public void Update(DateTime date)
        {
            if(_yearsPicker == null) return;
            
            CalendarDataSource.UpdateCalendar(date, _scope);
            _yearsPicker.itemsSource = CalendarDataSource.Values;
            _yearsPicker.Rebuild();
        }
        
        public DateTime Previous(DateTime date)
        {
            DateTime newDate = _scope switch
            {
                CalendarScope.Year => date.AddYears(-1),
                CalendarScope.Decade => date.AddYears(-10),
                CalendarScope.Century => date.AddYears(-100),
                _ => date
            };
            Update(newDate);
            return newDate;
        }
        
        public DateTime Next(DateTime date) {
            DateTime newDate = _scope switch
            {
                CalendarScope.Year => date.AddYears(1),
                CalendarScope.Decade => date.AddYears(10),
                CalendarScope.Century => date.AddYears(100),
                _ => date
            };
            Update(newDate);
            return newDate;
        }
    }
}