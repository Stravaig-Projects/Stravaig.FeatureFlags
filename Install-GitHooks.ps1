#!/usr/bin/env pwsh

if ($PSVersionTable.Platform -eq "Unix")
{
    Write-Output "chmod u+x `"$PSScriptRoot/.stravaig/gitHooks/commit-msg`""
    chmod u+x "$PSScriptRoot/.stravaig/gitHooks/commit-msg"    
}
	
git config --local core.hooksPath "./.stravaig/gitHooks"