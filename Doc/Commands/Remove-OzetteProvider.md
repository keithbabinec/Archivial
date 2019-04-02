---
external help file: OzettePowerShell.dll-Help.xml
Module Name: OzettePowerShell
online version:
schema: 2.0.0
---

# Remove-OzetteProvider

## SYNOPSIS
Removes the configured storage or messaging provider.

## SYNTAX

### ByName
```
Remove-OzetteProvider -ProviderName <String> [<CommonParameters>]
```

### ByObject
```
Remove-OzetteProvider -Provider <Provider> [<CommonParameters>]
```

## DESCRIPTION
Providers are used to connect to external services for cloud storage backup or message notifications.

Removing a cloud storage provider means that new or updated files will not be backed up to that provider anymore, but existing files stored at that provider will remain.

To view existing configured providers, run Get-OzetteProviders.
This command supports piping from Get-OzetteProviders or manual invoke from the provider name.

## EXAMPLES

### EXAMPLE 1
```
C:\> Remove-OzetteProvider -ProviderName "Azure"
```

Removes the configured Azure provider.

### EXAMPLE 2
```
C:\> Get-OzetteProvider | Where Name -eq "Azure" | Remove-OzetteProvider
```

Removes the configured Azure provider, but using the pipeline scenario.

## PARAMETERS

### -ProviderName
Specify the name of the provider to remove.

```yaml
Type: String
Parameter Sets: ByName
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Provider
Specify the object (from pipeline) to remove.

```yaml
Type: Provider
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

### OzetteLibrary.Providers.Provider
Specify the object (from pipeline) to remove.

## OUTPUTS

## NOTES

## RELATED LINKS
