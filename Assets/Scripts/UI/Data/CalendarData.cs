using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SerializedCalendar.Extension;
using SerializedCalendar.Utils;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializedCalendar.UI
{
    [Serializable]
    public class DateTimeCellData
    {
        public string dateValue;

        public DateTimeCellData(string newValue)
        {
            dateValue = newValue;
        }
    }
    
    [Serializable]
    public class DateTimeRowData
    {
        public List<DateTimeCellData> cells;

        public DateTimeRowData(List<int> row)
        {
            cells = row.ConvertAll(cellValue => new DateTimeCellData(cellValue.ToString()));
        }
        
        public DateTimeRowData(List<string> row)
        {
            cells = row.ConvertAll(cellValue => new DateTimeCellData(cellValue));
        }
    }
    
    [Serializable]
    public class CalendarData : IDataSourceViewHashProvider, INotifyBindablePropertyChanged
    {
        public string title = "Test";
        public List<DateTimeRowData> values = new();

        [CreateProperty]
        public string Title
        {
            get => title;
            set
            {
                if (Title == value)
                    return;
                Title = value;
                Notify();
            }
        }

        [CreateProperty]
        public List<DateTimeRowData> Values
        {
            get => values;
            set
            {
                if (Values == value)
                    return;
                Values = value;
                Notify();
            }
        }

        public int ColumnCount => values.Count > 0 ? values.First().cells.Count : 0;

        public void UpdateCalendar(DateTime newDate, CalendarScope scope = CalendarScope.Month)
        {
            values.Clear();
            
            switch (scope)
            {
                case CalendarScope.Time:
                    break;

                case CalendarScope.Year:
                    values = CalendarUtils.GetMonthsMatrix().ConvertAll(row => new DateTimeRowData(row));
                    title = $"{newDate:yyyy}";
                    break;

                case CalendarScope.Decade:
                    SetRangeTitle(newDate.GenerateDecadeCalendarMatrix());
                    break;

                case CalendarScope.Century:
                    SetRangeTitle(newDate.GenerateCenturyCalendarMatrix());
                    break;

                case CalendarScope.Month:
                default:
                    values = newDate.GenerateMonthlyCalendarMatrix().ConvertAll(row => new DateTimeRowData(row));
                    title = $"{newDate:MMMM} {newDate:yyyy}";
                    break;
            }
        }

        private void SetRangeTitle(List<List<int>> matrix)
        {
            values = matrix.ConvertAll(row => new DateTimeRowData(row));
            var first = values.FirstOrDefault()?.cells.FirstOrDefault()?.dateValue;
            var last = values.LastOrDefault()?.cells.LastOrDefault()?.dateValue;
            title = $"{first}-{last}";
        }
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        
        public long GetViewHashCode()
        {
            return HashCode.Combine(title, values.GetHashCode());
        }

        private void Notify([CallerMemberName] string property = "")
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }
    }
}