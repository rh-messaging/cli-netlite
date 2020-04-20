#build dotnet core client
dotnet build -f netcoreapp3.0 -c Release ./src/dotNetCore/NetCoreSender/NetCoreSender.csproj
dotnet build -f netcoreapp3.0 -c Release ./src/dotNetCore/NetCoreReceiver/NetCoreReceiver.csproj
dotnet build -f netcoreapp3.0 -c Release ./src/dotNetCore/NetCoreConnector/NetCoreConnector.csproj

dotnet publish -f netcoreapp3.0 -c Release ./src/dotNetCore/NetCoreSender/NetCoreSender.csproj
dotnet publish -f netcoreapp3.0 -c Release ./src/dotNetCore/NetCoreReceiver/NetCoreReceiver.csproj
dotnet publish -f netcoreapp3.0 -c Release ./src/dotNetCore/NetCoreConnector/NetCoreConnector.csproj

rm ./dist/netcore -rf
mkdir ./dist/netcore -p
cp ./src/dotNetCore/NetCoreSender/bin/Release/netcoreapp3.0/publish/*.dll ./dist/netcore
cp ./src/dotNetCore/NetCoreSender/bin/Release/netcoreapp3.0/publish/*.runtimeconfig.json ./dist/netcore
cp ./src/dotNetCore/NetCoreSender/bin/Release/netcoreapp3.0/publish/cli-netlite-core-sender ./dist/netcore
cp ./src/dotNetCore/NetCoreReceiver/bin/Release/netcoreapp3.0/publish/*.dll ./dist/netcore
cp ./src/dotNetCore/NetCoreReceiver/bin/Release/netcoreapp3.0/publish/*.runtimeconfig.json ./dist/netcore
cp ./src/dotNetCore/NetCoreReceiver/bin/Release/netcoreapp3.0/publish/cli-netlite-core-receiver ./dist/netcore
cp ./src/dotNetCore/NetCoreConnector/bin/Release/netcoreapp3.0/publish/*.dll ./dist/netcore
cp ./src/dotNetCore/NetCoreConnector/bin/Release/netcoreapp3.0/publish/*.runtimeconfig.json ./dist/netcore
cp ./src/dotNetCore/NetCoreConnector/bin/Release/netcoreapp3.0/publish/cli-netlite-core-connector ./dist/netcore

cd dist/netcore && zip -r amqpnetlitecore.zip .
