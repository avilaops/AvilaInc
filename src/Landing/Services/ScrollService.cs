using Microsoft.JSInterop;

namespace Landing.Services;

public class ScrollService
{
    private readonly IJSRuntime _jsRuntime;

    public ScrollService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ScrollToIdAsync(string id)
    {
        await _jsRuntime.InvokeVoidAsync("scrollToId", id);
    }

    public async Task OpenNewTabAsync(string url)
    {
        await _jsRuntime.InvokeVoidAsync("openNewTab", url);
    }

    public async Task InitScrollRevealAsync()
    {
        await _jsRuntime.InvokeVoidAsync("initScrollReveal");
    }

    public async Task InitBackToTopAsync()
    {
        await _jsRuntime.InvokeVoidAsync("initBackToTop");
    }

    public async Task InitPhoneMaskAsync()
    {
        await _jsRuntime.InvokeVoidAsync("initPhoneMask");
    }

    public async Task<int> GetScrollYAsync()
    {
        return await _jsRuntime.InvokeAsync<int>("getScrollY");
    }

    public string BuildWhatsAppUrl(string phone, string message)
    {
        return $"https://wa.me/{phone}?text={Uri.EscapeDataString(message)}";
    }
}
