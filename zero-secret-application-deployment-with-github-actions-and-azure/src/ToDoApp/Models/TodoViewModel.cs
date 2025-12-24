using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ToDoApp.Models;

public class TodoViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [Display(Name = "Completed")]
    public bool IsComplete { get; set; }
    
    [Required]
    [Display(Name = "Priority")]
    public Priority Priority { get; set; } = Priority.Medium;
    
    [Display(Name = "Due Date")]
    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }
    
    [Display(Name = "Category")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
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
