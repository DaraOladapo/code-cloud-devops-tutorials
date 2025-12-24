using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Controllers;

[Route("/")]
public class TodoController : Controller
{
    private readonly TodoContext _context;
    private readonly ILogger<TodoController> _logger;
    
    // Constants for TempData keys
    private const string SuccessMessageKey = "SuccessMessage";
    private const string ErrorMessageKey = "ErrorMessage";
    
    // Constants for common messages
    private const string TaskNotFoundMessage = "Task not found.";
    private const string TaskCreatedMessage = "Task created successfully!";
    private const string TaskUpdatedMessage = "Task updated successfully!";
    private const string TaskDeletedMessage = "Task deleted successfully!";
    private const string TaskAddedMessage = "Task added successfully!";
    
    public TodoController(TodoContext context, ILogger<TodoController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? filterCategory, Priority? filterPriority, bool? filterCompleted, string? searchTerm)
    {
        try
        {
            var query = _context.TodoItems.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filterCategory))
            {
                query = query.Where(t => t.Category == filterCategory);
            }

            if (filterPriority.HasValue)
            {
                query = query.Where(t => t.Priority == filterPriority);
            }

            if (filterCompleted.HasValue)
            {
                query = query.Where(t => t.IsComplete == filterCompleted);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.Title.Contains(searchTerm) || 
                                       (t.Description != null && t.Description.Contains(searchTerm)));
            }

            var items = await query
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            var viewModel = new TodoListViewModel
            {
                TodoItems = items,
                FilterCategory = filterCategory,
                FilterPriority = filterPriority,
                FilterCompleted = filterCompleted,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todo items");
            ModelState.AddModelError("", "An error occurred while loading your tasks. Please try again.");
            return View(new TodoListViewModel());
        }
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new CreateTodoItemViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTodoItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var todoItem = new TodoItem
            {
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                DueDate = model.DueDate,
                Category = model.Category,
                CreatedAt = DateTime.UtcNow
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            TempData[SuccessMessageKey] = TaskCreatedMessage;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo item");
            ModelState.AddModelError("", "An error occurred while creating the task. Please try again.");
            return View(model);
        }
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item == null)
            {
                TempData[ErrorMessageKey] = TaskNotFoundMessage;
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new EditTodoItemViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Priority = item.Priority,
                DueDate = item.DueDate,
                Category = item.Category,
                IsComplete = item.IsComplete,
                CreatedAt = item.CreatedAt,
                CompletedAt = item.CompletedAt
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todo item with id {Id}", id);
            TempData[ErrorMessageKey] = "An error occurred while loading the task.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditTodoItemViewModel model)
    {
        if (id != model.Id)
        {
            TempData[ErrorMessageKey] = "Invalid task ID.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var existingItem = await _context.TodoItems.FindAsync(id);
            if (existingItem == null)
            {
                TempData[ErrorMessageKey] = TaskNotFoundMessage;
                return RedirectToAction(nameof(Index));
            }

            // Update properties
            existingItem.Title = model.Title;
            existingItem.Description = model.Description;
            existingItem.Priority = model.Priority;
            existingItem.DueDate = model.DueDate;
            existingItem.Category = model.Category;

            // Handle completion status change
            if (model.IsComplete && !existingItem.IsComplete)
            {
                existingItem.IsComplete = true;
                existingItem.CompletedAt = DateTime.UtcNow;
            }
            else if (!model.IsComplete && existingItem.IsComplete)
            {
                existingItem.IsComplete = false;
                existingItem.CompletedAt = null;
            }

            _context.Update(existingItem);
            await _context.SaveChangesAsync();

            TempData[SuccessMessageKey] = TaskUpdatedMessage;
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error updating todo item with id {Id}", id);
            ModelState.AddModelError("", "The task was modified by another user. Please refresh and try again.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo item with id {Id}", id);
            ModelState.AddModelError("", "An error occurred while updating the task. Please try again.");
            return View(model);
        }
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item == null)
            {
                TempData[ErrorMessageKey] = TaskNotFoundMessage;
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todo item details with id {Id}", id);
            TempData[ErrorMessageKey] = "An error occurred while loading the task details.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("add")]
    [ActionName("Add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string title, string? category)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            TempData[ErrorMessageKey] = "Task title is required.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var todoItem = new TodoItem
            {
                Title = title.Trim(),
                Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            TempData[SuccessMessageKey] = TaskAddedMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding quick todo item");
            TempData[ErrorMessageKey] = "An error occurred while adding the task.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("complete/{id}")]
    [ActionName("Complete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item != null) 
            { 
                item.IsComplete = !item.IsComplete; // Toggle completion
                item.CompletedAt = item.IsComplete ? DateTime.UtcNow : null;
                
                _context.Update(item);
                await _context.SaveChangesAsync();
                
                TempData[SuccessMessageKey] = item.IsComplete 
                    ? "Task marked as complete!" 
                    : "Task marked as pending!";
            }
            else
            {
                TempData[ErrorMessageKey] = TaskNotFoundMessage;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling completion for todo item with id {Id}", id);
            TempData[ErrorMessageKey] = "An error occurred while updating the task.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id}")]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item != null) 
            { 
                _context.TodoItems.Remove(item); 
                await _context.SaveChangesAsync();
                TempData[SuccessMessageKey] = TaskDeletedMessage;
            }
            else
            {
                TempData[ErrorMessageKey] = TaskNotFoundMessage;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo item with id {Id}", id);
            TempData[ErrorMessageKey] = "An error occurred while deleting the task.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("bulk-action")]
    [ActionName("BulkAction")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkAction(string action, int[] selectedIds)
    {
        if (selectedIds == null || selectedIds.Length == 0)
        {
            TempData[ErrorMessageKey] = "No tasks selected.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var items = await _context.TodoItems
                .Where(t => selectedIds.Contains(t.Id))
                .ToListAsync();

            switch (action.ToLower())
            {
                case "complete":
                    foreach (var item in items.Where(i => !i.IsComplete))
                    {
                        item.IsComplete = true;
                        item.CompletedAt = DateTime.UtcNow;
                    }
                    TempData[SuccessMessageKey] = $"{items.Count(i => !i.IsComplete)} tasks marked as complete!";
                    break;

                case "incomplete":
                    foreach (var item in items.Where(i => i.IsComplete))
                    {
                        item.IsComplete = false;
                        item.CompletedAt = null;
                    }
                    TempData[SuccessMessageKey] = $"{items.Count(i => i.IsComplete)} tasks marked as pending!";
                    break;

                case "delete":
                    _context.TodoItems.RemoveRange(items);
                    TempData[SuccessMessageKey] = $"{items.Count} tasks deleted!";
                    break;

                default:
                    TempData[ErrorMessageKey] = "Invalid action.";
                    return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk action {Action} on items {Items}", action, selectedIds);
            TempData[ErrorMessageKey] = "An error occurred while performing the bulk action.";
        }

        return RedirectToAction(nameof(Index));
    }
}
