using System;
using System.Linq;
using Fractural.Utils;

namespace Fractural.Information
{
	public struct VersionInfo
	{
		public VersionInfo(string versionString) : this(
			versionString.Split('.').Select(x => int.Parse(x)).ToArray()) { }

		public VersionInfo(int[] info)
		{
			Major = info.Length >= 1 ? info[0] : 0;
			Minor = info.Length >= 2 ? info[1] : 0;
			Patch = info.Length >= 3 ? info[2] : 0;		
		}

		public VersionInfo(int major, int minor, int patch)
		{
			Major = major;
			Minor = minor;
			Patch = patch;
		}

		public int Major { set; get; }
		public int Minor { set; get; }
		public int Patch { set; get; }

		public override string ToString()
		{
			return $"{Major}.{Minor}.{Patch}";
		}

		public override bool Equals(object obj)
		{			
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			
			return (VersionInfo) obj == this;
		}
		
		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 31 + Major.GetHashCode();
				hash = hash * 31 + Minor.GetHashCode();
				hash = hash * 31 + Patch.GetHashCode();
				return hash;
			}
		}

		public static bool operator >(VersionInfo a, VersionInfo b)
		{
			if (a.Major > b.Major)
				return true;
			else if (a.Major == b.Major)
			{
				if (a.Minor > b.Minor)
					return true;
				else if (a.Minor == b.Minor)
				{
					if (a.Patch > b.Patch)
						return true;
					return false;
				}
			}
			return false;
		}

		public static bool operator <(VersionInfo a, VersionInfo b)
		{
			return b > a;
		}

		public static bool operator ==(VersionInfo a, VersionInfo b)
		{
			return a.Major == b.Major
				&& a.Minor == b.Minor
				&& a.Patch == b.Patch;
		}

		public static bool operator !=(VersionInfo a, VersionInfo b)
		{
			return !(a == b);
		}

		public static bool operator >=(VersionInfo a, VersionInfo b)
		{
			if (a == b)
				return true;
			return a > b;
		}

		public static bool operator <=(VersionInfo a, VersionInfo b)
		{
			return b >= a;
		}

		public static implicit operator VersionInfo(string str) => new VersionInfo(str);
	}
}