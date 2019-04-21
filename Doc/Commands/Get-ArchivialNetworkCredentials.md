---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Get-ArchivialNetworkCredentials

## SYNOPSIS
Returns all of the saved Network Credentials used to connect to Network Sources.

## SYNTAX

```
Get-ArchivialNetworkCredentials [<CommonParameters>]
```

## DESCRIPTION
Some Network Sources (UNC Paths) being monitored for backup will require authentication (username and password).
This command will return the list of named credentials that have been saved.

Note: Only the name of the credential will be returned.
The encrypted username and password values will not be returned in the output.

The output from this command can be piped to the Remove-ArchivialNetworkCredential cmdlet.

## EXAMPLES

### EXAMPLE 1
```
C:\> Get-ArchivialNetworkCredentials
```

Returns all of the configured Network Credentials saved in the system.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
