using System;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorClient.Services
{
    /// <summary>
    /// Scoped service for managing application-wide state
    /// </summary>
    public class AppState
    {
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authStateProvider;

        // UI state
        public bool IsLoading { get; private set; }
        public int UnreadNotificationsCount { get; private set; }
        public UserDto? CurrentUser { get; private set; }
        public string? Theme { get; private set; } = "system";  // system, dark, light
        
        // Events
        public event Action? OnChange;
        public event Action<int>? OnUnreadNotificationsCountChanged;
        public event Action<UserDto?>? OnUserChanged;
        public event Action<string>? OnThemeChanged;

        public AppState(
            NavigationManager navigationManager,
            AuthenticationStateProvider authStateProvider)
        {
            _navigationManager = navigationManager;
            _authStateProvider = authStateProvider;
            
            // Subscribe to auth state changes
            _authStateProvider.AuthenticationStateChanged += AuthStateChangedCallback;

            // Set the theme from localStorage (this could be moved to an initialize method)
            Theme = "system";  // Default, will be replaced by localStorage value when ThemeProvider loads
        }
        
        private async void AuthStateChangedCallback(Task<AuthenticationState> task)
        {
            var authState = await task;
            var isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
            
            if (!isAuthenticated)
            {
                CurrentUser = null;
            }
            
            // Let components know that the auth state changed
            NotifyStateChanged();
        }
        
        // Theme methods
        public void SetTheme(string theme)
        {
            Theme = theme;
            OnThemeChanged?.Invoke(theme);
            NotifyStateChanged();
        }
        
        // Loading indicator methods
        public void SetLoading(bool isLoading)
        {
            IsLoading = isLoading;
            NotifyStateChanged();
        }
        
        // Notification methods
        public void SetUnreadNotificationsCount(int count)
        {
            if (UnreadNotificationsCount != count)
            {
                UnreadNotificationsCount = count;
                OnUnreadNotificationsCountChanged?.Invoke(count);
                NotifyStateChanged();
            }
        }
        
        public void IncrementUnreadNotificationsCount()
        {
            UnreadNotificationsCount++;
            OnUnreadNotificationsCountChanged?.Invoke(UnreadNotificationsCount);
            NotifyStateChanged();
        }
        
        // User methods
        public void SetCurrentUser(UserDto? user)
        {
            CurrentUser = user;
            OnUserChanged?.Invoke(user);
            NotifyStateChanged();
        }
        
        // Helper method to notify subscribers
        private void NotifyStateChanged() => OnChange?.Invoke();
        
        // Cleanup when the service is disposed
        public void Dispose()
        {
            _authStateProvider.AuthenticationStateChanged -= AuthStateChangedCallback;
            
            // Clear all event handlers
            OnChange = null;
            OnUnreadNotificationsCountChanged = null;
            OnUserChanged = null;
            OnThemeChanged = null;
        }
    }
}
