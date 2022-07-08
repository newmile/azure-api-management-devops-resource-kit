@ECHO OFF
cd .\%1
dotnet build -c Release
dotnet pack -c Release
@ECHO OFF
CD src/ArmTemplates/bin/Release
FOR /F "delims=|" %%I IN ('DIR "*.nupkg" /B /O:D') DO SET NugetPackage=%%I
@ECHO ON
dotnet nuget push -s "newmile" --api-key az .\%NugetPackage% 
@ECHO OFF
CD ..\..\..
@ECHO ON