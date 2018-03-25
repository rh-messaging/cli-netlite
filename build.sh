#build dotnet core client
dotnet build -f netcoreapp2.0 ./NetCoreSender/NetCoreSender.csproj
dotnet build -f netcoreapp2.0 ./NetCoreReceiver/NetCoreReceiver.csproj
dotnet build -f netcoreapp2.0 ./NetCoreConnector/NetCoreConnector.csproj

dotnet publish -f netcoreapp2.0 ./NetCoreSender/NetCoreSender.csproj -r rhel.7-x64
dotnet publish -f netcoreapp2.0 ./NetCoreReceiver/NetCoreReceiver.csproj -r rhel.7-x64
dotnet publish -f netcoreapp2.0 ./NetCoreConnector/NetCoreConnector.csproj -r rhel.7-x64

rm ./dist/netcore -rf
mkdir ./dist/netcore -p
cp ./NetCoreSender/bin/Debug/netcoreapp2.0/rhel.7-x64/* ./dist/netcore
cp ./NetCoreReceiver/bin/Debug/netcoreapp2.0/rhel.7-x64/* ./dist/netcore
cp ./NetCoreConnector/bin/Debug/netcoreapp2.0/rhel.7-x64/* ./dist/netcore

cd dist/netcore && zip -r amqpnetlitecore.zip .
