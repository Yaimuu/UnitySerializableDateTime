using System;
using UnityEngine;

namespace SerializableDateTime
{
    [Serializable]
    public class SerializableDateTime
    {
        [SerializeField] private string dateInput = DateTime.Now.ToString("o");
        private DateTime dateTime = DateTime.Now;

        public DateTime DateTime
        {
            get => dateTime;
            set
            {
                dateTime = value;
                dateInput = dateTime.ToString("o");
            } 
        }

        public string ExposedDate
        {
            get => dateInput;
            set => dateInput = value;
        }
    }
}
