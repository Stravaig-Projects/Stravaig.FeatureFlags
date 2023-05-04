$packagesDir = "$PSScriptRoot/packages"
$project = [System.IO.Path]::GetFullPath("$PSScriptRoot/../src/Stravaig.FeatureFlags/Stravaig.FeatureFlags.csproj");

#$now = (Get-Date);
#$version = $now.ToString("yy.M.d");
#$suffix = $now.ToString("HHmmss");
$version = "0.0.0";
$suffix = "000000";


if ((Test-Path $packagesDir) -eq $false)
{
    New-Item $packagesDir -ItemType Directory
}

& dotnet pack $project --configuration Release --output $packagesDir --include-symbols --include-source /p:VersionPrefix="$version" --version-suffix "example-$suffix" -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
