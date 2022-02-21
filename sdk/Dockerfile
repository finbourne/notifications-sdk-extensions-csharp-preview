FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN mkdir -p /usr/src
WORKDIR /usr/src

COPY Finbourne.Notifications.Sdk.Extensions.sln /usr/src/
COPY Finbourne.Notifications.Sdk.Extensions/Finbourne.Notifications.Sdk.Extensions.csproj /usr/src/Finbourne.Notifications.Sdk.Extensions/
COPY Finbourne.Notifications.Sdk.Extensions.Tests/Finbourne.Notifications.Sdk.Extensions.Tests.csproj /usr/src/Finbourne.Notifications.Sdk.Extensions.Tests/
COPY Finbourne.Notifications.Sdk.Extensions.IntegrationTests/Finbourne.Notifications.Sdk.Extensions.IntegrationTests.csproj /usr/src/Finbourne.Notifications.Sdk.Extensions.IntegrationTests/
RUN dotnet restore Finbourne.Notifications.Sdk.Extensions

ENV FBN_NOTIFICATIONS_API_URL ${FBN_NOTIFICATIONS_API_URL}

# ENTRYPOINT [ "/bin/bash" ]
ENTRYPOINT dotnet test --configuration Release --verbosity normal Finbourne.Notifications.Sdk.Extensions