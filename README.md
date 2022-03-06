# Home Automation Microservices

This project originaly was for internet monitor. It has expanded to be a microservices platform 
that integrates with Home Assistant for home automation. Several IOT devices have web interfaces, such as 
routers, but do not have APIs. Thus this API was created so that an API could be called by the home 
automation system, to perform tasks that via the device's HTML interface.

## Internet Monitor

Reboots the DD-WRT router when there are no WiFi clients connected to it.

### Problem

My router would occasionally disconnect any of the wireless clients that were connected to it. Since
I have wired and wireless devices connected, some of the time I would not know that the wifi had
stopped working until my phone or laptop wouldn't connect or I noticed that a automation did not
trigger when it should have.

### Solution

After updating to the latest DD-WRT firmware, the problem persisted.  Given that I work building
automations to improve business processes, I decided that I would build and automation that would
connect to the router’s web interface and see if there are any wireless clients connected to it.
If there are no connected wireless clients, then there is a great chance that the wifi process that
authenticates clients has crashed.  As a result, the process would need to be restarted and the easiest
way to restart the process would be to reboot the router.

### Project Information

For more information about this project and help documentation, visit 
[https://thealmostengineer.com/internetmonitor](https://thealmostengineer.com/internetmonitor).

## HD Home Run

Can install updates and check the status of the HD HomeRun.

### Problem

HD HomeRun receives updates via the web and displays a notification on the web UI of the device. Unless 
you visit the web UI of the device, you will not know that an update is ready to be installed.

### Solution 

Create a Selenium Webdriver automation that is triggered by an API call from Home Assistant to see 
if there updates available. When updates are available, Home Assistant will show a notification about 
the updates. This also has functionality to get the system and tuner status of the HD HomeRun and 
display it in Home Assistant.

## Transcript Cleaner

Cleans transcripts that have been download from YouTube. 

This application is used to create blog posts on by business website [rhtservices.net](https://rhtservices.net),
[business Facebook page](https://www.facebook.com/rhtservicesllc), 
my blog [thealmostengineer.com](https://thealmostengineer.com),
as well as to be used on [YouTube](https://www.youtube.com/channel/UC4HCouBLtXD1j1U_17aBqig?sub_confirmation=1)
after they have been proofread and properly formatted.

To use this application, you will have to compile it like any other .NET Core application either via the 
command line or using Visual Studio.

This application is designed to only process SRT caption files. May be expanded in the future to 
process additional caption and subtitle files.
