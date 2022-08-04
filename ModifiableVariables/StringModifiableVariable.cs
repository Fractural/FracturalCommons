using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoCustomResourceRegistry;

namespace Fractural
{
	[RegisteredType(nameof(StringModifiableVariable))]
	public class StringModifiableVariable : ModifiableVariable<string>
	{
		
	}
}
