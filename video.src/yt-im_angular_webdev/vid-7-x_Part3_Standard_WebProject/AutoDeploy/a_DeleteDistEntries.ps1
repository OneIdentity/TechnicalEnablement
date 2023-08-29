# ******************************************************
# ** One Identity Technical Enablement Script Automation
# ** - Delete files from dist folder -
# ** Last change: 2023-08-16, by HRA
# ******************************************************

### ---------------- FUNCTIONS -------------------###
function Write-MSG {
    # Write-MSG 'MSG' "Some Message Text"
    param (
        [parameter(Mandatory = $true)][string] $MyMSGtype,
        [parameter(Mandatory = $false)][string] $MyMessage,
        [parameter(Mandatory = $false)][bool] $MyISsystemError = $false
    )
  
    #Possible Options: 
    #  ERR=Error, NFO=Info, WRN=Warning, NLN=New empty line, 
    #  TTL=Title, STL=Sub-title, OUT=Unformated output, 
    switch ($MyMSGtype.ToUpper()) {
        'NFO' {
            Write-Host "  --> INFO: $MyMessage"
        }
  
        'WRN' {
            Write-Host "  ==> WARNING: $MyMessage" -ForegroundColor yellow
        }
  
        'ERR' {
            if ($MyISsystemError) {
                Write-Host "  ==> Script: $Myinvocation.MyCommand.Name : ", $MyMessage -ForegroundColor red
            }
            else {
                Write-Host "  ==> ERROR: $MyMessage" -ForegroundColor red
            }
        }
  
        'NLN' {
            Write-Host ""
        }
  
        'OUT' {
            Write-Host $MyMessage
        }
  
        'TTL' {
            Write-Host $MyMessage -ForegroundColor Green
        }
  
        'STL' {
            Write-Host $MyMessage -ForegroundColor Cyan
        }
  
        else {
            Write-Host "  ==> $MyMessage" -ForegroundColor red
        }
    }
}
  
function Get-NumberOfFileElements {
    param (
        [parameter(Mandatory = $true)][String]$MyFolderPath
    )
    if (test-path -path $MyFolderPath) {
        $MyCount = $(Get-Childitem -path $MyFolderPath -File | Measure-Object).count
    }
    else {
        $MyCount = 0
    }
    return $MyCount
}
  
function Get-NumberOfFolderElements {
    param (
        [parameter(Mandatory = $true)][String]$MyFolderPath
    )
    if (test-path -path $MyFolderPath) {
        $MyCount = $(Get-Childitem -path $MyFolderPath -Directory | Measure-object).count
    }
    else {
        $MyCount = 0
    }
    return $MyCount
}
  
### ---------------- MAIN -------------------###
Write-MSG 'NLN'
Write-MSG 'TTL' '******************************************************'
Write-MSG 'TTL' '** One Identity Technical Enablement Script Automation'
Write-MSG 'TTL' '** - Delete Files from Dist Folder -'  
Write-MSG 'TTL' '******************************************************'
  
#--> Path to this script
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
  
try {
    $IsError = $false
    $deploymentfolder = ".autodeploy"
    $distributionSubPath = "bin\imxweb\custom" 
    $deploymentPath = "$scriptPath\$deploymentfolder\$distributionSubPath"
    $distributionPath = "$scriptPath\imxweb\dist"

    # --> Create or delete content in the deployment folder. Execution stops if sub-folder are detected (no reason for)
    if (-not (Test-Path $deploymentPath)) {
        New-Item -Path $deploymentPath -Force -ItemType Directory
    }
    else {
        if ($(Get-NumberOfFolderElements -MyFolderPath $deploymentPath) -gt 0 ) {
            Write-MSG 'ERR' "Sub-folder found in deployment folder: ""$deploymentPath"" where no sub-folder should exist. You may like to remove them!"
            exit
        }
        else {
            $numberFiles = Get-NumberOfFileElements -MyFolderPath $deploymentPath
            if ( $numberFiles -gt 0 ) {
                Remove-Item -Path "$deploymentPath\*.*" -Force -recurse
                write-msg 'NFO' "All $numberFiles files in ""$deploymentPath"" deleted!"
            }
            else {
                write-msg 'NFO' "Deployment path: ""$deploymentPath"" was empty!"

            }
        }
    }

    # --> Delete content from Distribution path
    if (($(Get-NumberOfFolderElements -MyFolderPath $distributionPath) -gt 0) -or ($(Get-NumberOfFileElements -MyFolderPath $distributionPath))) {
        $ChoiceToDelete = read-host "Do you like to delete all folder in Dist? (y/n)"
        if ($ChoiceToDelete -eq "y") {
            Remove-Item -Path "$distributionPath\*.*" -Force -recurse
            write-msg 'NFO' "All files and folders in ""$distributionPath"" deleted!"
        }
        else {
            foreach ($candiateToDelete in $(Get-ChildItem -Path $distributionPath)) {
                $deleteCurrent = read-host "Do you like to delete element ""$($candiateToDelete.Name)"" (y/n)?"
                if ($deleteCurrent -eq "y") {
                    Remove-Item -Path "$deploymentPath\$($candiateToDelete.Name)" -Force -recurse
                    write-msg 'NFO' "Element ""$deploymentPath\$($candiateToDelete.Name)"" deleted!"
                }
                else {
                    write-msg 'NFO' "Skipped element ""$deploymentPath\$($candiateToDelete.Name)""!"
                }
            }
                
        }
    }
    else {
        write-msg 'NFO' "Distribution folder: ""$distributionPath"" was empty."
    }
    Write-MSG 'NFO' "Script executed!"
}
#--> In case of an error
catch {
    $IsError = $True
    $errmsg = $_
    Write-MSG 'ERR' $errmsg $true
}
  
#--> Execute every time
finally {
    if ($IsError) {
        Write-MSG 'ERR' '!! ERROR SCRIPT EXECUTION !!'
    }  
    Write-MSG 'NLN'
    Write-Host -NoNewLine 'Press any key to continue...';
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
    exit
}