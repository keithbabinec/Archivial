# Archivial Cloud Backup
Data backup agent software for Windows that automatically archives your local data to a cloud storage provider.

# Contents
* [About](#about)
* [Development Status](#development-status)
* [Screenshots](#screenshots)
* [Installation](#installation)
* [Command Reference](#command-reference)

## About
Archivial is a lightweight but highly configurable backup agent that supports cloud storage destinations, automated progress notifications, and full management through PowerShell commands. 

Pull request and integration builds are run via Azure DevOps CI. Releases are automatically published to the Github releases tab.

This project was recently renamed from the former temporary codename (Ozette). References are in the process of being updated.

[![Build status](https://ozette.visualstudio.com/ozette-project/_apis/build/status/ozette-project-CI)](https://ozette.visualstudio.com/ozette-project/_build/latest?definitionId=1)

## Development Status
This project is still considered alpha phase and under active development. Breaking changes may occur without notice. This grid shows progress by feature.

| Storage Providers | Azure Support | AWS Support |
| --- | --- | --- |
| Install / Tools | Complete | N/A |
| Backup | In-Progress | N/A |
| Restore | N/A | N/A |

| Messaging Providers | SMS (Twilio) | Email (SendGrid) |
| --- | --- | --- |
| Backup Status | Complete | N/A |

## Screenshots

*Screenshot: PowerShell installation*

![Screenshot: PowerShell installation](Doc/Screenshots/ps-install.PNG?raw=true "Screenshot: PowerShell installation")

*Screenshot: Program Files / Local Logs*

![Screenshot: Program Files / Local Logs](Doc/Screenshots/logs-folder.PNG?raw=true "Screenshot: Program Files / Local Logs")

*Screenshot: Configuration through PowerShell*

![Screenshot: Configuration through PowerShell](Doc/Screenshots/ps-config.png?raw=true "Screenshot: Configuration through PowerShell")

*Screenshot: Files Backed up to Azure*

![Screenshot: Files Backed up to Azure](Doc/Screenshots/azure-files.png?raw=true "Screenshot: Files Backed up to Azure")

*Screenshot: Automated SMS/Text Status Updates*

![Screenshot: Automated SMS/Text Status Updates](Doc/Screenshots/twilio-status.png?raw=true "Screenshot: Automated SMS/Text Status Updates")

## Installation

### Prerequisites
* A Windows Operating System running .NET 4.6.1 (or later).
* Windows PowerShell 5.1 or later.

### Step 1: Install SQL Server Express 2017 or later.
* SQL Server is required to maintain the state database. Express edition is available for free from Microsoft at [this link](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express). 
* Run the installer with the default options.

### Step 2: Install ArchivialPowerShell Module (Management Tools)
* Installation and management tasks are performed through the [OzettePowerShell](https://www.powershellgallery.com/packages/ArchivialPowerShell) module. 
* Open an elevated (run-as Administrator) PowerShell prompt and then execute the following to install that module:
``` powershell
Install-Module -Name ArchivialPowerShell -Scope AllUsers
```

### Step 3: Install and Start the Archivial Backup Agent
* After installing the management tools, run the main installation command. 
* This will copy the program files, create the initial state database, then create and start the Archivial Client windows service (the backup agent).
``` powershell
Install-ArchivialCloudBackup
```

## Command Reference
| Command | Description |
| --- | --- |
| [Add-ArchivialLocalSource](Doc/Commands/Add-ArchivialLocalSource.md) | Adds a local folder to the Archivial backup folders list. |
| [Add-ArchivialNetworkSource](Doc/Commands/Add-ArchivialNetworkSource.md) | Adds a network (UNC path) folder to the Archivial backup folders list. |
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
