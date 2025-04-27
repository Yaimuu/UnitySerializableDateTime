using System;
using System.Globalization;
using UnityEngine;

namespace SerializedCalendar
{
    [Serializable]
    public class SerializedDateTime
    {
        [SerializeField] private string dateInput = DateTime.Now.ToString(CultureInfo.InvariantCulture);

        public DateTime Date
        {
            get => DateTime.Parse(dateInput);
            set => dateInput = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
