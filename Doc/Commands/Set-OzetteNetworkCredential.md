---
external help file: OzettePowerShell.dll-Help.xml
Module Name: OzettePowerShell
online version:
schema: 2.0.0
---

# Set-OzetteNetworkCredential

## SYNOPSIS
Saves the credentials required to connect to an authenticated network resource (such as a UNC path share).

## SYNTAX

```
Set-OzetteNetworkCredential -CredentialName <String> -ShareUsername <String> -SharePassword <String>
 [<CommonParameters>]
```

## DESCRIPTION
Network Sources can be authenticated (require username/password), or unauthenticated (open access).
If this Network Source requires authenticated access, you must provide use this command to pre-store the authentication details so the backup engine can connect to the resource.

The credential username and password are both encrypted and saved to the database.

## EXAMPLES

### EXAMPLE 1
```
C:\> Set-OzetteNetworkCredential -CredentialName "drobo-device" -ShareUser "drobo-private-user" -SharePassword ****
```

Encrypts and stores the network resource credentials in the database.

## PARAMETERS

### -CredentialName
Specify the friendly name (description) to refer to this stored credential.

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

### -ShareUsername
Specify the username required to connect to the network resource.

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

### -SharePassword
Specify the password required to connect to the network resource.

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
