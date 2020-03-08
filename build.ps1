#!/usr/bin/env pwsh

param (
    [string]$Configuration = "Debug",
    [string]$Verbosity = "m",
    [string[]]$RuntimeIdentifiers = ("win10-x64", "osx.10.14-x64", "linux-x64"),
    [bool]$Test = $true
)

$ErrorActionPreference = "Stop"

$ArtifactsPath = Join-Path -Path $PSScriptRoot -ChildPath _artifacts

[int]$Global:SectionCount = 0

function Write-Section($message) {
    if ($Global:SectionCount++ -gt 0) {
        Write-Host
    }
    $bar = "=" * $message.Length
    Write-Host -ForegroundColor green $message
    Write-Host -ForegroundColor green $bar
}

Write-Section "Restoring Packages"
& dotnet restore -v $Verbosity
if ($LastExitCode -ne 0) {
    exit $LastExitCode
}

Write-Section "Primary Build ($Configuration)"
& dotnet build -c $Configuration -v $Verbosity --no-restore
if ($LastExitCode -ne 0) {
    exit $LastExitCode
}

foreach ($rid in $RuntimeIdentifiers) {
    Write-Section "Publishing for $rid ($Configuration)"
    & dotnet publish -c $Configuration -r $rid -v $Verbosity
    if ($LastExitCode -ne 0) {
        exit $LastExitCode
    }

    $VersionFile = Join-Path -Path $ArtifactsPath -ChildPath VERSION
    $Version = (Get-Content $VersionFile -Raw).Trim()

    $PublishPath = Join-Path `
        -Path src `
        -ChildPath Latham.CommandLine `
        -AdditionalChildPath bin,$Configuration,netcoreapp3.1,$rid,publish

    $ArtifactsPublishPath = Join-Path `
        -Path $ArtifactsPath `
        -ChildPath "latham-${Version}_${rid}"

    $ArtifactsPublishZipPath = "${ArtifactsPublishPath}.zip"

    if (Test-Path -Path $ArtifactsPublishPath) {
        Remove-Item -Path $ArtifactsPublishPath -Recurse
    }

    Move-Item -Path $PublishPath -Destination $ArtifactsPublishPath

    if (Test-Path -Path $ArtifactsPublishZipPath) {
        Remove-Item -Path $ArtifactsPublishZipPath
    }

    Compress-Archive `
        -Path $ArtifactsPublishPath `
        -DestinationPath $ArtifactsPublishZipPath
}

Write-Section "Packaging NuGets"

& dotnet pack -c $Configuration -v $Verbosity --no-restore --no-build
if ($LastExitCode -ne 0) {
    exit $LastExitCode
}

if ($Test) {
    Write-Section "Running Tests"
    & dotnet test -c $Configuration -v $Verbosity --no-restore --no-build
    if ($LastExitCode -ne 0) {
        exit $LastExitCode
    }
}