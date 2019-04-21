---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Get-ArchivialProviders

## SYNOPSIS
Returns all of the configured Archivial Providers.

## SYNTAX

```
Get-ArchivialProviders [<CommonParameters>]
```

## DESCRIPTION
Returns all of the configured Archivial Providers.
An Archivial Provider is a connection to an external service for either cloud storage (ex: Azure, AWS) or message notifications (ex: Sendgrid email, Twilio SMS/text).

Note: Only the name and ID of the provider will be returned.
The encrypted secure setting values will not returned in the output.

The output from this command can be piped to the Remove-ArchivialProvider cmdlet.

## EXAMPLES

### EXAMPLE 1
```
C:\> Get-ArchivialProviders
```

Returns all of the configured Archivial Providers.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
