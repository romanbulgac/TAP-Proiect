window.showToast = (title, message, type = 'info', duration = 5000) => {
    // Create toast container if it doesn't exist
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'fixed top-4 right-4 z-50 flex flex-col space-y-2';
        document.body.appendChild(toastContainer);
    }
    
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `max-w-md w-full bg-white shadow-lg rounded-lg pointer-events-auto flex ring-1 ring-black ring-opacity-5 transform transition-all duration-300 opacity-0 translate-y-2`;
    
    // Set toast color based on type
    let iconSvg = '';
    let bgColor = '';
    
    switch(type) {
        case 'success':
            bgColor = 'bg-green-50';
            iconSvg = `<svg class="h-6 w-6 text-green-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>`;
            break;
        case 'error':
            bgColor = 'bg-red-50';
            iconSvg = `<svg class="h-6 w-6 text-red-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>`;
            break;
        case 'warning':
            bgColor = 'bg-yellow-50';
            iconSvg = `<svg class="h-6 w-6 text-yellow-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>`;
            break;
        default:
            bgColor = 'bg-blue-50';
            iconSvg = `<svg class="h-6 w-6 text-blue-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>`;
    }
    
    // Set toast content
    toast.innerHTML = `
        <div class="p-4 ${bgColor} rounded-lg w-full flex">
            <div class="flex-shrink-0">
                ${iconSvg}
            </div>
            <div class="ml-3 w-0 flex-1 pt-0.5">
                <p class="text-sm font-medium text-gray-900">${title}</p>
                <p class="mt-1 text-sm text-gray-500">${message}</p>
            </div>
            <div class="ml-4 flex-shrink-0 flex">
                <button class="bg-transparent rounded-md inline-flex text-gray-400 hover:text-gray-500 focus:outline-none">
                    <span class="sr-only">Close</span>
                    <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </button>
            </div>
        </div>
    `;
    
    // Add toast to container
    toastContainer.appendChild(toast);
    
    // Close button functionality
    const closeButton = toast.querySelector('button');
    closeButton.addEventListener('click', () => removeToast(toast));
    
    // Show toast with animation
    setTimeout(() => {
        toast.classList.remove('opacity-0', 'translate-y-2');
        toast.classList.add('opacity-100', 'translate-y-0');
    }, 10);
    
    // Auto-remove after duration
    const timeoutId = setTimeout(() => removeToast(toast), duration);
    
    // Helper function to remove toast
    function removeToast(toast) {
        toast.classList.remove('opacity-100', 'translate-y-0');
        toast.classList.add('opacity-0', 'translate-y-2');
        
        setTimeout(() => {
            toast.remove();
            
            // If this was the last toast, remove the container as well
            if (toastContainer.children.length === 0) {
                toastContainer.remove();
            }
        }, 300);
        
        clearTimeout(timeoutId);
    }
    
    // Return toast element in case caller wants to manipulate it further
    return toast;
};
