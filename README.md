# Ozette
Data backup agent software for Windows that automatically archives your local data to a cloud storage provider.

# Help Contents
* [Project Information](#project-information)
  * [About](#about)
  * [CI Status](#ci-status)
  * [Feature Progress](#feature-progress)
  * [Upload Performance](#upload-performance)
* [Installation](#installation)
  * [Prerequisites](#prerequisites)
  * [How to install Ozette](#how-to-install-ozette)
* [Ozette Configuration](#ozette-configuration)
  * [How to view backup progress](#how-to-view-backup-progress)
  * [How to configure providers](#how-to-configure-providers)
  * [How to configure sources](#how-to-configure-sources)

# Project Information

## About
The goal of Ozette is to provide a lightweight but highly configurable backup agent that supports cloud storage destinations. There are of course other software products that exist in this space. However none that had the exact feature set I was looking for and I also thought it would be a great software project to pick up some knowledge in areas I had wanted to learn.

## CI Status
The source code for this project is hosted here on Github. Pull request and integration builds are run via Azure DevOps CI. Releases are automatically published to the releases tab.

[![Build status](https://ozette.visualstudio.com/ozette-project/_apis/build/status/ozette-project-CI)](https://ozette.visualstudio.com/ozette-project/_build/latest?definitionId=1)

## Feature Progress
This project is under active development and not fully usuable yet. Breaking changes may occur without notice. This grid shows progress by feature.

| Storage Providers | Azure Support | AWS Support |
| --- | --- | --- |
| Install / Tools | Complete | N/A |
| Backup | In-Progress | N/A |
| Restore | N/A | N/A |

| Messaging Providers | SMS (Twilio) | Email (SendGrid) |
| --- | --- | --- |
| Backup Status | Complete | N/A |

## Upload Performance
The upload speed performance you can see with Ozette will depend on a variety of factors including internet bandwidth, computer/storage specs, and configured Ozette options.

Using the default Ozette options, an Intel Core i7 laptop, and an Xfinity 75/5 (Mbps up/down) internet line- I have been able to average 50 GB of upload per day when uploading large files. The biggest limiting factor in my test scenarios is the internet line.

# Installation

## Prerequisites

* A Windows Operating System running .NET 4.6.1 (or later).
* SQL Server 2017 Express (or later). Express edition is available for free from Microsoft at [this link](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express).

## How to install Ozette

1. Open an elevated (Run-As Administrator) PowerShell prompt.
2. Download latest binaries from [Releases](https://github.com/keithbabinec/Ozette/releases):
``` powershell
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$latestUri = 'https://api.github.com/repos/keithbabinec/Ozette/releases/latest'
$latestDownloadUri = (Invoke-RestMethod -Method Get -Uri $latestUri).assets[0].browser_download_url
Invoke-RestMethod -Method Get -Uri $latestDownloadUri -OutFile $home\downloads\OzetteBin.zip
```
3. Unblock/extract the archive:
``` powershell
Unblock-File -Path $home\downloads\OzetteBin.zip
Expand-Archive $home\downloads\OzetteBin.zip -DestinationPath $home\downloads\OzetteBin
```
4. Navigate to the unzipped release folder:
``` powershell
cd $home\downloads\OzetteBin
```
5. Run the install command:
``` powershell
.\OzetteCmd.exe install
```
6. Cleanup:
``` powershell
cd $home
Remove-Item -Force -Recurse $home\downloads\OzetteBin*
```

# Ozette Configuration

OzetteCmd.exe is used to configure the Ozette installation. The commands below can be used to add, remove, or list sources (folders you want to backup) and providers (cloud destinations) for an existing Ozette installation.

Note: Once Ozette is installed, OzetteCmd.exe should be available in your system path. This means you can run these configuration commands from any command prompt path location. On windows you will need to close and re-open the PowerShell prompt for this change to take effect.

Usage: OzetteCmd.exe &lt;command&gt; --Option1Name Option1Value --Option2Name Option2Value

## How to view backup progress

**Example: Print the current status/progress of your backup (files/size transferred, remaining, failed, etc).**

```
OzetteCmd.exe show-status
```

## How to configure providers

There are two types of providers available: Storage Providers and Messaging Providers.
* A storage provider is your cloud storage backup destination/target. Examples include Azure blob storage, Amazon S3, etc.
* A messaging provider sends you notifications about backup status, usually through email or SMS text messaging. Examples include Twilio, Sendgrid, etc.

### Storage Providers

**Example: Set Microsoft Azure as one of your cloud storage providers.**

Note: These secrets will be saved as encrypted values in the local database.
```
OzetteCmd.exe configure-azure --azurestorageaccountname "myaccount" --azurestorageaccounttoken "mytoken"
```

### Messaging Providers

**Example: Set Twilio as your messaging provider (SMS/Text message updates).**

You must have a pre-configured Twilio account and Twilio phone number to use this feature. One or more phone numbers can be used for the destination. If using multiple phone numbers, use a semicolon ';' character to seperate the list. The phone number(s) should be specified in [E.164](https://www.twilio.com/docs/glossary/what-e164) format, as shown below.

Note: These secrets will be saved as encrypted values in the local database.
```
OzetteCmd.exe configure-twilio --twilioaccountid "myaccount" --twilioauthtoken "mytoken" --twiliosourcephone "+12065551234" --twiliodestinationphones "+12065554567;+12065556789"
```

### Manage Existing Providers

**Example: List the existing providers you have configured**
```
OzetteCmd.exe list-providers
```

**Example: Remove one of the configured providers by ID**

Note: see *list-providers* to view the existing providers with IDs.
```
OzetteCmd.exe remove-provider --providerid 1
```

## How to configure sources
Sources are the known folder locations (local or remote/UNC) that Ozette will periodically scan for files to backup. The commands below are used to control the folders that Ozette will watch/scan.

The priority assigned to a source determines how frequently it will be scanned for changes. The default priority assigned to a new source (unless specified) is Medium.

| Source Priority | Scan Frequency |
| --- | --- |
| Low | Every 48 Hours |
| Medium | Every 12 hours |
| High | Every 1 Hour |

**Example: Add a local source folder to backup, using default options.**
```
OzetteCmd.exe add-localsource --folderpath "C:\users\me\downloads"
```

**Example: Add a local source folder to backup, using all of the optional arguments.**

Note: --Priority accepts values 'Low', 'Medium', or 'High'. --MatchFilter accepts Windows file matching wildcards.
```
OzetteCmd.exe add-localsource --folderpath "C:\users\me\documents" --priority High --revisions 3 --matchfilter "*.docx"
```

**Example: Add an unauthenticated network/UNC source folder to backup, using default options.**
```
OzetteCmd.exe add-netsource --uncpath "\\networkshare\public\media\playlists"
```

**Example: Add a network credential, then add an authenticated network/UNC source folder to backup, using all of the optional arguments.**

Note: The credential must be stored using the add-netcredential command prior to saving an authenticated netsource.
```
OzetteCmd.exe add-netcredential --credentialname 'Drobo-NAS-Device' --username 'drobo_read_only_user' --password '******'
OzetteCmd.exe add-netsource --uncpath "\\networkshare\private\docs\taxes" --credentialname 'Drobo-NAS-Device' --priority High --revisions 3 --matchfilter "*.pdf"
```

**Example: List the source folders you have configured**
```
OzetteCmd.exe list-sources
```

**Example: Remove one of the local source folders by ID**

Note: see *list-sources* to view the existing sources with IDs.
```
OzetteCmd.exe remove-source --sourceid 1 --sourcetype 'Local'
```

**Example: Remove one of the network source folders by ID**

Note: see *list-sources* to view the existing sources with IDs.
```
OzetteCmd.exe remove-source --sourceid 1 --sourcetype 'Network'
```

**Example: Queue a rescan for a local source folder (outside of the scheduled scan frequency)**

Note: see *list-sources* to view the existing sources with IDs.
```
OzetteCmd.exe rescan-source --sourceid 1 --sourcetype 'Local'
```

**Example: Queue a rescan for a network source folder (outside of the scheduled scan frequency)**

Note: see *list-sources* to view the existing sources with IDs.
```
OzetteCmd.exe rescan-source --sourceid 1 --sourcetype 'Network'
```

**Example: List the stored network credentials you have configured**
```
OzetteCmd.exe list-netcredentials
```

**Example: Remove one of the stored network credentials by name**

Note: see *list-netcredentials* to view the existing credentials.
```
OzetteCmd.exe remove-netcredential --credentialname 'Drobo-NAS-Device'
```
