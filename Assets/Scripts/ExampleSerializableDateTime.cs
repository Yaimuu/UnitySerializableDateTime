using UnityEngine;
using UnityEngine.Serialization;

namespace SerializedDateTime
{
    public class ExampleSerializableDateTime : MonoBehaviour
    {
        [FormerlySerializedAs("serializableDateTime")] [SerializeField]
        SerializedDateTime serializedDateTime;
    }
}
