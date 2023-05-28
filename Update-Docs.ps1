# Fix up the contributions for the docs.
$contrib = Get-Content "$PSScriptRoot/contributors.md"
for ($i = 0; $i -lt $contrib.Count; $i++) {
    $line = $contrib[$i];
    if ($line.Contains(":octocat:")) { $line = $line.Replace(":octocat:", "* ")}
    if ($line.Contains(":date:")) { $line = $line.Replace(":date:", "* ")}

    $contrib[$i] = $line
}

Set-Content -Path "$PSScriptRoot/docs/contributors.md" -Value $contrib -Encoding UTF8 -Force

# Set up the release notes docs.
Copy-Item "$PSScriptRoot/release-notes/release-notes-*.md" "$PSScriptRoot/docs/release-notes/"

$releaseNotesIndexFile = @(
"# Release Notes"
""
"The releases on this package most recent first."
""
);

Get-ChildItem "$PSScriptRoot/release-notes/release-notes*.md" |
    Sort-Object -Descending -Property Name |
    ForEach-Object -Process {
        $name = $_.Name;
        $releaseDate = $_.LastWriteTimeUtc.ToString("dddd, d MMMM yyyy 'at' HH:mm");
        $null = $name -match "release-notes-(?<version>[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})\.md";
        $version = $Matches.version;
        $releaseNotesIndexFile += "- **[v$version]($name) released on $releaseDate UTC**";
        $releaseNotesIndexFile += "  - [GitHub Release](https://github.com/Stravaig-Projects/<repo-name>/releases/tag/v$version)"
        $releaseNotesIndexFile += "  - [Nuget Package](https://www.nuget.org/packages/<package-name>/$version)"
    }

Set-Content "$PSScriptRoot/docs/release-notes/index.md" $releaseNotesIndexFile -Encoding UTF8 -Force