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

public class LocalProductController_Get
{
    [Fact]
    public void BadPageStart()
    {
        // Arrange
        string locale = "Euro";
        int pageStart = -1;
        int pageSize = 50;
        var logger = Mock.Of<ILogger<LocalProductController>>();
        var dataAccess = Mock.Of<IDataAccess<Product>>();
        var exchangeRateAccess = Mock.Of<IDataAccessSingle<ExchangeRate, string>>();
        var target = new LocalProductController(logger, dataAccess, exchangeRateAccess);
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
        var logger = Mock.Of<ILogger<LocalProductController>>();
        var dataAccess = Mock.Of<IDataAccess<Product>>();
        var exchangeRateAccess = Mock.Of<IDataAccessSingle<ExchangeRate, string>>();
        var target = new LocalProductController(logger, dataAccess, exchangeRateAccess);
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
        var logger = Mock.Of<ILogger<LocalProductController>>();
        var dataAccess = Mock.Of<IDataAccess<Product>>();
        var exchangeRateAccess = Mock.Of<IDataAccessSingle<ExchangeRate, string>>();
        var target = new LocalProductController(logger, dataAccess, exchangeRateAccess);
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
        var logger = Mock.Of<ILogger<LocalProductController>>();
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        var mockExchangeRateAccess = new Mock<IDataAccessSingle<ExchangeRate, string>>();
        mockExchangeRateAccess
               .Setup(t => t.Get(locale))
               .Returns(new ExchangeRate(locale, exchangeRate));
        var target = new LocalProductController(logger, mockDataAccess.Object, mockExchangeRateAccess.Object);



        // Act
        var result = target.Get(locale, pageStart, pageSize).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(0);
        mockExchangeRateAccess.Verify(t => t.Get(locale), Times.Once);
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
        var logger = Mock.Of<ILogger<LocalProductController>>();
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        var mockExchangeRateAccess = new Mock<IDataAccessSingle<ExchangeRate, string>>();
        mockDataAccess
            .Setup(t => t.List(pageStart, pageSize))
            .Returns(
                () => new []
                {
                    new Product { Name = "A", PriceInPounds = 1.0m },
                    new Product { Name = "A", PriceInPounds = 2.0m },
                    new Product { Name = "A", PriceInPounds = 3.0m },
                });
        mockExchangeRateAccess
               .Setup(t => t.Get(locale))
               .Returns(new ExchangeRate(locale, exchangeRate));
        var target = new LocalProductController(logger, mockDataAccess.Object, mockExchangeRateAccess.Object);



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
        mockExchangeRateAccess.Verify(t => t.Get(locale), Times.Once);
    }
}