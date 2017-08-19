properties {
    $version = "1.1.0.0"
    $signAssemblies = $false
    $signKeyPath = "C:\snkey\skazantsev.snk"

    $baseDir = resolve-path ..
    $srcDir = "$baseDir\src"
    $slnFile = "$srcDir\WebDav.sln"
    $releaseDir = "$baseDir\release"
    $nugetPath = "$srcDir\.nuget\nuget.exe"
    $xunitPath = "$srcDir\packages\xunit.runner.console.2.1.0\tools\xunit.console.exe"
}

task default -depends Build, RunTests, CreateNuGetPackage

task Build -depends RestorePackages {
    Update-AssemblyInfoVersion $srcDir $version

    $constants = "TRACE"
    if ($signAssemblies -eq $true) {
        $constants += ";SIGNED"
    }
    exec { msbuild /nologo $slnFile "/t:Clean;Rebuild" /p:Configuration=Release /p:TargetFrameworkVersion=v4.5 /p:DefineConstants=`"$constants`" "/p:SignAssembly=$signAssemblies" "/p:AssemblyOriginatorKeyFile=$signKeyPath"  }
}

task RestorePackages {
    exec { . $nugetPath restore $slnFile }
}

task RunTests -depends Build {
    exec {. $xunitPath "$srcDir\WebDav.Client.Tests\bin\Release\WebDav.Client.Tests.dll" }
}

task CreateNuGetPackage -depends Build, RunTests {
    mkdir "$releaseDir\lib\net45" -force | Out-Null
    rm "$releaseDir\lib\net45\*"
    cp "$srcDir\WebDav.Client\bin\Release\WebDav.Client.dll" "$releaseDir\lib\net45"
    cp "$baseDir\build\WebDav.Client.nuspec" "$releaseDir"

    $nuspec = [xml](cat "$releaseDir\WebDav.Client.nuspec" -encoding UTF8)
    $nuspec.package.metadata.version = $version

    write-host "Creating a nuget package with nuspec file:"
    write-host $nuspec.OuterXml

    $nuspec.Save("$releaseDir\WebDav.Client.nuspec")

    exec { . $nugetPath pack "$releaseDir\WebDav.Client.nuspec" -BasePath "$releaseDir" -OutputDirectory "$releaseDir" }

    if ($signAssemblies -eq $true) {
        write-host "Created signed package!" -foreground green
    } else {
        write-host "[WARN] Created package is not signed!" -foreground yellow
    } 
}

function Update-AssemblyInfoVersion([string] $srcDir, [string] $version) {
    get-childitem -path $srcDir -recurse -filter AssemblyInfo.cs | foreach {
        $filename = $_.FullName
        write-host "Setting $version version for $filename"

        (cat $filename -encoding UTF8) `
            -replace "AssemblyVersion\(""[0-9]+(\.([0-9]+|\*)){1,3}""\)", "AssemblyVersion(""$version"")" `
            -replace "AssemblyFileVersion\(""[0-9]+(\.([0-9]+|\*)){1,3}""\)", "AssemblyFileVersion(""$version"")" `
        | out-file $filename -encoding UTF8
    }
}