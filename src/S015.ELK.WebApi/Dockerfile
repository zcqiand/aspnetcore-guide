#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["ELK.WebApi01/ELK.WebApi01.csproj", "ELK.WebApi01/"]
RUN dotnet restore "ELK.WebApi01/ELK.WebApi01.csproj"
COPY . .
WORKDIR "/src/ELK.WebApi01"
RUN dotnet build "ELK.WebApi01.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ELK.WebApi01.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ELK.WebApi01.dll"]