# Archivial Cloud Backup
Archivial is data backup software for Windows that automatically archives your local data to a cloud storage provider. 

# Contents
* [About](#about)
* [Development Status](#development-status)
* [Screenshots](#screenshots)
* [Install Archivial](#install-archivial)
* [Update Archivial](#update-archivial)
* [Remove Archivial](#remove-archivial)
* [Command Reference](#command-reference)

## About
Archivial is a lightweight but highly configurable backup agent that supports cloud storage destinations, automated progress notifications, and full management through PowerShell commands. 

The [ArchivialPowerShell](https://www.powershellgallery.com/packages/ArchivialPowerShell) module package contains the Archivial installation and management commands and can be installed from PSGallery. 

Pull request and integration builds are run via Azure DevOps CI. Releases are automatically published to the Github releases tab.

[![Build status](https://ozette.visualstudio.com/ozette-project/_apis/build/status/ozette-project-CI)](https://ozette.visualstudio.com/ozette-project/_build/latest?definitionId=1)

## Development Status
This project is still considered alpha phase and under active development. Breaking changes may occur without notice. This grid shows progress by feature.

| Storage Providers | Azure Support | AWS Support |
| --- | --- | --- |
| Install / Tools | Complete | N/A |
| Backup | Complete | N/A |
| Restore | In-Progress | N/A |

| Messaging Providers | SMS (Twilio) | Email (SendGrid) |
| --- | --- | --- |
| Backup Status | Complete | N/A |

## Screenshots

*Screenshot: PowerShell installation*

![Screenshot: PowerShell installation](Doc/Screenshots/1-archivial-install.png?raw=true "Screenshot: PowerShell installation")

*Screenshot: Application Configuration through PowerShell*

![Screenshot: Application Configuration through PowerShell](Doc/Screenshots/2-archivial-configuration.png?raw=true "Screenshot: Application Configuration through PowerShell")

*Screenshot: Help documentation inside PowerShell*

![Screenshot: Help documentation inside PowerShell](Doc/Screenshots/3-archivial-help-from-ps.png?raw=true "Screenshot: Help documentation inside PowerShell")

*Screenshot: Program Files / Local Logs*

![Screenshot: Program Files / Local Logs](Doc/Screenshots/4-archivial-text-logs.png?raw=true "Screenshot: Program Files / Local Logs")

*Screenshot: Files Backed up to Azure*

![Screenshot: Files Backed up to Azure](Doc/Screenshots/5-archivial-data-in-azure.png?raw=true "Screenshot: Files Backed up to Azure")

*Screenshot: Automated SMS/Text Status Updates*

![Screenshot: Automated SMS/Text Status Updates](Doc/Screenshots/6-archivial-update-notify.png?raw=true "Screenshot: Automated SMS/Text Status Updates")

## Install Archivial

### Prerequisites
* A Windows Operating System running .NET 4.6.1 (or later).
* Windows PowerShell 5.1 or later.

### Step 1: Install Microsoft SQL Server Express 2017 or later.
* Microsoft SQL Server is required to maintain the state database. You can use any edition as long as it has full-text search feature support.
    * **SQL Server Express with Advanced Services Edition** includes FTS feature support and is free under the [Visual Studio Dev Essentials program](https://visualstudio.microsoft.com/dev-essentials/). 
* Run the installer with the default options.

### Step 2: Install ArchivialPowerShell Module (Management Tools)
* Installation and management tasks are performed through the [ArchivialPowerShell](https://www.powershellgallery.com/packages/ArchivialPowerShell) module. 
* Open an elevated (run-as Administrator) PowerShell prompt and then execute the following to install that module:
``` powershell
Install-Module -Name ArchivialPowerShell -Scope AllUsers
```

### Step 3: Install and Start the Archivial Cloud Backup agent.
* After installing the management tools, run the main installation command. 
* This will copy the program files, create the initial state database, then create and start the Archivial Client windows service (the backup agent).
``` powershell
Install-ArchivialCloudBackup
```

## Update Archivial

### Step 1: Update ArchivialPowerShell Module (Management Tools)
* Open an elevated (run-as Administrator) PowerShell prompt and then execute the following to update your module to latest.
* NOTE: If you have already loaded the ArchivialPowerShell module in your current session, then you will need to close then reopen PowerShell before you can perform the upgrade command in step 2.
``` powershell
$latestVersion = (Find-Module -Name ArchivialPowerShell).Version
Update-Module -Name ArchivialPowerShell -RequiredVersion $latestVersion
```

### Step 2: Update your Archivial Cloud Backup software installation
* Open an elevated (run-as Administrator) PowerShell prompt and then execute the following:
``` powershell
Update-ArchivialCloudBackup
```

## Remove Archivial

### Step 1: Remove your Archivial Cloud Backup software installation
* Open an elevated (run-as Administrator) PowerShell prompt and then execute the following:
``` powershell
Uninstall-ArchivialCloudBackup
```

### Step 2: Remove ArchivialPowerShell Module (Management Tools)
* Open an elevated (run-as Administrator) PowerShell prompt and then execute the following to remove that module:
``` powershell
Uninstall-Module -Name ArchivialPowerShell -AllVersions
```

## Command Reference
| Command | Description |
| --- | --- |
| [Add-ArchivialLocalSource](Doc/Commands/Add-ArchivialLocalSource.md) | Adds a local folder to the Archivial backup folders list. |
| [Add-ArchivialNetworkSource](Doc/Commands/Add-ArchivialNetworkSource.md) | Adds a network (UNC path) folder to the Archivial backup folders list. |
| [Find-ArchivialFilesToRestore](Doc/Commands/Find-ArchivialFilesToRestore.md) | Finds backup files that are available to restore. |
| [Get-ArchivialCloudBackupOptions](Doc/Commands/Get-ArchivialCloudBackupOptions.md) | Gets the application options for Archivial Cloud Backup. |
| [Get-ArchivialCloudBackupStatus](Doc/Commands/Get-ArchivialCloudBackupStatus.md) | Returns the current status of the cloud backup progress. |
| [Get-ArchivialLocalSources](Doc/Commands/Get-ArchivialLocalSources.md) | Returns all of the Local Source folders being monitored by Archivial. |
| [Get-ArchivialNetworkCredentials](Doc/Commands/Get-ArchivialNetworkCredentials.md) | Returns all of the saved Network Credentials used to connect to Network Sources. |
| [Get-ArchivialNetworkSources](Doc/Commands/Get-ArchivialNetworkSources.md) | Returns all of the Network Source folders being monitored by Archivial. |
| [Get-ArchivialProviders](Doc/Commands/Get-ArchivialProviders.md) | Returns all of the configured Archivial Providers. |
| [Install-ArchivialCloudBackup](Doc/Commands/Install-ArchivialCloudBackup.md) | Installs the Archivial Cloud Backup software on this computer. |
| [Remove-ArchivialLocalSource](Doc/Commands/Remove-ArchivialLocalSource.md) | Removes the specified Local Source. |
| [Remove-ArchivialNetworkCredential](Doc/Commands/Remove-ArchivialNetworkCredential.md) | Removes the stored network credential used for connecting to network resources. |
| [Remove-ArchivialNetworkSource](Doc/Commands/Remove-ArchivialNetworkSource.md) | Removes the specified Network Source. |
| [Remove-ArchivialProvider](Doc/Commands/Remove-ArchivialProvider.md) | Removes the configured storage or messaging provider. |
| [Set-ArchivialAzureProviderOptions](Doc/Commands/Set-ArchivialAzureProviderOptions.md) | Configures the Azure cloud storage provider as a backup destination. |
| [Set-ArchivialCloudBackupOptions](Doc/Commands/Set-ArchivialCloudBackupOptions.md) | Sets one or more application options for Archivial Cloud Backup. |
| [Set-ArchivialNetworkCredential](Doc/Commands/Set-ArchivialNetworkCredential.md) | Saves the credentials required to connect to an authenticated network resource (such as a UNC path share). |
| [Set-ArchivialTwilioProviderOptions](Doc/Commands/Set-ArchivialTwilioProviderOptions.md) | Configures the Twilio messaging provider as a status update recipient. |
| [Start-ArchivialLocalSourceRescan](Doc/Commands/Start-ArchivialLocalSourceRescan.md) | Forces the re-scan of a Local Source being monitored by Archivial. |
| [Start-ArchivialNetworkSourceRescan](Doc/Commands/Start-ArchivialNetworkSourceRescan.md) | Forces the re-scan of a Network Source being monitored by Archivial. |
| [Uninstall-ArchivialCloudBackup](Doc/Commands/Uninstall-ArchivialCloudBackup.md) | Uninstalls the Archivial Cloud Backup software from this computer. |
| [Update-ArchivialCloudBackup](Doc/Commands/Update-ArchivialCloudBackup.md) | Updates the Archivial Cloud Backup software on this computer. |
