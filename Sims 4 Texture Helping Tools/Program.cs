using System.Diagnostics;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;
using Sims_4_Texture_Helping_Tools.Converters;
using Sims_4_Texture_Helping_Tools.Data.DBPF;

namespace Sims_4_Texture_Helping_Tools;

internal static class Program
{
	static void Main()
	{
		MainMenu();
	}

	static void MainMenu()
	{
		while (true)
		{
			Console.WriteLine("Welcome to the Sims 4 Texture Helping Tools. Please choose one of the following tasks:");
			Console.WriteLine("1. Extract a package to a destination folder.");
			Console.WriteLine("2. Package a source folder into a package.");
			Console.WriteLine("3. Convert files from one format to another.");
			Console.WriteLine("4. Exit program");

			if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > 4)
			{
				Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
				continue;
			}

			Console.WriteLine();

			switch (choice)
			{
				case 1:
					ExtractFiles();
					break;
				case 2:
					//PackageFiles();
					Console.WriteLine("Currently not implemented");
					break;
				case 3:
					ConvertFiles();
					break;
				case 4:
					return;
			}

			Console.WriteLine();
		}
	}

	static void ExtractFiles()
	{
		// TODO menu that allows only extracting one filetype
		Console.WriteLine();
		Console.WriteLine("Please enter the source file path");
		string filepath = Console.ReadLine()!;

		if (File.Exists(filepath))
		{
			if (Path.GetExtension(filepath).ToUpperInvariant() != ".PACKAGE")
			{
				Console.WriteLine("File does not end in .package, are you sure this is a package file? (y/n): ");

				if (char.ToUpperInvariant(Console.ReadLine()[0]) != 'Y')
					return;
			}
		}
		else
		{
			Console.WriteLine("The source folder does not exist.");
			return;
		}

		DBPFPackage package = new(filepath);

		Console.WriteLine("Please enter the destination folder path:");
		filepath = Console.ReadLine()!;

		Console.WriteLine("Should extracted files be converted (y/n): ");
		bool shouldConvert = char.ToUpperInvariant(Console.ReadLine()[0]) == 'Y';

		Stopwatch stopwatch = new();
		stopwatch.Start();

		package.Decompress(filepath, shouldConvert);

		stopwatch.Stop();
		// Print a message when all done
		Console.WriteLine($"All files extracted and converted in {stopwatch.Elapsed}.");
	}

	static void ConvertFiles()
	{
		while (true)
		{
			Console.WriteLine("Please select a format from the list of supported formats:");
			Console.WriteLine("1. PNG (file.{compression level}.png for setting compression)");
			Console.WriteLine("2. DDS (file.dds)");
			Console.WriteLine("3. Exit");

			if (!int.TryParse(Console.ReadLine(), out int format) || format < 0 || format > 3)
			{
				Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
				continue;
			}

			Console.WriteLine();

			Console.WriteLine("Please enter the source folder/file path:");
			string sourcePath = Console.ReadLine();

			string[] files;

			if (File.Exists(sourcePath))
			{
				files = new string[]{ sourcePath };
			}
			else if (Directory.Exists(sourcePath))
			{
				files = Directory.GetFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly);
			}
			else
			{
				Console.WriteLine("The source folder does not exist.");
				continue;
			}

			Console.WriteLine("Please enter the destination folder path:");
			string destPath = Console.ReadLine();

			if (!Directory.Exists(destPath))
			{
				Console.WriteLine("The destination folder does not exist.");
				continue;
			}

			Stopwatch sw = Stopwatch.StartNew();

			switch (format)
			{
				case 1:
					ConvertPNGFile(files, destPath);
					break;
				case 2:
					ConvertDDSFile(files, destPath);
					break;
				case 3:
					return;
			}

			sw.Stop();

			Console.WriteLine();
			Console.WriteLine($"Finished converting in {sw.Elapsed}.");
			Console.WriteLine();
		}
	}

	public static void ConvertPNGFile(string[] files, string destPath)
	{
		Parallel.ForEach(files, file =>
		{
			try
			{
				// Get the file extension
				string extension = Path.GetExtension(file).ToUpperInvariant();

				// Check if the file is already in the output format
				if (extension != ".PNG")
				{
					Console.WriteLine($"Skipping {file} as it is not a png.");
					return;
				}

				// Get the file name without extension
				string fileName = Path.GetFileNameWithoutExtension(file);

				CompressionFormat format = CompressionFormat.Bgra;

				if (!string.IsNullOrEmpty(Path.GetExtension(fileName)))
				{
					string compressionType = Path.GetExtension(fileName)[1..];

					Enum.TryParse(compressionType, true, out format);
				}

				fileName = Path.GetFileNameWithoutExtension(fileName);

				// Get the output file path
				string outputFile = Path.Combine(destPath, Path.ChangeExtension(fileName, "dds"));

				Image tmp = Image.Load(file);
				using Image<Rgba32> image = tmp.CloneAs<Rgba32>();
				tmp.Dispose();

				using FileStream fs = File.OpenWrite(outputFile);
				DdsFile dds = ImageConverters.ConvertPNGToDDS(image, format);
				dds.Write(fs);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error on {file}:\n{e}");
			}
		});
	}

	public static void ConvertDDSFile(string[] files, string destPath)
	{
		Parallel.ForEach(files, file =>
		{
			try
			{
				// Get the file extension
				string extension = Path.GetExtension(file)[1..].ToUpperInvariant();

				// Check if the file is already in the output format
				if (extension != "DDS")
				{
					Console.WriteLine($"Skipping {file} as it is not a dds.");
					return;
				}

				// Get the output file path
				string outputFile = Path.Combine(destPath, Path.ChangeExtension(file, "png"));

				using FileStream fs = File.OpenRead(file);
				DdsFile ddsFile = DdsFile.Load(fs);

				Image image = ImageConverters.ConvertDDSToPNG(ddsFile);
				image.Save(outputFile);
				image.Dispose();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error on {file}:\n{e}");
			}
		});
	}
}