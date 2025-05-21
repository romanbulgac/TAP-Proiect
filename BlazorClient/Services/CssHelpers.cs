using System;

namespace BlazorClient.Services
{
    public static class CssHelpers
    {
        /// <summary>
        /// Gets the CSS classes for a navigation link
        /// </summary>
        /// <param name="isActive">Whether the link is currently active</param>
        /// <returns>CSS classes string</returns>
        public static string GetNavLinkClasses(bool isActive)
        {
            return isActive 
                ? "flex items-center p-2 text-notion-text-light dark:text-notion-text-dark rounded-md bg-notion-hover-light dark:bg-notion-hover-dark transition-colors"
                : "flex items-center p-2 text-notion-text-secondary-light dark:text-notion-text-secondary-dark hover:text-notion-text-light dark:hover:text-notion-text-dark rounded-md hover:bg-notion-hover-light dark:hover:bg-notion-hover-dark transition-colors";
        }

        /// <summary>
        /// Gets the CSS classes for a button
        /// </summary>
        /// <param name="isPrimary">Whether the button is primary</param>
        /// <returns>CSS classes string</returns>
        public static string GetButtonClasses(bool isPrimary = true)
        {
            return isPrimary 
                ? "inline-flex items-center justify-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                : "inline-flex items-center justify-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-notion-text-light dark:text-notion-text-dark bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500";
        }

        /// <summary>
        /// Gets the CSS classes for a form input
        /// </summary>
        /// <returns>CSS classes string</returns>
        public static string GetInputClasses()
        {
            return "w-full rounded-md border border-notion-border-light dark:border-notion-border-dark py-2 px-3 text-notion-text-light dark:text-notion-text-dark bg-white dark:bg-gray-800 placeholder-gray-400 shadow-sm focus:outline-none focus:border-blue-500 focus:ring-blue-500";
        }

        /// <summary>
        /// Gets the CSS classes for a card container
        /// </summary>
        /// <returns>CSS classes string</returns>
        public static string GetCardClasses()
        {
            return "bg-notion-card-light dark:bg-notion-card-dark rounded-lg shadow-md p-6 mb-4";
        }
    }
}
