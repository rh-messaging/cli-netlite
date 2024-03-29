#!/bin/bash

TARGET_DIR=./dist/netlite

# Create directory for finished product
rm -rf $TARGET_DIR
mkdir -p $TARGET_DIR

# Restore dependency packages
curl -LO "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
./nuget.exe restore src/dotNet/ClientLib/packages.config -PackagesDirectory packages -ConfigFile NuGet.Config

# Build the projects
/cygdrive/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe /p:Configuration=Release /p:TargetFrameworkVersion=v4.7.2 src/dotNet/ClientLib/ClientLib.csproj
/cygdrive/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe /p:Configuration=Release /p:TargetFrameworkVersion=v4.7.2 src/dotNet/Connector/Connector.csproj
/cygdrive/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe /p:Configuration=Release /p:TargetFrameworkVersion=v4.7.2 src/dotNet/Receiver/Receiver.csproj
/cygdrive/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe /p:Configuration=Release /p:TargetFrameworkVersion=v4.7.2 src/dotNet/Sender/Sender.csproj

# Create new zip file including all necessary bin files
cp ./src/dotNet/ClientLib/bin/Release/Amqp.net.dll $TARGET_DIR
cp ./src/dotNet/ClientLib/bin/Release/Newtonsoft.Json.dll $TARGET_DIR
cp ./src/dotNet/ClientLib/bin/Release/ClientLib.dll $TARGET_DIR
cp ./src/dotNet/Connector/bin/Release/cli-netlite-connector.exe $TARGET_DIR
cp ./src/dotNet/Receiver/bin/Release/cli-netlite-receiver.exe $TARGET_DIR
cp ./src/dotNet/Sender/bin/Release/cli-netlite-sender.exe $TARGET_DIR

