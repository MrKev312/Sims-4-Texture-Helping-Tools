using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCnEncoder.Decoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared.ImageFiles;
using Sims_4_Texture_Helping_Tools.Converters;

namespace Sims_4_Texture_Helping_Tools.Data.DBPF;
public class DBPFPackage
{
	public DBPFHeader Header { get; }
	public IndexFlags IndexFlags { get; }
	public uint Type { get; }
	public uint Group { get; }
	public ulong Instance { get; }
	public string FilePath { get; }

	public Dictionary<ResourceKey, DBPFResourcePointer> Resources { get; }

	public DBPFPackage(string filePath)
	{
		FilePath = filePath;
		Resources = new();

		using Stream stream = File.OpenRead(filePath);
		Header = new DBPFHeader(stream);

		stream.Seek(Header.IndexOffset, SeekOrigin.Begin);
		using BinaryReader binaryReader = new(stream);

		// Go to Index start and read IndexFlags
		IndexFlags = (IndexFlags)binaryReader.ReadUInt32();
		if (IndexFlags.HasFlag(IndexFlags.ConstantType))
			Type = binaryReader.ReadUInt32();

		if (IndexFlags.HasFlag(IndexFlags.ConstantGroup))
			Group = binaryReader.ReadUInt32();

		if (IndexFlags.HasFlag(IndexFlags.ConstantInstanceEx))
			Instance |= (ulong)binaryReader.ReadUInt32() << 32;

		for (int i = 0; i < Header.IndexEntryCount; i++)
		{
			DBPFResourcePointer resource = new()
			{
				ResourceKey = new() {
					Type = IndexFlags.HasFlag(IndexFlags.ConstantType) ? Type : binaryReader.ReadUInt32(),
					Group = IndexFlags.HasFlag(IndexFlags.ConstantGroup) ? Group : binaryReader.ReadUInt32()
				}
			};

			ulong resourceInstance = IndexFlags.HasFlag(IndexFlags.ConstantInstanceEx) ? (Instance >> 32) : binaryReader.ReadUInt32();
			uint fileLocation = binaryReader.ReadUInt32();
			resource.ResourceKey.InstanceID = (resourceInstance << 32) + fileLocation;
			resource.Offset = binaryReader.ReadUInt32();
			uint fileSize = binaryReader.ReadUInt32();
			resource.Size = fileSize & 0x7FFFFFFFu;
			resource.SizeDecompressed = binaryReader.ReadUInt32();

			if ((fileSize & 0x80000000u) != 0)
			{
				resource.CompressionType = (DBPFCompressionType)binaryReader.ReadUInt16();
				resource.Committed = binaryReader.ReadUInt16();
			}

			Resources.Add(resource.ResourceKey, resource);
		}
	}

	public void Decompress(string outputPath)
	{
        Parallel.ForEach(Resources, keyValue =>
        {
			DBPFResourcePointer resource = keyValue.Value;
			if (keyValue.Key.Group == 0x00064DC9u)
				return;

			string savePath = Path.Combine(outputPath, $"{keyValue.Key.Type:X8}!{keyValue.Key.Group:X8}!{keyValue.Key.InstanceID:X16}.bin");

			try
			{
				using FileStream input = File.OpenRead(FilePath);

				byte[] buffer = new byte[resource.Size];
				input.Seek(resource.Offset, SeekOrigin.Begin);
				input.Read(buffer);

				if (keyValue.Key.Type is 3066607264u or 11720834u or 877907861u or 3129306232u)
				{
					buffer = DBPFCompressor.Decompress(buffer, resource.CompressionType);
					using MemoryStream stream = new(buffer);
					DdsFile dds = DdsFile.Load(stream);

					Image png;
					if (keyValue.Key.Group == 0x00064DCAu)
					{
						ResourceKey secondKey = new()
						{
							Type = keyValue.Key.Type,
							Group = 0x00064DC9u,
							InstanceID = keyValue.Key.InstanceID
						};

						DBPFResourcePointer secondResource = Resources[secondKey];
						buffer = new byte[secondResource.Size];
						input.Seek(secondResource.Offset, SeekOrigin.Begin);
						input.Read(buffer);

						buffer = DBPFCompressor.Decompress(buffer, secondResource.CompressionType);
						using MemoryStream stream2 = new(buffer);
						DdsFile dds2 = DdsFile.Load(stream2);

						png = ImageConverters.ConvertDDSToPNG(dds, dds2);

						savePath = savePath.Replace("!00064DCA!", "!combined!", StringComparison.Ordinal);
					}
					else
					{
						png = ImageConverters.ConvertDDSToPNG(dds);
					}

					png.Save(Path.ChangeExtension(savePath, CompressionFormatHelper.GetCompressionFormat(dds) + ".png"));
                    png.Dispose();
				}
				//else
				//{
				//    using FileStream output = File.Create(savePath);
				//    output.Write(DBPFCompressor.Decompress(buffer, resource.CompressionType));

				//}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in {savePath} with {resource.CompressionType}:\n{ex}");
			}
		});
	}
}
