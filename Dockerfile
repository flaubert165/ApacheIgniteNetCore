FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
ARG PAT
ENV TZ=America/Sao_Paulo

WORKDIR /app
COPY . ./
WORKDIR /app/ApacheIgniteExample
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM grupoxp.azurecr.io/xp-aspnetcore:latest AS runtime
RUN apk add tzdata
RUN cp /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime
RUN echo "America/Sao_Paulo" >  /etc/timezone
RUN apk add icu-libs

# https://www.abhith.net/blog/docker-sql-error-on-aspnet-core-alpine/
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
WORKDIR /app
RUN addgroup -S app && adduser -S app -G app
USER app
COPY --from=build-env /app/Clear.Service.AssetGroup.Api/out .
ENTRYPOINT ["dotnet", "ApacheIgniteExample.dll"]
#CMD tail -f /dev/null
