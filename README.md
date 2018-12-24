# Ozette
Data backup agent software for Windows that automatically archives your local data to a cloud storage provider.

# Build/Release
[![Build status](https://ozette.visualstudio.com/ozette-project/_apis/build/status/ozette-project-CI)](https://ozette.visualstudio.com/ozette-project/_build/latest?definitionId=1)

# Project Status
This project is under active development and not fully usuable yet. Breaking changes may occur without notice. This grid shows progress by feature.

| Storage Providers | Azure Support | AWS Support |
| --- | --- | --- |
| Install / Tools | Complete | N/A |
| Backup | In-Progress | N/A |
| Restore | N/A | N/A |

| Messaging Providers | SMS (Twilio) | Email (SendGrid) |
| --- | --- | --- |
| Backup Status | In-Progress | N/A |

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

# Configuration and Status

OzetteCmd.exe is used to configure the Ozette installation. The commands below can be used to add, remove, or list sources (folders you want to backup) and providers (cloud destinations) for an existing Ozette installation.

Note: Once Ozette is installed, OzetteCmd.exe should be available in your system path. This means you can run these configuration commands from any command prompt path location.

Usage: OzetteCmd.exe &lt;command&gt; --Option1Name Option1Value --Option2Name Option2Value

## Backup Progress

**Example: Print the current status/progress of your backup (files/size transferred, remaining, failed, etc).**

```
OzetteCmd.exe show-status
```

## Providers

There are two types of providers available: Storage Providers and Messaging Providers.
* A storage provider is your cloud storage backup destination/target. Examples include Azure storage, Amazon S3, etc.
* A messaging provider sends you notifications about backup status, usually through email or SMS text messaging. Examples include Twilio, Sendgrid, etc.

### Storage Providers

**Example: Set Microsoft Azure as one of your cloud storage providers.**

Note: These secrets will be saved as encrypted values in the local database.
```
OzetteCmd.exe configure-azure --azurestorageaccountname "myaccount" --azurestorageaccounttoken "mytoken"
```

### Messaging Providers

**Example: Set Twilio as your messaging provider (SMS/Text message updates).**

You must have a pre-configured Twilio account and Twilio phone number to use this feature. One or more phone numbers can be used for the destination. If using multiple phone numbers, use a semicolon ';' character to seperate the list. The phone number format should include country code and area code, as specified in the below example.

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

## Sources

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

**Example: Remove one of the source folders by ID**

Note: see *list-sources* to view the existing sources with IDs.
```
OzetteCmd.exe remove-source --sourceid 1
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