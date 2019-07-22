---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Install-ArchivialCloudBackup

## SYNOPSIS
Installs the Archivial Cloud Backup software on this computer.

## SYNTAX

```
Install-ArchivialCloudBackup [-InstallDirectory <String>] [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Installs the Archivial Cloud Backup software on this computer.
The default installation will be placed in the Program Files directory, but this can optionally be changed by specifying the -InstallDirectory parameter.

This command requires an elevated (run-as administrator) PowerShell prompt to complete.
It will also prompt for comfirmation unless the -Force switch is applied.

IMPORTANT: Microsoft SQL Server is required to maintain the state database.
You can use any edition as long as it has full-text search feature support.

SQL Server Express with Advanced Services Edition includes FTS feature support and is free under the Visual Studio Dev Essentials program (https://visualstudio.microsoft.com/dev-essentials/).

NOTE: This command is used for fresh installations.
For upgrades to existing installations use the Update-ArchivialCloudBackup command.

## EXAMPLES

### EXAMPLE 1
```
C:\> Install-ArchivialCloudBackup
```

Starts the installation with default options.
The user will be prompted for confirmation.

### EXAMPLE 2
```
C:\> Install-ArchivialCloudBackup -InstallDirectory "D:\Applications\Archivial Cloud Backup" -Force
```

Starts the installation to the custom directory and suppresses the confirmation prompt.

## PARAMETERS

### -InstallDirectory
Specify a custom installation directory, otherwise the default Program Files location will be used.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
