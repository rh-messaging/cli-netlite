#! /bin/bash

usage() {
        echo "usage: $(basename "$0") [-h|--help] <clients_version> <netcoreapp_version> [clients_build]"
}

if [ $# -lt 2 ] || [[ "$@" = *"-h"* ]]; then
        usage
        exit 0
fi

BASE_DIR=/tmp/tkas-reproducer-env
REPRODUCER_DIR=TcpKeepAliveSettings-reproducer/TcpKeepAliveSettings
CLI_VER=$1
NETCOREAPP_VER=$2
CLI_BUILD=${3:-""}

if [[ "$CLI_BUILD" != "" ]]; then
	CLI_BUILD=".$CLI_BUILD"
fi

# Create isolated environment
mkdir $BASE_DIR

# Copy reproducer to temporary environment
cp -r TcpKeepAliveSettings-reproducer/ $BASE_DIR

# Search and replace Package AMQPNetLite.Core with AMQP.dll
sed -i 's/PackageReference Include="AMQPNetLite.Core" Version="2.3.0/Reference Include="AMQP.dll/g' $BASE_DIR/$REPRODUCER_DIR/TcpKeepAliveSettings.csproj

# Change directory to isolated environment
cd $BASE_DIR

# Download dotnet core client
if ! wget -q -t 10 "http://download-node-02.eng.bos.redhat.com/devel/candidates/amq/AMQ-CLIENTS-$CLI_VER$CLI_BUILD/amq-clients-$CLI_VER-dotnet-core.zip"; then
	echo "Could not download dotnet-core client version"
	exit 1
fi

# Unzip dotnet core client
unzip -o $BASE_DIR/amq-clients-$CLI_VER-dotnet-core.zip
rm -f $BASE_DIR/amq-clients-$CLI_VER-dotnet-core.zip

# Copy AMQP.dll lib into the reproducers dir
cp $BASE_DIR/amq-clients-$CLI_VER-dotnet-core/bin/netstandard2.0/AMQP.dll $BASE_DIR/$REPRODUCER_DIR/

# Compile the files
dotnet build -f netcoreapp$NETCOREAPP_VER $BASE_DIR/$REPRODUCER_DIR/TcpKeepAliveSettings.csproj

# Create new zip file including all necessary bin files
cd $BASE_DIR/$REPRODUCER_DIR/bin/Debug/netcoreapp$NETCOREAPP_VER
mkdir TcpKeepAliveSettings-reproducer-$CLI_VER
cp AMQP.dll TcpKeepAliveSettings TcpKeepAliveSettings.dll TcpKeepAliveSettings.runtimeconfig.json TcpKeepAliveSettings-reproducer-$CLI_VER
zip -r TcpKeepAliveSettings-reproducer-$CLI_VER\.zip TcpKeepAliveSettings-reproducer-$CLI_VER

# Generate checksums
# ARTIFACT_MD5_CHECKSUM=$(md5sum TcpKeepAliveSettings-reproducer-2.9.0.zip | awk '{print $1}')
# ARTIFACT_SHA1_CHECKSUM=$(shasum -a 1 TcpKeepAliveSettings-reproducer-2.9.0.zip | awk '{ print $1 }')

# Upload to artifactory
# curl --header "X-Checksum-MD5:${ARTIFACT_MD5_CHECKSUM}" --header "X-Checksum-Sha1:${ARTIFACT_SHA1_CHECKSUM}" -u anonymous: -X PUT "http://messaging-qe-repo.usersys.redhat.com:8081/artifactory/cli-netlite/reproducers/" -T "TcpKeepAliveSettings-reproducer-$CLI_VER.zip"
curl -u anonymous: -X PUT "http://messaging-qe-repo.usersys.redhat.com:8081/artifactory/cli-netlite/reproducers/" -T "TcpKeepAliveSettings-reproducer-$CLI_VER.zip"

# Delete the environment
cd && rm -rf $BASE_DIR
