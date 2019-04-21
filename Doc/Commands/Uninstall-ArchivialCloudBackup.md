---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Uninstall-ArchivialCloudBackup

## SYNOPSIS
Uninstalls the Archivial Cloud Backup software from this computer.

## SYNTAX

```
Uninstall-ArchivialCloudBackup [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Uninstalls the Archivial Cloud Backup software from this computer.
This will permenantly delete the installation folder, state database, and log files.
This action is not reversable.

Although all local installation data is deleted, any of the data already backed up to a cloud provider will not be removed.
You must remove that manually if you wish to delete that data.

This command requires an elevated (run-as administrator) PowerShell prompt to complete.
It will also prompt for comfirmation unless the -Force switch is applied.

## EXAMPLES

### EXAMPLE 1
```
C:\> Uninstall-ArchivialCloudBackup
```

Starts the uninstallation process.
The user will be prompted for confirmation.

### EXAMPLE 2
```
C:\> Uninstall-ArchivialCloudBackup -Force
```

Starts the uninstallation and suppresses the confirmation prompt.

## PARAMETERS

### -Force
Suppresses the confirmation prompt.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
