package com.thealmostengineer.rebootrouter;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.util.Properties;

/**
 * Handles the loading of a properties file. 
 * 
 * @author almostengr, Kenny Robinson, thealmostengineer.com
 *
 */
public class PropertiesFileHandler {
	/**
	 * Read in the properties file and load it into a variable.
	 * 
	 * @param filePath The file path to the properties file to be loaded
	 * @return
	 */
	Properties readPropertyFile(String filePath) {
//		UserInterface.logMessage("Loading properties file " + filePath);
		Properties properties = new Properties();
		File file = new File(filePath);
		FileReader fileReader = null;
		
		try {
			fileReader = new FileReader(file);
			properties.load(fileReader);
			fileReader.close(); // close properties file
		} catch (FileNotFoundException e) {
			e.printStackTrace();
			properties = null;
		} catch(IOException e) {
			e.printStackTrace();
			properties = null;
		} // end try
		
		return properties;
	} // end function
}
