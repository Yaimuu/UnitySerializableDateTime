using System;
using SerializedCalendar.Utils;
using Unity.Properties;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SerializedCalendar.UI
{
    public class CalendarUI : BaseCalendarUI
    {
        #region Variables

        private CalendarScope _scope = CalendarScope.Month;
        private CalendarScope Scope
        {
            get => _scope;
            set
            {
                _scope = value;
                
                switch (_scope)
                {
                    case CalendarScope.Month:
                        _monthlyCalendarUI.Show();
                        _yearlyCalendarUI.Hide();
                        _scopeButton.text = _monthlyCalendarUI.Title;
                        break;
                    case CalendarScope.Year:
                    case CalendarScope.Decade:
                    case CalendarScope.Century:
                        _yearlyCalendarUI.Show();
                        _monthlyCalendarUI.Hide();
                        _scopeButton.text = _yearlyCalendarUI.Title;
                        break;
                    case CalendarScope.Time:
                    default:
                        break;
                }
            }
        }

        private readonly MonthlyCalendarUI _monthlyCalendarUI;
        private readonly YearlyCalendarUI _yearlyCalendarUI;
        
        public MonthlyCalendarUI Monthly => _monthlyCalendarUI;
        public YearlyCalendarUI Yearly => _yearlyCalendarUI;

        private Button _scopeButton;
        private Button _nextButton;
        private Button _previousButton;
        
        private DateTime _currentDate;

        private DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                CalendarDataSource.UpdateCalendar(_currentDate, Scope);
                // _scopeButton?.SetBinding(nameof(_scopeButton.text), new DataBinding()
                // {
                //     dataSourcePath = PropertyPath.FromName(nameof(CalendarDataSource.Title)),
                //     dataSource = CalendarDataSource
                // });
                _scopeButton.text = CalendarDataSource.title;
                DateChanged?.Invoke(_currentDate);
            }
        }

        #endregion
        
        public event UnityAction<DateTime> DateChanged;

        private CalendarUI(TemplateContainer template) : base(template) { }

        public CalendarUI(TemplateContainer template, DateTime initDate) :
            this(template)
        {
            Init();

            InitInputEvents();
            
            CurrentDate = initDate;

            var monthlyCalendarContainer = template.Q<TemplateContainer>(UIConstants.DaysPickerId);
            var yearlyCalendarContainer = template.Q<TemplateContainer>(UIConstants.YearsPickerId);
            _monthlyCalendarUI = new MonthlyCalendarUI(monthlyCalendarContainer, CurrentDate);
            _yearlyCalendarUI = new YearlyCalendarUI(yearlyCalendarContainer, CurrentDate);

            _monthlyCalendarUI.DaySelected += OnDaySelected;
            _yearlyCalendarUI.ScopeChanged += OnYearScopeDescends;
            
            _monthlyCalendarUI.Hide();
            _yearlyCalendarUI.Hide();
        }

        /// <summary>
        /// Triggered when scope is descending.
        /// </summary>
        /// <param name="newScope"></param>
        /// <param name="dateString"></param>
        private void OnYearScopeDescends(CalendarScope newScope, string dateString)
        {
            DateTime newDate;
            switch (newScope)
            {
                case CalendarScope.Month:
                    newDate = DateTime.Parse(dateString);
                    _monthlyCalendarUI.UpdateFromYear(newDate);
                    CalendarDataSource.UpdateCalendar(newDate);
                    break;
                case CalendarScope.Year:
                case CalendarScope.Decade:
                case CalendarScope.Century:
                    newDate = DateTime.Parse(dateString);
                    _yearlyCalendarUI.UpdateCalendarScope(newDate, newScope);
                    break;
                case CalendarScope.Time:
                default:
                    break;
            }
            Scope = newScope;
        }

        private void Init()
        {
            Root.BringToFront();
            
            _nextButton = Root.Q<Button>(UIConstants.NavRightArrowId);
            _previousButton = Root.Q<Button>(UIConstants.NavLeftArrowId);
            _scopeButton = Root.Q<Button>(UIConstants.NavTitleId);
            
            Hide();
        }

        private void InitInputEvents()
        {
            _nextButton.clickable = new Clickable(() =>
            {
                switch (Scope)
                { 
                    case CalendarScope.Month:
                        CurrentDate = _monthlyCalendarUI.Next(CurrentDate);
                        break;
                    case CalendarScope.Year:
                    case CalendarScope.Decade:
                    case CalendarScope.Century:
                        CurrentDate = _yearlyCalendarUI.Next(CurrentDate);
                        break;
                    case CalendarScope.Time:
                    default:
                        break;
                }
                
                // DateChanged?.Invoke(CurrentDate);
            });
            
            _previousButton.clickable = new Clickable(() =>
            {
                switch (Scope)
                {
                    case CalendarScope.Month:
                        CurrentDate = _monthlyCalendarUI.Previous(CurrentDate);
                        break;
                    case CalendarScope.Year:
                    case CalendarScope.Decade:
                    case CalendarScope.Century:
                        CurrentDate = _yearlyCalendarUI.Previous(CurrentDate);
                        break;
                    case CalendarScope.Time:
                    default:
                        break;
                }
                
                // DateChanged?.Invoke(CurrentDate);
            });

            _scopeButton.clickable = new Clickable((() =>
            {
                CalendarScope nextScope = Scope switch
                {
                    CalendarScope.Time => CalendarScope.Month,
                    CalendarScope.Month => CalendarScope.Year,
                    CalendarScope.Year => CalendarScope.Decade,
                    CalendarScope.Decade => CalendarScope.Century,
                    _ => Scope
                };

                _yearlyCalendarUI.UpdateCalendarScope(CurrentDate, nextScope);
                
                Scope = nextScope;
            }));
        }

        public override void Show()
        {
            if (Scope == CalendarScope.Month)
            {
                _monthlyCalendarUI?.Show();
                _yearlyCalendarUI?.Hide();
            }
            else
            {
                _yearlyCalendarUI?.Show();
                _monthlyCalendarUI?.Hide();
            }
        }

        public override void Hide()
        {
            _monthlyCalendarUI?.Show();
            _yearlyCalendarUI?.Hide();
        }
        
        private void OnDaySelected(DateTime newDate)
        {
            CurrentDate = newDate;
        }
    }
}