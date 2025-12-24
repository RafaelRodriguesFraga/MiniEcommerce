using DotnetBaseKit.Components.Shared.Notifications;

namespace ProductService.Application.DTOs;

public class ProductRequestDto : Notifiable<Notification>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}