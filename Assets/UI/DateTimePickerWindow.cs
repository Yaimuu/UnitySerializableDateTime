using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableDateTime
{
    public class DateTimePickerWindow : EditorWindow
    {
        private static DateTimePickerWindow Instance { get; set; }

        public static bool IsOpen => Instance != null;

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset;

        [MenuItem("Window/UI Toolkit/DateTimePicker")]
        public static void ShowExample()
        {
            DateTimePickerWindow wnd = GetWindow<DateTimePickerWindow>();
            wnd.titleContent = new GUIContent("DateTimePicker");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
        }
        
        void OnEnable() {
            Instance = this;
        }
    }
}
