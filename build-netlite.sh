#!/bin/bash

usage() {                                                                                                                  
        echo "usage: $(basename "$0") [-h|--help] <clients_version> [clients_build]"                  
}                                                                                                                          
                                                                                                                           
if [ $# -lt 1 ] || [[ "$@" = *"-h"* ]]; then                                                                               
        usage                                                                                                              
        exit 0                                                                                                             
fi                                                                                                                         
                                                                                                                                                                                                  
TARGET_DIR=./dist/netlite
CLI_VER=$1                                                                                                                 
CLI_BUILD=${2:-""}                                                                                                         
                                                                                                                           
if [[ "$CLI_BUILD" != "" ]]; then                                                                                          
    CLI_BUILD=".$CLI_BUILD"                                                                                                
fi                                                                                                                         

# Create directory for finished product
rm -rf $TARGET_DIR
mkdir -p $TARGET_DIR                                                                                                    

# Install dependency packages
# nuget install src/dotNet/ClientLib/packages.config -OutputDirectory packages
# 
# # Build the projects
# /cygdrive/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe /p:Configuration=Release /p:TargetFrameworkVersion=v4.7.2 src/dotNet/ClientLib/ClientLib.csproj
# /cygdrive/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe /p:Configuration=Release /p:TargetFrameworkVersion=v4.7.2 src/dotNet/Connector/Connector.csproj
# /cygdrive/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe /p:Configuration=Release /p:TargetFrameworkVersion=v4.7.2 src/dotNet/Receiver/Receiver.csproj
# /cygdrive/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe /p:Configuration=Release /p:TargetFrameworkVersion=v4.7.2 src/dotNet/Sender/Sender.csproj

dotnet build -f netcoreapp3.1 -c Release src/dotNet/ClientLib/ClientLib.csproj
dotnet build -f netcoreapp3.1 -c Release src/dotNet/Connector/Connector.csproj
dotnet build -f netcoreapp3.1 -c Release src/dotNet/Receiver/Receiver.csproj
dotnet build -f netcoreapp3.1 -c Release src/dotNet/Sender/Sender.csproj

# Publish all projects
dotnet publish -f netcoreapp3.1 -c Release src/dotNet/ClientLib/ClientLib.csproj
dotnet publish -f netcoreapp3.1 -c Release src/dotNet/Connector/Connector.csproj
dotnet publish -f netcoreapp3.1 -c Release src/dotNet/Receiver/Receiver.csproj
dotnet publish -f netcoreapp3.1 -c Release src/dotNet/Sender/Sender.csproj

# Create new zip file including all necessary bin files
cp ./src/dotNet/ClientLib/bin/Release/Amqp.net.dll $TARGET_DIR
cp ./src/dotNet/ClientLib/bin/Release/Newtonsoft.Json.dll $TARGET_DIR
cp ./src/dotNet/ClientLib/bin/Release/ClientLib.dll $TARGET_DIR
cp ./src/dotNet/Connector/bin/Release/cli-netlite-connector.exe $TARGET_DIR
cp ./src/dotNet/Receiver/bin/Release/cli-netlite-receiver.exe $TARGET_DIR
cp ./src/dotNet/Sender/bin/Release/cli-netlite-sender.exe $TARGET_DIR

tar -cf cli-netlite-$CLI_VER.tar $TARGET_DIR

