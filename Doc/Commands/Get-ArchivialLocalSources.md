---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Get-ArchivialLocalSources

## SYNOPSIS
Returns all of the Local Source folders being monitored by Archivial.

## SYNTAX

```
Get-ArchivialLocalSources [<CommonParameters>]
```

## DESCRIPTION
A Local Source is a folder on your computer (or a directly attached external drive) that Archivial backs up and automatically monitors for new and updated files.

The output from this command can be piped to the Remove-ArchivialLocalSource cmdlet.

## EXAMPLES

### EXAMPLE 1
```
C:\> Get-ArchivialLocalSources
```

Returns all of the Local Source folders being monitored by Archivial.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
