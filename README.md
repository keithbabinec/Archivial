# Ozette
Data backup agent software for Windows that automatically archives your local data to a cloud storage provider.

# Build/Release
[![Build status](https://ozette.visualstudio.com/ozette-project/_apis/build/status/ozette-project-CI)](https://ozette.visualstudio.com/ozette-project/_build/latest?definitionId=1)

# Project Status
This project is under active development and not fully usuable yet. This grid shows progress by feature.

| Feature | Azure Support | AWS Support |
| --- | --- | --- |
| Install / Tools | Complete | N/A |
| Backup | In-Progress | N/A |
| Restore | N/A | N/A |

# Installation

1. Open an elevated (Run-As Administrator) PowerShell prompt.
2. Download latest binaries from [Releases](https://github.com/keithbabinec/Ozette/releases):
```
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$latestUri = 'https://api.github.com/repos/keithbabinec/Ozette/releases/latest'
$latestDownloadUri = (Invoke-RestMethod -Method Get -Uri $latestUri).assets[0].browser_download_url
Invoke-RestMethod -Method Get -Uri $latestDownloadUri -OutFile $home\downloads\OzetteBin.zip
```
3. Unblock/extract the archive:
```
Unblock-File -Path $home\downloads\OzetteBin.zip
Expand-Archive $home\downloads\OzetteBin.zip -DestinationPath $home\downloads\OzetteBin
```
4. Navigate to the unzipped release folder:
```
cd $home\downloads\OzetteBin
```
5. Run the install command:
```
.\OzetteCmd.exe install
```
6. Cleanup:
```
cd $home
Remove-Item -Force -Recurse $home\downloads\OzetteBin*
```

# Configuration

OzetteCmd.exe is used to configure the Ozette installation. The commands below can be used to add, remove, or list sources (folders you want to backup) and providers (cloud destinations) for an existing Ozette installation.

Note: Once Ozette is installed, OzetteCmd.exe should be available in your system path. This means you can run these configuration commands from any command prompt path location.

Usage: OzetteCmd.exe &lt;command&gt; --Option1Name Option1Value --Option2Name Option2Value

## Providers

**Example: Set Microsoft Azure as one of your cloud storage providers.**

Note: These secrets will be saved as encrypted values in the local database.
```
OzetteCmd.exe configure-azure --azurestorageaccountname "myaccount" --azurestorageaccounttoken "mytoken"
```

**Example: List the existing cloud storage providers you have configured**
```
OzetteCmd.exe list-providers
```

**Example: Remove one of the configured cloud providers by ID**

Note: see *list-providers* to view the existing cloud providers with IDs.
```
OzetteCmd.exe remove-provider --providerid 1
```

## Sources

**Example: Add a source folder to backup.**
```
OzetteCmd.exe add-source --folderpath "C:\users\me\downloads"
```

**Example: Add a source folder to backup, using all of the optional arguments.**

Note: --Priority accepts values 'Low', 'Medium', or 'High'. --MatchFilter accepts Windows file matching wildcards.
```
OzetteCmd.exe add-source --folderpath "C:\users\me\documents" --priority High --revisions 3 --matchfilter "*.docx"
```

**Example: List the source folders you have configured**
```
OzetteCmd.exe list-sources
```

**Example: Remove one of the source folders by ID**

Note: see *list-sources* to view the existing sources with IDs.
```
OzetteCmd.exe remove-source --sourceid 1
```
