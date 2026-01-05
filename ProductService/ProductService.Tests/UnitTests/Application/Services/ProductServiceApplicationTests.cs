using DotnetBaseKit.Components.Shared.Notifications;
using Moq;
using ProductService.Application.DTOs;
using ProductService.Application.Services;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using Xunit;

namespace ProductService.Tests.UnitTests.Application.Services;

public class ProductServiceApplicationTests
{
    private readonly Mock<IProductReadRepository> _readRepositoryMock;
    private readonly Mock<IProductWriteRepository> _writeRepositoryMock;
    private readonly NotificationContext _notificationContext;
    private readonly ProductServiceApplication _service;

    public ProductServiceApplicationTests()
    {
        _readRepositoryMock = new Mock<IProductReadRepository>();
        _writeRepositoryMock = new Mock<IProductWriteRepository>();
        _notificationContext = new NotificationContext();

        _service = new ProductServiceApplication(
            _notificationContext,
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object
        );
    }

    [Fact(DisplayName = "GetByIdAsync should return dto when found")]
    public async Task GetByIdAsync_Should_Return_Dto_When_Found()
    {
        
        var id = Guid.NewGuid();
        var product = new Product("Name", "Desc", 10, "SKU1", "Cat", "img.png");
        
        _readRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(product.Name, result.Name);
    }

    [Fact(DisplayName = "GetByIdAsync should return null when not found")]
    public async Task GetByIdAsync_Should_Return_Null_When_NotFound()
    {
        _readRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }


    [Fact(DisplayName = "GetBySlugAsync should return dto when found")]
    public async Task GetBySlugAsync_Should_Return_Dto_When_Found()
    {
        var slug = "test-product";
        var product = new Product("Test Product", "Desc", 10, "SKU1", "Cat", "img.png");
        
        _readRepositoryMock.Setup(r => r.GetBySlugAsync(slug)).ReturnsAsync(product);

        var result = await _service.GetBySlugAsync(slug);

        Assert.NotNull(result);
        Assert.Equal(product.Name, result.Name);
    }

    [Fact(DisplayName = "CreateAsync should add notification when DTO is invalid")]
    public async Task CreateAsync_Should_Add_Notification_When_Dto_Invalid()
    {
     
        var dto = new ProductRequestDto(); 
      
        dto.AddNotification("Error", "Manual Error to simulate invalid state");

        await _service.CreateAsync(dto);

        Assert.True(_notificationContext.HasNotifications);
        _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact(DisplayName = "CreateAsync should notify when SKU already exists")]
    public async Task CreateAsync_Should_Notify_When_Sku_Already_Exists()
    {
        var dto = new ProductRequestDto 
        { 
            Name = "New", Price = 10, Sku = "EXISTING-SKU", Category = "C", ImageUrl = "U", Description = "D" 
        };

        _readRepositoryMock.Setup(r => r.ExistsBySkuAsync(dto.Sku)).ReturnsAsync(true);

     
        await _service.CreateAsync(dto);

        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Sku");
        _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact(DisplayName = "CreateAsync should notify when slug exists")]
    public async Task CreateAsync_Should_Notify_When_Slug_Exists()
    {
        var dto = new ProductRequestDto 
        { 
            Name = "Iphone 15", Price = 10, Sku = "NEW-SKU", Category = "C", ImageUrl = "U", Description = "D" 
        };

        _readRepositoryMock.Setup(r => r.ExistsBySkuAsync(dto.Sku)).ReturnsAsync(false);
        _readRepositoryMock.Setup(r => r.ExistsBySlugAsync(It.IsAny<string>())).ReturnsAsync(true);

        await _service.CreateAsync(dto);

        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Name");
        _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact(DisplayName = "CreateAsync should create when valid")]
    public async Task CreateAsync_Should_Create_When_Valid()
    {
        var dto = new ProductRequestDto 
        { 
            Name = "Valid Product", Price = 10, Sku = "VALID-SKU", Category = "C", ImageUrl = "U", Description = "D" 
        };

        _readRepositoryMock.Setup(r => r.ExistsBySkuAsync(dto.Sku)).ReturnsAsync(false);
        _readRepositoryMock.Setup(r => r.ExistsBySlugAsync(It.IsAny<string>())).ReturnsAsync(false);

        await _service.CreateAsync(dto);

        Assert.False(_notificationContext.HasNotifications);
        _writeRepositoryMock.Verify(w => w.InsertAsync(It.Is<Product>(p => p.Sku == "VALID-SKU")), Times.Once);
    }  

