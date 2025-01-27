using Business_layer_main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI.pages
{
    public partial class Charts : Page
    {
        private ObservableCollection<SubjectSeries> SubjectData { get; set; }
        private const int CHART_PADDING = 40;
        private const int POINT_RADIUS = 4;
        private const double MIN_SCORE = 0;
        private const double DEFAULT_SCORE = 50;

                //   Here we add dictionary for colour ,  and course difficlute


        private readonly Dictionary<int, string> SubjectColors = new Dictionary<int, string>
        {
            { 1, "#FF1976D2" },  // Math - Blue
            { 2, "#FFF44336" },  // Arabic - Red
            { 3, "#FF4CAF50" },  // Logic - Green
            { 4, "#FFFF9800" },  // C++ - Orange
            { 5, "#FF9C27B0" },  // Computer - Purple
            { 6, "#FF795548" }   // Democracy - Brown
        };

        private readonly Dictionary<int, string> SubjectNames = new Dictionary<int, string>
        {
            { 1, "Math" },
            { 2, "Arabic" },
            { 3, "Logic" },
            { 4, "C++" },
            { 5, "Computer" },
            { 6, "Democracy" }
        };

        


        private Dictionary<int, int> SubjectDifficulties;

        public Charts()
        {
            InitializeComponent();
            SubjectDifficulties = new Dictionary<int, int>();
            InitializeChart();
            LoadData();
        }

        private void LoadSubjectDifficulties()
        {
            try
            {
                var dt = clsBusinessLayer.GetAllCourses();
                foreach (DataRow row in dt.Rows)
                {
                    int courseId = Convert.ToInt32(row["ID"]);  //key
                    int difficulty = Convert.ToInt32(row["Difficulty"]); //value 
                    SubjectDifficulties[courseId] = difficulty;
                    // stor the Difficulties value for this id 
                    //Difficultie == id 

                }
            }                                   
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading course difficulties: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeChart()
        {
            try
            {
                SubjectData = new ObservableCollection<SubjectSeries>();
                LoadSubjectDifficulties();

                foreach (var subject in SubjectNames)
                {
                    SubjectData.Add(new SubjectSeries
                    {
                        SubjectId = subject.Key,
                        SubjectName = subject.Value,
                        Color = SubjectColors[subject.Key],
                        IsVisible = true,
                        Points = new List<TaskDataPoint>(),
                        Score = DEFAULT_SCORE // Start with a default score
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing chart: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                var tasks = clsBusinessLayer.GetAllRecord();
                var now = DateTime.Now;

                // Group tasks by subject and calculate scores
                foreach (var series in SubjectData)
                {
                    double currentScore = DEFAULT_SCORE; // Start with default score
                    series.Points.Clear(); // Clear existing points

                    // Add initial point
                    series.Points.Add(new TaskDataPoint
                    {
                        Date = now.AddMonths(-1), // Start from a month ago
                        Score = currentScore
                    });

                    var subjectTasks = tasks.AsEnumerable()
                        .Where(r => !Convert.IsDBNull(r["CourseID"]) && 
                                  Convert.ToInt32(r["CourseID"]) == series.SubjectId)
                        .OrderBy(r => Convert.ToDateTime(r["start date"]))
                        .ToList();

                    foreach (var task in subjectTasks)
                    {
                        var startDate = Convert.ToDateTime(task["start date"]);
                        var endDate = Convert.ToDateTime(task["end date"]);
                        var mode = Convert.ToByte(task["mode"]); // 0 for incomplete, 1 for complete

                        // Get difficulty, default to 1 if not found
                        int difficulty = 1;
                        if (SubjectDifficulties.ContainsKey(series.SubjectId))
                        {
                            difficulty = SubjectDifficulties[series.SubjectId];
                        }

                        // Calculate score change based on completion and deadline
                        if (mode == 1) // Task completed
                        {
                            currentScore += difficulty * 10;
                        }
                        else if (now > endDate) // Task overdue
                        {
                            currentScore = Math.Max(MIN_SCORE, currentScore - (difficulty * 5));
                        }

                        // Add points for this task
                        series.Points.Add(new TaskDataPoint
                        {
                            Date = startDate,
                            Score = currentScore
                        });

                        if (endDate != startDate)
                        {
                            series.Points.Add(new TaskDataPoint
                            {
                                Date = endDate,
                                Score = currentScore
                            });
                        }
                    }

                    // Add final point if needed
                    if (subjectTasks.Any())
                    {
                        series.Points.Add(new TaskDataPoint
                        {
                            Date = now,
                            Score = currentScore
                        });
                    }

                    series.Score = currentScore;
                }

                DrawChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DrawChart()
        {
            try
            {
                ChartCanvas.Children.Clear();

                var visibleSeries = SubjectData.Where(s => s.IsVisible && s.Points.Any()).ToList();
                if (!visibleSeries.Any())
                    return;

                // Find the date range and score range
                var allPoints = visibleSeries.SelectMany(s => s.Points).ToList();
                var minDate = allPoints.Min(p => p.Date);
                var maxDate = allPoints.Max(p => p.Date);
                var minScore = Math.Min(MIN_SCORE, allPoints.Min(p => p.Score));
                var maxScore = allPoints.Max(p => p.Score);

                // Ensure we have a valid range
                if (maxDate <= minDate || maxScore <= minScore)
                    return;

                // Add some padding to the score range
                maxScore += 10;
                if (minScore > 0) minScore = Math.Max(0, minScore - 10);

                // Calculate scaling factors
                double xScale = (ChartCanvas.ActualWidth - 2 * CHART_PADDING) / (maxDate - minDate).TotalDays;
                double yScale = (ChartCanvas.ActualHeight - 2 * CHART_PADDING) / (maxScore - minScore);

                // Draw axes
                DrawAxes(minDate, maxDate, minScore, maxScore);

                // Draw lines for each visible subject
                foreach (var series in visibleSeries)
                {
                    var color = (Color)ColorConverter.ConvertFromString(series.Color);
                    var brush = new SolidColorBrush(color);

                    for (int i = 0; i < series.Points.Count - 1; i++)
                    {
                        var p1 = series.Points[i];
                        var p2 = series.Points[i + 1];

                        double x1 = CHART_PADDING + (p1.Date - minDate).TotalDays * xScale;
                        double y1 = ChartCanvas.ActualHeight - (CHART_PADDING + (p1.Score - minScore) * yScale);
                        double x2 = CHART_PADDING + (p2.Date - minDate).TotalDays * xScale;
                        double y2 = ChartCanvas.ActualHeight - (CHART_PADDING + (p2.Score - minScore) * yScale);

                        // Draw line
                        var line = new Line
                        {
                            X1 = x1,
                            Y1 = y1,
                            X2 = x2,
                            Y2 = y2,
                            Stroke = brush,
                            StrokeThickness = 2
                        };
                        ChartCanvas.Children.Add(line);

                        // Draw point
                        var point = new Ellipse
                        {
                            Width = POINT_RADIUS * 2,
                            Height = POINT_RADIUS * 2,
                            Fill = brush
                        };
                        Canvas.SetLeft(point, x1 - POINT_RADIUS);
                        Canvas.SetTop(point, y1 - POINT_RADIUS);
                        ChartCanvas.Children.Add(point);
                    }

                    // Draw last point
                    if (series.Points.Any())
                    {
                        var lastPoint = series.Points.Last();
                        double x = CHART_PADDING + (lastPoint.Date - minDate).TotalDays * xScale;
                        double y = ChartCanvas.ActualHeight - (CHART_PADDING + (lastPoint.Score - minScore) * yScale);

                        var point = new Ellipse
                        {
                            Width = POINT_RADIUS * 2,
                            Height = POINT_RADIUS * 2,
                            Fill = brush
                        };
                        Canvas.SetLeft(point, x - POINT_RADIUS);
                        Canvas.SetTop(point, y - POINT_RADIUS);
                        ChartCanvas.Children.Add(point);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error drawing chart: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DrawAxes(DateTime minDate, DateTime maxDate, double minScore, double maxScore)
        {
            try
            {
                // Draw axes lines
                var axisColor = new SolidColorBrush(Colors.Gray);
                
                // X-axis
                var xAxis = new Line
                {
                    X1 = CHART_PADDING,
                    Y1 = ChartCanvas.ActualHeight - CHART_PADDING,
                    X2 = ChartCanvas.ActualWidth - CHART_PADDING,
                    Y2 = ChartCanvas.ActualHeight - CHART_PADDING,
                    Stroke = axisColor,
                    StrokeThickness = 1
                };
                ChartCanvas.Children.Add(xAxis);

                // Y-axis
                var yAxis = new Line
                {
                    X1 = CHART_PADDING,
                    Y1 = CHART_PADDING,
                    X2 = CHART_PADDING,
                    Y2 = ChartCanvas.ActualHeight - CHART_PADDING,
                    Stroke = axisColor,
                    StrokeThickness = 1
                };
                ChartCanvas.Children.Add(yAxis);

                // Add date labels
                var dateRange = (maxDate - minDate).TotalDays;
                var dateStep = Math.Max(1, Math.Ceiling(dateRange / 5)); // Show at most 5 date labels

                for (var date = minDate; date <= maxDate; date = date.AddDays(dateStep))
                {
                    var x = CHART_PADDING + (date - minDate).TotalDays * 
                        (ChartCanvas.ActualWidth - 2 * CHART_PADDING) / dateRange;

                    var label = new TextBlock
                    {
                        Text = date.ToShortDateString(),
                        FontSize = 10
                    };
                    Canvas.SetLeft(label, x - label.FontSize * 2);
                    Canvas.SetTop(label, ChartCanvas.ActualHeight - CHART_PADDING + 5);
                    ChartCanvas.Children.Add(label);
                }

                // Add score labels
                var scoreRange = maxScore - minScore;
                var scoreStep = Math.Max(1, Math.Ceiling(scoreRange / 5)); // Show at most 5 score labels

                for (var score = Math.Floor(minScore); score <= Math.Ceiling(maxScore); score += scoreStep)
                {
                    var y = ChartCanvas.ActualHeight - (CHART_PADDING + (score - minScore) * 
                        (ChartCanvas.ActualHeight - 2 * CHART_PADDING) / scoreRange);

                    var label = new TextBlock
                    {
                        Text = score.ToString("F0"),
                        FontSize = 10
                    };
                    Canvas.SetLeft(label, 5);
                    Canvas.SetTop(label, y - label.FontSize / 2);
                    ChartCanvas.Children.Add(label);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error drawing axes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is CheckBox button && button.Tag != null)
                {
                    int subjectId = Convert.ToInt32(button.Tag);
                    var series = SubjectData.FirstOrDefault(s => s.SubjectId == subjectId);
                    if (series != null)
                    {
                        series.IsVisible = button.IsChecked ?? false;
                        DrawChart();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling subject visibility: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class TaskDataPoint
    {
        public DateTime Date { get; set; }
        public double Score { get; set; }
    }

    public class SubjectSeries
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Color { get; set; }
        public bool IsVisible { get; set; }
        public List<TaskDataPoint> Points { get; set; }
        public double Score { get; set; }
    }
}

    