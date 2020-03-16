using DbSeeder.Model.Models;
using System;
using System.IO;

/// <summary>
/// This is the entry class to check if args is given - to process the rules
/// </summary>
public class Program
{
	public static void Main(string[] args)
	{
		SeedDetails seedDetails = new SeedDetails();
		// Check if arguments are provided
		if (args.Length == 0)
		{
			PrintHelp();
			Environment.Exit(160);
		}

		// Check if -c flag is used with a file with all details
		var cFlagIndex = Array.IndexOf(args, "-c");
		if (cFlagIndex != -1)
		{
			// Retrieve the name of Complex file
			var complexFileName = args[cFlagIndex + 1];
			// Create a FileInfo and check if File exists
			var complexFile = new FileInfo(complexFileName);
			// If FileInfo does not exists - try to create it with full path
			if (!complexFile.Exists)
			{
				var fullPath = Path.Combine(Environment.CurrentDirectory, complexFileName);
				complexFile = new FileInfo(fullPath);
			}
			// If FileInfo cannot be retrieved, exit with invalid argument error code.
			if (!complexFile.Exists)
			{
				Console.WriteLine("Complex file not found! {0,20}" , complexFileName);
				Environment.Exit(160);
			}

			// Setup default separator according to filetype
			switch (complexFile.Extension)
			{
				case "txt":
					seedDetails.Separator = " ";
					break;
				case "csv":
					seedDetails.Separator = ",";
					break;
				case "tsv":
					seedDetails.Separator = "\t";
					break;
				default:
					break;
			}

			// Read file line by line and feed seedDetails
			// 1st line for URL
				// find { and } - add content in between to seedDetails.UrlKeys --> repeat with updated index
			// 2nd line for keys - add each to seedDetails.JsonKeys.Keys
			// 3rd line for key types -- add each type to seedDetails.JsonKeys.Values
			// 4th line for separator -- add to seedDetails.Separator
			// As of 5th line, by line:
				// Create an instance of SeedItem
				// split line by Separator
				// iterate through array - 
					// if i < UrlKeys.length and string replace with values from the line -- save updated URL to seedItem.Uri
					// else , get JsonKeys[i-UrlKeys.length] - and add the current value to seedItem.JsonParameters as key-value pair
		}
		else 
		{
			// TODO: iterate through args - and seed data to SeedDetails
		}
		
	}

	private static void PrintHelp()
	{
		Console.WriteLine("DbSeeder Console Help");
		Console.WriteLine("\nFollowing arguments are possible");
		Console.WriteLine("\n{0,-5} : {1}", "-e", "Stands for the name of the file with the sample data. Must be followed by a filename.");
		Console.WriteLine("\tNote - The first line of the file should represent respective keys - in order which values are passed.");
		Console.WriteLine("\t\turiParam1,uriParam2,firstname,lastname\n\t\tgym,users,John,Doe\n\t\tgym,users,Hulk,Hogan");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1}", "-e", "examples.csv");

		Console.WriteLine("\n{0,-5} : {1}", "-u", "Stands for the URL to be triggered");
		Console.WriteLine("\tNote - uriParameters must match the name of the keys ");
		Console.WriteLine("\tNote - uriParameters must be placed between curly braces: {}");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1}", "-u", "https://localhost:5001/myApi/endPoint/{uriParam1}/{uriParam2}");
		Console.WriteLine("\tExample: DbSeeder.exe {0} {1}", "-u", "https://55.146.23.33/myApi/endPoint/{uriParam1}/{uriParam2}");

		Console.WriteLine("\n{0,-5} : {1}", "-k", "Stands for the filename storing the keys and their types. Must be followed by a filename.");
		Console.WriteLine("\tNote - Default type is string.");
		Console.WriteLine("\tNote - Default separator is comma");
		Console.WriteLine("\tNote - Key names must be unique!");
		Console.WriteLine("\t\tPossible key types are:" );
		Console.WriteLine("\t\t\tstring, int, bol");
		Console.WriteLine("\t\tExample: \n\t\t\turiParam1, string\n\t\t\turiParam2, string,\n\t\t\tJsonParam1, int");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1}", "-k", "keys.csv");

		Console.WriteLine("\n{0, -5} : {1}", "-s", "To overwrite default separator.");
		Console.WriteLine("\tNote - there are predefined, common separators you can use - or specify your own keyword for it.");
		Console.WriteLine("\t\tExample:\n\t\t\t{0,-5} : {1}\n\t\t\t{2,-5} : {3}\n\t\t\t{4,-5} : {5}", "!t!", "Separate by tabs", "!s!", "Separate by spaces", "!n!", "Separate by newlines");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1} \t {2,20}", "-s", "!", "Exclamation mark will be used as separator.");
		Console.WriteLine("\tExample: DbSeeder.exe {0} {1} \t {2,20}", "-s", "tab", "The word of tab will be used as separator.");
		Console.WriteLine("\tExample: DbSeeder.exe {0} {1} \t {2,20}", "-s", "!t!", "Use Tab as separator");

		Console.WriteLine("\n{0, -5} : {1}", "-m", "To overwride the default method to be used.");
		Console.WriteLine("\tNote - Default is POST");
		Console.WriteLine("\tNote - Available: PATCH, PUT, DELETE, POST");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1} \t {2, 20}", "-m", "PATCH", "HttpRequest will be PATCH");

		Console.WriteLine("\n{0,-5} : {1}", "-c", "Receive a file with all details. If -c argument is given, others are ignored.");
		Console.WriteLine("\tNote - First line should contain the URL as explained in -u flag");
		Console.WriteLine("\tNote - Second line should contain all the keys in order separated by comma. uriParams must NOT be included!");
		Console.WriteLine("\tNote - Third line should contain the type of keys separated by comma. Types must be in the same order as keys in second line!");
		Console.WriteLine("\tNote - Fourth line should state the separator only. ");
		Console.WriteLine("\t\t{0, -5} \t\t {1, -20}", "!t!", "Use Tab as separator");
		Console.WriteLine("\t\t{0, -5} \t\t {1, -20}", "!s!", "Use Space as separator");
		Console.WriteLine("\t\t{0, -5} \t\t {1, -20}", "!n!", "Use NewLine as separator");
		Console.WriteLine("\t\t{0, -5} \t\t {1, -20}", "tab", "The word of tab will be used as separator.");
		Console.WriteLine("\tNote - As of fifth line, sample data is required.");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1}", "-c", "fullConfigFile.csv");
		Console.WriteLine("\t\tFile content as:");
		Console.WriteLine("\t\t\t{0}\n\t\t\t{1}\n\t\t\t{2}\n\t\t\t{3}\n\t\t\t{4}", 
			"https://localhost:5001/myApi/endPoint/{uriParam1}/{uriParam2}", 
			"uriParam1,uriParam2,firstName,lastName", 
			"string,string,string,string", 
			"gym,users,John,Doe", 
			"gym,users,Hulk,Hogan");

	}
}
