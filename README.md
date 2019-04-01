# Ozette
Data backup agent software for Windows that automatically archives your local data to a cloud storage provider.

# Help Contents
* [Meta](#meta)
  * [About](#about)
  * [CI Status](#ci-status)
  * [Feature Progress](#feature-progress)
  * [Upload Performance](#upload-performance)
* [Installation](#installation)
  * [Prerequisites](#prerequisites)
  * [How to install Ozette](#how-to-install-ozette)
* [Configuration](#configuration)
  * [How to view backup progress](#how-to-view-backup-progress)
  * [How to configure providers](#how-to-configure-providers)
  * [How to configure sources](#how-to-configure-sources)

# Meta

## About
The goal of Ozette is to provide a lightweight but highly configurable backup agent that supports cloud storage destinations. There are of course other software products that exist in this space. However none that had the exact feature set I was looking for and I also thought it would be a great software project to pick up some knowledge in areas I had wanted to learn.

## CI Status
The source code for this project is hosted here on Github. Pull request and integration builds are run via Azure DevOps CI. Releases are automatically published to the Github releases tab.

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

Updated installation and configuration notes coming shortly, due to recent move from a command-line installer to PowerShell cmdlet support.


