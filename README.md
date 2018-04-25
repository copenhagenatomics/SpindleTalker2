![alt text](https://github.com/copenhagenatomics/SpindleTalker2/blob/master/ScreenShots/gauge.png)

I'm pushing this out slightly undercooked as I'm heading off on holidays for a month :-) As such it's probably got a fair few bugs, I've only really done 'happy path' testing i.e. I haven't gone out of my way to do odd things to see what happens. There's limited error handling and I'd be fairly confident that there are a number of scenarios that will get the polling into a twist but restarting the app should reset things.

I'm releasing it now to (a) gauge interest and (b) allow some other people to do beta testing if they're interested in helping. All feedback welcome.

This is a Windows application to control the Huanyang Inverter that seems to be a popular choice for mid-range hobbyist CNC users. The model I have is the HY01D523B. This inverter and the accompanying spindle are frequently referred to as "the Chinese spindle" on some forums.

Just extract the contents of the zip file below into the directory of your choice. The only dependency should be .NET 4.0 or above. (edit - just remembered I used a non-standard font for the gauges screen and it's out of proportion when it defaults to the system fonts. If you want to install the font I used it's "Digital 7" true type font. Google "Digital 7 Font" for a download or here for the creator's page Digital-7 )

The program is designed to allow standalone control of the spindle. There is a Mach3 plugin out there which I eventually hope to use myself but I've written this for use with my grbl controller and ShapeOKO2.

The application requires the version 4.0 or above of the .NET Framework. It is written in Visual C# using Visual Studio 2013.

It is based heavily on the work of user 'Scruffoid' and his 'Spindle Talker' application from http://www.woodworkforums.com/showthread.php?t=96380&page=8 I hope he doesn't mind me using the name, I mean it as an acknowledgement of his work :-)

While I've written the graphical interface myself from scratch the app wouldn't have been possible without that app to help me understand the mechanics of the rs485 protocol and Huanyang's slightly eccentric implementation of it.

I ordered the following inverter/spindle combo along with the RS485 USB/Serial converter below that.

http://cgi.ebay.co.uk/ws/eBayISAPI.dll?ViewItem&item=250951586255 (Inverter/Spindle)
http://cgi.ebay.co.uk/ws/eBayISAPI.dll?ViewItem&item=221389229691 (RS485 adapter)

I configured the following settings on my VFD (almost totally copied from http://make-a-project.blogspot.co.uk/2013/05/connecting-huanyang-hy01d523b-vfd-speed.html). I won't pretend to understand all of these so if you think you should be using different values, you are probably right.
```
PD013 = 8 (reset to factory settings, seems to have solved issues for a LOT of people)
PD005 = 400 (max frequency 400Hz)
PD004 = 400 (base frequency 400Hz)
PD003 = 400 (main frequency 400Hz)
PD001 = 2 (set communication port as source of run commands)
PD002 = 2 (set communication port as source of operating frequency)
PD023 = 1 (enable run-backwards, only needed if you want the option to run either direction)
PD163 = 1 (slave address 1)
PD164 = 1 (baud rate 9600 bps)
PD165 = 3 (8N1 for RTU mode)
```
The settings below this are spindle/locale specific. I have a 1.5KW two pole, air-cooled spindle rated to 24,000 RPM @ 400Hz and with a minimum frequency requirement of 100Hz to enable cooling. It is important to make sure you have the right settings for your spindle.

```
PD008 = 220 (max voltage 220V)
PD009 = 15 (intermediate voltage 15V)
PD010 = 8 (min voltage 8V)
PD011 = 100 (frequency lower limit 100Hz, to limit lower RPM settings, particularly important for air cooled spindles, mine doesn't start to cool at anything under 100Hz)
PD014 = 10.0 (acceleration time, 10 seconds)
PD015 = 10.0 (deceleration time, this seems higher than most people recommend but I was testing)
PD025 = 0 (starting mode: from starting frequency - the URL above recommended setting this to 1 but I found it caused my spindle to run at full speed almost immediately)
PD142 = 7 (max current 7 A)
PD143 = 2 (specific to my 1.5 KW spindle: number of poles - 2)
PD144 = 3000 (PD144 is the speed in RPM that your spindle will run when the output frequency is 50Hz)
```
When my application connects to the VFD initially it queries PD005, PD011 and PD144 to obtain the maximum supported frequency, the minimum supported frequency and what RPM 50Hz equates to respectively. It then uses these figures to calculate the maximum and minimum RPM and will not send commands to exceed these. Therefore it is important that these are set correctly.

I've also left as much of the RS485 protocol command stack exposed as is practical to assist anyone else trying to develop their own software.

Some additional screenshots

![alt text](https://github.com/copenhagenatomics/SpindleTalker2/blob/master/ScreenShots/terminal.png)

![alt text](https://github.com/copenhagenatomics/SpindleTalker2/blob/master/ScreenShots/settings.png)
