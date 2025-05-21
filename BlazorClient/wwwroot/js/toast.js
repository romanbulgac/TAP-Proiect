/**
 * Toast notification system for Blazor client
 * Compatible with ToastService.cs
 */

// Toast container element
let toastContainer;

// Toast defaults
const DEFAULT_DURATION = 4000; // milliseconds

/**
 * Initialize the toast notification system
 * Should be called when the application starts
 */
window.initializeToasts = () => {
    // Get or create the toast container
    toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'fixed bottom-4 right-4 z-50 flex flex-col space-y-2';
        document.body.appendChild(toastContainer);
    }
    
    console.log('Toast notification system initialized');
};

/**
 * Create and display a toast notification
 * @param {string} type - The toast type (success, error, info, warning)
 * @param {string} message - The message to display
 * @param {string} title - Optional title for the toast
 * @param {number} duration - How long to show the toast (ms)
 */
function createToast(type, message, title = null, duration = DEFAULT_DURATION) {
    // Ensure the container exists
    if (!toastContainer) {
        initializeToasts();
    }
    
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `flex items-center w-full max-w-xs p-4 mb-4 text-gray-500 bg-white rounded-lg shadow-lg transition-all transform translate-x-0 opacity-100 ease-out duration-300`;
    toast.style.minWidth = '300px';
    
    // Set toast color based on type
    const iconColor = getIconColorClass(type);
    const borderColor = getBorderColorClass(type);
    
    toast.classList.add(borderColor);
    
    // Create toast content
    const icon = getIconForType(type);
    
    toast.innerHTML = `
        <div class="${iconColor} flex-shrink-0">
            ${icon}
        </div>
        <div class="ms-3 text-sm font-normal flex-1">
            ${title ? `<span class="font-semibold block text-gray-900">${title}</span>` : ''}
            ${message}
        </div>
        <button type="button" class="ms-auto -mx-1.5 -my-1.5 bg-white text-gray-400 hover:text-gray-900 rounded-lg p-1.5 hover:bg-gray-100 inline-flex items-center justify-center h-8 w-8">
            <svg class="w-3 h-3" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 14 14">
                <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M1 1l12 12M1 13L13 1"/>
            </svg>
        </button>
    `;
    
    // Add the toast to the container
    toastContainer.appendChild(toast);
    
    // Setup close button
    const closeButton = toast.querySelector('button');
    closeButton.addEventListener('click', () => {
        removeToast(toast);
    });
    
    // Auto dismiss after duration
    setTimeout(() => {
        removeToast(toast);
    }, duration);
}

/**
 * Remove a toast element with animation
 * @param {HTMLElement} toast - The toast element to remove
 */
function removeToast(toast) {
    toast.classList.add('opacity-0', 'translate-x-full');
    setTimeout(() => {
        if (toast && toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }
    }, 300);
}

/**
 * Get the icon HTML based on toast type
 * @param {string} type - The toast type
 * @returns {string} The SVG icon HTML
 */
function getIconForType(type) {
    switch(type) {
        case 'success':
            return `<svg class="w-5 h-5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 20 20">
                        <path d="M10 .5a9.5 9.5 0 1 0 9.5 9.5A9.51 9.51 0 0 0 10 .5Zm3.707 8.207-4 4a1 1 0 0 1-1.414 0l-2-2a1 1 0 0 1 1.414-1.414L9 10.586l3.293-3.293a1 1 0 0 1 1.414 1.414Z"/>
                    </svg>`;
        case 'error':
            return `<svg class="w-5 h-5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 20 20">
                        <path d="M10 .5a9.5 9.5 0 1 0 9.5 9.5A9.51 9.51 0 0 0 10 .5Zm2.5 13.5a1 1 0 1 1-2 0 1 1 0 0 1 2 0Zm0-5a1 1 0 0 1-2 0V6a1 1 0 0 1 2 0v3Z"/>
                    </svg>`;
        case 'warning':
            return `<svg class="w-5 h-5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 20 20">
                        <path d="M10 .5a9.5 9.5 0 1 0 9.5 9.5A9.51 9.51 0 0 0 10 .5ZM10 15a1 1 0 1 1 0-2 1 1 0 0 1 0 2Zm1-4a1 1 0 0 1-2 0V6a1 1 0 0 1 2 0v5Z"/>
                    </svg>`;
        case 'info':
        default:
            return `<svg class="w-5 h-5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 20 20">
                        <path d="M10 .5a9.5 9.5 0 1 0 9.5 9.5A9.51 9.51 0 0 0 10 .5ZM10 15a1 1 0 1 1 0-2 1 1 0 0 1 0 2Zm1-4a1 1 0 0 1-2 0V6a1 1 0 0 1 2 0v5Z"/>
                    </svg>`;
    }
}

/**
 * Get icon color class based on toast type
 * @param {string} type - The toast type
 * @returns {string} The Tailwind CSS color class
 */
function getIconColorClass(type) {
    switch(type) {
        case 'success': return 'text-green-500';
        case 'error': return 'text-red-500';
        case 'warning': return 'text-yellow-500';
        case 'info': 
        default: return 'text-blue-500';
    }
}

/**
 * Get border color class based on toast type
 * @param {string} type - The toast type
 * @returns {string} The Tailwind CSS border color class
 */
function getBorderColorClass(type) {
    switch(type) {
        case 'success': return 'border-l-4 border-green-500';
        case 'error': return 'border-l-4 border-red-500';
        case 'warning': return 'border-l-4 border-yellow-500';
        case 'info': 
        default: return 'border-l-4 border-blue-500';
    }
}

// Export toast methods globally
window.showToast = {
    success: (message, title) => createToast('success', message, title),
    error: (message, title) => createToast('error', message, title),
    info: (message, title) => createToast('info', message, title),
    warning: (message, title) => createToast('warning', message, title)
};
