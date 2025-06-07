using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SerializedCalendar.Settings
{
    public class SerializedDateTimeSettings : ScriptableObject
    {
        public const string k_MyCustomSettingsPath = "Assets/Resources/SerializedDateTimeSettings.asset";

        [SerializeField]
        private int m_Number;

        [SerializeField]
        private string m_SomeString;

        private static SerializedDateTimeSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<SerializedDateTimeSettings>(k_MyCustomSettingsPath);
            if (settings == null)
            {
                settings = CreateInstance<SerializedDateTimeSettings>();
                settings.m_Number = 42;
                settings.m_SomeString = "The answer to the universe";
                AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}