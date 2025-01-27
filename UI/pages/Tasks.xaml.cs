using Business_layer_main;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using UI.user_controll;



namespace UI.pages
{
    public partial class Tasks : Page
    {
        public Tasks()
        {
            InitializeComponent();
            Loaded += Tasks_Loaded;
        }
        private void Tasks_Loaded(object sender, RoutedEventArgs e)
        {
            GetAllRecord();
            _ChanageDate();
            _ChangeCounterlab();
        }
        public  void GetAllRecord()
        {
            // Do not repeat the children if they exist
            stackPanel.Children.Clear();

            DataTable dt = clsBusinessLayer.GetAllRecord();

            foreach (DataRow row in dt.Rows)
            {
                Border taskCard = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(221, 221, 221)),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10),
                    Background = Brushes.White
                };

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Create the CheckBox (mode)
                CheckBox checkBox = new CheckBox
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    IsChecked = Convert.ToBoolean(row["mode"]) 
                };
                Grid.SetColumn(checkBox, 0);

                // Add checkbox changed event handler
                checkBox.Checked += (s, e) => UpdateTaskMode(taskCard, (CheckBox)s, true);
                checkBox.Unchecked += (s, e) => UpdateTaskMode(taskCard, (CheckBox)s, false);

                TextBlock titleBlock = new TextBlock
                {
                    Text = row["Title"].ToString(),
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51)),
                    Margin = new Thickness(10, 0, 0, 0)
                };
                Grid.SetColumn(titleBlock, 1);

                // Create the Delete Button
                Button deleteButton = new Button
                {
                    Content = "🗑", // Trash emoji
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136)),
                    Cursor = Cursors.Hand,
                    Tag = row["ID"] // Store the task ID in the button's Tag property
                };
                deleteButton.Click += DeleteTask; // Attach the Click event
                Grid.SetColumn(deleteButton, 2);

                // Add controls to the Grid
                grid.Children.Add(checkBox);
                grid.Children.Add(titleBlock);
                grid.Children.Add(deleteButton);

                // Add the Grid to the Border
                taskCard.Child = grid;

                // Store the record ID as a Tag for future use
                taskCard.Tag = row["ID"]; // Primary tag for record ID

                // Add click event for editing
                taskCard.MouseLeftButtonUp += (s, e) => 
                {
                    if (e.Source is CheckBox || e.Source is Button) return; // Don't trigger if clicking checkbox or delete button
                    EditTask(Convert.ToInt32(((Border)s).Tag));
                };
                taskCard.Cursor = Cursors.Hand;

                // Add the Border to the StackPanel
                stackPanel.Children.Add(taskCard);
            }
        }
        private void AddTaskBtn(object sender, RoutedEventArgs e)
        {
            var popup = new AddTaskPopup(this);
            popup.ShowDialog();                
        }
        public  void _ChanageDate()
        {

            lab_Day.Text = clsDate.GetDayAsString(DateTime.Now);
        }
        void _ChangeCounterlab ()
        {
            // Here i diclayer varible byte becouse it is not wise do open or connection with database manytime 
            byte counter = clsBusinessLayer.ReturnNumberOfRecordBYMode(0); 
            

            lab_taskcouner.Text = (counter > 0) ? $"YOU HAVE {counter} task Dont Complet yet " : "😀 No task "; 



        }
        public static void ReloadPage()
        {
            //var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            //if (currentWindow != null)
            //{
            //    var newWindow = (Window)Activator.CreateInstance(currentWindow.GetType());
            //    newWindow.Show();
            //    currentWindow.Close();
            //}

            
        }
        private void DeleteTask(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.Tag is int taskId)
            {
                // Call a method from your business layer to delete the record by its ID
                bool isDeleted = clsBusinessLayer.DeleteTaskById(taskId);

                if (isDeleted)
                {
                    MessageBox.Show("Task deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh the task list
                    GetAllRecord();
                    _ChangeCounterlab();
                }
                else
                {
                    MessageBox.Show("Failed to delete the task. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateTaskMode(Border taskCard, CheckBox checkBox, bool isChecked)
        {
            try
            {
                int taskId = Convert.ToInt32(taskCard.Tag);
                if (clsBusinessLayer.UpdateTaskMode(taskId))
                {
                    // Update the UI to reflect the task's completion status
                    var grid = taskCard.Child as Grid;
                    var titleBlock = grid.Children.OfType<TextBlock>().FirstOrDefault();
                    
                    if (isChecked)
                    {
                        // Task is completed
                        taskCard.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                        if (titleBlock != null)
                        {
                            titleBlock.TextDecorations = TextDecorations.Strikethrough;
                        }
                    }
                    else
                    {
                        // Task is not completed
                        taskCard.Background = new SolidColorBrush(Colors.White);
                        if (titleBlock != null)
                        {
                            titleBlock.TextDecorations = null;
                        }
                    }

                    ReloadPage();
                }
                else
                {
                    MessageBox.Show("Failed to update task status. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    checkBox.IsChecked = !isChecked; // Revert the checkbox
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating task status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                checkBox.IsChecked = !isChecked; // Revert the checkbox
            }
        }

        private void EditTask(int taskId)
        {
            var popup = new AddTaskPopup(this, taskId);
            popup.ShowDialog();
        }
    }

}
