using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microcharts;
using myTask.Domain.Models;
using myTask.Services.AssignmentsManager;
using myTask.Services.Database.Repositories;
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

        private Timer _timer;

        private SKColor[] _colors = new[]
        {
            SKColor.Parse("#0277BD"),
            SKColor.Parse("#00838F"),
            SKColor.Parse("#00695C"),
            SKColor.Parse("#689F38"),
            SKColor.Parse("#388E3C"),
            SKColor.Parse("#9E9D24"),
            SKColor.Parse("#F57C00"),
            SKColor.Parse("#FFA000"),
            SKColor.Parse("#FBC02D"),
            SKColor.Parse("#F9A825"),
            SKColor.Parse("#FFC400"),
            SKColor.Parse("#FF9100"),
            SKColor.Parse("#D84315"),
            SKColor.Parse("#455A64"),
            SKColor.Parse("#455A64"),
            SKColor.Parse("#4527A0"),
            SKColor.Parse("#1565C0"),
            SKColor.Parse("#283593"),
            SKColor.Parse("#6A1B9A"),
            SKColor.Parse("#AD1457"),
            SKColor.Parse("#C62828"),
        };

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
            _timer = new Timer(100000);
            _timer.Enabled = true;
            _timer.Elapsed += async (sender, args) =>
            {
                await Init(null);
            };
        }

        private IEnumerable<ChartEntry> GenerateEntries(ChartType chartType)
        {
            switch (chartType)
            {
                case ChartType.Radial:
                {
                    var total = _weeklyTimetable
                        .Timetables
                        .SelectMany(x => x.Assignments)
                        .Count();
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
                        new ChartEntry(createdAssignments.Count() / 10)
                        {
                            ValueLabel = createdAssignments.Count().ToString(),
                            Color = SKColor.Parse("#512DA8"),
                            ValueLabelColor = SKColor.Parse("#512DA8"),
                            Label = "Created",
                        },
                        new ChartEntry(goingAssignments.Count() / 10)
                        {
                            ValueLabel = goingAssignments.Count().ToString(),
                            Color = SKColor.Parse("#C2185B"),
                            ValueLabelColor = SKColor.Parse("#C2185B"),
                            Label = "Going",
                        },
                        new ChartEntry(finishedAssignments.Count() / 10)
                        {
                            ValueLabel = finishedAssignments.Count().ToString(),
                            Color = SKColor.Parse("#00796B"),
                            ValueLabelColor = SKColor.Parse("#00796B"),
                            Label = "Finished",
                        },
                    };
                }
                case ChartType.Donut:
                {
                    return _tags.Select(x =>
                    {
                        int i = _tags.IndexOf(x);
                        if (i > _colors.Length - 1)
                        {
                            i = 0;
                        }
                        var color = _colors[i];
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
            _timer.Stop();
            _config = await _configManager.GetConfigAsync();
            _totalKinbens = _config.Kinbens;
            _weeklyTimetable = await _assignmentsManager.LoadWeeklyTimetableAsync(
                _calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday));
            _tags = await _tagRepository.GetAllItemsAsync();
            _radialEntries = GenerateEntries(ChartType.Radial);
            _donutEntries = GenerateEntries(ChartType.Donut);
            _timer.Start();
        }

        enum ChartType
        {
            Radial,
            Donut
        }
    }
}