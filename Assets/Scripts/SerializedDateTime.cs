using System;
using System.Globalization;
using UnityEngine;

namespace SerializedCalendar
{
    [Serializable]
    public class SerializedDateTime
    {
        [SerializeField] private string dateInput = "01/01/1980";

        public DateTime Date
        {
            get => DateTime.Parse(dateInput);
            set => dateInput = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
