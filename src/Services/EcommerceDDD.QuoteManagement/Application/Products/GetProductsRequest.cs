﻿namespace EcommerceDDD.QuoteManagement.Application.Products;

public record class GetProductsRequest(string CurrencyCode, Guid[] ProductIds);