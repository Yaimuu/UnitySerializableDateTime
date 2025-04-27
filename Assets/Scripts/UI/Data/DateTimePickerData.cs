using System;
using System.Collections.Generic;
using SerializedCalendar.Extension;
using UnityEngine;

namespace SerializedCalendar.UI
{
    [Serializable]
    public class DateTimeCellData : ScriptableObject
    {
        public string dateValue;
    }
    
    [Serializable]
    public class DateTimeRowData
    {
        public List<DateTimeCellData> cells;

        public DateTimeRowData(List<int> row)
        {
            cells = row.ConvertAll(cellValue =>
            {
                DateTimeCellData cell = ScriptableObject.CreateInstance<DateTimeCellData>();
                cell.dateValue = cellValue.ToString();
                return cell;
            });
        }
    }
    
    public class DateTimePickerData : ScriptableObject
    {
        public string title = "Date";
        public List<DateTimeRowData> values = new();

        public void UpdateCalendar(DateTime newDate)
        {
            var newValues = newDate.GenerateCalendarMatrix();
            values.Clear();
            values = newValues.ConvertAll(row => new DateTimeRowData(row));
            title = $"{newDate:MMMM} {newDate:yyyy}";
        }
    }
}