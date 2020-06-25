# runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

# build image
FROM microsoft/dotnet:2.1-sdk AS build

# copy and restore nuget package
WORKDIR /
COPY KhV.sln .
COPY ./KhV.WebApi /KhV.WebApi
COPY ./KhV.MongoDb /KhV.MongoDb
COPY ./KhV.Ultis /KhV.Ultis
RUN dotnet restore

# build
WORKDIR /KhV.WebApi
RUN dotnet build KhV.WebApi.csproj -c Release -o /app

# publish
FROM build AS publish
RUN dotnet publish KhV.WebApi.csproj --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KhV.WebApi.dll"]
