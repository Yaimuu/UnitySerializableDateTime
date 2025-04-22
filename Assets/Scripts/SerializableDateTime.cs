using System;
using UnityEngine;

namespace SerializableDateTime
{
    [Serializable]
    public class SerializableDateTime
    {
        [SerializeField] private string dateInput = DateTime.Now.ToString("o");
        private DateTime _dateTime = DateTime.Now;

        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                dateInput = _dateTime.ToString("o");
            }
        }

        public string ExposedDate
        {
            get => dateInput;
            set => dateInput = value;
        }
    }
}
