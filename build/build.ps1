properties {
    $baseDir = resolve-path ..
    $srcDir = "$baseDir\src"
    $slnPath = "$srcDir\WebDav.sln"
    $releaseDir = "$baseDir\release"
    $global:config = "release"
}

task default -depends build, test, pack

task build {
    exec { dotnet build $slnPath -c $config  /v:m /nologo }
}

task test -depends Build {
    exec { dotnet test $srcDir\WebDav.Client.Tests\WebDav.Client.Tests.csproj -c $config /nologo }
}

task pack {
    exec { dotnet pack $srcDir\WebDav.Client\WebDav.Client.csproj -c $config -o $releaseDir --no-build }
}