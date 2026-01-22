namespace Landing.Models;

public class Testimonial
{
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = "/images/avatar-placeholder.png";
    public bool IsExample { get; set; } = true;
}
