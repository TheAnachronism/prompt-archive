FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /app/backend

COPY src/backend/ ./
RUN dotnet restore PromptArchive.sln
RUN dotnet publish PromptArchive.sln -c Release -o out

FROM node:23-alpine AS frontend-build
WORKDIR /app/frontend

COPY src/frontend/package*.json ./
RUN npm ci

COPY src/frontend/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:9.0
RUN apt-get update && apt-get install -y libfontconfig1 libice6 libsm6
WORKDIR /app

# Copy the published backend
COPY --from=backend-build /app/backend/out ./

# Copy the built frontend to the wwwroot directory
# This assumes your backend is configured to serve static files from wwwroot
COPY --from=frontend-build /app/frontend/dist ./wwwroot/

# Expose the port your app runs on
EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "PromptArchive.dll"]