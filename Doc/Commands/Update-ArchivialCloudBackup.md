---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Update-ArchivialCloudBackup

## SYNOPSIS
Updates the Archivial Cloud Backup software on this computer.

## SYNTAX

```
Update-ArchivialCloudBackup [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
IMPORTANT: The Archivial version that will be installed with this command is tied to the version number of this module.
For example if this module is Archivial PowerShell version v1.0.0, then running this command will attempt to upgrade your current installation to v1.0.0.

To ensure you upgrade using the latest software, always update this PowerShell module (then restart PowerShell) before running this upgrade command.
See the examples for more details.

This command requires an elevated (run-as administrator) PowerShell prompt to complete.
It will also prompt for comfirmation unless the -Force switch is applied.

## EXAMPLES

### EXAMPLE 1
```
C:\> $latestVersion = (Find-Module -Name ArchivialPowerShell).Version
```

C:\\\> Update-Module -Name ArchivialPowerShell -RequiredVersion $latestVersion

The two above commands will update your Archivial PowerShell module to latest.
After that has completed, close and restart the PowerShell window.

C:\\\> Update-ArchivialCloudBackup

With the latest management tools installed, this command updates your installation.

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
