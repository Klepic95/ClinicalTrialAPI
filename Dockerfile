FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Docker

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ClinicalTrialAPI.sln", "./"]
COPY ["ClinicalTrial.API/ClinicalTrial.API.csproj", "ClinicalTrial.API/"]
COPY ["ClinicalTrial.Business/ClinicalTrial.Business.csproj", "ClinicalTrial.Business/"]
COPY ["ClinicalTrial.DAL/ClinicalTrial.DAL.csproj", "ClinicalTrial.DAL/"]
COPY ["ClinicalTrial.UnitTests/ClinicalTrial.UnitTests.csproj", "ClinicalTrial.UnitTests/"]

RUN dotnet restore "ClinicalTrialAPI.sln"

COPY . .

RUN dotnet build "ClinicalTrialAPI.sln" -c Release -o /app/build

RUN dotnet publish "ClinicalTrialAPI.sln" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicalTrial.API.dll"]
