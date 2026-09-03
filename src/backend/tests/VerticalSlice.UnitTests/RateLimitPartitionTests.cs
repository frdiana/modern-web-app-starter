using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VerticalSlice.Api.Extensions;

namespace VerticalSlice.UnitTests;

public sealed class RateLimitPartitionTests
{
    [Fact]
    public void GetPartitionKey_ForDifferentAddresses_ReturnsDifferentKeys()
    {
        var first = new DefaultHttpContext();
        first.Connection.RemoteIpAddress = IPAddress.Parse("192.0.2.1");
        var second = new DefaultHttpContext();
        second.Connection.RemoteIpAddress = IPAddress.Parse("192.0.2.2");

        var firstKey = ServiceCollectionExtensions.GetPartitionKey(first);
        var secondKey = ServiceCollectionExtensions.GetPartitionKey(second);

        Assert.NotEqual(firstKey, secondKey);
    }

    [Fact]
    public void GetPartitionKey_ForAuthenticatedUser_PrefersSubject()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim("sub", "user-123")],
                "Test"))
        };
        context.Connection.RemoteIpAddress = IPAddress.Parse("192.0.2.1");

        var key = ServiceCollectionExtensions.GetPartitionKey(context);

        Assert.Equal("user:user-123", key);
    }
}