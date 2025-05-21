window.showToast = {
    success: function (message, title) {
        toastr.success(message, title);
    },
    error: function (message, title) {
        toastr.error(message, title);
    },
    info: function (message, title) {
        toastr.info(message, title);
    },
    warning: function (message, title) {
        toastr.warning(message, title);
    }
};

// Configure toastr
toastr.options = {
    closeButton: true,
    progressBar: true,
    positionClass: "toast-top-right",
    timeOut: 5000
};

window.tokenStorage = {
    getAccessToken: function() {
        return localStorage.getItem('accessToken');
    },
    setAccessToken: function(token) {
        localStorage.setItem('accessToken', token);
    },
    removeAccessToken: function() {
        localStorage.removeItem('accessToken');
    },
    getRefreshToken: function() {
        return localStorage.getItem('refreshToken');
    },
    setRefreshToken: function(token) {
        localStorage.setItem('refreshToken', token);
    },
    removeRefreshToken: function() {
        localStorage.removeItem('refreshToken');
    },
    clearTokens: function() {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
    }
};

window.setupClickOutsideListener = function(dotNetHelper) {
    // Use a delayed setup to avoid immediate triggering if the menu was just opened
    setTimeout(() => {
        const handleClickOutside = function(event) {
            // Check if click is outside the profile menu toggle button AND the profile menu itself
            const profileMenuToggle = document.querySelector('[data-profile-menu-toggle]');
            const profileMenu = document.querySelector('.profile-menu');

            let clickedOutside = true;
            if (profileMenuToggle && profileMenuToggle.contains(event.target)) {
                clickedOutside = false;
            }
            if (profileMenu && profileMenu.contains(event.target)) {
                clickedOutside = false;
            }
            
            if (clickedOutside) {
                // Ensure dotNetHelper and its methods are still valid
                try {
                    dotNetHelper.invokeMethodAsync('CloseProfileMenu');
                } catch (e) {
                    console.warn("DotNet helper for CloseProfileMenu no longer valid.", e);
                    // Optionally, remove the listener if the helper is invalid
                    if (window._clickOutsideProfileMenuHandler) {
                        document.removeEventListener('click', window._clickOutsideProfileMenuHandler);
                        window._clickOutsideProfileMenuHandler = null;
                    }
                }
            }
        };
        
        // Remove any existing handlers first to avoid duplicates
        if (window._clickOutsideProfileMenuHandler) {
            document.removeEventListener('click', window._clickOutsideProfileMenuHandler);
        }
        
        // Set up the new handler
        window._clickOutsideProfileMenuHandler = handleClickOutside;
        document.addEventListener('click', handleClickOutside);
    }, 50); // Small delay
};

window.removeClickOutsideListener = function() {
    if (window._clickOutsideProfileMenuHandler) {
        document.removeEventListener('click', window._clickOutsideProfileMenuHandler);
        window._clickOutsideProfileMenuHandler = null;
    }
};
