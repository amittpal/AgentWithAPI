# AgentWithAPI

Monorepo containing a .NET Web API backend (`AgentAPI`) and an Angular frontend (`chatbot`).

## Projects
- AgentAPI: [AgentAPI](AgentAPI) - .NET Web API project.
- chatbot: [chatbot](chatbot) - Angular frontend application.

## Quick start

Run backend:

```bash
cd AgentAPI
dotnet restore
dotnet build
dotnet run
```

Run frontend:

```bash
cd chatbot
npm install
npm start
# or
# npx ng serve --open
```

## Notes
- The repository root `.gitignore` ignores common .NET and Angular artifacts.
- Adjust commands to fit your local SDK/CLI versions.
