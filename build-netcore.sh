#!/bin/bash                                                                                                               

set -Exeuo pipefail
                                                                                                                           
TARGET_DIR=./dist/netcore

# Create directory for finished product
rm -rf $TARGET_DIR
mkdir -p $TARGET_DIR                                                                                                    


# Compile the projects                                                                                                     
dotnet build -f netcoreapp3.1 -c Release ./src/dotNetCore/NetCoreSender/NetCoreSender.csproj                               
dotnet build -f netcoreapp3.1 -c Release ./src/dotNetCore/NetCoreReceiver/NetCoreReceiver.csproj                           
dotnet build -f netcoreapp3.1 -c Release ./src/dotNetCore/NetCoreConnector/NetCoreConnector.csproj
                       
# Publish the projects
dotnet publish -f netcoreapp3.1 -c Release ./src/dotNetCore/NetCoreSender/NetCoreSender.csproj                             
dotnet publish -f netcoreapp3.1 -c Release ./src/dotNetCore/NetCoreReceiver/NetCoreReceiver.csproj                         
dotnet publish -f netcoreapp3.1 -c Release ./src/dotNetCore/NetCoreConnector/NetCoreConnector.csproj

# Create new zip file including all necessary bin files                                                                    
cp ./src/dotNetCore/NetCoreSender/bin/Release/netcoreapp3.1/publish/*.dll $TARGET_DIR
cp ./src/dotNetCore/NetCoreSender/bin/Release/netcoreapp3.1/publish/*.runtimeconfig.json $TARGET_DIR
cp ./src/dotNetCore/NetCoreSender/bin/Release/netcoreapp3.1/publish/cli-netlite-core-sender $TARGET_DIR
cp ./src/dotNetCore/NetCoreReceiver/bin/Release/netcoreapp3.1/publish/*.dll $TARGET_DIR
cp ./src/dotNetCore/NetCoreReceiver/bin/Release/netcoreapp3.1/publish/*.runtimeconfig.json $TARGET_DIR
cp ./src/dotNetCore/NetCoreReceiver/bin/Release/netcoreapp3.1/publish/cli-netlite-core-receiver $TARGET_DIR
cp ./src/dotNetCore/NetCoreConnector/bin/Release/netcoreapp3.1/publish/*.dll $TARGET_DIR
cp ./src/dotNetCore/NetCoreConnector/bin/Release/netcoreapp3.1/publish/*.runtimeconfig.json $TARGET_DIR
cp ./src/dotNetCore/NetCoreConnector/bin/Release/netcoreapp3.1/publish/cli-netlite-core-connector $TARGET_DIR

