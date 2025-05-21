using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorClient.Services
{
    public class ToastService
    {
        private readonly IJSRuntime _jsRuntime;

        public ToastService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task ShowSuccessAsync(string message, string title = "Success")
        {
            await _jsRuntime.InvokeVoidAsync("showToast.success", message, title);
        }

        public async Task ShowErrorAsync(string message, string title = "Error")
        {
            await _jsRuntime.InvokeVoidAsync("showToast.error", message, title);
        }

        public async Task ShowInfoAsync(string message, string title = "Information")
        {
            await _jsRuntime.InvokeVoidAsync("showToast.info", message, title);
        }

        public async Task ShowWarningAsync(string message, string title = "Warning")
        {
            await _jsRuntime.InvokeVoidAsync("showToast.warning", message, title);
        }
    }
}
