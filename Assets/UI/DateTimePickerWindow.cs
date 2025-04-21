using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace SerializableDateTime
{
    public class DateTimePickerWindow : EditorWindow
    {
        private static DateTimePickerWindow Instance { get; set; }

        public static bool IsOpen => Instance != null;

        [SerializeField]
        private VisualTreeAsset visualTreeAsset;

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
            VisualElement labelFromUXML = visualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
        }
        
        void OnEnable() {
            Instance = this;
        }
    }
}
