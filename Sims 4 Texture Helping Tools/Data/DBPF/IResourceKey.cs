using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sims_4_Texture_Helping_Tools.Data.DBPF;
public interface IResourceKey
{
    public uint Type { get; set; }
    public uint Group { get; set; }
    public ulong InstanceID { get; set; }
}
