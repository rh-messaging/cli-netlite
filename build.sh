#build dotnet core client
dotnet build -f netcoreapp3.0 ./src/dotNetCore/NetCoreSender/NetCoreSender.csproj
dotnet build -f netcoreapp3.0 ./src/dotNetCore/NetCoreReceiver/NetCoreReceiver.csproj
dotnet build -f netcoreapp3.0 ./src/dotNetCore/NetCoreConnector/NetCoreConnector.csproj

dotnet publish -f netcoreapp3.0 ./src/dotNetCore/NetCoreSender/NetCoreSender.csproj
dotnet publish -f netcoreapp3.0 ./src/dotNetCore/NetCoreReceiver/NetCoreReceiver.csproj
dotnet publish -f netcoreapp3.0 ./src/dotNetCore/NetCoreConnector/NetCoreConnector.csproj

rm ./dist/netcore -rf
mkdir ./dist/netcore -p
cp ./src/dotNetCore/NetCoreSender/bin/Debug/netcoreapp3.0/publish/* ./dist/netcore
cp ./src/dotNetCore/NetCoreReceiver/bin/Debug/netcoreapp3.0/publish/* ./dist/netcore
cp ./src/dotNetCore/NetCoreConnector/bin/Debug/netcoreapp3.0/publish/* ./dist/netcore

cd dist/netcore && zip -r amqpnetlitecore.zip .
