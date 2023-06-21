using System.IO.Compression;
using Gibbed.RefPack;

namespace Sims_4_Texture_Helping_Tools.Data.DBPF;

public static class DBPFCompressor
{
	public static byte[] Decompress(byte[] data, DBPFCompressionType type)
	{
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        return type switch
		{
			DBPFCompressionType.Uncompressed => data,
			DBPFCompressionType.Zlib => DecompressZlib(data),
			DBPFCompressionType.InternalCompression => Decompression.Decompress(data),
			DBPFCompressionType.DeletedRecord => CheckDeletedRecord(data),
			_ => throw new ArgumentException($"Unrecognized compression type {type}")
		};
	}

	private static byte[] DecompressZlib(byte[] data)
	{
		using MemoryStream input = new(data);
		using MemoryStream output = new();
		using ZLibStream zLibStream = new(input, CompressionMode.Decompress);
		zLibStream.CopyTo(output);
		return output.ToArray();
	}

	private static byte[] CheckDeletedRecord(byte[] data)
	{
		return data.Length == 0 
			? data 
			: throw new InvalidDataException("Unexpected data length on empty resource.");
	}
}

