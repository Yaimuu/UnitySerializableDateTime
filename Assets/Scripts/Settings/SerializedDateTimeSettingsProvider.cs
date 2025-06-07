using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SerializedCalendar.Settings
{
    public class SerializedDateTimeSettingsProvider : SettingsProvider
    {
        private SerializedObject m_CustomSettings;

        class Styles
        {
            public static GUIContent number = new("My Number");
            public static GUIContent someString = new("Some string");
        }

        const string k_MyCustomSettingsPath = "Assets/Resources/SerializedDateTimeSettings.asset";
        public SerializedDateTimeSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) {}

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_MyCustomSettingsPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            m_CustomSettings = SerializedDateTimeSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            // Use IMGUI to display UI:
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_Number"), Styles.number);
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_SomeString"), Styles.someString);
            m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (!IsSettingsAvailable()) return null;
            var provider = new SerializedDateTimeSettingsProvider("SerializedDateTime/DateTimeSettingsProvider", SettingsScope.Project)
                {
                    // Automatically extract all keywords from the Styles.
                    keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
                };

            return provider;

            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
        }
    }
} 