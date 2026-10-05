FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY src/creche_cad.Domain src/creche_cad.Domain
COPY src/creche_cad.Service src/creche_cad.Service
COPY src/creche_cad.Data src/creche_cad.Data
COPY src/creche_cad.Worker src/creche_cad.Worker
RUN dotnet publish src/creche_cad.Worker/creche_cad.Worker.csproj -c Release -o /output
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /output .
USER app
ENTRYPOINT ["dotnet", "creche_cad.Worker.dll"]
