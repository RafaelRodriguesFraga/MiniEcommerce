using System.Text;
using System.Text.RegularExpressions;
using DotnetBaseKit.Components.Domain.Sql.Entities.Base;

namespace ProductService.Domain.Entities;

public class Product : BaseEntity
{
    protected Product()
    {
    }

    public Product(string name, string description, decimal price, string sku, string category, string imageUrl)
    {
        Name = name;
        Description = description;
        Price = price;
        Sku = sku.ToUpper();
        Category = category;
        ImageUrl = imageUrl;
        Active = true;

        Slug = name.ToLower().Replace(" ", "-");

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Sku { get; private set; }
    public string Slug { get; private set; }
    public string Category { get; private set; }
    public string ImageUrl { get; private set; }
    public decimal Price { get; private set; }
    public bool Active { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void Update(string name, string description, decimal price, string category, string imageUrl)
    {
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        ImageUrl = imageUrl;

        Slug = GenerateSlug(name);

        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() => Active = true;
    public void Deactivate() => Active = false;

    public override void Validate()
    {
    }

    private static string GenerateSlug(string name)
    {
        var normalized = name
            .ToLower()
            .Normalize(NormalizationForm.FormD);

        var slug = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");

        return slug.Trim('-');
    }
}