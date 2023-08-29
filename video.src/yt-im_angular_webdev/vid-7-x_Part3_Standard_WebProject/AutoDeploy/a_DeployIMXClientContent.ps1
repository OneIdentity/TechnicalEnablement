# '*********************************************************'
# '** One Identity Technical Enablement Script Automation **'
# '**  - Upload Distribution files into an IM database    **'
# '**  - Last change: 2023-08-18, by HRA                  **'
# '** THIS IS A WORKLING SAMPLE, PLEASE USE THIS FOR YOUR **'
# '** OWN RISK. THIS IS NOT A PIECE OF Identity Manager   **'
# '*********************************************************'

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
Write-MSG 'TTL' '*********************************************************'
Write-MSG 'TTL' '** One Identity Technical Enablement Script Automation **'
Write-MSG 'TTL' '**  - Upload Distribution files into an IM database -  **'  
Write-MSG 'TTL' '*********************************************************'
  
#--> Path to this script
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
  
try {
    $IsError = $false

    #--> $true: I can decide which element to handle; 
    #    $false: All elements beside $staticModules are getting handled
    $manualDecisionPerDistributionElement = $true

    #--> Path information
    $deploymentfolder = ".autodeploy"
    $deploymentPath = "$scriptPath\$deploymentfolder"
    $webDeploymentSubPath = "bin\imxweb\custom"
    $distributionPath = "$scriptPath\imxweb\dist"
    $staticModules = @('qbm', 'qer')

    #--> IM Softwareloader Configuration
    $IMInstPath = "$env:ProgramFiles\One Identity\One Identity Manager"
    $IMDBServer = "[your Web-Server DN]"
    $IMDBName = "[your Database name]"
    $IMDBUser = "[your SQL Server account name]"
    $IMDBUserPW = "[your SQL Server account password]"
    $IMSystemUser = "[Your system user name]"
    $IMSystemUserPW = "[your system user password]"

    #--> Only if distribution path exists
    if (Test-Path -path $distributionPath) {

        #--> Only if elements in distribution folder exist 
        #--> Ignore qbm and qer, they are part of the app like all static modules, list can be extended above
        if ((Get-NumberOfFolderElements -MyFolderPath $distributionPath) -gt 0) {
            $distFolderItems = Get-ChildItem -Path $distributionPath -Directory -exclude $staticModules
            Write-MSG 'nfo' "Deployment folder: ""$deploymentpath\$webDeploymentSubPath"""

            if (-not (Test-Path -path "$deploymentPath\$webDeploymentSubPath")) {
                #--> If deployment path did not exist, create a new deployment path
                New-item -ItemType Directory -path "$deploymentPath\$webDeploymentSubPath"
            }

            foreach ($folder in $distFolderItems) {
                #--> Ask if current element should be handled, manual decission by developer if activated (configured above)
                $uploadme = 'n'
                if ($manualDecisionPerDistributionElement) {
                    $uploadme = read-host "Do you like to upload element: ""$($folder.Name)""? (y/n)"
                }
                else {
                    $uploadme = 'y'
                }
                
                if ($uploadme -eq "y") {
                    #--> Upload...
                    if (test-path -path $distributionPath.replace("\dist", "\projects\$($folder.name)\src\environments")) {
                        #--> This is an app
                        Write-MSG 'NFO' "Application: ""$($folder.name)"" detected."

                        #--> html folder needs to be moved before compressing
                        if (test-path -path "$($folder.FullName)\html") {
                            #--> Move html folder out if exist
                            Move-Item -Path "$($folder.FullName)\html" -Destination "$env:TEMP\$($folder.Name)_html" -Force
                            Write-MSG 'NFO' "App html folder: ""$($folder.fullname)\html"" moved to: ""$env:TEMP\$($folder.Name)_html""."
                        }

                        #--> Compress and move the App
                        Compress-Archive -Path "$($folder.FullName)\*" -DestinationPath "$deploymentPath\$webDeploymentSubPath\Html_$($folder.name).zip" -Force
                        Write-MSG 'NFO' "File: ""$deploymentPath\$webDeploymentSubPath\Html_$($folder.name).zip"" created."

                        if (Test-Path -path "$env:TEMP\$($folder.Name)_html") {
                        #--> If exists move html folder back in
                        Move-Item -Path "$env:TEMP\$($folder.Name)_html" -Destination "$($folder.fullname)\html" -Force
                            Write-MSG 'NFO' "App html folder: ""$env:TEMP\$($folder.Name)_html"" moved back to: ""$($folder.fullname)\html""."
                        }
                    }
                    else {
                        #--> This is a lib
                        Write-MSG 'NFO' "Module or Plugin: ""$($folder.name)"" detected."

                        #--> Compress and move module or plugin
                        Compress-Archive -Path "$($folder.FullName)\*" -DestinationPath "$deploymentPath\$webDeploymentSubPath\Html_$($folder.name).zip" -Force
                        Write-MSG 'NFO' "File: ""$deploymentPath\$webDeploymentSubPath\Html_$($folder.name).zip"" created."
                    }
                }
                else {
                    #--> Element was not handled
                    Write-MSG 'NFO' "Element: ""$($folder.Name)"" skipped by user."
                }
            }

            #--> Upload distributed elements to IM Database
            $deployFolderItems = Get-ChildItem -Path "$deploymentPath\$webDeploymentSubPath\*.zip"
            foreach ($zipFile in $deployFolderItems) {
                Write-MSG 'NFO' "Try to upload and remove file: ""$deploymentPath\$($zipFile.Name)""."
                $cmdln = """$IMInstPath\SoftwareLoaderCMD.exe"""
                $cmdln += " /Conn=""Data Source=$IMDBServer;Initial Catalog=$IMDBName;User ID=$IMDBUser;Password=$IMDBUserPW"""
                $cmdln += " /Auth=""Module=DialogUser;User=$IMSystemUser;Password=$IMSystemUserPW"""
                $cmdln += " /Root=""$deploymentPath"""
                $cmdln += " -I /Files=""$webDeploymentSubPath\$($zipFile.Name)|BusinessApiServer"""
                cmd.exe /c $cmdln
                remove-item -Force -Path "$deploymentPath\$webDeploymentSubPath\$($zipFile.Name)"
            }
        }
        else {
            #--> Distribution folder is empty
            Write-MSG 'WRN' "No distributions detected in distribution folder: $distributionPath."
        }
    }
    else {
        #--> No distribution folder ([DevProject]\dist)
        Write-MSG 'WRN' "No distribution folder: $distributionPath detected."
        exit
    }
    #--> End of the script reached
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
        #--> System errors only
        Write-MSG 'ERR' '!! ERROR SCRIPT EXECUTION !!'
    }  
    #--> Wait to treminate to allow messaage reading
    Write-MSG 'NLN'
    Write-Host -NoNewLine 'Press any key to continue...';
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
    exit
}