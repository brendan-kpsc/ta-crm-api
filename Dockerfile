FROM mcr.microsoft.com/dotnet/sdk:10.0

COPY . .

RUN mkdir ./app
RUN dotnet publish ./TaCrmApi.sln -o ./app

WORKDIR /app

CMD ["dotnet", "TaCrm.Api.dll"]