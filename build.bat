rem build complete client
SET MS_COMMUNITY_PATH="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64\MSBuild.exe"
SET MS_PRO_PATH="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\amd64\MSBuild.exe"
if exist %MS_COMMUNITY_PATH% (
    %MS_COMMUNITY_PATH% cli-netlite.sln /verbosity:minimal
) else (
    %MS_PRO_PATH% cli-netlite.sln /verbosity:minimal
)

rem publish netcore client
dotnet publish -f netcoreapp2.0 .\NetCoreSender\NetCoreSender.csproj -r win7-x64
dotnet publish -f netcoreapp2.0 .\NetCoreReceiver\NetCoreReceiver.csproj -r win7-x64
dotnet publish -f netcoreapp2.0 .\NetCoreConnector\NetCoreConnector.csproj -r win7-x64

dotnet publish -f netcoreapp2.0 .\NetCoreSender\NetCoreSender.csproj -r rhel.7-x64
dotnet publish -f netcoreapp2.0 .\NetCoreReceiver\NetCoreReceiver.csproj -r rhel.7-x64
dotnet publish -f netcoreapp2.0 .\NetCoreConnector\NetCoreConnector.csproj -r rhel.7-x64