FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY backend/DailyTaskTracker.Api/DailyTaskTracker.Api.csproj backend/DailyTaskTracker.Api/
RUN dotnet restore backend/DailyTaskTracker.Api/DailyTaskTracker.Api.csproj
COPY backend/DailyTaskTracker.Api/ backend/DailyTaskTracker.Api/
RUN dotnet publish backend/DailyTaskTracker.Api/DailyTaskTracker.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet","DailyTaskTracker.Api.dll"]
