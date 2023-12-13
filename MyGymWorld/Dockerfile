FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MyGymWorld/MyGymWorld.Web.csproj", "MyGymWorld/"]
COPY ["MyGymWorld.Core/MyGymWorld.Core.csproj", "MyGymWorld.Core/"]
COPY ["MyGymWorld.Common/MyGymWorld.Common.csproj", "MyGymWorld.Common/"]
COPY ["MyGymWorld.Core.Mapping/MyGymWorld.Core.Mapping.csproj", "MyGymWorld.Core.Mapping/"]
COPY ["MyGymWorld.Data/MyGymWorld.Data.csproj", "MyGymWorld.Data/"]
COPY ["MyGymWorld.Data.Common/MyGymWorld.Data.Common.csproj", "MyGymWorld.Data.Common/"]
COPY ["MyGymWorld.Web.ViewModels/MyGymWorld.Web.ViewModels.csproj", "MyGymWorld.Web.ViewModels/"]
COPY ["MyGymWorld.Core.Utilities/MyGymWorld.Core.Utilities.csproj", "MyGymWorld.Core.Utilities/"]
COPY ["MyGymWorld.Web.Infrastructure/MyGymWorld.Web.Infrastructure.csproj", "MyGymWorld.Web.Infrastructure/"]
RUN dotnet restore "MyGymWorld/MyGymWorld.Web.csproj"
COPY . .
WORKDIR "/src/MyGymWorld"
RUN dotnet build "MyGymWorld.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyGymWorld.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyGymWorld.Web.dll"]