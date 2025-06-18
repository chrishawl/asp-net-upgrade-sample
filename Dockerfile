# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all source code (since network restore is failing, we'll do it together)
COPY . .

# Build and publish the application
WORKDIR "/src/src/MVCRandomAnswerGenerator.Core.Web"
RUN dotnet publish "MVCRandomAnswerGenerator.Core.Web.csproj" -c Release -o /app/publish --disable-parallel -p:NuGetAudit=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published application
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Configure health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "MVCRandomAnswerGenerator.Core.Web.dll"]