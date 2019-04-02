# Ozette Cloud Backup
Data backup agent software for Windows that automatically archives your local data to a cloud storage provider.

# Help Contents
* [About](#about)
* [Development Status](#development-status)
* [Installation](#installation)
* [Command Reference](#command-reference)

## About
Ozette is a lightweight but highly configurable backup agent that supports cloud storage destinations, automated progress notifications, and full management through PowerShell commands.

Pull request and integration builds are run via Azure DevOps CI. Releases are automatically published to the Github releases tab.

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

## Installation

### Prerequisites
* A Windows Operating System running .NET 4.6.1 (or later).
* Windows PowerShell 5.1 or later.

### Step 1: Install SQL Server Express 2017 or later.
* SQL Server is required to maintain the state database. Express edition is available for free from Microsoft at [this link](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express). 
* Run the installer with the default options.

### Step 2: Install OzettePowerShell Module (Management Tools)
* Installation and management tasks are performed through the [OzettePowerShell](https://www.powershellgallery.com/packages/OzettePowerShell/) module. 
* Open an elevated (run-as Administrator) PowerShell prompt and then execute the following to install that module:
``` powershell
Install-Module -Name OzettePowerShell -Scope AllUsers
```

### Step 3: Install and Start the Ozette Backup Agent
* After installing the management tools, run the main installation command. 
* This will copy the program files, create the initial state database, then create and start the Ozette Client windows service (the backup agent).
``` powershell
Install-OzetteCloudBackup
```

## Command Reference
| Command | Description |
| --- | --- |
| [Add-OzetteLocalSource](Doc/Commands/v0.2.1/Add-OzetteLocalSource.md) | Adds a local folder to the Ozette backup folders list. |
| [Add-OzetteNetworkSource](Doc/Commands/v0.2.1/Add-OzetteNetworkSource.md) | Adds a network (UNC path) folder to the Ozette backup folders list. |
| [Get-OzetteCloudBackupStatus](Doc/Commands/v0.2.1/Get-OzetteCloudBackupStatus.md) | Returns the current status of the cloud backup progress. |
| [Get-OzetteLocalSources](Doc/Commands/v0.2.1/Get-OzetteLocalSources.md) | Returns all of the Local Source folders being monitored by Ozette. |
| [Get-OzetteNetworkCredentials](Doc/Commands/v0.2.1/Get-OzetteNetworkCredentials.md) | Returns all of the saved Network Credentials used to connect to Network Sources. |
| [Get-OzetteNetworkSources](Doc/Commands/v0.2.1/Get-OzetteNetworkSources.md) | Returns all of the Network Source folders being monitored by Ozette. |
| [Get-OzetteProviders](Doc/Commands/v0.2.1/Get-OzetteProviders.md) | Returns all of the configured Ozette Providers. |
| [Install-OzetteCloudBackup](Doc/Commands/v0.2.1/Install-OzetteCloudBackup.md) | Installs the Ozette Cloud Backup software on this computer. |
| [Remove-OzetteLocalSource](Doc/Commands/v0.2.1/Remove-OzetteLocalSource.md) | Removes the specified Local Source. |
| [Remove-OzetteNetworkCredential](Doc/Commands/v0.2.1/Remove-OzetteNetworkCredential.md) | Removes the stored network credential used for connecting to network resources. |
| [Remove-OzetteNetworkSource](Doc/Commands/v0.2.1/Remove-OzetteNetworkSource.md) | Removes the specified Network Source. |
| [Remove-OzetteProvider](Doc/Commands/v0.2.1/Remove-OzetteProvider.md) | Removes the configured storage or messaging provider. |
| [Set-OzetteAzureProviderOptions](Doc/Commands/v0.2.1/Set-OzetteAzureProviderOptions.md) | Configures the Azure cloud storage provider as a backup destination. |
| [Set-OzetteNetworkCredential](Doc/Commands/v0.2.1/Set-OzetteNetworkCredential.md) | Saves the credentials required to connect to an authenticated network resource (such as a UNC path share). |
| [Set-OzetteTwilioProviderOptions](Doc/Commands/v0.2.1/Set-OzetteTwilioProviderOptions.md) | Configures the Twilio messaging provider as a status update recipient. |
| [Start-OzetteLocalSourceRescan](Doc/Commands/v0.2.1/Start-OzetteLocalSourceRescan.md) | Forces the re-scan of a Local Source being monitored by Ozette. |
| [Start-OzetteNetworkSourceRescan](Doc/Commands/v0.2.1/Start-OzetteNetworkSourceRescan.md) | Forces the re-scan of a Network Source being monitored by Ozette. |
| [Uninstall-OzetteCloudBackup](Doc/Commands/v0.2.1/Uninstall-OzetteCloudBackup.md) | Uninstalls the Ozette Cloud Backup software from this computer. |
