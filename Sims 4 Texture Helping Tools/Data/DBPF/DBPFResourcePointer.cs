using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sims_4_Texture_Helping_Tools.Data.DBPF;
public class DBPFResourcePointer
{
	public ResourceKey? ResourceKey {get; set;}

	public DBPFCompressionType CompressionType { get; set; }

	public uint Offset { get; set; }
	public uint Size { get; set; }
	public uint SizeDecompressed { get; set; }
	public bool Compressed { get; }
	public ushort Committed { get; set; }
}
