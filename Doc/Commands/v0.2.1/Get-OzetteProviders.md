---
external help file: OzettePowerShell.dll-Help.xml
Module Name: OzettePowerShell
online version:
schema: 2.0.0
---

# Get-OzetteProviders

## SYNOPSIS
Returns all of the configured Ozette Providers.

## SYNTAX

```
Get-OzetteProviders [<CommonParameters>]
```

## DESCRIPTION
Returns all of the configured Ozette Providers.
An Ozette Provider is a connection to an external service for either cloud storage (ex: Azure, AWS) or message notifications (ex: Sendgrid email, Twilio SMS/text).

Note: Only the name and ID of the provider will be returned.
The encrypted secure setting values will not returned in the output.

The output from this command can be piped to the Remove-OzetteProvider cmdlet.

## EXAMPLES

### EXAMPLE 1
```
C:\> Get-OzetteProviders
```

Returns all of the configured Ozette Providers.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
