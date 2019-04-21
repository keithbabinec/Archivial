---
external help file: ArchivialPowerShell.dll-Help.xml
Module Name: ArchivialPowerShell
online version:
schema: 2.0.0
---

# Remove-ArchivialNetworkCredential

## SYNOPSIS
Removes the stored network credential used for connecting to network resources.

## SYNTAX

### ByName
```
Remove-ArchivialNetworkCredential -CredentialName <String> [<CommonParameters>]
```

### ByObject
```
Remove-ArchivialNetworkCredential -NetCredential <NetCredential> [<CommonParameters>]
```

## DESCRIPTION
Authenticated Network Source locations have an encrypted credential stored with them.
This command is used to remove that stored credential.

To view existing network credentials, run Get-ArchivialNetworkCredentials.
This command supports piping from Get-ArchivialNetworkCredentials or manual invoke from credential name

## EXAMPLES

### EXAMPLE 1
```
C:\> Remove-ArchivialNetworkCredential -CredentialName "drobo-nas"
```

Removes the stored network credential with the specified name.

### EXAMPLE 2
```
C:\> Get-ArchivialNetworkCredentials | Where CredentialName -eq "drobo-nas" | Remove-ArchivialNetworkCredential
```

Removes the stored network credential, but using the pipeline scenario.

## PARAMETERS

### -CredentialName
Specify the name of the credential to remove.

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

### -NetCredential
Specify the object (from pipeline) to remove.

```yaml
Type: NetCredential
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

### ArchivialLibrary.Secrets.NetCredential
Specify the object (from pipeline) to remove.

## OUTPUTS

## NOTES

## RELATED LINKS
