using FluentValidation;

namespace VerticalSlice.Api.Filters;

public sealed class ValidationFilter<TRequest>(IValidator<TRequest> validator)
    : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().First();
        var result = await validator.ValidateAsync(
            request,
            context.HttpContext.RequestAborted);

        if (result.IsValid)
        {
            return await next(context);
        }

        var errors = result.Errors
            .GroupBy(static failure => failure.PropertyName)
            .ToDictionary(
                static group => group.Key,
                static group => group
                    .Select(static failure => failure.ErrorMessage)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray());

        return TypedResults.ValidationProblem(errors);
    }
}