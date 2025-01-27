using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UI.pages
{
    public partial class Timer : Page
    {
        private DispatcherTimer timer;
        private TimeSpan remainingTime;
        private const int TOTAL_MINUTES = 30;
        private bool isRunning = false;

        public Timer()
        {
            InitializeComponent();
            InitializeTimer();
            DrawProgressArc(1.0); // Full circle
        }

        private void InitializeTimer()
        {
            remainingTime = TimeSpan.FromMinutes(TOTAL_MINUTES);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            UpdateDisplay();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
            
            if (remainingTime.TotalSeconds <= 0)
            {
                TimerComplete();
            }
            else
            {
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            TimeDisplay.Text = $"{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
            double progress = remainingTime.TotalSeconds / (TOTAL_MINUTES * 60.0);
            DrawProgressArc(progress);
        }

        private void DrawProgressArc(double progress)
        {
            double radius = 90; // Slightly smaller than the background circle
            double centerX = 100;
            double centerY = 100;

            // Calculate start and sweep angles (in degrees)
            double startAngle = -90; // Start from top
            double sweepAngle = 360 * progress;

            // Convert to radians for calculations
            double startRad = startAngle * Math.PI / 180;
            double sweepRad = sweepAngle * Math.PI / 180;
            double endRad = startRad + sweepRad;

            // Calculate start and end points
            double startX = centerX + (radius * Math.Cos(startRad));
            double startY = centerY + (radius * Math.Sin(startRad));
            double endX = centerX + (radius * Math.Cos(endRad));
            double endY = centerY + (radius * Math.Sin(endRad));

            // Create the path geometry
            bool isLargeArc = sweepAngle > 180;
            
            string pathData = $"M {centerX},{centerY} " + // Move to center
                            $"L {startX},{startY} " + // Line to start point
                            $"A {radius},{radius} 0 {(isLargeArc ? 1 : 0)},1 {endX},{endY} " + // Arc to end point
                            "Z"; // Close path to center

            ProgressArc.Data = Geometry.Parse(pathData);
        }

        private void TimerComplete()
        {
            timer.Stop();
            isRunning = false;
            StatusText.Text = "Time's up!";
            StartButton.Content = "Start";
            
            // Play a notification sound
            System.Media.SystemSounds.Asterisk.Play();
            
            // Show completion message
            MessageBox.Show("Pomodoro session complete!", "Timer Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Reset the timer
            InitializeTimer();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                // Start the timer
                timer.Start();
                isRunning = true;
                StartButton.Content = "Pause";
                StatusText.Text = "Focus time!";
            }
            else
            {
                // Pause the timer
                timer.Stop();
                isRunning = false;
                StartButton.Content = "Resume";
                StatusText.Text = "Paused";
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            isRunning = false;
            StartButton.Content = "Start";
            StatusText.Text = "Ready to start";
            InitializeTimer();
        }
    }
}
