using VerticalSlice.Api.Configuration;

namespace VerticalSlice.UnitTests;

public sealed class ConfigurationOptionsValidatorTests
{
    [Fact]
    public void ApplicationValidator_WithMissingName_Fails()
    {
        var validator = new ApplicationOptionsValidator();

        var result = validator.Validate(null, new ApplicationOptions());

        Assert.True(result.Failed);
    }

    [Fact]
    public void ApplicationValidator_WithName_Succeeds()
    {
        var validator = new ApplicationOptionsValidator();

        var result = validator.Validate(null, new ApplicationOptions { Name = "Sample" });

        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(0, 60, 0)]
    [InlineData(60, 0, 0)]
    [InlineData(60, 60, -1)]
    public void RateLimitingValidator_WithInvalidValue_Fails(
        int permitLimit,
        int windowSeconds,
        int queueLimit)
    {
        var validator = new ApiRateLimitingOptionsValidator();
        var options = new ApiRateLimitingOptions
        {
            PermitLimit = permitLimit,
            WindowSeconds = windowSeconds,
            QueueLimit = queueLimit
        };

        var result = validator.Validate(null, options);

        Assert.True(result.Failed);
    }
}