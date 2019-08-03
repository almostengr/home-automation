package com.thealmostengineer.rebootrouter;

import java.util.Properties;
import java.util.concurrent.TimeUnit;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.firefox.FirefoxDriver;

//import com.thealmostengineer.drupal7.webdriver.PropertiesFileHandler;

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
	static WebDriver clickRebootButton (WebDriver dWebDriver, String routerUrl, String username, String password) throws InterruptedException {
		String protocol = routerUrl.substring(0, routerUrl.indexOf("://"));
		String hostname = routerUrl.substring(routerUrl.indexOf("://")+3);
		String authenticationCredentials = username + ":" + password;
		String authenticatedUrl = protocol + "://" + authenticationCredentials + "@" + hostname;

		System.out.println("Going to Management page");
		dWebDriver.get(authenticatedUrl + "Management.asp");
		
		System.out.println("Clicking Reboot button");
		dWebDriver.findElement(By.name("reboot_button")).click();
		
		String bodyText = dWebDriver.findElement(By.tagName("body")).getText();
		
		if (bodyText.contains("Unit is rebooting now. Please wait a moment...")) {
			System.out.println("Reboot was successful");
			TimeUnit.SECONDS.sleep(30);
		}
		else {
			System.err.println("Error occurred when attempting to click reboot button");
		}
		
		return dWebDriver;
	}
	
    public static void main( String[] args )
    {
    	int exitCode = 1;
    	WebDriver webDriver = null;
    	
    	try {
        	PropertiesFileHandler propertiesFileHandler = new PropertiesFileHandler();
        	Properties properties = propertiesFileHandler.readPropertyFile(args[0]);
        	
    		WebDriverSetup webDriverSetup = new WebDriverSetup();
    		
    		webDriverSetup.setDriverProperties(properties.getProperty("geckoDriverPath"));
    		
    		webDriver = new FirefoxDriver();
    		webDriver = webDriverSetup.setTimeouts(webDriver, 30);
    		
    		for (int i = 0; i < 5; i++) {
	    		webDriver.get(properties.getProperty("routerUrl"));
	    		
	    		String wirelessTableText = webDriver.findElement(By.id("wireless_table")).getText();
	    		System.out.println(wirelessTableText);
    		
	    		if (wirelessTableText.contains("ath0") || wirelessTableText.contains("ath1")) {
	    			// no further action is required
	    			System.out.println("Wireless table is not empty. No further action is required.");
	    			break;
	    		}
	    		else {
	    			// perform reboot steps
	    			System.out.println("Wireless table is empty");
	        		clickRebootButton(webDriver, properties.getProperty("routerUrl"), properties.getProperty("username"), properties.getProperty("password"));
	    		}
    		} // end for
    		
			exitCode = 0;
		} catch (Exception e) {
			System.err.println("Unexpected error occurred");
			e.printStackTrace();
		}
    	
    	if (webDriver != null) {
    		webDriver.quit();
    	}
    	
    	System.exit(exitCode);
    }
}
