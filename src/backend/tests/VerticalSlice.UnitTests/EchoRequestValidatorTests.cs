using VerticalSlice.Api.Endpoints.Examples;

namespace VerticalSlice.UnitTests;

public sealed class EchoRequestValidatorTests
{
    private readonly Echo.RequestValidator validator = new();

    [Fact]
    public async Task Validate_WithValidMessage_IsValid()
    {
        var result = await validator.ValidateAsync(
            new Echo.Request { Message = "hello" },
            TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WithMissingMessage_HasMessageError(string? message)
    {
        var result = await validator.ValidateAsync(
            new Echo.Request { Message = message },
            TestContext.Current.CancellationToken);

        Assert.Contains(result.Errors, static failure => failure.PropertyName == "Message");
    }

    [Fact]
    public async Task Validate_WithMessageLongerThanMaximum_HasMessageError()
    {
        var request = new Echo.Request { Message = new string('a', 201) };

        var result = await validator.ValidateAsync(request, TestContext.Current.CancellationToken);

        Assert.Contains(result.Errors, static failure => failure.PropertyName == "Message");
    }
}