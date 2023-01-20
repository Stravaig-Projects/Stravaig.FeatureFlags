function Invoke-Process {
    # Based on https://www.powershellgallery.com/packages/Invoke-Process/1.4/Content/Invoke-Process.ps1
    [CmdletBinding(SupportsShouldProcess)]
    param
    (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string]$FilePath,

        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [string]$ArgumentList
    )

    $ErrorActionPreference = 'Stop'

    $tempDir = $env:TEMP
    if ($null -eq $tempDir)
    {
        if ($PSVersionTable.Platform -eq "Unix")
        {
            $tempDir="/tmp"
        }
    }

    try {
        $tempPrefix = "$tempDir/$((New-Guid).Guid)"
        $stdOutTempFile = "$tempPrefix-out.txt"
        $stdErrTempFile = "$tempPrefix-err.txt"

        $startProcessParams = @{
            FilePath               = $FilePath
            ArgumentList           = $ArgumentList
            RedirectStandardError  = $stdErrTempFile
            RedirectStandardOutput = $stdOutTempFile
            Wait                   = $true;
            PassThru               = $true;
            NoNewWindow            = $true;
        }
        if ($PSCmdlet.ShouldProcess("Process [$($FilePath)]", "Run with args: [$($ArgumentList)]")) {
            $cmd = Start-Process @startProcessParams
            $cmdOutput = Get-Content -Path $stdOutTempFile -Raw
            $cmdError = Get-Content -Path $stdErrTempFile -Raw
            if ($cmd.ExitCode -ne 0) {
                if ($cmdError) {
                    throw $cmdError.Trim()
                }
                if ($cmdOutput) {
                    throw $cmdOutput.Trim()
                }
            } else {
                if ([string]::IsNullOrEmpty($cmdOutput) -eq $false) {
                    return $cmdOutput.Trim();
                }
            }
        }
    } catch {
        $PSCmdlet.ThrowTerminatingError($_)
    } finally {
        Remove-Item -Path $stdOutTempFile, $stdErrTempFile -Force -ErrorAction Ignore
    }
}

Export-ModuleMember -Function Invoke-Process