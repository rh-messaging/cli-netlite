#! /bin/bash

TARGET_DIR=./dist/tcpkeepalivesettings-reproducer
REPRODUCER_DIR=./TcpKeepAliveSettings-reproducer/TcpKeepAliveSettings

# Create directory for finished product
rm -rf $TARGET_DIR
mkdir -p $TARGET_DIR

# Compile the files
dotnet build -f netcoreapp3.1 -c Release $REPRODUCER_DIR/TcpKeepAliveSettings.csproj

# Publish TcpKeepAliveSettings reproducer
dotnet publish -f netcoreapp3.1 -c Release $REPRODUCER_DIR/TcpKeepAliveSettings.csproj

# Create new zip file including all necessary bin files
cp $REPRODUCER_DIR/bin/Release/netcoreapp3.1/AMQP.dll $TARGET_DIR
cp $REPRODUCER_DIR/bin/Release/netcoreapp3.1/TcpKeepAliveSettings $TARGET_DIR
cp $REPRODUCER_DIR/bin/Release/netcoreapp3.1/TcpKeepAliveSettings.dll $TARGET_DIR
cp $REPRODUCER_DIR/bin/Release/netcoreapp3.1/TcpKeepAliveSettings.runtimeconfig.json $TARGET_DIR

