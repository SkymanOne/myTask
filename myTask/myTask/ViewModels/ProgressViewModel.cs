using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microcharts;
using myTask.Domain.Models;
using myTask.Services.AssignmentsManager;
using myTask.Services.Database.Repositories;
using myTask.Services.FeedService;
using myTask.Services.Navigation;
using myTask.Services.UserConfigManager;
using myTask.ViewModels.Base;
using myTask.Views;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Timer = System.Timers.Timer;

namespace myTask.ViewModels
{
    public class ProgressViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(ProgressPage);

        private readonly IRepository<Tag> _tagRepository;
        private readonly IAssignmentsManager _assignmentsManager;
        private readonly IUserConfigManager _configManager;
        private readonly Calendar _calendar = new GregorianCalendar(GregorianCalendarTypes.Localized);

        private UserConfig _config;
        private int _totalKinbens = 0;
        private WeeklyTimetable _weeklyTimetable;
        private IEnumerable<Tag> _tags;
        private IEnumerable<ChartEntry> _radialEntries;
        private IEnumerable<ChartEntry> _donutEntries;


        public string TotalKinbens => _totalKinbens.ToString();

        public Chart RadialChart =>
            new RadialGaugeChart()
            {
                Entries = _radialEntries,
                AnimationDuration = new TimeSpan(0, 0, 0, 1),
                IsAnimated = true,
                LabelTextSize = 40
            };

        public Chart DonutChart =>
            new DonutChart()
            {
                Entries = _donutEntries,
                AnimationDuration = new TimeSpan(0, 0, 0, 1),
                IsAnimated = true,
                LabelTextSize = 40
            };

        public ProgressViewModel(INavigationService navigationService,
            IRepository<Tag> tagRepository,
            IAssignmentsManager assignmentsManager,
            IUserConfigManager configManager) : base(navigationService)
        {
            _tagRepository = tagRepository;
            _assignmentsManager = assignmentsManager;
            _configManager = configManager;
            MessagingCenter.Subscribe<FeedService>(this, "New Update", 
                async model =>
                {
                    await Init(null);
                    OnPropertyChanged(nameof(DonutChart));
                    OnPropertyChanged(nameof(RadialChart));
                    OnPropertyChanged(nameof(TotalKinbens));
                });
        }

        private IEnumerable<ChartEntry> GenerateEntries(ChartType chartType)
        {
            switch (chartType)
            {
                case ChartType.Radial:
                {
                    var createdAssignments = _weeklyTimetable
                        .Timetables
                        .SelectMany(x => x.Assignments)
                        .Where(x => x.Status == Status.Created);
                    var goingAssignments = _weeklyTimetable
                        .Timetables
                        .SelectMany(x => x.Assignments)
                        .Where(x => x.Status == Status.Going || x.Status == Status.Paused);
                    var finishedAssignments = _weeklyTimetable
                        .Timetables
                        .SelectMany(x => x.Assignments)
                        .Where(x => x.Status == Status.Finished);
                    return new List<ChartEntry>()
                    {
                        new ChartEntry(finishedAssignments.Count())
                        {
                            ValueLabel = finishedAssignments.Count().ToString(),
                            Color = SKColor.Parse("#00796B"),
                            ValueLabelColor = SKColor.Parse("#00796B"),
                            Label = "Finished",
                        },
                        new ChartEntry(goingAssignments.Count())
                        {
                            ValueLabel = goingAssignments.Count().ToString(),
                            Color = SKColor.Parse("#C2185B"),
                            ValueLabelColor = SKColor.Parse("#C2185B"),
                            Label = "Going",
                        },
                        new ChartEntry(createdAssignments.Count())
                        {
                            ValueLabel = createdAssignments.Count().ToString(),
                            Color = SKColor.Parse("#512DA8"),
                            ValueLabelColor = SKColor.Parse("#512DA8"),
                            Label = "Created",
                        },
                    };
                }
                case ChartType.Donut:
                {
                    return _tags.Select(x =>
                    {
                        int i = _tags.IndexOf(x);
                        if (i > Constants.COLORS.Length - 1)
                        {
                            i = 0;
                        }
                        var color = Constants.COLORS[i];
                        return new ChartEntry(x.Assignments.Count)
                        {
                            Label = x.Title,
                            Color = color,
                            ValueLabelColor = color
                        };
                    });
                }
                default:
                    throw new NotSupportedException();
            }
        }

        public override async Task Init(object param)
        {
            _config = await _configManager.GetConfigAsync();
            _totalKinbens = _config.Kinbens;
            _weeklyTimetable = await _assignmentsManager.LoadWeeklyTimetableAsync(
                _calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday));
            _tags = await _tagRepository.GetAllItemsAsync();
            _radialEntries = GenerateEntries(ChartType.Radial);
            _donutEntries = GenerateEntries(ChartType.Donut);
        }

        enum ChartType
        {
            Radial,
            Donut
        }
    }
}