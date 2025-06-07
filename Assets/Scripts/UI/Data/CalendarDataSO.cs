using UnityEngine;
using UnityEngine.UIElements;

namespace SerializedCalendar.UI
{
    [CreateAssetMenu(menuName = "SerializedDateTime/DateTimePickerDataSO", fileName = "DateTimePickerDataSO")]
    public class CalendarDataSO : ScriptableObject
    {
        [SerializeField] private PanelSettings dateTimePickerPanelSettings;
        [SerializeField] private VisualTreeAsset headerTemplate;
        [SerializeField] private VisualTreeAsset cellTemplate;

        public PanelSettings DateTimePickerPanelSettings => dateTimePickerPanelSettings;

        public VisualTreeAsset HeaderTemplate => headerTemplate;

        public VisualTreeAsset CellTemplate => cellTemplate;
    }
}