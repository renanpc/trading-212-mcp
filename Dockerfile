FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Trading212Mcp.csproj", "./"]
RUN dotnet restore "./Trading212Mcp.csproj"

COPY . .
RUN dotnet publish "./Trading212Mcp.csproj" \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:EnableCompressionInSingleFile=true \
    -p:DebugType=None \
    -p:DebugSymbols=false \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENV HTTP_PORT=8080
ENV TRADING212_BASE_URL=https://demo.trading212.com
EXPOSE 8080

ENTRYPOINT ["./Trading212Mcp"]
