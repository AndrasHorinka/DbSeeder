using DbSeeder.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// This is the entry class to check if args is given - to process the rules
/// </summary>
public class Program
{
	public static void Main(string[] args)
	{
		// Exit if arguments are not provided
		if (args.Length == 0)
		{
			PrintHelp();
			Environment.Exit(160);
		}
		try
		{
			SeedDetails seedDetails = new SeedDetails();
			seedDetails.SeedItems = new List<SeedItem>();
			// Check if -c flag is used with a file with all details
			var cFlagIndex = Array.IndexOf(args, "-c");
			if (cFlagIndex != -1)
			{
				// Retrieve the name of Complex file and create a FileInfo
				var complexFileName = args[cFlagIndex + 1];
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
					Console.WriteLine("Complex file not found! {0,20}", complexFileName);
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
				try
				{
					using (StreamReader reader = new StreamReader(complexFile.FullName))
					{
						string line;
						int lineCounter = 0;
						IList<string> keys = new List<string>();
						IList<string> keyTypes = new List<string>();

						// Read file line by line and feed seedDetails
						while ((line = reader.ReadLine()) != null)
						{
							lineCounter++;
							switch (lineCounter)
							{
								// 1st line for URL
								case 1:
									seedDetails.DefaultUrl = line;
									// find { and } - add content in between to seedDetails.UrlKeys --> repeat with updated index
									seedDetails.UriKeys = new List<string>();
									int startIndex = -1;
									int endIndex = 0;
									while (endIndex+1 < line.Length)
									{
										startIndex = line.IndexOf("{", startIndex+1, StringComparison.CurrentCultureIgnoreCase);
										endIndex = line.IndexOf("}", endIndex+1, StringComparison.CurrentCultureIgnoreCase);

										// Break out if { or } are not found (i.e.: only opening/closing curcly-bracket is used || OR || no uriParams are given
										if (startIndex == -1 || endIndex == -1)
										{
											Console.WriteLine("NOTE - no parameters found in the URL : {0}", line);
											break;
										}
										
										seedDetails.UriKeys.Add(line[startIndex..(endIndex+1)]);
									}
									break;
								// 2nd line for keys - add each to seedDetails.JsonKeys.Keys
								case 2:
									keys = line.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
									seedDetails.JsonKeys = new Dictionary<string, string>();
									break;
								// 3rd line for key types -- add each type to seedDetails.JsonKeys.Values
								case 3:
									keyTypes = line.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();

									// ERROR CHECK - if number of keys do not match the number of keyTypes
									if (keys.Count != keyTypes.Count)
									{
										Console.WriteLine("Number of keys and their types do not match!");
										var longerList = keys.Count > keyTypes.Count ? keys : keyTypes;

										for (var i = 0; i < longerList.Count; i++)
										{
											Console.WriteLine("{0, -20} : {1, 20}", keys[i], keyTypes[i]);
										}
										Environment.Exit(160);
									}

									for (var i = 0; i < keys.Count; i++)
									{
										try
										{
											seedDetails.JsonKeys.Add(keys[i], keyTypes[i]);
										}
										catch (ArgumentException)
										{
											Console.WriteLine("Duplicated keys provided! Key: {0, -5}", keys[i]);
											Environment.Exit(160);
										}

									}
									break;
								// 4th line for separator -- add to seedDetails.Separator
								case 4:
									seedDetails.Separator = line;
									break;
								// As of 5th line, by line:
								default:
									var seedItem = new SeedItem();
									var values = line.Split(seedDetails.Separator, StringSplitOptions.RemoveEmptyEntries);

									for (var i = 0; i < values.Length; i++)
									{
										// if i < UrlKeys.length replace key with values from the line --> save updated URL to seedItem.Uri
										if (i < seedDetails.UriKeys.Count)
										{
											var keyToReplaceInUri = seedDetails.UriKeys[i];
											seedItem.Uri = seedDetails.DefaultUrl.Replace(keyToReplaceInUri, values[i]);
										}
										// else , get keys[i-UriKeys.Count] - and add the current value to seedItem.JsonParameters as key-value pair
										else
										{
											var keyToJson = keys[i - seedDetails.UriKeys.Count];
											var keyType = keyTypes[i - seedDetails.UriKeys.Count];

											switch (keyType)
											{
												case "string":
													seedItem.JsonParameters.Add(keyToJson, values[i].ToString());
													break;
												case "int":
													seedItem.JsonParameters.Add(keyToJson, Convert.ToInt32(values[i]));
													break;
												case "long":
													seedItem.JsonParameters.Add(keyToJson, Convert.ToInt64(values[i]));
													break;
												case "bol":
													seedItem.JsonParameters.Add(keyToJson, Convert.ToBoolean(values[i]));
													break;
												default:
													Console.WriteLine("Invalid type ({0}) provided for key ({1})", keyType, keyToJson);
													Environment.Exit(160);
													break;
											}
										}
									}
									seedDetails.SeedItems.Add(seedItem);
									break;
							}
						}
					}
				}
				catch (OutOfMemoryException)
				{
					Console.WriteLine("There is not enough memory to process the file. Try to split it into smaller files!");
					Environment.Exit(8);
				}
			}
			else
			{
				// TODO: iterate through args - and seed data to SeedDetails
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("Something went wrong. Please create a new issue with error details, possible sample data at https://github.com/AndrasHorinka/DbSeeder Thanks!");
			Console.WriteLine("Error details:\n{0}", e.Message);
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
		Console.WriteLine("\t\t\tstring, int, long, bol");
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
