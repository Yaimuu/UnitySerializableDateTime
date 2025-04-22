using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
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

        private MultiColumnListView _daysPicker;
        private MultiColumnListView _yearsPicker;
        
        private readonly DateTimePickerData _dateTimePickerData;

        public VisualElement Root { get; private set; }

        public DateTimePickerUI(TemplateContainer template)
        {
            Root = template;
            _dateTimePickerData = ScriptableObject.CreateInstance<DateTimePickerData>();
            Init();
        }

        void Init()
        {
            _yearsPicker = Root.Q<MultiColumnListView>(YearsPickerId);
        }

        public void UpdateMonthDaysList(DateTime newDate)
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
                        Label fallbackCell = new Label();
                        fallbackCell.name = DayCellId;
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