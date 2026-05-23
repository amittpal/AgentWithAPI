# AgentWithAPI

In the Microsoft Agent Framework, this project defines an agent that uses NLP to invoke C# app methods and functions for student management, including add, delete, and other operations such as DB access.

Monorepo containing a .NET Web API backend (`AgentAPI`) and an Angular frontend (`chatbot`).

## Projects
- `AgentAPI`: .NET 10 Web API backend.
- `chatbot`: Angular 21 frontend with Bootstrap UI.

## Functionality
- Student chatbot UI with a Bootstrap-powered chat interface.
- Sends user messages to `StudentController` -> `StudentAgent` via `/Student/StudentsAgent`.
- Supports session preservation across requests using `sessionId`.
- Clicking **New Chat** resets the session and starts a fresh conversation.
- Bot replies stream back word-by-word with auto-scroll behavior.
- The Send button toggles to Stop while the bot is responding, allowing cancellation.

## Quick start

### Backend

```bash
cd AgentAPI
dotnet restore
dotnet build
dotnet run
```


### Frontend

```bash
cd chatbot
npm install
npm start
```


## Notes
- The frontend uses `src/environments/environment.ts` to configure the API URL.
- The backend enables CORS globally for development.
- Use `New Chat` to clear session history and force the backend to create a new session ID.
- If the Angular app is served from another origin, update the CORS policy or environment URL accordingly.
