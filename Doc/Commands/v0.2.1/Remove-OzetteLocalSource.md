---
external help file: OzettePowerShell.dll-Help.xml
Module Name: OzettePowerShell
online version:
schema: 2.0.0
---

# Remove-OzetteLocalSource

## SYNOPSIS
Removes the specified Local Source.

## SYNTAX

### ByName
```
Remove-OzetteLocalSource -SourceID <Int32> [<CommonParameters>]
```

### ByObject
```
Remove-OzetteLocalSource -LocalSource <LocalSourceLocation> [<CommonParameters>]
```

## DESCRIPTION
A Local Source is a folder on your computer (or a directly attached external drive) that Ozette backs up and automatically monitors for new and updated files.

Removing a Local Source means that new or updated files from that location will not be backed up anymore, but existing files already backed up to cloud storage will remain.

To view existing Local Sources, run Get-OzetteLocalSource.
This command supports piping from Get-OzetteLocalSource or manual invoke from the specified source ID.

## EXAMPLES

### EXAMPLE 1
```
C:\> Remove-OzetteLocalSource -ID 3
```

Removes the Local Source with the specified ID.

### EXAMPLE 2
```
C:\> Get-OzetteLocalSource | Where Path -like "*C:\users\test\documents*" | Remove-OzetteLocalSource
```

Removes any configured Local Source that matches a path containing the specified filter (using the pipeline scenario).

## PARAMETERS

### -SourceID
Specify the ID of the Local Source to remove.

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

### -LocalSource
Specify the object (from pipeline) to remove.

```yaml
Type: LocalSourceLocation
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

### OzetteLibrary.Folders.LocalSourceLocation
Specify the object (from pipeline) to remove.

## OUTPUTS

## NOTES

## RELATED LINKS
