using Business_layer_main;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using UI.pages;

namespace UI
{
    public partial class AddTaskPopup : Window
    {
        private Tasks _tasksPage;
        private int? _editTaskId = null;

        public AddTaskPopup(Tasks tasksPage, int? taskId = null)
        {
            InitializeComponent();
            _tasksPage = tasksPage;
            _editTaskId = taskId;

            if (_editTaskId.HasValue)
            {
                LoadTaskData(_editTaskId.Value);
                btnAddTask.Content = "Update";
                this.Title = "Edit Task";
            }
        }

        private void LoadTaskData(int taskId)
        {
            DataTable taskData = clsBusinessLayer.GetTaskById(taskId);
            if (taskData != null && taskData.Rows.Count > 0)
            {
                DataRow row = taskData.Rows[0];
                TitleInput.Text = row["Title"].ToString();
                DescriptionInput.Text = row["Description"].ToString();

                // Set the hashtag based on CourseID
                if (row["CourseID"] != DBNull.Value)
                {
                    int courseId = Convert.ToInt32(row["CourseID"]);
                    foreach (var child in HashtagPanel.Children)
                    {
                        if (child is RadioButton radioButton && 
                            radioButton.Tag != null && 
                            Convert.ToInt32(radioButton.Tag) == courseId)
                        {
                            radioButton.IsChecked = true;
                            break;
                        }
                    }
                }

                // Set dates
                if (row["start date"] != DBNull.Value)
                    StartDateInput.SelectedDate = Convert.ToDateTime(row["start date"]);
                if (row["end date"] != DBNull.Value)
                    EndDateInput.SelectedDate = Convert.ToDateTime(row["end date"]);
            }
        }

        string _GetTitle()
        {
            return TitleInput.Text;
        }

        string _GetDesc()
        {
            return DescriptionInput.Text;
        }

        int? _GetCourseId()
        {
            foreach (var child in HashtagPanel.Children)
            {
                if (child is RadioButton radioButton && radioButton.IsChecked == true)
                {
                    return Convert.ToInt32(radioButton.Tag);
                }
            }
            return null;
        }

        bool _IsReminderChecked()
        {
            return ReminderCheckbox.IsChecked ?? false;
        }

        void _ClearAllInputs()
        {
            TitleInput.Text = string.Empty;
            DescriptionInput.Text = string.Empty;

            foreach (var child in HashtagPanel.Children)
            {
                if (child is RadioButton radioButton)
                {
                    radioButton.IsChecked = false;
                }
            }

            ReminderCheckbox.IsChecked = false;
            StartDateInput.SelectedDate = null;
            EndDateInput.SelectedDate = null;
        }

        void SaveTaskData()
        {
            DateTime start = StartDateInput.SelectedDate ?? DateTime.Now;
            DateTime end = StartDateInput.SelectedDate ?? start.AddDays(1);
            int? courseId = _GetCourseId();

            try
            {
                if (_editTaskId.HasValue)
                {
                    // Update existing task
                    clsBusinessLayer.EditTask(
                        _editTaskId.Value,
                        _GetTitle(),
                        _GetDesc(),
                        courseId.ToString(),
                        start,
                        end
                    );
                }
                else
                {
                    // Create new task
                    clsBusinessLayer.SendDataToDatabaseLaer(
                        _GetTitle(),
                        _GetDesc(),
                        courseId.ToString(),
                        0,  // mode = 0 for new tasks
                        start,
                        end
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void ReloadPage()
        {
            if (_tasksPage != null)
            {
                _tasksPage.GetAllRecord();
            }
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_GetTitle()))
            {
                MessageBox.Show("Please enter a title for the task.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_GetCourseId() == null)
            {
                MessageBox.Show("Please select a subject for the task.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveTaskData();
            ReloadPage();
            this.Close();
        }

        private void btnClosepopup_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
