using System;
using System.Globalization;
using UnityEngine;

namespace SerializedDateTime
{
    [Serializable]
    public class SerializedDateTime
    {
        [SerializeField] private string dateInput = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        private DateTime _dateTime = DateTime.Now;

        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                dateInput = _dateTime.ToString(CultureInfo.InvariantCulture);
            }
        }

        public string ExposedDate
        {
            get => dateInput;
            set => dateInput = value;
        }
    }
}
