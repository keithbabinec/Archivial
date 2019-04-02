---
external help file: OzettePowerShell.dll-Help.xml
Module Name: OzettePowerShell
online version:
schema: 2.0.0
---

# Set-OzetteAzureProviderOptions

## SYNOPSIS
Configures the Azure cloud storage provider as a backup destination.

## SYNTAX

```
Set-OzetteAzureProviderOptions -AzureStorageAccountName <String> -AzureStorageAccountToken <String>
 [<CommonParameters>]
```

## DESCRIPTION
In order to backup files to the cloud, at least one cloud storage provider must be configured.
This command will enable the Azure cloud storage provider for this purpose.

This command assumes that you have already deployed an Azure storage account and have the access token ready.

If your access token has changed, you can safely re-run this command with the new token, and then restart the Ozette Cloud Backup service for the changes to take effect.

If you would like to disable this provider, please run the Remove-OzetteProvider cmdlet.

All provided options here (account name and token) are encrypted before saving to the database.

## EXAMPLES

### EXAMPLE 1
```
C:\> Set-OzetteAzureProviderOptions -AzureStorageAccountName "myaccount" -AzureStorageAccountToken "--my token--"
```

Configures Azure as a cloud storage backup destination.

## PARAMETERS

### -AzureStorageAccountName
Specify the name of the Azure storage account to upload backup data to.

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

### -AzureStorageAccountToken
Specify the access token of the Azure storage account.

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
