using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal sealed class Lifetime
	{
		private readonly int m_addedVersion;

		private readonly int m_removedVersion;

		private static readonly Lifetime UnspecifiedInstance = new Lifetime(0, 0);

		public int AddedVersion
		{
			get
			{
				return this.m_addedVersion;
			}
		}

		public bool HasAddedVersion
		{
			get
			{
				return this.m_addedVersion != 0;
			}
		}

		public int RemovedVersion
		{
			get
			{
				return this.m_removedVersion;
			}
		}

		public bool HasRemovedVersion
		{
			get
			{
				return this.m_removedVersion != 0;
			}
		}

		public static Lifetime Unspecified
		{
			get
			{
				return Lifetime.UnspecifiedInstance;
			}
		}

		private Lifetime(int addedVersion, int removedVersion)
		{
			this.m_addedVersion = addedVersion;
			this.m_removedVersion = removedVersion;
		}

		public bool IncludesVersion(int compatVersion)
		{
			if (compatVersion == 0)
			{
				return true;
			}
			bool flag = this.m_addedVersion == 0 || this.m_addedVersion <= compatVersion;
			bool result = this.m_removedVersion == 0 || this.m_removedVersion > compatVersion;
			if (flag)
			{
				return result;
			}
			return false;
		}

		public static Lifetime AddedIn(int addedVersion)
		{
			Global.Tracer.Assert(addedVersion > 0, "Invalid addedVersion");
			return new Lifetime(addedVersion, 0);
		}

		public static Lifetime RemovedIn(int removedVersion)
		{
			Global.Tracer.Assert(removedVersion > 0, "Invalid addedVersion");
			return new Lifetime(0, removedVersion);
		}

		public static Lifetime Spanning(int addedVersion, int removedVersion)
		{
			Global.Tracer.Assert(addedVersion > 0, "Invalid addedVersion");
			Global.Tracer.Assert(removedVersion > 0, "Invalid removedVersion");
			Global.Tracer.Assert(removedVersion > addedVersion, "removedVersion must be later than addedVersion");
			return new Lifetime(addedVersion, removedVersion);
		}
	}
}
