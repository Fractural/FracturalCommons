namespace Fractural.Utils
{
	/// <summary>
	/// Utility class for bit manipulation.
	/// </summary>
	public static class BitUtils
	{
		public static int ReverseBits(this int target, int numberOfBits = sizeof(int) * 8)
		{
			int reversed = 0;
			for (int i = 0; i < numberOfBits; i++)
			{
				// If the ith bit of x is toggled, toggle the ith bit from the right of reversed
				reversed |= (target & (1 << i)) != 0 ? 1 << (numberOfBits - 1 - i) : 0;
			}
			return reversed;
		}

		public static long ReverseBits(this long target, int numberOfBits = sizeof(long) * 8)
		{
			long reversed = 0;
			for (int i = 0; i < numberOfBits; i++)
			{
				// If the ith bit of x is toggled, toggle the ith bit from the right of reversed
				reversed |= (target & (1L << i)) != 0 ? 1L << (numberOfBits - 1 - i) : 0;
			}
			return reversed;
		}

		public static uint ReverseBits(this uint target, int numberOfBits = sizeof(uint) * 8)
		{
			uint reversed = 0;
			for (int i = 0; i < numberOfBits; i++)
			{
				// If the ith bit of x is toggled, toggle the ith bit from the right of reversed
				reversed |= (target & (1U << i)) != 0 ? 1U << (numberOfBits - 1 - i) : 0;
			}
			return reversed;
		}

		public static ulong ReverseBits(this ulong target, int numberOfBits = sizeof(ulong) * 8)
		{
			ulong reversed = 0;
			for (int i = 0; i < numberOfBits; i++)
			{
				// If the ith bit of x is toggled, toggle the ith bit from the right of reversed
				reversed |= (target & (1UL << i)) != 0 ? 1UL << (numberOfBits - 1 - i) : 0;
			}
			return reversed;
		}

	}
}