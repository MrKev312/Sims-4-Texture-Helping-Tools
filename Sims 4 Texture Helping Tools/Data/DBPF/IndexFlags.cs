using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sims_4_Texture_Helping_Tools.Data.DBPF;

[Flags]
public enum IndexFlags : uint
{
	None = 0u,
	ConstantType = 1u,
	ConstantGroup = 2u,
	ConstantInstanceEx = 4u
}