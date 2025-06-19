using System;
using UnityEngine.UIElements;

namespace SerializedCalendar.UI
{
    [Serializable]
    public abstract class BaseCalendarUI
    {
        public VisualElement Root { get; protected set; }
        
        protected readonly CalendarData CalendarDataSource;
        
        public string Title => CalendarDataSource.title;

        protected BaseCalendarUI(TemplateContainer template)
        {
            Root = template;
            CalendarDataSource = new CalendarData();
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