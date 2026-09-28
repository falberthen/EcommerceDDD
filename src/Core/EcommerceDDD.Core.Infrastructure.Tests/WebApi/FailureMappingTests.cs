namespace EcommerceDDD.Core.Infrastructure.Tests.WebApi;

public class FailureMappingTests
{
	public static TheoryData<IError, int> Failures => new()
	{
		{ new ForbiddenError("not yours"), StatusCodes.Status403Forbidden },
		{ new RecordNotFoundError("missing"), StatusCodes.Status404NotFound },
		{ new ValidationError("business rule"), StatusCodes.Status422UnprocessableEntity },
		{ new Error("downstream unavailable"), StatusCodes.Status500InternalServerError }
	};

	[Theory]
	[MemberData(nameof(Failures))]
	public void MapFailure_ShouldMapErrorTypeToStatusCode(IError error, int expectedStatus)
	{
		var result = new TestController().Map(Result.Fail(error));

		var objectResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal(expectedStatus, objectResult.StatusCode);
		Assert.Equal(error.Message, Assert.IsAssignableFrom<ProblemDetails>(objectResult.Value).Detail);
	}

	private class TestController : CustomControllerBase
	{
		public IActionResult Map(IResultBase result) => MapFailure(result);
	}
}
