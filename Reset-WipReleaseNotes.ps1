$content = @(
"# Release Notes",
"",
"## Version X",
"",
"Date: ???",
""
"### Bugs"
"",
"### Features",
"",
"### Miscellaneous",
"",
"### Dependent Packages",
"",
"- .NET 5.0",
"  - No changes",
"- .NET Core 3.1",
"  - No changes",
"- General",
"  - No changes",
"",
"---",
"",
""
)

Set-Content "$PSScriptRoot/release-notes/wip-release-notes.md" $content -Encoding UTF8 -Force
