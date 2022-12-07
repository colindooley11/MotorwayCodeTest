FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

WORKDIR /
# copy sln and csproj files into the image
COPY *.sln .
COPY /src/OrderFraudCheck/*.csproj /src/OrderFraudCheck/
COPY /tests/OrderFraudCheck.UnitTests/*.csproj /tests/OrderFraudCheck.UnitTests/

COPY . .

# create a new build target called testrunner
FROM build
# navigate to the unit test directory
WORKDIR /tests/OrderFraudCheck.UnitTests
# when you run this build target it will run the unit tests
CMD ["dotnet", "test", "--logger:trx"]

