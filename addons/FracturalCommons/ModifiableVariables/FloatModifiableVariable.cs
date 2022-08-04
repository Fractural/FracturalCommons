using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoCustomResourceRegistry;

namespace Fractural
{
	[RegisteredType(nameof(FloatModifiableVariable))]
	public class FloatModifiableVariable : ModifiableVariable<float>
	{
		
	}
}
