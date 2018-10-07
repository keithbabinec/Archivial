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

1. Download and unzip the latest release archive from [Releases](https://github.com/keithbabinec/Ozette/releases).
```
-- code to download
```
2. Open an elevated PowerShell prompt and navigate to the unzipped release folder.
```
cd $home\downloads\OzetteInstallation
```
3. Run the install command:
```
.\OzetteCmd.exe install
```
