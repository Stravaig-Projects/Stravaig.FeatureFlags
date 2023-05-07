$packagesDir = "$PSScriptRoot/packages"
$project = [System.IO.Path]::GetFullPath("$PSScriptRoot/../src/Stravaig.FeatureFlags/Stravaig.FeatureFlags.csproj");

function GetIndent([string]$file)
{
    $indent = "";
    for($i = 0; $i -lt $file.Length; $i++) {
        if ($file[$i] -eq '/')
        {
            $indent += "  |";
        }
    }
    return $indent;
}

$now = (Get-Date);
$version = $now.ToString("yy.M.d");
$suffix = $now.ToString("HHmmss");
#$version = "0.0.0";
#$suffix = "000000";


if ((Test-Path $packagesDir) -eq $false)
{
    New-Item $packagesDir -ItemType Directory
}

& dotnet pack $project --configuration Release --output $packagesDir --include-symbols --include-source /p:VersionPrefix="$version" --version-suffix "example-$suffix" -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

$tempDir = [System.IO.Path]::GetTempPath() + "Stravaig-" + (New-Guid);
$null = New-Item $tempDir -ItemType Directory;

Expand-Archive -Path "$packagesDir/Stravaig.FeatureFlags.$version-example-$suffix.nupkg" -DestinationPath $tempDir

Get-Content -Path "$tempDir/Stravaig.FeatureFlags.nuspec"

$files = Get-ChildItem $tempDir -Recurse |
            Sort-Object -Property FullName

Write-Host "Package Root"
foreach($file in $files)
{
    $name = $file.FullName;
    $isDir = $file.PSIsContainer;
    $name = $name.Substring($tempDir.Length)
    $indent = GetIndent($name)
    if ($isDir){
        $indent = $indent + ">-+ ";
    }
    else
    {
        $indent = $indent + " ";
    }
    
    $name = $name.Substring($name.LastIndexOf("/")+1);
    Write-Host "$indent $name"
}

Get-ChildItem $tempDir -Recurse | Sort-Object -Property FullName -Descending | Remove-Item -Force -Recurse
