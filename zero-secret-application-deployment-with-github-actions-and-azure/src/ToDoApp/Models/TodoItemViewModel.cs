using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ToDoApp.Models;

public class CreateTodoItemViewModel
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    [Display(Name = "Task Title")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [Display(Name = "Priority")]
    public Priority Priority { get; set; } = Priority.Medium;
    
    [Display(Name = "Due Date")]
    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }
    
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    [Display(Name = "Category")]
    public string? Category { get; set; }
    
    // Helper method for Priority dropdown
    public static List<SelectListItem> GetPriorityOptions()
    {
        return new List<SelectListItem>
        {
            new() { Value = "1", Text = "Low" },
            new() { Value = "2", Text = "Medium" },
            new() { Value = "3", Text = "High" },
            new() { Value = "4", Text = "Critical" }
        };
    }
    
    // Helper method for common categories
    public static List<SelectListItem> GetCategoryOptions()
    {
        return new List<SelectListItem>
        {
            new() { Value = "", Text = "-- Select Category --" },
            new() { Value = "Work", Text = "Work" },
            new() { Value = "Personal", Text = "Personal" },
            new() { Value = "Shopping", Text = "Shopping" },
            new() { Value = "Health", Text = "Health" },
            new() { Value = "Finance", Text = "Finance" },
            new() { Value = "Education", Text = "Education" },
            new() { Value = "Other", Text = "Other" }
        };
    }
}

public class EditTodoItemViewModel : CreateTodoItemViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Mark as Complete")]
    public bool IsComplete { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
}

public class TodoListViewModel
{
    public List<TodoItem> TodoItems { get; set; } = new();
    public string? FilterCategory { get; set; }
    public Priority? FilterPriority { get; set; }
    public bool? FilterCompleted { get; set; }
    public string? SearchTerm { get; set; }
    
    // Quick add properties
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? QuickAddTitle { get; set; }
    
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? QuickAddCategory { get; set; }
    
    // Statistics
    public int TotalTasks => TodoItems.Count;
    public int CompletedTasks => TodoItems.Count(x => x.IsComplete);
    public int PendingTasks => TotalTasks - CompletedTasks;
    public int OverdueTasks => TodoItems.Count(x => !x.IsComplete && x.DueDate.HasValue && x.DueDate.Value.Date < DateTime.Now.Date);
    public double CompletionRate => TotalTasks > 0 ? Math.Round((double)CompletedTasks / TotalTasks * 100, 1) : 0;
    
    // Get overdue tasks for debugging
    public List<TodoItem> GetOverdueTasks() => TodoItems
        .Where(x => !x.IsComplete && x.DueDate.HasValue && x.DueDate.Value.Date < DateTime.Now.Date)
        .ToList();
    
    // Categories
    public List<string> GetCategories() => TodoItems
        .Where(x => !string.IsNullOrEmpty(x.Category))
        .Select(x => x.Category!)
        .Distinct()
        .OrderBy(x => x)
        .ToList();
        
    // Helper methods for dropdown options
    public static List<SelectListItem> GetPriorityFilterOptions()
    {
        return new List<SelectListItem>
        {
            new() { Value = "", Text = "All Priorities" },
            new() { Value = "1", Text = "Low" },
            new() { Value = "2", Text = "Medium" },
            new() { Value = "3", Text = "High" },
            new() { Value = "4", Text = "Critical" }
        };
    }
    
    public static List<SelectListItem> GetCompletedFilterOptions()
    {
        return new List<SelectListItem>
        {
            new() { Value = "", Text = "All Tasks" },
            new() { Value = "false", Text = "Pending" },
            new() { Value = "true", Text = "Completed" }
        };
    }
}
