using DbSeeder.Model.Models;
using DbSeeder.Services.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// This is the entry class to check if args is given - to process the rules
/// </summary>
public class Program
{
	public static async Task Main(string[] args)
	{
		// Exit if arguments are not provided
		if (args.Length == 0)
		{
			PrintHelp();
			Environment.Exit(160);
		}
		try
		{
			SeedDetails seedDetails = new SeedDetails()
			{
				Method = HttpMethod.Post,
				SeedItems = new List<SeedItem>(),
				JsonKeys = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
		};

			// Check if -c flag is used with a file with all details
			var cFlagIndex = Array.IndexOf(args, "-c");
			if (cFlagIndex != -1)
			{
				// Retrieve the name of Complex file and create a FileInfo
				var complexFileName = args[cFlagIndex + 1];
				var complexFile = ValidateFileInput(complexFileName);

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
								ExtractParamsFromDefaultUrl(ref seedDetails);
								break;
							// 2nd line for keys - add each to seedDetails.JsonKeys.Keys
							case 2:
								keys = line.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
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
								seedDetails.Separator = line.Trim() switch
								{
									"!n!" => Environment.NewLine,
									"!t!" => "\t",
									"!s!" => " ",
									_ => line,
								};
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
										seedItem.Url = seedDetails.DefaultUrl.Replace(keyToReplaceInUri, values[i]);
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
			// If -c flag is not provided
			else
			{
				// Store all core info or references
				FileInfo fileWithSamples = null;
				//IList<string> keys = new List<string>();
				for (var i = 0; i < args.Length-1; i += 2)
				{
					var valueForParam = args[i + 1];
					switch (args[i])
					{
						// Example data - only to store file name reference to be processed once all other flags are processed
						case "-e":
							fileWithSamples = ValidateFileInput(valueForParam);
							break;
						// URL
						case "-u":
							seedDetails.DefaultUrl = valueForParam;
							ExtractParamsFromDefaultUrl(ref seedDetails);
							break;
						// Keys
						case "-k":
							// first - keyName
							// 2nd - can be unique if so --> 
								// 3rd - type - can be regex if so -->
									// 4th onwards must be concatenated
							// if not unique, 2nd is type --> can be regex if so -->
								// 3rd onwards must be concatenated
							break;
						// Separator
						case "-s":
							seedDetails.Separator = valueForParam switch
							{
								"!n!" => Environment.NewLine,
								"!t!" => "\t",
								"!s!" => " ",
								_ => valueForParam
							};
							break;
						// Method
						case "-m":
							seedDetails.Method = valueForParam switch
							{
								"POST" => HttpMethod.Post,
								"PATCH" => HttpMethod.Patch,
								"PUT" => HttpMethod.Put,
								"DELETE" => HttpMethod.Delete,
								_ => null
							};
							// ERROR CHECK - if invalid method is provided
							if (seedDetails.Method is null)
							{
								Console.WriteLine("Incorrect method provided: {0}", valueForParam);
								Environment.Exit(160);
							}
							break;
						// If invalid flag provided
						default:
							Console.WriteLine("Invalid flag provided: {0}", args[i]);
							Console.WriteLine("Run the file without parameters to receive");
							Environment.Exit(160);
							break;
					}
				}

				// TODO: process file from -e flag
				// First line keys
				// as of second, values
				using (StreamReader reader = new StreamReader(fileWithSamples.FullName))
				{
					string line;
					int lineCounter = 0;

					IList<string> keysForJson = new List<string>();
					// Read file line by line and feed seedDetails
					while ((line = reader.ReadLine()) != null)
					{
						lineCounter++;
						if (lineCounter == 1)
						{
							keysForJson = line.Split(seedDetails.Separator, StringSplitOptions.RemoveEmptyEntries).ToList();
							// ERROR CHECK - if keys in sample file have the same amount of keys as keys in key file
							if (keysForJson.Count != seedDetails.JsonKeys.Count)
							{
								Console.WriteLine("ERROR - Number of keys in key file does not match number of keys provided in sample file!");
								Environment.Exit(160);
							}
							// ERROR CHECK - if keys in sample file match keys in key file
							foreach (var key in keysForJson)
							{
								if (seedDetails.JsonKeys.Keys.Contains(key)) continue;

								Console.WriteLine("ERROR - Mismatch between keys in in key file and sample file \n{0, 10} is not in key file!", key);
							}

							// check if key is part of 
						}
						switch (lineCounter)
						{
							// 1st line for keys
							case 1:
								
								foreach (var key in keys)
								{
									keysForJson.Add(key);
								}
								break;
							default:
								var seedItem = new SeedItem
								{
									UriParameters = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase),
									JsonParameters = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase)
								};
								break;
						}
					}
				}
			}

			// Seed Content
			await SeedContent(seedDetails);
		}
		catch (OutOfMemoryException)
		{
			Console.WriteLine("There is not enough memory to process the file. Try to split it into smaller files!");
			Environment.Exit(8);
		}
		catch (Exception e)
		{
			Console.WriteLine("Something went wrong. Please create a new issue with error details, possible sample data at https://github.com/AndrasHorinka/DbSeeder Thanks!");
			Console.WriteLine("Error details:\n{0}", e.Message);
		}
	}

	private static async Task SeedContent(SeedDetails seedDetails)
	{
		var seederService = new SeederService(seedDetails);
		seedDetails = await seederService.Seed();
		int success = 0;

		// Create a log file for the results
		string initFileName = $"DbSeeder-ErrorLog-{DateTime.Now:yy / MM / dd - H:mm:ss}";
		int version = 1;
		FileInfo file;

		// Create a File with a name not existing yet
		do
		{
			StringBuilder builder = new StringBuilder(initFileName);
			builder.Append(version.ToString());
			file = new FileInfo(builder.ToString());

			// if FileName exists - create a new version
			if (file.Exists) version++;

		} while (!file.Exists);

		// Iterate through items and print out Summary + write log file
		using (StreamWriter writer = new StreamWriter(file.Name))
		{
			for (var i = 0; i < seedDetails.SeedItems.Count; i++)
			{
				var seedItem = seedDetails.SeedItems[i];
				// Request was successful continue to next
				if (seedItem.ResponseMessage.IsSuccessStatusCode)
				{
					success++;
					continue;
				}

				await writer.WriteLineAsync($"\n---- {i + 1}. element failed ----\nReason: {seedItem.ResponseMessage.StatusCode} - {seedItem.ResponseMessage.ReasonPhrase}\n{seedItem.ResponseMessage}\nURL: {seedItem.Url}");
				foreach (var item in seedItem.JsonParameters)
				{
					await writer.WriteLineAsync($"{item.Key, -20} : {item.Value, 20}");
				}
			}
		}
		Console.WriteLine("\nSeeding completed. Out of {0:n} items {1:n} were succesful.", seedDetails.SeedItems.Count, success);
		Console.WriteLine("Errors -with error messages- are loged into: {0}", file.FullName);
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
		Console.WriteLine("\tNote - Only JsonKeys must be included - not uriKeys.");
		Console.WriteLine("\tNote - Default separator is comma");
		Console.WriteLine("\tNote - Key names must be unique!");
		Console.WriteLine("\t\tPossible key types are:" );
		Console.WriteLine("\t\t\tstring, int, long, bol, regex - Except bol all types can be unique by preceeding it with unique keyword!");
		Console.WriteLine("\t\t\t\tIf type is regex - a third value must be given with regex expresssion");
		Console.WriteLine("\t\tExample: \n\t\t\tJsonParam1, int\n\t\t\tJsonParam2, long\n\t\t\tJsonParam3, unique, int\n\t\t\tJsonParam4, regex, [0-9a-z]+[0-9a-z]?");
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
	
	private static void ExtractParamsFromDefaultUrl(ref SeedDetails seedDetails)
	{
		seedDetails.UriKeys = new List<string>();
		int startIndex = -1;
		int endIndex = 0;
		while (endIndex + 1 < seedDetails.DefaultUrl.Length)
		{
			startIndex = seedDetails.DefaultUrl.IndexOf("{", startIndex + 1, StringComparison.CurrentCultureIgnoreCase);
			endIndex = seedDetails.DefaultUrl.IndexOf("}", endIndex + 1, StringComparison.CurrentCultureIgnoreCase);

			// Break out if { or } are not found (i.e.: only opening/closing curcly-bracket is used || OR || no uriParams are given
			if (startIndex == -1 || endIndex == -1)
			{
				Console.WriteLine("NOTE - no parameters found in the URL : {0}", seedDetails.DefaultUrl);
				break;
			}

			seedDetails.UriKeys.Add(seedDetails.DefaultUrl[startIndex..(endIndex + 1)]);
		}
	}

	private static FileInfo ValidateFileInput(string fileName)
	{
		var file = new FileInfo(fileName);

		// If FileInfo does not exists - try to create it with full path
		if (!file.Exists)
		{
			var fullPath = Path.Combine(Environment.CurrentDirectory, fileName);
			file = new FileInfo(fullPath);
		}
		// If FileInfo cannot be retrieved, exit with invalid argument error code.
		if (!file.Exists)
		{
			Console.WriteLine("Complex file not found! {0,20}", fileName);
			Environment.Exit(160);
		}

		return file;
	}
}
