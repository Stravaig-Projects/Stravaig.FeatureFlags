# stravaig-template

## TO DO

First install the githooks, ensure powershell 7.x is installed and then run `Install-GitHooks.ps1` (PowerShell 7.2 won't work Windows https://github.com/PowerShell/PowerShell/issues/16480). Any work perfomed in feature branches must have a related _issue_ associated with them. The branch name will have to be prefixed with the issue number, followed by a slash then the branch name. (e.g. `#123/my-feature-branch`)

The following then needs to be updated:

* Rename files:
  * Rename file `/src/Stravaig.XXXX.sln` (XXXX = name of the solution within the `Stravaig` namespace)
  * Rename file `/src/Stravaig.XXXX.sln.DotSettings` (XXXX = name of the solution within the `Stravaig` namespace)
  * Rename folder `/src/.idea/.idea.XXXX` (XXXX = name of solution without the file extension)
* Add project files
  * Add the initial main project that will be packaged and tests to the solution.
  * Move the `/src/stravaig-icon.png` file into the package project folder.
  * Update the main project `.csproj` file with the details in the "package details" (below)
* Update the `.github/workflows/build.yml` file with
  * The name of the project (line 1)
  * The environment variables in `jobs` \ `build` \ `env` to point to the new solution project and tests

## Package Details

This should be added to the main `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <StravaigBuildTime>$([System.DateTime]::Now.ToString("dddd, d MMMM yyyy 'at' HH:mm:ss zzzz"))</StravaigBuildTime>
        <StravaigCopyrightYear>$([System.DateTime]::Now.ToString("yyyy"))</StravaigCopyrightYear>
        <StravaigGitHubCommit>$(GITHUB_SHA)</StravaigGitHubCommit>
        <StravaigWorkflowUrl>$(GITHUB_SERVER_URL)/$(GITHUB_REPOSITORY)/actions/runs/$(GITHUB_RUN_ID)</StravaigWorkflowUrl>
        <StravaigReleaseNotes>https://github.com/$(GITHUB_REPOSITORY)/releases/tag/$(STRAVAIG_RELEASE_TAG)</StravaigReleaseNotes>
    </PropertyGroup>

    <PropertyGroup>
        <TargetFrameworks>netstandard2.0;net5.0</TargetFrameworks>
        <Title>Stravaig XXXX</Title>
        <Authors>Colin Angus Mackay</Authors>
        <Copyright>©2020-$(StravaigCopyrightYear) Stravaig Projects. See licence for more information.</Copyright>
        <PackageProjectUrl>https://github.com/$(GITHUB_REPOSITORY)/blob/$(StravaigGitHubCommit)/README.md</PackageProjectUrl>
        <PackageLicenseExpression>MIT</PackageLicenseExpression>
        <RepositoryUrl>https://github.com/$(GITHUB_REPOSITORY)</RepositoryUrl>
        <PackageIcon>stravaig-icon.png</PackageIcon>
        <PackageTags>XXXX</PackageTags>
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
        <Description>XXXX.
        
Built on $(StravaigBuildTime).
Build run details at: $(StravaigWorkflowUrl)
Release notes at: $(StravaigReleaseNotes)
        </Description>
    </PropertyGroup>

    <ItemGroup>
        <None Include="stravaig-icon.png" Pack="true" PackagePath="/" />
    </ItemGroup>

    <!-- Other things here -->
</Project>
```

Any part with `XXXX` should be replaced with appropriate information.

---

## Contributing / Getting Started

* Ensure you have PowerShell 7.1.x or higher installed
* At a PowerShell prompt
    * Navigate to the root of this repository
    * Run `./Install-GitHooks.ps1`
