#! /bin/bash

usage() {
        echo "usage: $(basename "$0") [-h|--help] <clients_version> [clients_build]"
}

if [ $# -lt 1 ] || [[ "$@" = *"-h"* ]]; then
        usage
        exit 0
fi

TARGET_DIR=./dist/tcpkeepalivesettings-reproducer
REPRODUCER_DIR=./TcpKeepAliveSettings-reproducer/TcpKeepAliveSettings
CLI_VER=$1
CLI_BUILD=${2:-""}

if [[ "$CLI_BUILD" != "" ]]; then
	CLI_BUILD=".$CLI_BUILD"
fi

# Create directory for finished product
rm -rf $TARGET_DIR
mkdir -p $TARGET_DIR

# Compile the files
dotnet build -f netcoreapp3.1 -c Release $REPRODUCER_DIR/TcpKeepAliveSettings.csproj

# Publish TcpKeepAliveSettings reproducer
dotnet publish -f netcoreapp3.1 -c Release $REPRODUCER_DIR/TcpKeepAliveSettings.csproj

# Create new zip file including all necessary bin files
mkdir $TARGET_DIR/TcpKeepAliveSettings-reproducer-$CLI_VER
cp $REPRODUCER_DIR/bin/Release/netcoreapp3.1/AMQP.dll $TARGET_DIR/TcpKeepAliveSettings-reproducer-$CLI_VER
cp $REPRODUCER_DIR/bin/Release/netcoreapp3.1/TcpKeepAliveSettings $TARGET_DIR/TcpKeepAliveSettings-reproducer-$CLI_VER
cp $REPRODUCER_DIR/bin/Release/netcoreapp3.1/TcpKeepAliveSettings.dll $TARGET_DIR/TcpKeepAliveSettings-reproducer-$CLI_VER
cp $REPRODUCER_DIR/bin/Release/netcoreapp3.1/TcpKeepAliveSettings.runtimeconfig.json $TARGET_DIR/TcpKeepAliveSettings-reproducer-$CLI_VER
cd $TARGET_DIR && zip -r TcpKeepAliveSettings-reproducer-$CLI_VER\.zip TcpKeepAliveSettings-reproducer-$CLI_VER

