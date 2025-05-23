global using EcommerceDDD.Core.CQRS.CommandHandling;
global using EcommerceDDD.Core.CQRS.QueryHandling;
global using EcommerceDDD.Core.Testing;
global using EcommerceDDD.PaymentProcessing.API.Controllers;
global using EcommerceDDD.PaymentProcessing.API.Controllers.Requests;
global using EcommerceDDD.PaymentProcessing.Application.CancelingPayment;
global using EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;
global using EcommerceDDD.PaymentProcessing.Application.RequestingPayment;
global using EcommerceDDD.PaymentProcessing.Domain;
global using EcommerceDDD.PaymentProcessing.Domain.Commands;
global using EcommerceDDD.PaymentProcessing.Domain.Events;
global using Microsoft.AspNetCore.Mvc;
global using NSubstitute;
global using Xunit;
