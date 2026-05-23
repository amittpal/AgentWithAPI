using AgentAPI.AgentTools;
using AgentAPI.Model;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Collections.Concurrent;
using System.Text.Json;

namespace AgentAPI.Agents
{
    public class StudentAgent
    {
        private readonly AIAgent _agent;
        public AIAgent Agent => _agent;
        private readonly ConcurrentDictionary<string, JsonElement> sessionStore = new();

        public StudentAgent(StudentTool studentTool, IConfiguration configuration)
        {
            //this api key is from github models
            var apiKey = configuration["OpenAIKey"] ?? throw new ArgumentNullException("OpenAi key is not available");

            var model = configuration["Model"] ?? "gpt-4o-mini";

            IChatClient chatClient = new ChatClient(model,
     new ApiKeyCredential(apiKey)
     , new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") }).AsIChatClient();

            _agent = new ChatClientAgent(chatClient,
               name: "StudentAgent",
               instructions: """
                You are a helpful student assistant that return student information.
                    You have access to the following tools to manage student information:
                    - GetStudents: Retrieve a list of all students.
                    - AddStudent: Add a new student to the list.
                    - DeleteStudent: Remove a student from the list.
        """,
               tools:
               [
                   AIFunctionFactory.Create(studentTool.GetStudents,name:nameof(studentTool.GetStudents)),
                   AIFunctionFactory.Create(studentTool.AddStudent,name:nameof(studentTool.AddStudent)),
                   AIFunctionFactory.Create(studentTool.DeleteStudent,name:nameof(studentTool.DeleteStudent)),
               ]);



        }
        public async Task<Model.ChatResponse> ChatAsync(ChatRequest chatRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(chatRequest.Message))
            {
                throw new ArgumentException("Message is required");
            }
            var sessionKey = chatRequest.SessionId ?? Guid.NewGuid().ToString("N");
            AgentSession session;

            if (chatRequest.SessionId is not null && sessionStore.TryGetValue(chatRequest.SessionId, out var serializedSession))
            {
                session = await _agent.DeserializeSessionAsync(serializedSession, cancellationToken: cancellationToken);
            }
            else
            {
                session = await _agent.CreateSessionAsync(cancellationToken: cancellationToken);
            }

            var result = await _agent.RunAsync(chatRequest.Message, session
            , cancellationToken: cancellationToken);

            var savedSession = await _agent.SerializeSessionAsync(session, cancellationToken: cancellationToken);
            sessionStore[sessionKey] = savedSession.Clone();

            return new Model.ChatResponse(sessionKey, result.Text);
        }
    }
}
