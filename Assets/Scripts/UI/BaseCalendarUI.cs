using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializedCalendar.UI
{
    [Serializable]
    public abstract class BaseCalendarUI
    {
        public VisualElement Root { get; protected set; }
        
        protected readonly CalendarData CalendarData;
        
        public string Title => CalendarData.title;

        protected BaseCalendarUI(TemplateContainer template)
        {
            Root = template;
            CalendarData = ScriptableObject.CreateInstance<CalendarData>();
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