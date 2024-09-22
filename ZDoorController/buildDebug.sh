#!/bin/bash
git pull
dotnet build ZDoorController.Interface.App/ZDoorController.Interface.App.csproj -c Debug -r linux-arm
dotnet ZDoorController.Interface.App/bin/Debug/net8.0/linux-arm/ZDoorController.Interface.App.dll
