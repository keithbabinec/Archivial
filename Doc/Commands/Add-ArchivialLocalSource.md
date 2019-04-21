---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Add-ArchivialLocalSource

## SYNOPSIS
Adds a local folder to the Archivial backup folders list.

## SYNTAX

```
Add-ArchivialLocalSource -FolderPath <String> -Priority <String> -Revisions <Int32> [-MatchFilter <String>]
 [<CommonParameters>]
```

## DESCRIPTION
A Local Source is a folder on your computer (or a directly attached external drive) that you would like Archivial to backup and automatically monitor for new and updated files.

The priority of the source determines how frequently it will be scanned for changes.
The automated scanning schedule for Low priority sources is once every 48 hours.
Medium priority sources are scanned every 12 hours.
High priority sources are scanned every hour.

The optional MatchFilter parameter allows you to narrow the scope of files in the folder to be monitored.
For example, by file extension.
Any windows file path wildcard expression will be accepted here.

## EXAMPLES

### EXAMPLE 1
```
C:\> Add-ArchivialLocalSource -FolderPath "C:\users\test\documents" -Priority High -Revisions 3
```

Adds the specified folder to backup with high priority, and to retain up to 3 revisions of file history.

### EXAMPLE 2
```
C:\> Add-ArchivialLocalSource -FolderPath "C:\users\test\music\playlists" -Priority High -Revisions 3 -MatchFilter *.m3u
```

Adds the specified folder to backup with high priority, but only files that match the wildcard extension filter.

## PARAMETERS

### -FolderPath
Specify the folder path that should be backed up and monitored.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Priority
Specify the priority of this source (which determines how frequently it will be scanned for changes).

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Revisions
Specify the maximum number of revisions to store in the cloud for the files in this folder.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -MatchFilter
Optionally specify a wildcard expression to filter the files to be backed up or monitored.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: *
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
