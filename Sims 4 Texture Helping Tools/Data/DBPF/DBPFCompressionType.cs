using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sims_4_Texture_Helping_Tools.Data.DBPF;

public enum DBPFCompressionType : ushort
{
    Uncompressed = 0,
    Zlib = 23106,
    InternalCompression = ushort.MaxValue,
    StreamableCompression = 65534,
    DeletedRecord = 65504
}