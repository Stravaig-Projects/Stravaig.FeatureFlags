Clear-Host;

$docsPath = "$PSScriptRoot/docs";
$sitePath = "$PSScriptRoot/_site";

Write-Host "& jekyll build --source `"$docsPath`" --destination `"$sitePath`" --verbose;"
& jekyll build --source "$docsPath" --destination "$sitePath" --verbose;

Write-Host "& jekyll serve --source `"$docsPath`" --destination `"$sitePath`" --port 4001;"
& jekyll serve --source "$docsPath" --destination "$sitePath" --port 4001;