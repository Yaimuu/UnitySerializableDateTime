using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace SerializedCalendar.UI
{
    public class YearlyCalendarUI : BaseDateTimePickerUI
    {
        private MultiColumnListView _yearsPicker;
        
        private CalendarScope _scope = CalendarScope.Year;
        
        public event UnityAction<CalendarScope, string> ScopeChanged;
        
        public YearlyCalendarUI(TemplateContainer template, DateTime date) : base(template)
        {
            Init(date);
        }

        void Init(DateTime date)
        {
            DateTimePickerData.UpdateCalendar(date, CalendarScope.Year);
            
            _yearsPicker = Root.Q<MultiColumnListView>(UIConstants.YearlyCalendarId);
            _yearsPicker.bindingPath = "values";
            _yearsPicker.columns.Clear();
            _yearsPicker.showFoldoutHeader = false;

            var headerTemplate = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Calendars/HeaderCellTemplate.uxml");
            var cellTemplate = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Calendars/CellTemplate.uxml");

            for (int i = 0; i < 3; i++)
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
                        var root = headerTemplate.Instantiate();
                        root.style.display = DisplayStyle.None;
                        return root;
                    },
                    headerTemplate = headerTemplate,
                    cellTemplate = cellTemplate,
                    bindCell = (element, rowIndex) =>
                    {
                        var cellButton = element.Q<Button>(UIConstants.DayCellId);
                        cellButton.text = DateTimePickerData.values[rowIndex].cells[id].dateValue;
                        
                        cellButton.RemoveFromClassList("disabled");
                        cellButton.RemoveFromClassList("selected");

                        switch (_scope)
                        {
                            case CalendarScope.Year:
                                if (cellButton.text.Equals(date.Month.ToString()))
                                {
                                    cellButton.AddToClassList("selected");
                                }
                                break;
                            case CalendarScope.Decade:
                                if (short.Parse(cellButton.text) == date.Year)
                                {
                                    cellButton.AddToClassList("selected");
                                }
                                break;
                            case CalendarScope.Century:
                                if (cellButton.text.Contains(date.Year.ToString().Substring(0, 2)))
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
                            _scope = _scope switch
                            {
                                CalendarScope.Year => CalendarScope.Month,
                                CalendarScope.Decade => CalendarScope.Year,
                                CalendarScope.Century => CalendarScope.Decade,
                                _ => _scope
                            };
                            ScopeChanged?.Invoke(_scope, cellButton.text);
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
            
            var so = new SerializedObject(DateTimePickerData);
            _yearsPicker.Bind(so);
        }

        public void UpdateCalendarScope(DateTime date, CalendarScope newScope)
        {
            _scope = newScope;
            if(_scope == CalendarScope.Year)
                Init(date);
            
            DateTimePickerData.UpdateCalendar(date, _scope);
        }
    }
}