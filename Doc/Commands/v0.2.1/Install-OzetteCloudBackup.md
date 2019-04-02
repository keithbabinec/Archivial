---
external help file: OzettePowerShell.dll-Help.xml
Module Name: OzettePowerShell
online version:
schema: 2.0.0
---

# Install-OzetteCloudBackup

## SYNOPSIS
Installs the Ozette Cloud Backup software on this computer.

## SYNTAX

```
Install-OzetteCloudBackup [-InstallDirectory <String>] [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Installs the Ozette Cloud Backup software on this computer.
The default installation will be placed in the Program Files directory, but this can optionally be changed by specifying the -InstallDirectory parameter.

This command requires an elevated (run-as administrator) PowerShell prompt to complete.
It will also prompt for comfirmation unless the -Force switch is applied.

## EXAMPLES

### EXAMPLE 1
```
C:\> Install-OzetteCloudBackup
```

Starts the installation with default options.
The user will be prompted for confirmation.

### EXAMPLE 2
```
C:\> Install-OzetteCloudBackup -InstallDirectory "D:\Applications\Ozette Cloud Backup" -Force
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
Default value: C:\Program Files\Ozette Cloud Backup
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
