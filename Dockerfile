###############
# Backend Build #
###############
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /app/backend

# Copy only project files first to leverage Docker layer caching for package restore
COPY src/backend/PromptArchive.sln ./
COPY src/backend/PromptArchive/*.csproj ./PromptArchive/
RUN dotnet restore PromptArchive.sln

# Copy the rest of the source code
COPY src/backend/ ./
RUN dotnet publish PromptArchive.sln -c Release -o out

################
# Frontend Build #
################
FROM node:slim AS frontend-build
WORKDIR /app/frontend

# Copy package files first to leverage Docker layer caching for npm install
COPY src/frontend/package*.json ./
RUN npm ci

# Copy the rest of the frontend source code
COPY src/frontend/ ./
RUN npm run build

####################
# Final Runtime Image #
####################
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Create a non-root user to run the application
RUN adduser --disabled-password --gecos "" appuser

# Install dependencies
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libice6 \
    libsm6 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy the published backend
COPY --from=backend-build /app/backend/out ./

# Copy the built frontend to the wwwroot directory
COPY --from=frontend-build /app/frontend/dist ./wwwroot/

# Set proper ownership
RUN chown -R appuser:appuser /app

# Configure environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Switch to non-root user
USER appuser

# Expose the ports
EXPOSE 8080

# Add health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "PromptArchive.dll"]
