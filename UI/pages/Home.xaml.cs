using Business_layer_main;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI.pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private List<TaskItem> todayTasks;

        public Home()
        {
            InitializeComponent();
            LoadTodayTasks();
            StartAutoRefresh();
        }

        private void StartAutoRefresh()
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1); // Refresh every minute
            timer.Tick += (s, e) => LoadTodayTasks();
            timer.Start();
        }

        private void LoadTodayTasks()
        {
            try
            {
                var tasks = clsBusinessLayer.GetAllRecord();
                todayTasks = new List<TaskItem>();
                int totalTasks = 0;
                int completedTasks = 0;

                foreach (DataRow row in tasks.Rows)
                {
                    var startDate = Convert.ToDateTime(row["start date"]);
                    if (startDate.DayOfWeek == DateTime.Now.DayOfWeek)
                    {
                        totalTasks++;
                        var isCompleted = Convert.ToInt32(row["mode"]) == 1;
                        if (isCompleted) completedTasks++;

                        todayTasks.Add(new TaskItem
                        {
                            Id = Convert.ToInt32(row["ID"]),
                            Title = Convert.ToString(row["Title"]),
                            Description = Convert.ToString(row["Description"]),
                            StartDate = startDate,
                            EndDate = Convert.ToDateTime(row["end date"]),
                            IsCompleted = isCompleted,
                            StatusColor = isCompleted ? "#4CAF50" : "#FFC107" // Green for completed, Yellow for pending
                        });
                    }
                }

                // Update UI
                TodayTasksList.ItemsSource = todayTasks;
                UpdateProgressCircle(totalTasks > 0 ? (double)completedTasks / totalTasks : 0);
                UpdateTaskStats(totalTasks, completedTasks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateProgressCircle(double percentage)
        {
            // Calculate the arc path for the progress circle
            double radius = 92.5; // Accounting for stroke width
            double angle = percentage * 360;
            double startAngle = -90; // Start from top
            double endAngle = startAngle + angle;

            // Convert angles to radians
            double startRad = startAngle * Math.PI / 180;
            double endRad = endAngle * Math.PI / 180;

            // Calculate start and end points
            double centerX = 100;
            double centerY = 100;
            
            double startX = centerX + (radius * Math.Cos(startRad));
            double startY = centerY + (radius * Math.Sin(startRad));
            double endX = centerX + (radius * Math.Cos(endRad));
            double endY = centerY + (radius * Math.Sin(endRad));

            // Create the arc path
            bool isLargeArc = angle > 180;
            var pathData = $"M {startX},{startY} A {radius},{radius} 0 {(isLargeArc ? 1 : 0)},1 {endX},{endY}";
            
            ProgressArc.Data = Geometry.Parse(pathData);
            ProgressText.Text = $"{(percentage * 100):F0}%";
        }

        private void UpdateTaskStats(int totalTasks, int completedTasks)
        {
            TaskCountText.Text = $"Total Tasks: {totalTasks}";
            CompletedTasksText.Text = $"Completed: {completedTasks}";
        }

        private void TaskItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is TaskItem task)
            {
                // Navigate to Tasks page
                NavigationService?.Navigate(new Uri("/pages/Tasks.xaml", UriKind.Relative));
            }
        }
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCompleted { get; set; }
        public string StatusColor { get; set; }
    }
}
