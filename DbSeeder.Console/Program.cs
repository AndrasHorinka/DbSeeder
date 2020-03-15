using System;


/// <summary>
/// This is the entry class to check if args is given - to process the rules
/// </summary>
public class Program
{
	public static void Main(string[] args)
	{
		// Check if arguments are provided
		if (args.Length == 0)
		{
			PrintHelp();
			Environment.Exit(160);
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
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1}", "-u", "https://localhost:5001/myApi/endPoint/{uriParam1}/{uriParam2}");
		Console.WriteLine("\tExample: DbSeeder.exe {0} {1}", "-u", "https://55.146.23.33/myApi/endPoint/{uriParam1}/{uriParam2}");

		Console.WriteLine("\n{0,-5} : {1}", "-k", "Stands for the filename storing the keys and their types. Must be followed by a filename.");
		Console.WriteLine("\tNote - Default type is string.");
		Console.WriteLine("\tNote - Default separator is comma");
		Console.WriteLine("\tNote - Key names must be unique!");
		Console.WriteLine("\t\tExample: \n\t\t\turiParam1, string\n\t\t\turiParam2, int");
		Console.WriteLine("\t\tPossible key tpes are:" );
		Console.WriteLine("\t\t\tstring, int, bol");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1}", "-k", "keys.csv");

		Console.WriteLine("\n{0, -5} : {1}", "-s", "To overwrite default separator.");
		Console.WriteLine("\tNote - there are predefined, common separators you can use - or specify your own keyword for it.");
		Console.WriteLine("\t\tExample:\n\t\t\t{0,-5} : {1}\n\t\t\t{2,-5} : {3}\n\t\t\t{4,-5} : {5}", "!t", "Separate by tabs", "!s", "Separate by spaces", "!n", "Separate by newlines");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1} \t {2,20}", "-s", "!", "Exclamation mark will be used as separator.");
		Console.WriteLine("\tExample: DbSeeder.exe {0} {1} \t {2,20}", "-s", "tab", "The word of tab will be used as separator.");
		Console.WriteLine("\tExample: DbSeeder.exe {0} {1} \t {2,20}", "-s", "!t", "Tabs will be used as separator.");

		Console.WriteLine("\n{0, -5} : {1}", "-m", "To overwride the default method to be used.");
		Console.WriteLine("\tNote - Default is POST");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1} \t {2, 20}", "-m", "PATCH", "HttpRequest will be PATCH");

		Console.WriteLine("\n{0,-5} : {1}", "-c", "Receive a full file with all details. If -c argument is given, others are ignored.");
		Console.WriteLine("\tNote - First line should contain the URL as explained in -u flag");
		Console.WriteLine("\tNote - Second line should contain all the keys in order, in which values will be passed. uriParams must be included as well!");
		Console.WriteLine("\tNote - Third line should contain the type of keys. uriParams MUST be string.");
		Console.WriteLine("\tNote - As of fourth line, sample data is required.");
		Console.WriteLine("\n\tExample: DbSeeder.exe {0} {1}", "-c", "fullConfigFile.csv");
		Console.WriteLine("\tFile content as:");
		Console.WriteLine("\t\t{0}\n\t\t{1}\n\t\t{2}\n\t\t{3}\n\t\t{4}", 
			"https://localhost:5001/myApi/endPoint/{uriParam1}/{uriParam2}", 
			"uriParam1,uriParam2,firstName,lastName", 
			"string,string,string,string", 
			"gym,users,John,Doe", 
			"gym,users,Hulk,Hogan");

	}
}