    [Fact(DisplayName = "UpdateAsync should notify when not found")]
    public async Task UpdateAsync_Should_Notify_When_NotFound()
    {
        _readRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

        await _service.UpdateAsync(Guid.NewGuid(), new ProductRequestDto());

        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Product");
    }

    [Fact(DisplayName = "UpdateAsync should notify when changing SKU to one that already exists")]
    public async Task UpdateAsync_Should_Notify_When_NewSku_Exists()
    {
        var id = Guid.NewGuid();
        var existingProduct = new Product("Old Name", "Desc", 10, "OLD-SKU", "Cat", "img");

        var dto = new ProductRequestDto { Sku = "ANOTHER-SKU" };
        
        _readRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingProduct);
        _readRepositoryMock.Setup(r => r.ExistsBySkuAsync("ANOTHER-SKU")).ReturnsAsync(true);

        await _service.UpdateAsync(id, dto);

        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Sku");
        _writeRepositoryMock.Verify(w => w.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact(DisplayName = "UpdateAsync should update successfully")]
    public async Task UpdateAsync_Should_Update_Successfully()
    {
        var id = Guid.NewGuid();
        var existingProduct = new Product("Old Name", "Desc", 10, "OLD-SKU", "Cat", "img");
        
        var dto = new ProductRequestDto 
        { 
            Name = "New Name", 
            Description = "New Desc", 
            Price = 20, 
            Sku = "OLD-SKU", 
            Category = "New Cat", 
            ImageUrl = "new.jpg" 
        };

        _readRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingProduct);
        _readRepositoryMock.Setup(r => r.ExistsBySkuAsync("OLD-SKU")).ReturnsAsync(true); 

        await _service.UpdateAsync(id, dto);

        Assert.False(_notificationContext.HasNotifications);
        Assert.Equal("New Name", existingProduct.Name); // Verifica se a entidade em memória foi alterada
        _writeRepositoryMock.Verify(w => w.UpdateAsync(existingProduct), Times.Once);
    }

    [Fact(DisplayName = "ToggleStatusAsync should toggle active status")]
    public async Task ToggleStatusAsync_Should_Toggle_Status()
    {
        var id = Guid.NewGuid();
        var product = new Product("Name", "Desc", 10, "SKU", "Cat", "img"); 

        _readRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        await _service.ToggleStatusAsync(id);
        Assert.False(product.Active);
        _writeRepositoryMock.Verify(w => w.UpdateAsync(product), Times.Once);

        await _service.ToggleStatusAsync(id);
        Assert.True(product.Active);
        _writeRepositoryMock.Verify(w => w.UpdateAsync(product), Times.Exactly(2));
    }

    [Fact(DisplayName = "DeleteAsync should call delete on repository when found")]
    public async Task DeleteAsync_Should_Call_Repository_When_Found()
    {
        var id = Guid.NewGuid();
        var product = new Product("Name", "Desc", 10, "SKU", "Cat", "img");

        _readRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        await _service.DeleteAsync(id);

        _writeRepositoryMock.Verify(w => w.DeleteByIdAsync(id), Times.Once);
    }

    [Fact(DisplayName = "DeleteAsync should notify when not found")]
    public async Task DeleteAsync_Should_Notify_When_NotFound()
    {
        _readRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

        await _service.DeleteAsync(Guid.NewGuid());

        Assert.True(_notificationContext.HasNotifications);
        _writeRepositoryMock.Verify(w => w.DeleteByIdAsync(It.IsAny<Guid>()), Times.Never);
    }
}