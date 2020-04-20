rem build complete client
SET MS_COMMUNITY_PATH="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64\MSBuild.exe"
SET MS_PRO_PATH="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\amd64\MSBuild.exe"
if exist %MS_COMMUNITY_PATH% (
    %MS_COMMUNITY_PATH% cli-netlite.sln /verbosity:minimal
) else (
    %MS_PRO_PATH% cli-netlite.sln /verbosity:minimal
)

rem publish netcore client
dotnet publish -f netcoreapp3.0 .\src\dotNetCore\NetCoreSender\NetCoreSender.csproj
dotnet publish -f netcoreapp3.0 .\src\dotNetCore\NetCoreReceiver\NetCoreReceiver.csproj
dotnet publish -f netcoreapp3.0 .\src\dotNetCore\NetCoreConnector\NetCoreConnector.csproj