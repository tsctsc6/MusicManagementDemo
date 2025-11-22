namespace MusicManagementDemo.WebApi.Endpoints;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class Error : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/error", TypedResults.InternalServerError).WithName("ErrorGet");
        app.MapPost("/error", TypedResults.InternalServerError).WithName("ErrorPost");
    }
}
