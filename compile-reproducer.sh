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

# Search and replace Package AMQPNetLite.Core with AMQP.dll
sed -i 's/PackageReference Include="AMQPNetLite.Core" Version="2.3.0/Reference Include="AMQP.dll/g' $REPRODUCER_DIR/TcpKeepAliveSettings.csproj

# Change directory
cd $TARGET_DIR

# Download dotnet core client
if ! wget -q -t 10 "http://download-node-02.eng.bos.redhat.com/devel/candidates/amq/AMQ-CLIENTS-$CLI_VER$CLI_BUILD/amq-clients-$CLI_VER-dotnet-core.zip"; then
	echo "Could not download dotnet-core client version"
	exit 1
fi

# Unzip dotnet core client
unzip -o amq-clients-$CLI_VER-dotnet-core.zip
rm -f amq-clients-$CLI_VER-dotnet-core.zip

# Copy AMQP.dll lib into the reproducers dir
cp amq-clients-$CLI_VER-dotnet-core/bin/netstandard2.0/AMQP.dll $REPRODUCER_DIR

# Delete dotnet core client
rm -rf amq-clients-$CLI_VER-dotnet-core

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

# Generate checksums
# ARTIFACT_MD5_CHECKSUM=$(md5sum TcpKeepAliveSettings-reproducer-2.9.0.zip | awk '{print $1}')
# ARTIFACT_SHA1_CHECKSUM=$(shasum -a 1 TcpKeepAliveSettings-reproducer-2.9.0.zip | awk '{ print $1 }')

# Upload to artifactory
# curl --header "X-Checksum-MD5:${ARTIFACT_MD5_CHECKSUM}" --header "X-Checksum-Sha1:${ARTIFACT_SHA1_CHECKSUM}" -u anonymous: -X PUT "http://messaging-qe-repo.usersys.redhat.com:8081/artifactory/cli-netlite/reproducers/" -T "TcpKeepAliveSettings-reproducer-$CLI_VER.zip"
curl -u anonymous: -X PUT "http://messaging-qe-repo.usersys.redhat.com:8081/artifactory/cli-netlite/reproducers/" -T "TcpKeepAliveSettings-reproducer-$CLI_VER.zip"

