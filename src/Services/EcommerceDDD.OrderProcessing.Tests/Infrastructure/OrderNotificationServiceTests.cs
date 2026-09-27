using EcommerceDDD.ServiceClients.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using System.Net;
using System.Net.Http;

namespace EcommerceDDD.OrderProcessing.Tests.Infrastructure;

public class OrderNotificationServiceTests
{
	[Fact]
	public async Task UpdateOrderStatus_WhenSignalRFails_ShouldLogWarningInsteadOfThrowing()
	{
		// Given
		var handler = new FailingHandler();
		var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://signalr") };
		var signalRClient = new SignalRClient(
			new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient));
		var logger = Substitute.For<ILogger<OrderNotificationService>>();
		var notificationService = new OrderNotificationService(signalRClient, logger);

		// When
		var exception = await Record.ExceptionAsync(() => notificationService.UpdateOrderStatusAsync(
			Guid.NewGuid(), Guid.NewGuid(), "Placed", 1, CancellationToken.None));

		// Then
		Assert.Null(exception);
		Assert.True(handler.Called);
		Assert.Contains(logger.ReceivedCalls(), call =>
			call.GetMethodInfo().Name == nameof(ILogger.Log) && call.GetArguments()[0] is LogLevel.Warning);
	}

	private sealed class FailingHandler : HttpMessageHandler
	{
		public bool Called { get; private set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Called = true;
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
		}
	}
}
