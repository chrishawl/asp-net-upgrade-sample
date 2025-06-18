# Docker Containerization Guide

This document provides information about running the .NET 8 ASP.NET Core Random Answer Generator in Docker containers.

## Quick Start

### Build and Run with Docker

```bash
# Build the Docker image
docker build -t mvc-random-answer-generator:latest .

# Run the container
docker run -d -p 8080:8080 --name mvc-app mvc-random-answer-generator:latest

# Test the health check
curl http://localhost:8080/health

# Test the application (Note: Views are not implemented yet in this phase)
curl http://localhost:8080/
```

### Run with Docker Compose (Development)

```bash
# Start the development environment
docker compose up -d

# View logs
docker compose logs -f mvc-random-answer-generator

# Stop the environment
docker compose down
```

### Run with Docker Compose (Production)

```bash
# Start with production configuration
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Or with environment variables
TAG=v1.0.0 HTTP_PORT=80 docker compose -f docker-compose.prod.yml up -d
```

## Docker Image Details

### Multi-Stage Build

The Dockerfile uses a multi-stage build approach:

1. **Build Stage**: Uses `mcr.microsoft.com/dotnet/sdk:8.0` to compile and publish the application
2. **Runtime Stage**: Uses `mcr.microsoft.com/dotnet/aspnet:8.0` for the final lightweight image

### Security Features

- **Non-root user**: Application runs as `appuser` (non-root)
- **Minimal image**: Uses official Microsoft ASP.NET Core runtime image
- **Read-only filesystem**: Production configuration enables read-only root filesystem
- **Security options**: Disables privilege escalation in production

### Image Optimization

- **Layer caching**: Optimized for Docker layer caching
- **Minimal context**: Uses `.dockerignore` to exclude unnecessary files
- **Small runtime**: Final image only contains runtime dependencies

## Health Checks

The application includes comprehensive health checks:

```bash
# Container health check (automatically configured)
curl http://localhost:8080/health

# Example response:
{
  "status": "Healthy",
  "checks": [
    {
      "name": "answer_generator",
      "status": "Healthy",
      "description": "Answer generator is functioning correctly",
      "data": {
        "MaxStoredQuestions": 100,
        "EnableCaching": true,
        "TestAnswer": "Yes, definitely"
      }
    }
  ]
}
```

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Production` |
| `ASPNETCORE_URLS` | URLs to bind | `http://+:8080` |
| `ASPNETCORE_HTTP_PORT` | HTTP port | `8080` |
| `DOTNET_RUNNING_IN_CONTAINER` | Container detection | `true` |

## Docker Compose Files

### docker-compose.yml
Main development configuration with:
- HTTP only (port 5000)
- Development environment
- Health checks enabled
- Volume mounting for configuration

### docker-compose.override.yml
Development overrides with:
- Faster health check intervals
- Additional debugging options
- Log volume mounting

### docker-compose.prod.yml
Production configuration with:
- Security hardening
- Resource limits
- Read-only filesystem
- Optimized health checks

## Container Security

### Production Security Features

1. **Non-root execution**: Application runs as `appuser`
2. **Read-only filesystem**: Root filesystem is mounted read-only
3. **Temporary filesystems**: Writable directories use tmpfs
4. **No privilege escalation**: Security option prevents privilege escalation
5. **Resource limits**: CPU and memory limits configured

### Security Scanning

```bash
# Scan the image for vulnerabilities (requires additional tools)
# Example with docker scout (if available)
docker scout cves mvc-random-answer-generator:latest

# Or use other security scanning tools like:
# - Trivy: trivy image mvc-random-answer-generator:latest
# - Snyk: snyk container test mvc-random-answer-generator:latest
```

## Troubleshooting

### Common Issues

1. **Health check failures**: Verify application is starting correctly
   ```bash
   docker logs mvc-random-answer-generator-dev
   ```

2. **Port conflicts**: Check if ports are already in use
   ```bash
   netstat -tulpn | grep :5000
   ```

3. **Permission issues**: Ensure Docker daemon is running and accessible

### Debugging

```bash
# Access container shell
docker exec -it mvc-random-answer-generator-dev /bin/bash

# View application logs
docker logs -f mvc-random-answer-generator-dev

# Inspect container configuration
docker inspect mvc-random-answer-generator-dev
```

## Performance Considerations

### Resource Usage

- **Memory**: Typical usage ~100-200MB
- **CPU**: Minimal CPU usage for this simple application
- **Startup time**: ~5-10 seconds for container readiness

### Optimization Tips

1. Use multi-stage builds to minimize image size
2. Leverage Docker layer caching for faster builds
3. Use `.dockerignore` to exclude unnecessary files
4. Configure appropriate health check intervals
5. Set resource limits in production

## Deployment Options

### Local Development
```bash
docker compose up -d
```

### Cloud Deployment
The Docker image can be deployed to various cloud platforms:
- **Azure Container Instances**
- **AWS ECS/Fargate**
- **Google Cloud Run**
- **Kubernetes clusters**

Example Kubernetes deployment manifest available in the deployment documentation.

## Known Limitations

1. **Views not implemented**: The MVC views are not yet implemented in this migration phase
2. **HTTPS**: Currently configured for HTTP only in containers (HTTPS can be terminated at load balancer)
3. **Persistent storage**: No persistent volumes configured (data is in-memory)

---

*Note: This containerization is part of the .NET Framework to .NET 8 migration project. View implementation and additional features will be added in subsequent migration phases.*