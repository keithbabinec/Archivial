---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Start-ArchivialNetworkSourceRescan

## SYNOPSIS
Forces the re-scan of a Network Source being monitored by Archivial.

## SYNTAX

### ByName
```
Start-ArchivialNetworkSourceRescan -SourceID <Int32> [<CommonParameters>]
```

### ByObject
```
Start-ArchivialNetworkSourceRescan -NetworkSource <NetworkSourceLocation> [<CommonParameters>]
```

## DESCRIPTION
All sources are monitored for new or updated files on a regular schedule.
This cmdlet is used to request an immediate rescan, outside of its regular schedule.
The rescan will start as soon as there is scanning engine availability.

The automated scanning schedule for Low priority sources is once every 48 hours.
Medium priority sources are scanned every 12 hours.
High priority sources are scanned every hour.

Please see the Get-ArchivialNetworkSources command to find the ID of an existing source you would like to rescan.

## EXAMPLES

### EXAMPLE 1
```
C:\> Start-ArchivialNetworkSourceRescan -SourceID 2
```

Forces a rescan of the Network Source with the specified ID.

### EXAMPLE 2
```
C:\> Get-ArchivialNetworkSources | Start-ArchivialNetworkSourceRescan
```

Forces a rescan of all defined Network Sources being monitored by Archivial.

### EXAMPLE 3
```
C:\> Get-ArchivialNetworkSources | Where Path -like "*\\drobo-nas\*" | Start-ArchivialNetworkSourceRescan
```

Forces a rescan of any Network Sources that match the path filter.

## PARAMETERS

### -SourceID
Specify the ID of the Network Source to rescan.

```yaml
Type: Int32
Parameter Sets: ByName
Aliases:

Required: True
Position: Named
Default value: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -NetworkSource
Specify the Network Source object to rescan.

```yaml
Type: NetworkSourceLocation
Parameter Sets: ByObject
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### ArchivialLibrary.Folders.NetworkSourceLocation
Specify the Network Source object to rescan.

## OUTPUTS

## NOTES

## RELATED LINKS
