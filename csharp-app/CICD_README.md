# CI/CD Pipeline Documentation

This document outlines the Continuous Integration and Continuous Deployment pipeline for the Mockups application.

## Overview

The CI/CD pipeline is configured using GitHub Actions and follows these stages:

1. **Build and Unit Testing** - Compiles the application and runs unit tests
2. **Load Testing** - Deploys the application temporarily and runs load tests
3. **Containerization** - Builds and pushes Docker images to GitHub Container Registry
4. **Deployment** - Deploys the application to production environment

## Pipeline Configuration

### GitHub Secrets Required

To enable all features of the pipeline, the following GitHub Secrets should be configured in your repository:

#### Required Secrets:
- `GITHUB_TOKEN` - Automatically provided by GitHub Actions

#### Optional Secrets for Deployment:
- `AZURE_WEBAPP_PUBLISH_PROFILE` - For Azure deployments
- `KUBE_CONFIG_DATA` - Base64-encoded Kubernetes config for K8s deployments  
- `SSH_PRIVATE_KEY` - Private key for SSH deployments
- `SSH_USER` - SSH username for server access
- `SSH_HOST` - Hostname/IP of the deployment server
- `SLACK_WEBHOOK_URL` - For deployment notifications

#### Required Variables for Deployment:
- `AZURE_WEBAPP_NAME` - Name of the Azure Web App
- `KUBE_CONFIG` - Path to Kubernetes config
- `DOCKER_COMPOSE_FILE` - Path to Docker Compose file
- `DEPLOYMENT_PATH` - Path on the remote server for deployment

## Pipeline Triggers

The pipeline is triggered on:
- Push to `main` or `master` branches
- Pull requests to `main` or `master` branches

## Stages Breakdown

### 1. Build and Unit Test (`build-and-unit-test`)
- Sets up .NET 8.0 environment
- Restores NuGet packages
- Builds the solution
- Runs unit tests with code coverage
- Uploads test results and coverage reports as artifacts

### 2. Load Testing (`load-testing`)
- Requires successful completion of the build-and-unit-test stage
- Sets up SQL Server container as a service
- Installs SQL Server tools
- Applies database migrations
- Starts the application on port 8080
- Runs load tests for 30 seconds using the LoadTest project
- Loads test credentials from environment variables

### 3. Containerization (`containerize`)
- Builds a Docker image from the Dockerfile
- Tags the image with branch, PR, and SHA identifiers
- Pushes the image to GitHub Container Registry (GHCR)
- Only runs on push to main branch

### 4. Deployment (`deploy`)
- Runs only on push to main branch
- Deploys to the production environment using one of the following methods:
  - Azure Web Apps (if Azure secrets are configured)
  - Kubernetes cluster (if K8s secrets are configured)
  - Docker Compose on remote server (if SSH secrets are configured)
- Performs health checks after deployment
- Sends deployment notifications

## Docker Configuration

The application is containerized using the provided `Dockerfile` that:
- Uses .NET 8.0 runtime as the base image
- Exposes ports 80 and 443
- Publishes the application using .NET publish
- Sets the entrypoint to run the Mockups.dll

## Local Development

To run the application locally with Docker Compose:

```bash
docker-compose up --build
```

The application will be accessible at `http://localhost:8080` and the database will be accessible at `localhost:1433`.

## Troubleshooting

### Common Issues:

1. **SQL Server Connection Errors**: The database container takes time to initialize. The application will retry connections.

2. **Port Conflicts**: Make sure ports 8080 and 1433 are available on your system.

3. **Load Test Failures**: The load test expects specific user credentials ('Java@DlyaLox.ov' / '.NetDlyaPacan0v') to exist in the database.

### Testing the Load Testing Locally:

To run load tests locally:
```bash
cd Application/Mockups.Tests/LoadTest
dotnet run
```

You can customize the base URL using the `LOAD_TEST_BASE_URL` environment variable:
```bash
LOAD_TEST_BASE_URL=http://localhost:5146 dotnet run
```

## Security Notes

- The SQL Server password used in the pipeline is a sample password and should be changed in production environments
- All secrets are stored securely in GitHub Secrets and never printed to logs
- The pipeline uses the minimum required permissions for each stage