using System;
using UnityEngine;

namespace DateTimePicker
{
    [Serializable]
    public class SerializableDateTime
    {
        [SerializeField] private string exposedDate;
        private DateTime dateTime;

        public DateTime DateTime
        {
            get => dateTime;
            set => dateTime = value;
        }

        public string ExposedDate
        {
            get => exposedDate;
            set => exposedDate = value;
        }
    }
}
