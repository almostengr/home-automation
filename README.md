# Internet Monitor

Reboots the DD-WRT router when there are no WiFi clients connected to it.

## Background

My router would occasionally disconnect any of the wireless clients that were connected to it. Since
I have wired and wireless devices connected, some of the time I would not know that the wifi had
stopped working until my phone or laptop wouldn't connect or I noticed that a automation did not
trigger when it should have.

## Solution

After updating to the latest DD-WRT firmware, the problem persisted.  Given that I work building
automations to improve business processes, I decided that I would build and automation that would
connect to the router’s web interface and see if there are any wireless clients connected to it.
If there are no connected wireless clients, then there is a great chance that the wifi process that
authenticates clients has crashed.  As a result, the process would need to be restarted and the easiest
way to restart the process would be to reboot the router.

## The Technology

This solution uses Selenium Webdriver to connect to the DD-WRT router web interface. I built this using
.NET Core, but it could be done on Python, Java, or one of the other platforms that Webdriver works on.
I have it set up on server that is always running and it runs as a system service.

## Author

Kenny Robinson, @almostengr

[https://thealmostengineer.com](themostengineer.com)

## Checking Job Log

To see the log for the job, run the command

```sh
journalctl -u internetmonitor -b
```

or

```sh
journalctl -u internetmonitor -b -f
```

## Create System Service on Ubuntu 20.04

```bash
cp internetmonitor.service /lib/systemd/system
sudo systemctl daemon-reload
sudo systemctl enable internetmonitor
sudo systemctl start internetmonitor
sudo systemctl status internetmonitor
```

Once all of the commands above have been ran, you should see an output similar to the following:

```txt
iamadmin@media:/usr/lib$ sudo systemctl status internetmonitor
 internetmonitor.service - Internet Connectivity Monitor by almostengr
     Loaded: loaded (/lib/systemd/system/internetmonitor.service; disabled; vendor preset: enabled)
     Active: active (running) since Fri 2021-01-29 09:03:45 CST; 13s ago
       Docs: https://github.com/almostengr/internetmonitor
   Main PID: 7251 (Almostengr.Inte)
      Tasks: 14 (limit: 38351)
     Memory: 19.0M
     CGroup: /system.slice/internetmonitor.service
             └─7251 /usr/lib/internetmonitor/Almostengr.InternetMonitor

Jan 29 09:03:45 media Almostengr.InternetMonitor[7279]: ChromeDriver was started successfully.
Jan 29 09:03:46 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Performing checks at 01/29/2021 09:03:46 -06:00
Jan 29 09:03:46 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Converting router URL
Jan 29 09:03:51 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Wireless clients are connected. 11 devices found
Jan 29 09:03:51 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Checking the modem status page
Jan 29 09:03:53 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Checking the CM State page
Jan 29 09:03:53 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Browser has been closed
Jan 29 09:03:53 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Done performing checks at 01/29/2021 09:03:53 -06:00
Jan 29 09:03:53 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Sleeping for 600 seconds
Jan 29 09:03:53 media Almostengr.InternetMonitor[7251]: Microsoft.Hosting.Lifetime[0] Application started. Hosting environment: Production; Content root path>
```

## Remove System Service on Ubuntu 20.04

```sh
sudo systemctl disable internetmonitor
sudo systemctl stop internetmonitor
sudo systemctl status internetmonitor
```

Once all of the commands above have been ran, you should see an output similar to the following:

```txt
iamadmin@media:/usr/lib$ sudo systemctl status internetmonitor
 internetmonitor.service - Internet Connectivity Monitor by almostengr
     Loaded: loaded (/lib/systemd/system/internetmonitor.service; disabled; vendor preset: enabled)
     Active: inactive (dead)
       Docs: https://github.com/almostengr/internetmonitor

Jan 29 11:47:48 media systemd[1]: Stopping Internet Connectivity Monitor by almostengr...
Jan 29 11:47:48 media Almostengr.InternetMonitor[7251]: Microsoft.Hosting.Lifetime[0] Application is shutting down...
Jan 29 11:47:48 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Shutting down monitor
Jan 29 11:47:48 media Almostengr.InternetMonitor[7251]: Almostengr.InternetMonitor.Worker[0] Browser has been closed
Jan 29 11:47:48 media systemd[1]: internetmonitor.service: Succeeded.
Jan 29 11:47:48 media systemd[1]: Stopped Internet Connectivity Monitor by almostengr.
```

