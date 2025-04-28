using System;
using UnityEngine;

namespace SerializedCalendar
{
    public class ExampleSerializedDateTime : MonoBehaviour
    {
        [SerializeField]
        SerializedDateTime serializedDateTime;

        public string test = DateTime.Parse("november 2024").ToString("o");
    }
}
