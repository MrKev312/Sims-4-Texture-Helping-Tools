using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sims_4_Texture_Helping_Tools.Data.IO.Serialization;

public interface ISerializable
{
    void Read(Stream stream);

    void Write(Stream stream);
}
