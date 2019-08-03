# Reboot Router 

Reboots the DD-WRT router when there are no WiFi clients connected to it.

Sometimes the WiFi related processes that allows for authentication crashes. When this happens, 
ethernet connections are not impacted. As a result, some connectivity issues can occur without 
warning. This automation resolves that problem by accessing the web UI and restarting the 
router if there are no wireless devices connected.

