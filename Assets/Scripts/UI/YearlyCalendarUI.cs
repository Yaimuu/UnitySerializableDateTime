using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Events;
using UnityEngine.UIElements;

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
            DateTimePickerData.UpdateCalendar(date);
            
            _yearsPicker = Root.Q<MultiColumnListView>(UIConstants.DaysPickerId);
            _yearsPicker.bindingPath = "values";
            _yearsPicker.columns.Clear();

            var headerTemplate = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Calendars/HeaderCellTemplate.uxml");
            var cellTemplate = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Calendars/CellTemplate.uxml");

            for (int i = 0; i < 4; i++)
            {
                Column column = new Column
                {
                    name = $"year-{i}",
                    bindingPath = "dateValue",
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
                        }

                        cellButton.clickable = new Clickable((() =>
                        {
                            switch (_scope)
                            {
                                case CalendarScope.Year:
                                    _scope = CalendarScope.Month;
                                    break;
                                case CalendarScope.Decade:
                                    _scope = CalendarScope.Year;
                                    break;
                                case CalendarScope.Century:
                                    _scope = CalendarScope.Decade;
                                    break;
                            }
                            ScopeChanged?.Invoke(_scope, cellButton.text);
                        }));
                    },
                };

                column.makeCell = () =>
                {
                    VisualElement newCell = column.cellTemplate.Instantiate();
                    return newCell;
                };
            }
            
            var so = new SerializedObject(DateTimePickerData);
            _yearsPicker.Bind(so);
        }

        public void UpdateCalendarScope(CalendarScope newScope)
        {
            _scope = newScope;
            
        }
    }
}