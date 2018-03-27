#build dotnet core client
dotnet build -f netcoreapp2.0 ./NetCoreSender/NetCoreSender.csproj
dotnet build -f netcoreapp2.0 ./NetCoreReceiver/NetCoreReceiver.csproj
dotnet build -f netcoreapp2.0 ./NetCoreConnector/NetCoreConnector.csproj

dotnet publish -f netcoreapp2.0 ./NetCoreSender/NetCoreSender.csproj
dotnet publish -f netcoreapp2.0 ./NetCoreReceiver/NetCoreReceiver.csproj
dotnet publish -f netcoreapp2.0 ./NetCoreConnector/NetCoreConnector.csproj

rm ./dist/netcore -rf
mkdir ./dist/netcore -p
cp ./NetCoreSender/bin/Debug/netcoreapp2.0/publish/* ./dist/netcore
cp ./NetCoreReceiver/bin/Debug/netcoreapp2.0/publish/* ./dist/netcore
cp ./NetCoreConnector/bin/Debug/netcoreapp2.0/publish/* ./dist/netcore

cd dist/netcore && zip -r amqpnetlitecore.zip .
