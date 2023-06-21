using System.Diagnostics;
using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;
using Sims_4_Texture_Helping_Tools.Converters;
using Sims_4_Texture_Helping_Tools.Data.DBPF;

namespace Sims_4_Texture_Helping_Tools;

internal class Program
{
	static void Main()
	{
		Console.WriteLine("Enter a filepath:");
		string filepath = Console.ReadLine()!;
		DBPFPackage package = new(filepath);
		Console.WriteLine("Enter a filepath:");
		filepath = Console.ReadLine()!;

		Stopwatch stopwatch = new();
		stopwatch.Start();

		package.Decompress(filepath);

		stopwatch.Stop();
		// Print a message when all done
		Console.WriteLine($"All files extracted and converted in {stopwatch.Elapsed}.");
	}

	//public static void ConvertImages()
	//{

	//    // Ask for a filepath
	//    Console.WriteLine("Enter a filepath:");
	//    string filepath = Console.ReadLine()!;

	//    // Check if the filepath is valid
	//    if (!Directory.Exists(filepath))
	//    {
	//        Console.WriteLine("Invalid filepath.");
	//        return;
	//    }

	//    // Ask if the files need to be converted to 1: png or 2: dds
	//    Console.WriteLine("Enter 1 for png or 2 for dds:");
	//    string choice = Console.ReadLine()!;

	//    // Check if the choice is valid
	//    if (choice is not "1" and not "2")
	//    {
	//        Console.WriteLine("Invalid choice.");
	//        return;
	//    }

	//    // Get the output format and folder name
	//    string outputFormat = choice == "1" ? ".png" : ".dds";
	//    string outputFolder = choice == "1" ? "png" : "dds";

	//    // Create the output folder if it doesn't exist
	//    string outputDir = Path.Combine(filepath, outputFolder);
	//    Directory.CreateDirectory(outputDir);

	//    // Get all the image files in the filepath
	//    string[] imageFiles = Directory.GetFiles(filepath, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.Split('!')[1] != "00064DC9").AsParallel().ToArray();

	//    Stopwatch stopwatch = new();
	//    stopwatch.Start();

	//    // Convert the images in parallel using tasks
	//    Task[] tasks = new Task[imageFiles.Length];
	//    for (int i = 0; i < imageFiles.Length; i++)
	//    {
	//        // Capture the current file name in a local variable
	//        string file = imageFiles[i];

	//        // Create a task to convert the file
	//        tasks[i] = Task.Run(() =>
	//        {
	//            try
	//            {
	//                // Get the file extension
	//                string extension = Path.GetExtension(file);

	//                // Check if the file is already in the output format
	//                if (extension == outputFormat)
	//                {
	//                    Console.WriteLine($"Skipping {file} as it is already in {outputFormat} format.");
	//                    return;
	//                }

	//                // Get the file name without extension
	//                string fileName = Path.GetFileNameWithoutExtension(file);

	//                string compressionType = Path.GetExtension(fileName).Replace(".", "");

	//                CompressionFormat format = CompressionFormat.Bgra;
	//                Enum.TryParse(compressionType, true, out format);

	//                fileName = Path.GetFileNameWithoutExtension(fileName);

	//                if (fileName.Split('!')[1] == "00064DCA")
	//                    fileName = fileName.Replace("00064DCA", "<YCoCg>");

	//                // Get the output file path
	//                string outputFile = Path.Combine(outputDir, fileName + outputFormat);

	//                // Convert the file depending on the output format
	//                if (outputFormat == ".png")
	//                {
	//                    ConvertDdsToPng(file, outputFile);
	//                }
	//                else
	//                {
	//                    ConvertPngToDds(file, outputFile, format);
	//                }
	//            }
	//            catch (Exception e)
	//            {
	//                Console.WriteLine($"Error on {file}:\n{e}");
	//            }
	//        });
	//    }

	//    // Wait for all tasks to finish
	//    Task.WaitAll(tasks);

	//    stopwatch.Stop();

	//    // Print a message when all done
	//    Console.WriteLine($"All files converted in {stopwatch.Elapsed}.");
	//}

	// A function that takes in a filepath pointing to a png and converts this to a dds file with a given output filepath
	public static void ConvertPngToDds(string pngFilepath, string ddsFilepath, CompressionFormat format)
	{
		BcEncoder encoder = new();
		encoder.OutputOptions.GenerateMipMaps = false;
		encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
		encoder.OutputOptions.Format = format;
		encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;

		if (pngFilepath.Contains("!combined!"))
		{
			using Image<Rgba32> image = Image.Load<Rgba32>(pngFilepath);

			(Image<Rgba32> image1, Image<Rgba32> image2) = ColorConverters.ConvertRGBAToYCoCgA(image);

			image2.Mutate(x => x.Resize(image2.Width / 2, image2.Height / 2));

			using FileStream fs1 = File.OpenWrite(ddsFilepath.Replace("!combined!", "!00064DCA!"));
			using FileStream fs2 = File.OpenWrite(ddsFilepath.Replace("!combined!", "!00064DC9!"));

			encoder.EncodeToStream(image1, fs1);
			encoder.EncodeToStream(image2, fs2);
		}
		else
		{
			using Image<Rgba32> image = Image.Load<Rgba32>(pngFilepath);
			using FileStream fs = File.OpenWrite(ddsFilepath);
			encoder.EncodeToStream(image, fs);
		}
	}
}
