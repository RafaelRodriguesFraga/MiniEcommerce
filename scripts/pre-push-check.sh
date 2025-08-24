#!/bin/bash

export DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION=true

dotnet test AuthService/AuthService.Test/AuthService.Test.csproj --no-build --logger "console;verbosity=detailed" --consoleLoggerParameters:ForceConsoleColor=true
dotnet test CustomerService/CustomerService.Tests/CustomerService.Tests.csproj --no-build --logger "console;verbosity=detailed" --consoleLoggerParameters:ForceConsoleColor=true
