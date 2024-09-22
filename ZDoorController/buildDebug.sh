#!/bin/bash
git pull
dotnet build ZDoorController.Interface.App/ZDoorController.Interface.App.csproj -c Debug
dotnet ZDoorController.Interface.App/bin/Debug/net8.0/ZDoorController.Interface.App.dll
