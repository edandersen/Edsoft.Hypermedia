set MSBuildDirPath=C:\Windows\Microsoft.NET\Framework\v4.0.30319\
@echo off
%MSBuildDirPath%msbuild.exe src\Edsoft.Hypermedia\Edsoft.Hypermedia.csproj /t:Clean;Rebuild /property:Configuration=Release /clp:ErrorsOnly
%MSBuildDirPath%msbuild.exe tools\Edsoft.Hypermedia.Benchmark\Edsoft.Hypermedia.Benchmark.csproj /t:Clean;Rebuild /p:Configuration=Release /clp:ErrorsOnly

tools\Edsoft.Hypermedia.Benchmark\bin\Release\Edsoft.Hypermedia.Benchmark.exe deserializer ^
    --iterations 5 --filepath tests\Edsoft.Hypermedia.Tests\Integration\TestData\Hal\AllLinkObjectProperties.json
