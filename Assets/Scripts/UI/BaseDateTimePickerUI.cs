using UnityEngine;
using UnityEngine.UIElements;

namespace SerializedCalendar.UI
{
    public abstract class BaseDateTimePickerUI
    {
        public VisualElement Root { get; protected set; }
        
        protected readonly DateTimePickerData DateTimePickerData;

        protected BaseDateTimePickerUI(TemplateContainer template)
        {
            Root = template;
            DateTimePickerData = ScriptableObject.CreateInstance<DateTimePickerData>();
        }

        public virtual void Show()
        {
            Root.style.display = DisplayStyle.Flex;
        }

        public virtual void Hide()
        {
            Root.style.display = DisplayStyle.None;
        }
    }
}