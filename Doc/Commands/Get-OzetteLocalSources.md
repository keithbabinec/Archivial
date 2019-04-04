---
external help file: OzettePowerShell.dll-Help.xml
Module Name: OzettePowerShell
online version:
schema: 2.0.0
---

# Get-OzetteLocalSources

## SYNOPSIS
Returns all of the Local Source folders being monitored by Ozette.

## SYNTAX

```
Get-OzetteLocalSources [<CommonParameters>]
```

## DESCRIPTION
A Local Source is a folder on your computer (or a directly attached external drive) that Ozette backs up and automatically monitors for new and updated files.

The output from this command can be piped to the Remove-OzetteLocalSource cmdlet.

## EXAMPLES

### EXAMPLE 1
```
C:\> Get-OzetteLocalSources
```

Returns all of the Local Source folders being monitored by Ozette.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
