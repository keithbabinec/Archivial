---
external help file: OzettePowerShell.dll-Help.xml
Module Name: OzettePowerShell
online version:
schema: 2.0.0
---

# Add-OzetteNetworkSource

## SYNOPSIS
Adds a network (UNC path) folder to the Ozette backup folders list.

## SYNTAX

```
Add-OzetteNetworkSource -UncPath <String> -Priority <String> -Revisions <Int32> [-CredentialName <String>]
 [-MatchFilter <String>] [<CommonParameters>]
```

## DESCRIPTION
A Network Source is a folder on your network (referenced by UNC Path) that you would like Ozette to backup and automatically monitor for new and updated files.

Network Sources can be authenticated (require username/password), or unauthenticated (open access).
If this Network Source requires authenticated access, you must provide the name of an already saved Network Credential (see examples).
Network sources can use either a DNS or IP Address for the host.

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
C:\> Add-OzetteNetworkSource -UncPath "\\drobo-nas-device\public\media\music" -Priority Medium -Revisions 1
```

Adds the specified unauthenticated network share to backup.

### EXAMPLE 2
```
C:\> Set-OzetteNetworkCredential -CredentialName "drobo-device" -ShareUser "drobo-private-user" -SharePassword ****; Add-OzetteNetworkSource -UncPath "\\drobo-nas-device\private\documents\taxes" -CredentialName "drobo-device" -Priority Medium -Revisions 4 -MatchFilter *.pdf
```

Encrypts and stores the network resource credentials, and then adds the specified authenticated network share to backup.

## PARAMETERS

### -UncPath
Specify the UNC folder path that should be backed up and monitored.

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

### -CredentialName
Optionally specify the name of a stored credential to authenticate this share with.

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
