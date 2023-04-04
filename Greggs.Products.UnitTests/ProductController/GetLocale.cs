using System;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using System.Linq;

namespace Greggs.Products.UnitTests;

public class ProductController_GetLocale
{
    [Fact]
    public void BadPageStart()
    {
        // Arrange
        string locale = "Euro";
        int pageStart = -1;
        int pageSize = 50;
        var mockLogger = new Mock<ILogger<ProductController>>();
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        var target = new ProductController(mockLogger.Object, mockDataAccess.Object);
        var act = () => { _ = target.Get(locale, pageStart, pageSize).ToList(); };

        // Act
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void BadPageSize()
    {
        // Arrange
        string locale = "Euro";
        int pageStart = 50;
        int pageSize = -1;
        var mockLogger = new Mock<ILogger<ProductController>>();
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        var target = new ProductController(mockLogger.Object, mockDataAccess.Object);
        var act = () => { _ = target.Get(locale, pageStart, pageSize).ToList(); };

        // Act
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void BadLocale()
    {
        // Arrange
        string locale = null;
        int pageStart = 50;
        int pageSize = 50;
        var mockLogger = new Mock<ILogger<ProductController>>();
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        var target = new ProductController(mockLogger.Object, mockDataAccess.Object);
        var act = () => { _ =target.Get(locale, pageStart, pageSize).ToList(); };

        // Act
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void NoData()
    {
        // Arrange
        decimal exchangeRate = 1.5m;
        string locale = "Euro";
        int pageStart = 50;
        int pageSize = 50;
        var mockLogger = new Mock<ILogger<ProductController>>();
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        mockDataAccess
               .Setup(t => t.GetExchangeRate(locale))
               .Returns(exchangeRate);
        var target = new ProductController(mockLogger.Object, mockDataAccess.Object);



        // Act
        var result = target.Get(locale, pageStart, pageSize).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(0);
        mockDataAccess.Verify(t => t.GetExchangeRate(locale), Times.Once);
        mockDataAccess.Verify(t => t.List(pageStart, pageSize), Times.Once);
    }

    [Fact]
    public void SomeData()
    {
        // Arrange
        decimal exchangeRate = 1.5m;
        string locale = "Euro";
        int pageStart = 50;
        int pageSize = 50;
        var mockLogger = new Mock<ILogger<ProductController>>();
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        mockDataAccess
            .Setup(t => t.List(pageStart, pageSize))
            .Returns(
                () => new []
                {
                    new Product { Name = "A", PriceInPounds = 1.0m },
                    new Product { Name = "A", PriceInPounds = 2.0m },
                    new Product { Name = "A", PriceInPounds = 3.0m },
                });
        mockDataAccess
               .Setup(t => t.GetExchangeRate(locale))
               .Returns(exchangeRate);
        var target = new ProductController(mockLogger.Object, mockDataAccess.Object);



        // Act
        var result = target.Get(locale, pageStart, pageSize).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].Should().BeEquivalentTo(new
            {
                Name = "A",
                PriceInPounds = 1.0m,
                PriceInLocal = exchangeRate
            });
        mockDataAccess.Verify(t => t.List(pageStart, pageSize), Times.Once);
        mockDataAccess.Verify(t => t.GetExchangeRate(locale), Times.Once);
    }
}