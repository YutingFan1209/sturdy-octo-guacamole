FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/ComplianceCaseManagement.EscalationWorker/ComplianceCaseManagement.EscalationWorker.csproj -c Release -o /app
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["dotnet","ComplianceCaseManagement.EscalationWorker.dll"]
