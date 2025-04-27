using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace SerializedCalendar.UI
{
    public class YearlyCalendarUI : BaseDateTimePickerUI
    {
        private MultiColumnListView _yearsPicker;
        
        public YearlyCalendarUI(TemplateContainer template) : base(template)
        {
        }

        void Init(DateTime date)
        {
            DateTimePickerData.UpdateCalendar(date);
            
            _yearsPicker = Root.Q<MultiColumnListView>(UIConstants.DaysPickerId);
            _yearsPicker.bindingPath = "values";
            _yearsPicker.columns.Clear();

            var headerTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/UI/Calendars/HeaderCellTemplate.uxml");
            var cellTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Calendars/CellTemplate.uxml");

            for (int i = 0; i < 4; i++)
            {
                Column column = new Column
                {
                    name = $"day-{i}",
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
                        
                    },
                };

                column.makeCell = () =>
                {
                    VisualElement newCell = column.cellTemplate.Instantiate();
                    return newCell;
                };
            }
        }
    }
}