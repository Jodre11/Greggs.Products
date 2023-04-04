using System;
using System.Collections.Generic;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LocalProductController : ControllerBase
{
    private readonly ILogger<LocalProductController> _logger;
    private readonly IDataAccess<Product> _dataAccess;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="dataAccess">The data access.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public LocalProductController(
        ILogger<LocalProductController> logger,
        IDataAccess<Product> dataAccess)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(dataAccess);

        _logger = logger;
        _dataAccess = dataAccess;
    }

    /// <summary>
    /// Gets a page of products from <see cref="IDataAccess{T}"/> of <see cref="Product"/>.
    /// </summary>
    /// <param name="locale">Locale of the requestor.</param>
    /// <param name="pageStart">The page start.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <returns>A sequence of <see cref="Product"/>.</returns>
    [HttpGet]
    public IEnumerable<LocalProduct> Get(string locale = "Euro", int pageStart = 0, int pageSize = 5)
    {
        _logger.LogTrace($"Begin call to {nameof(LocalProductController.Get)}");
        if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, null);
        if (pageStart < 0) throw new ArgumentOutOfRangeException(nameof(pageStart), pageStart, null);
        ArgumentNullException.ThrowIfNull(locale);

        var exchangeRate = _dataAccess.GetExchangeRate(locale);
        foreach(var product in _dataAccess.List(pageStart, pageSize))
        {
            yield return new LocalProduct
            {
                Name = product.Name,
                PriceInPounds = product.PriceInPounds,
                PriceInLocal = product.PriceInPounds * exchangeRate
            };
        }
    }
}