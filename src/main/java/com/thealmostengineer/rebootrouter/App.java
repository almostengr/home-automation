package com.thealmostengineer.rebootrouter;

import java.util.Properties;
import java.util.concurrent.TimeUnit;
import java.util.logging.Logger;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.firefox.FirefoxDriver;
import org.openqa.selenium.firefox.FirefoxOptions;

/**
 * Reboots the DD-WRT router when there are no WiFi clients connected to it.
 * 
 * Sometimes the WiFi related processes that allows for authentication crashes. When this happens, 
 * ethernet connections are not impacted. As a result, some connectivity issues can occur without 
 * warning. This automation resolves that problem by accessing the web UI and restarting the 
 * router if there are no wireless devices connected.
 * 
 * @author almostengr, Kenny Robinson, thealmostengineer.com
 *
 */
public class App 
{
	static Logger logger = Logger.getLogger(App.class.getName());
	static final int REBOOTTIMEOUT = 60;
	
	static WebDriver clickRebootButton (WebDriver dWebDriver, String routerUrl, String username, String password) throws InterruptedException {
		// split the URL into parts
		String protocol = routerUrl.substring(0, routerUrl.indexOf("://"));
		String hostname = routerUrl.substring(routerUrl.indexOf("://")+3);
		
		// reassemble URL with credentials included
		String authenticationCredentials = username + ":" + password;
		String authenticatedUrl = protocol + "://" + authenticationCredentials + "@" + hostname;

		logger.info("Going to management page");
		dWebDriver.get(authenticatedUrl + "Management.asp");
		
		logger.info("Clicking Reboot button");
		dWebDriver.findElement(By.name("reboot_button")).click();
		
		String bodyText = dWebDriver.findElement(By.tagName("body")).getText();
		
		if (bodyText.contains("Unit is rebooting now. Please wait a moment...")) {
			logger.info("Reboot request was successful");
			TimeUnit.SECONDS.sleep(REBOOTTIMEOUT);
		}
		else {
			logger.severe("Error occurred when attempting to click reboot button");
		}
		
		return dWebDriver;
	}
	
    public static void main( String[] args )
    {
    	logger.info("Starting process");
    	int exitCode = 1;
    	WebDriver webDriver = null;
    	
    	try {
        	PropertiesFileHandler propertiesFileHandler = new PropertiesFileHandler();
        	Properties properties = propertiesFileHandler.readPropertyFile(args[0]);
        	
    		WebDriverSetup webDriverSetup = new WebDriverSetup();
    		
    		webDriverSetup.setDriverProperties(properties.getProperty("geckoDriverPath"));
    		
    		// run headless to not interfere with user or desktop
    		FirefoxOptions firefoxOptions = new FirefoxOptions();
    		firefoxOptions.setHeadless(true);
    		
    		webDriver = new FirefoxDriver(firefoxOptions);
    		webDriver = webDriverSetup.setTimeouts(webDriver, REBOOTTIMEOUT);
    		
    		for (int i = 0; i < 5; i++) {
	    		webDriver.get(properties.getProperty("routerUrl"));
	    		
	    		String wirelessTableText = webDriver.findElement(By.id("wireless_table")).getText();
	    		logger.info(wirelessTableText);
	    		logger.info("Uptime: " + webDriver.findElement(By.id("uptime")).getText());
	    		
    		
	    		if (wirelessTableText.contains("ath0") || wirelessTableText.contains("ath1")) {
	    			// no further action is required
	    			logger.info("Wireless table is not empty. No further action is required");
	    			break;
	    		}
	    		else {
	    			// perform reboot steps
	    			logger.warning("Wireless table is empty");
	        		clickRebootButton(webDriver, properties.getProperty("routerUrl"), properties.getProperty("username"), properties.getProperty("password"));
	    		}
    		} // end for
    		
    		logger.info("Uptime: " + webDriver.findElement(By.id("uptime")).getText());
    		
			exitCode = 0;
		} catch (Exception e) {
			logger.severe("Unexpected error occurred");
			e.printStackTrace();
		}
    	
    	if (webDriver != null) {
    		logger.info("Closing browser");
    		webDriver.quit();
    	}
    	
    	logger.info("Exit code: " + exitCode);
    	System.exit(exitCode);
    }
}

