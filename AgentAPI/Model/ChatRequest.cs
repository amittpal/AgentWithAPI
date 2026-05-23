namespace AgentAPI.Model
{
    public sealed record ChatRequest(
        string Message,
        string? SessionId = null
        );
}
