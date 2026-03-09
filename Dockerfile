# use the official .NET 9.0 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

# set the working directory in the container
WORKDIR /app 

COPY ["Fatagram_API.sln", "./"]
COPY ["Fatagram.API/Fatagram.API.csproj", "Fatagram.API/"]
COPY ["Fatagram.Application/Fatagram.Application.csproj", "Fatagram.Application/"]
COPY ["Fatagram.Infrastructure/Fatagram.Infrastructure.csproj", "Fatagram.Infrastructure/"]
COPY ["Fatagram.Domain/Fatagram.Domain.csproj", "Fatagram.Domain/"]
COPY ["Fatagram.Shared/Fatagram.Shared.csproj", "Fatagram.Shared/"]

RUN dotnet restore

# Copy the entire project to the container
COPY . .

RUN dotnet publish "Fatagram.API/Fatagram.API.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build-env /app/publish .

ENTRYPOINT [ "dotnet", "Fatagram.API.dll" ]