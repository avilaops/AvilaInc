namespace Landing.Models;

public class FaqItem
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool IsOpen { get; set; } = false;
}
