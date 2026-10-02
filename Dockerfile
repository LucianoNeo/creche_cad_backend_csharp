FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY src/creche_cad.Domain src/creche_cad.Domain
COPY src/creche_cad.Data src/creche_cad.Data
COPY src/creche_cad.Service src/creche_cad.Service
COPY src/creche_cad.Api src/creche_cad.Api
RUN dotnet publish src/creche_cad.Api/creche_cad.Api.csproj -c Release -o /output
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /output .
RUN mkdir -p /data && chown -R app:app /data
ENV ASPNETCORE_HTTP_PORTS=8080 DataDirectory=/data
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "creche_cad.Api.dll"]
