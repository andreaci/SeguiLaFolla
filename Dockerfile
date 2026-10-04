FROM node:24-alpine AS frontend-build

WORKDIR /src/frontend
ARG VITE_BASE_PATH=/
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci

COPY frontend/ ./
ENV VITE_API_BASE_URL=""
ENV VITE_BASE_PATH=${VITE_BASE_PATH}
RUN npm run build


FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS backend-build

WORKDIR /src
COPY backend/EffettoMandria.Api/EffettoMandria.Api.csproj backend/EffettoMandria.Api/
RUN dotnet restore backend/EffettoMandria.Api/EffettoMandria.Api.csproj

COPY backend/EffettoMandria.Api/ backend/EffettoMandria.Api/
RUN dotnet publish backend/EffettoMandria.Api/EffettoMandria.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

COPY --from=frontend-build /src/frontend/dist/ /app/publish/wwwroot/


FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final

ARG APP_BASE_PATH=/
ENV APP_BASE_PATH=${APP_BASE_PATH}

WORKDIR /app
COPY --from=backend-build /app/publish/ ./

USER $APP_UID
EXPOSE 5080

ENTRYPOINT ["dotnet", "EffettoMandria.Api.dll"]
