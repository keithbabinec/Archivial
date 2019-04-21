---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Remove-ArchivialNetworkSource

## SYNOPSIS
Removes the specified Network Source.

## SYNTAX

### ByName
```
Remove-ArchivialNetworkSource -SourceID <Int32> [<CommonParameters>]
```

### ByObject
```
Remove-ArchivialNetworkSource -NetworkSource <NetworkSourceLocation> [<CommonParameters>]
```

## DESCRIPTION
A Network Source is a folder on your network (referenced by UNC Path) that Archivial backs up and automatically monitors for new and updated files.

Removing a Network Source means that new or updated files from that location will not be backed up anymore, but existing files already backed up to cloud storage will remain.

To view existing Network Sources, run Get-ArchivialNetworkSource.
This command supports piping from Get-ArchivialNetworkSource or manual invoke from the specified source ID.

## EXAMPLES

### EXAMPLE 1
```
C:\> Remove-ArchivialNetworkSource -ID 3
```

Removes the Network Source with the specified ID.

### EXAMPLE 2
```
C:\> Get-ArchivialNetworkSource | Where Path -like "*\\drobo-nas\documents*" | Remove-ArchivialNetworkSource
```

Removes any configured Network Source that matches a path containing the specified filter (using the pipeline scenario).

## PARAMETERS

### -SourceID
Specify the ID of the Network Source to remove.

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
Specify the object (from pipeline) to remove.

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
Specify the object (from pipeline) to remove.

## OUTPUTS

## NOTES

## RELATED LINKS
