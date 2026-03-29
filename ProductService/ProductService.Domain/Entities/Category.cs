
using System.Text;
using System.Text.RegularExpressions;
using DotnetBaseKit.Components.Domain.Sql.Entities.Base;

namespace ProductService.Domain.Entities;

public class Category : BaseEntity
{
    protected Category() { }

    public Category(string name)
    {
        Name = name;
        Slug = GenerateSlug(name);
        Active = true;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public bool Active { get; private set; } 
    public DateTime UpdatedAt { get; private set; }

    public ICollection<Product> Products { get; private set; } = new List<Product>();   

    public void Update(string name)
    {
        Name = name;
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