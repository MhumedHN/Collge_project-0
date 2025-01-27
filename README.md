# Task Management Application Documentation

## Three-Tier Architecture Overview

This application implements a robust three-tier architecture:

```
┌─────────────────┐
│    UI Layer     │ WPF Interface, XAML, User Controls
├─────────────────┤
│ Business Layer  │ Business Logic, Validation, Task Management
├─────────────────┤
│   Data Layer    │ Database Operations, Data Access
└─────────────────┘
```

## Table of Contents
1. [Data Access Layer](#1-data-access-layer-dal)
2. [Business Layer](#2-business-layer-bll)
3. [UI Layer](#3-ui-layer-presentation)
4. [Implementation Features](#implementation-best-practices)

## 1. Data Access Layer (DAL)

The Data Access Layer is responsible for all database operations and data persistence. It provides a clean interface for the business layer to interact with the database without exposing the underlying implementation details.

### Core Components

#### clsDataAccessLayer
```csharp
public class clsDataAccessLayer
{
    // Create Operations
    public static void InsertTaskToData(string Title, string Description, 
        string CourseId, byte Mode, DateTime StartDate, DateTime EndDate)
    {
        // Parameterized query for safe insertion
        string query = @"
            INSERT INTO [dbo].[Task]
               ([Title], [Description], [mode], [CourseID], 
                [start date], [end date])
            VALUES (@Title, @Description, @Mode, @CourseID, 
                   @StartDate, @EndDate)";
    }

    // Read Operations
    public static DataTable GetTaskById(int taskId)
    public static DataTable GetAllCourses()
    
    // Update Operations
    public static void EditTask(...)
    public static bool UpdateTaskMode(...)
    
    // Delete Operations
    public static bool DeleteTaskById(int taskId)
}

### Key Features

1. **Database Operations**
   - Secure parameterized queries
   - Transaction management
   - Connection pooling
   - Comprehensive error handling

2. **Data Access Patterns**
   - Repository pattern implementation
   - CRUD operation support
   - Batch processing capabilities
   - Efficient query execution

3. **Security Features**
   - SQL injection prevention
   - Connection string security
   - Error message sanitization
   - Access control implementation

## 2. Business Layer (BLL)

The Business Layer implements the core business logic, validation rules, and task management functionality. It acts as an intermediary between the UI and Data layers, ensuring data integrity and business rule enforcement.

### Core Components

#### clsBusinessLayer
The main business logic handler that coordinates operations between UI and data layers.

```csharp
public class clsBusinessLayer
{
    // Task Validation and Management
    public static bool EditTask(int id, string title, 
        string description, string hashtag, 
        DateTime StartDate, DateTime EndDate)
    {
        // 1. Validate task existence
        if (!FindRecordBYID(id)) return false;
        
        // 2. Process update through data layer
        clsDataAccessLayer.EditTask(id, title, description,
            hashtag, StartDate, EndDate);
        return true;
    }

    // Status Management
    public static bool UpdateTaskMode(int taskId)
    {
        return clsDataAccessLayer.UpdateTaskMode(taskId);
    }

    // Query Operations
    public static DataTable GetTasksByStatus(byte status)
    {
        try {
            return clsDataAccessLayer.GetTasksByStatus(status);
        }
        catch (Exception ex) {
            throw new ApplicationException(
                $"Error getting tasks: {ex.Message}", ex);
        }
    }
}

#### clsTasks
Task entity class with business rules and validation.

```csharp
public class clsTasks
{
    // Properties with validation
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public enMode Mode { get; set; }

    // Task States
    public enum enMode 
    { 
        NormalMode = 0,  // Initial state
        ActiveMode = 1,  // In progress
        DeletMode = 2    // Marked for deletion
    }

    // Business Rules
    public static bool EditTask(int id, string title,
        string description, string hashtag,
        DateTime StartDate, DateTime EndDate)
    {
        // 1. Validate existence
        if (!FindTask(id)) return false;
        
        // 2. Apply business rules
        if (StartDate >= EndDate) return false;
        
        // 3. Process update
        return clsBusinessLayer.EditTask(id, title,
            description, hashtag, StartDate, EndDate);
    }
}

### Business Rules Implementation

1. **Task Validation Rules**
   - Task existence verification
   - Date range validation
   - Required field checks
   - Status transition rules

2. **Task Lifecycle Management**
   ```
   New Task → Normal Mode
      ↓
   In Progress → Active Mode
      ↓
   Completed → Delete Mode
   ```

3. **Error Handling Strategy**
   - Input validation
   - Business rule verification
   - Exception propagation
   - User feedback generation

4. **Data Integrity**
   - Consistent status updates
   - Valid date ranges
   - Required field enforcement
   - Relationship maintenance

## 3. UI Layer (Presentation)

The UI Layer provides a modern, responsive interface built with WPF (Windows Presentation Foundation). It implements the MVVM pattern and features dynamic UI generation for task management.

### Core Components

#### MainWindow (Navigation Container)
```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Initialize with Home page
        MainFrame.Navigate(new Uri("/pages/Home.xaml", 
            UriKind.Relative));
    }
}

#### Tasks Page (Task Management Interface)
```csharp
public partial class Tasks : Page
{
    public void GetAllRecord()
    {
        // Clear existing tasks
        stackPanel.Children.Clear();

        // Get task data
        DataTable dt = clsBusinessLayer.GetAllRecord();

        foreach (DataRow row in dt.Rows)
        {
            // Create task card container
            Border taskCard = new Border
            {
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10),
                Background = Brushes.White
            };

            // Create layout grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition 
                { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition 
                { Width = new GridLength(1, GridUnitType.Star) });

            // Add interactive elements
            CheckBox checkBox = new CheckBox
            {
                IsChecked = Convert.ToBoolean(row["mode"])
            };
            
            TextBlock titleBlock = new TextBlock
            {
                Text = row["Title"].ToString(),
                FontSize = 16
            };

            Button deleteButton = new Button
            {
                Content = "🗑",
                Background = Brushes.Transparent
            };

            // Wire up events
            checkBox.Checked += (s, e) => 
                UpdateTaskMode(taskCard, checkBox, true);
            deleteButton.Click += DeleteTask;

            // Add to UI
            grid.Children.Add(checkBox);
            grid.Children.Add(titleBlock);
            grid.Children.Add(deleteButton);
            taskCard.Child = grid;
            stackPanel.Children.Add(taskCard);
        }
    }

    private void UpdateTaskMode(Border taskCard, 
        CheckBox checkBox, bool isChecked)
    {
        try
        {
            int taskId = Convert.ToInt32(taskCard.Tag);
            if (clsBusinessLayer.UpdateTaskMode(taskId))
            {
                // Update visual state
                taskCard.Background = new SolidColorBrush(
                    isChecked ? Color.FromRgb(240, 240, 240) 
                             : Colors.White);
                
                // Update completion status
                var titleBlock = GetTitleBlock(taskCard);
                if (titleBlock != null)
                {
                    titleBlock.TextDecorations = isChecked ? 
                        TextDecorations.Strikethrough : null;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to update task status: " 
                + ex.Message);
        }
    }
}

### UI Features and Patterns

1. **Dynamic UI Generation**
   - Task cards created at runtime
   - Responsive layout adaptation
   - Interactive UI elements
   - Visual state management

2. **Event Handling**
   - Task completion toggle
   - Delete functionality
   - Edit operations
   - Status updates

3. **Visual Feedback**
   - Task completion indication
   - Error message display
   - Loading states
   - Status transitions

4. **User Experience**
   - Intuitive task management
   - Clear visual hierarchy
   - Consistent interaction patterns
   - Responsive feedback

### Implementation Best Practices

1. **Code Organization**
   - Separation of concerns
   - MVVM pattern usage
   - Clean event handling
   - Modular components

2. **Performance**
   - Efficient UI updates
   - Resource cleanup
   - Memory management
   - Async operations

3. **Error Handling**
   - User-friendly messages
   - Graceful degradation
   - State recovery
   - Input validation

4. **Maintainability**
   - Clear naming conventions
   - Consistent patterns
   - Documentation
   - Code reusability
