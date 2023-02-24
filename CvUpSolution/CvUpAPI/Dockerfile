#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CvUpAPI/CvUpAPI.csproj", "CvUpAPI/"]
COPY ["AuthLibrary/AuthLibrary.csproj", "AuthLibrary/"]
COPY ["DataModelsLibrary/DataModelsLibrary.csproj", "DataModelsLibrary/"]
COPY ["GeneralLibrary/GeneralLibrary.csproj", "GeneralLibrary/"]
COPY ["EmailsLibrary/EmailsLibrary.csproj", "EmailsLibrary/"]
COPY ["CandsPositionsLibrary/CandsPositionsLibrary.csproj", "CandsPositionsLibrary/"]
COPY ["LuceneLibrary/LuceneLibrary.csproj", "LuceneLibrary/"]
COPY ["ContactsLibrary/ContactsLibrary.csproj", "ContactsLibrary/"]
COPY ["FoldersLibrary/FoldersLibrary.csproj", "FoldersLibrary/"]
RUN dotnet restore "CvUpAPI/CvUpAPI.csproj"
COPY . .
WORKDIR "/src/CvUpAPI"
RUN dotnet build "CvUpAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CvUpAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CvUpAPI.dll"]