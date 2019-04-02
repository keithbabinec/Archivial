---
external help file: OzettePowerShell.dll-Help.xml
Module Name: OzettePowerShell
online version:
schema: 2.0.0
---

# Set-OzetteTwilioProviderOptions

## SYNOPSIS
Configures the Twilio messaging provider as a status update recipient.

## SYNTAX

```
Set-OzetteTwilioProviderOptions -TwilioAccountID <String> -TwilioAuthToken <String> -TwilioSourcePhone <String>
 -TwilioDestinationPhones <String> [<CommonParameters>]
```

## DESCRIPTION
Messaging providers are an optional way to be automatically notified of your backup status/progress.
This command configures the Twilio (SMS/Text) provider for that purpose.

This command assumes that you have already setup a Twilio account, phone number, and have the required access token details ready.
Twilio expects phone numbers to be provided in the E.164 format.
If providing multiple destination phone numbers, they can be seperated by a semicolon.

If your access token has changed, you can safely re-run this command with the new token, and then restart the Ozette Cloud Backup service for the changes to take effect.

If you would like to disable this provider, please run the Remove-OzetteProvider cmdlet.

All provided options here (ex: account name, token, phone numbers) are encrypted before saving to the database.

## EXAMPLES

### EXAMPLE 1
```
C:\> Set-OzetteTwilioProviderOptions -TwilioAccountID "myaccount" -TwilioAuthToken "--token--" -TwilioSourcePhone "+12065551234" -TwilioDestinationPhones "+12065554567;+12065556789"
```

Configures Twilio as a status messaging recipient.

## PARAMETERS

### -TwilioAccountID
Specify the Twilio Account ID.

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

### -TwilioAuthToken
Specify the Twilio Authentication token.

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

### -TwilioSourcePhone
Specify the Twilio phone number (sender).

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

### -TwilioDestinationPhones
Specify the phone number(s) to send updates to.
If multiple, seperate by semicolon.

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
