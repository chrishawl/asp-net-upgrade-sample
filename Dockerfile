# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy entire source code for simpler build
COPY . .

# Restore and build
RUN dotnet restore "src/MVCRandomAnswerGenerator.Core.Web/MVCRandomAnswerGenerator.Core.Web.csproj"
RUN dotnet build "src/MVCRandomAnswerGenerator.Core.Web/MVCRandomAnswerGenerator.Core.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/MVCRandomAnswerGenerator.Core.Web/MVCRandomAnswerGenerator.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published application
COPY --from=publish /app/publish .

# Add health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "MVCRandomAnswerGenerator.Core.Web.dll"]