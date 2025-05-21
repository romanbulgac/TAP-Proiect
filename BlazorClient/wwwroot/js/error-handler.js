// Global error handler for JavaScript errors
window.addEventListener('error', function (event) {
    console.error('Global error handler caught an error:', event.error);
    // Prevent the browser from showing its own error dialog
    event.preventDefault();
    
    if (window.toastr) {
        toastr.error('An error occurred in the application. See console for details.', 'Application Error');
    }
    
    return true; // Prevent default browser error handling
});

// Handling unhandled promise rejections
window.addEventListener('unhandledrejection', function (event) {
    console.error('Unhandled Promise Rejection:', event.reason);
    
    if (window.toastr) {
        toastr.error('An unhandled promise rejection occurred. See console for details.', 'Application Error');
    }
    
    event.preventDefault();
});

// Log and report Blazor errors specifically
window.blazorErrorHandler = {
    reportError: function (message, stack) {
        console.error('Blazor Error:', message);
        console.error('Stack:', stack);
        
        if (window.toastr) {
            toastr.error(message, 'Blazor Error');
        }
    }
};
