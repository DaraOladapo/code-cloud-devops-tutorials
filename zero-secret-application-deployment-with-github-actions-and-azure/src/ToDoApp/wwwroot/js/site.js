// Enhanced Todo App JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize all interactive features
    initializeCheckboxes();
    initializeRippleEffects();
    initializeFormValidation();
    initializeToasts();
    
    // Auto-focus on quick add input
    const quickAddInput = document.querySelector('input[name="title"]');
    if (quickAddInput) {
        quickAddInput.focus();
    }
});

// Initialize custom checkboxes for task selection
function initializeCheckboxes() {
    const checkboxes = document.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function() {
            updateBulkActionButtons();
        });
    });
}

// Update bulk action buttons based on selection
function updateBulkActionButtons() {
    const selectedCheckboxes = document.querySelectorAll('input[name="selectedIds"]:checked');
    const bulkActionContainer = document.querySelector('.bulk-action-container');
    const selectedCount = document.querySelector('.selected-count');
    
    if (bulkActionContainer) {
        if (selectedCheckboxes.length > 0) {
            bulkActionContainer.style.display = 'block';
            if (selectedCount) {
                selectedCount.textContent = selectedCheckboxes.length;
            }
        } else {
            bulkActionContainer.style.display = 'none';
        }
    }
}

// Add ripple effect to buttons
function initializeRippleEffects() {
    const buttons = document.querySelectorAll('.btn-action, .btn-primary, .btn-success');
    
    buttons.forEach(button => {
        button.addEventListener('click', function(e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple-effect');
            
            this.appendChild(ripple);
            
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
}

// Enhanced form validation
function initializeFormValidation() {
    const forms = document.querySelectorAll('form');
    
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const requiredFields = this.querySelectorAll('input[required], textarea[required]');
            let isValid = true;
            
            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    field.classList.add('is-invalid');
                    isValid = false;
                } else {
                    field.classList.remove('is-invalid');
                }
            });
            
            if (!isValid) {
                e.preventDefault();
                showToast('Please fill in all required fields.', 'error');
            }
        });
    });
}

// Toast notification system
function initializeToasts() {
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            setTimeout(() => {
                alert.remove();
            }, 300);
        }, 5000);
    });
}

function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `alert alert-${type === 'error' ? 'danger' : type} alert-dismissible fade show`;
    toast.style.position = 'fixed';
    toast.style.top = '20px';
    toast.style.right = '20px';
    toast.style.zIndex = '9999';
    toast.style.minWidth = '300px';
    
    toast.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.remove();
    }, 5000);
}

// Smooth scrolling for anchor links
function smoothScrollTo(element) {
    element.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
    });
}

// Keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Ctrl/Cmd + Enter to submit quick add form
    if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
        const quickAddForm = document.querySelector('.quick-add-form');
        if (quickAddForm) {
            quickAddForm.submit();
        }
    }
    
    // Escape to clear selection
    if (e.key === 'Escape') {
        const selectedCheckboxes = document.querySelectorAll('input[name="selectedIds"]:checked');
        selectedCheckboxes.forEach(checkbox => {
            checkbox.checked = false;
        });
        updateBulkActionButtons();
    }
});

// Progress bar animation
function animateProgressBar() {
    const progressBars = document.querySelectorAll('.progress-bar');
    progressBars.forEach(bar => {
        const width = bar.style.width;
        bar.style.width = '0%';
        setTimeout(() => {
            bar.style.width = width;
        }, 100);
    });
}

// Call progress bar animation on page load
window.addEventListener('load', animateProgressBar);

// Enhanced date picker behavior
document.addEventListener('DOMContentLoaded', function() {
    const datePickers = document.querySelectorAll('input[type="datetime-local"]');
    datePickers.forEach(picker => {
        // Set minimum date to today
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        
        picker.min = `${year}-${month}-${day}T${hours}:${minutes}`;
    });
});
