---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Find-ArchivialFilesToRestore

## SYNOPSIS
Finds backup files that are available to restore.

## SYNTAX

### ByFilter (Default)
```
Find-ArchivialFilesToRestore -MatchFilter <String> [-LimitResults <Int32>] [<CommonParameters>]
```

### BySource
```
Find-ArchivialFilesToRestore -Source <SourceLocation> [-LimitResults <Int32>] [<CommonParameters>]
```

### ByHash
```
Find-ArchivialFilesToRestore -FileHash <String> [-LimitResults <Int32>] [<CommonParameters>]
```

### All
```
Find-ArchivialFilesToRestore [-All] [<CommonParameters>]
```

## DESCRIPTION
Backup files are available to restore if they have completed an upload to at least one cloud storage provider account.

This command searches the backup index to find files that are eligible for restore and filters the result using the specified options.

The output from this command can be passed into Start-ArchivialFileRestore, which is used to initiate the restore.

## EXAMPLES

### EXAMPLE 1
```
C:\> Find-ArchivialFilesToRestore -MatchFilter "*.docx" -LimitResults 100
```

Searches for any files that match the extension filter and limits the results to no more than 100 items.

### EXAMPLE 2
```
C:\> Find-ArchivialFilesToRestore -MatchFilter "D:\music\*"
```

Searches for any files that contain a match to the specified path.
Does not limit the number of results returned.

### EXAMPLE 3
```
C:\> Get-ArchivialNetworkSource | Where Path -like "*\\drobo-nas\documents*" | Find-ArchivialFilesToRestore
```

Searches for any files that originated from a Network source that matches the documents path filter.

### EXAMPLE 4
```
C:\> Get-ArchivialLocalSource | Find-ArchivialFilesToRestore
```

Searches for any files that originated from any of the Local sources.

### EXAMPLE 5
```
C:\> Find-ArchivialFilesToRestore -FileHash "A37CC82F2876DB6CF59BA29B4EB148C7BF5CC920"
```

Searches for any files that match the provided file hash.

## PARAMETERS

### -Source
Specify a source location to search for files that can be restored.

```yaml
Type: SourceLocation
Parameter Sets: BySource
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -FileHash
Specify a file hash to search for files that can be restored.

```yaml
Type: String
Parameter Sets: ByHash
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MatchFilter
Specify a directory/file path filter to search for files that can be restored.
This match behaves similar to a 'Contains' operation and supports the * wildcard.

```yaml
Type: String
Parameter Sets: ByFilter
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LimitResults
Optionally specify the maximum number of results to return from a search.

```yaml
Type: Int32
Parameter Sets: ByFilter, BySource, ByHash
Aliases:

Required: False
Position: Named
Default value: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -All
Specify this option to return all files eligible for restore, without any filtering.

```yaml
Type: SwitchParameter
Parameter Sets: All
Aliases:

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### ArchivialLibrary.Folders.SourceLocation
Specify a source location to search for files that can be restored.

## OUTPUTS

## NOTES

## RELATED LINKS
