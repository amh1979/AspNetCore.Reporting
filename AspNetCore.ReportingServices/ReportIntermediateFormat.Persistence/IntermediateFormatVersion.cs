using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	[Serializable]
	internal class IntermediateFormatVersion : IPersistable
	{
		private int m_major;

		private int m_minor;

		private int m_build;

		[NonSerialized]
		private static readonly Declaration m_Declaration;

		[NonSerialized]
		private static readonly IntermediateFormatVersion m_current;

		[NonSerialized]
		private static IntermediateFormatVersion m_rtm2008;

		[NonSerialized]
		private static IntermediateFormatVersion m_biRefresh;

		[NonSerialized]
		private static IntermediateFormatVersion m_sql16;

		internal int Major
		{
			get
			{
				return this.m_major;
			}
			set
			{
				this.m_major = value;
			}
		}

		internal int Minor
		{
			get
			{
				return this.m_minor;
			}
			set
			{
				this.m_minor = value;
			}
		}

		internal int Build
		{
			get
			{
				return this.m_build;
			}
			set
			{
				this.m_build = value;
			}
		}

		internal bool IsOldVersion
		{
			get
			{
				if (this.CompareTo(IntermediateFormatVersion.m_current.Major, IntermediateFormatVersion.m_current.Minor, IntermediateFormatVersion.m_current.Build) < 0)
				{
					return true;
				}
				return false;
			}
		}

		internal static IntermediateFormatVersion Current
		{
			get
			{
				return IntermediateFormatVersion.m_current;
			}
		}

		internal static IntermediateFormatVersion SQL16
		{
			get
			{
				if (IntermediateFormatVersion.m_sql16 == null)
				{
					IntermediateFormatVersion.m_sql16 = new IntermediateFormatVersion(12, 3, 0);
				}
				return IntermediateFormatVersion.m_sql16;
			}
		}

		internal static IntermediateFormatVersion RTM2008
		{
			get
			{
				if (IntermediateFormatVersion.m_rtm2008 == null)
				{
					IntermediateFormatVersion.m_rtm2008 = new IntermediateFormatVersion(11, 2, 0);
				}
				return IntermediateFormatVersion.m_rtm2008;
			}
		}

		internal static IntermediateFormatVersion BIRefresh
		{
			get
			{
				if (IntermediateFormatVersion.m_biRefresh == null)
				{
					IntermediateFormatVersion.m_biRefresh = new IntermediateFormatVersion(12, 1, 0);
				}
				return IntermediateFormatVersion.m_biRefresh;
			}
		}

		internal IntermediateFormatVersion()
		{
			this.SetCurrent();
		}

		internal IntermediateFormatVersion(int major, int minor, int build)
		{
			this.m_major = major;
			this.m_minor = minor;
			this.m_build = build;
		}

		static IntermediateFormatVersion()
		{
			IntermediateFormatVersion.m_Declaration = IntermediateFormatVersion.GetDeclaration();
			int majorVersion = PersistenceConstants.MajorVersion;
			int minorVersion = PersistenceConstants.MinorVersion;
			int current_build = 0;
            /*
            try
            {
                var vv = System.Diagnostics.FileVersionInfo.GetVersionInfo("G:\\MyProjects\\AspNetCore\\trunk\\Test\\LibraryTest\\bin\\Debug\\netcoreapp2.0\\ReflectorReport.dll");
                var v1 = IntermediateFormatVersion.EncodeFileVersion(vv);
            }
            catch (Exception ex)
            {

            }

            RevertImpersonationContext.Run(delegate
			{
				current_build = IntermediateFormatVersion.EncodeFileVersion(FileVersionInfo.GetVersionInfo(typeof(IntermediateFormatVersion).Assembly.Location));
			});
            */
			IntermediateFormatVersion.m_current = new IntermediateFormatVersion(majorVersion, minorVersion, current_build);
		}

		private static int EncodeFileVersion(FileVersionInfo fileVersion)
		{
			int num = fileVersion.FileMajorPart % 20;
			num *= 10;
			num += fileVersion.FileMinorPart % 10;
			num *= 100000;
			num += fileVersion.FileBuildPart % 100000;
			num *= 100;
			return num + fileVersion.FilePrivatePart % 100;
		}

		internal static void DecodeFileVersion(int version, out int major, out int minor, out int build, out int buildminor)
		{
			major = 0;
			minor = 0;
			build = 0;
			buildminor = 0;
			if (version > 0)
			{
				buildminor = version % 100;
				version -= buildminor;
				version /= 100;
				build = version % 100000;
				version -= build;
				version /= 100000;
				minor = version % 10;
				version -= minor;
				version /= 10;
				major = version % 20;
			}
		}

		internal void SetCurrent()
		{
			this.m_major = IntermediateFormatVersion.m_current.Major;
			this.m_minor = IntermediateFormatVersion.m_current.Minor;
			this.m_build = IntermediateFormatVersion.m_current.Build;
		}

		internal int CompareTo(int major, int minor, int build)
		{
			int num = this.Compare(this.m_major, major);
			if (num == 0)
			{
				num = this.Compare(this.m_minor, minor);
				if (num == 0 && this.m_major < 10)
				{
					num = this.Compare(this.m_build, build);
				}
			}
			return num;
		}

		internal int CompareTo(IntermediateFormatVersion version)
		{
			int num = this.Compare(this.m_major, version.Major);
			if (num == 0)
			{
				num = this.Compare(this.m_minor, version.Minor);
				if (num == 0 && this.m_major < 10)
				{
					num = this.Compare(this.m_build, version.Build);
				}
			}
			return num;
		}

		private int Compare(int x, int y)
		{
			if (x < y)
			{
				return -1;
			}
			if (x > y)
			{
				return 1;
			}
			return 0;
		}

		public override string ToString()
		{
			if (this.m_major < 10)
			{
				return this.m_major.ToString(CultureInfo.InvariantCulture) + "." + this.m_minor.ToString(CultureInfo.InvariantCulture) + "." + this.m_build.ToString(CultureInfo.InvariantCulture);
			}
			int num = default(int);
			int num2 = default(int);
			int num3 = default(int);
			int num4 = default(int);
			IntermediateFormatVersion.DecodeFileVersion(this.m_build, out num, out num2, out num3, out num4);
			return this.m_major.ToString(CultureInfo.InvariantCulture) + "." + this.m_minor.ToString(CultureInfo.InvariantCulture) + "." + num3.ToString(CultureInfo.InvariantCulture) + "." + num4.ToString(CultureInfo.InvariantCulture);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.IntermediateFormatVersionMajor, Token.Int32));
			list.Add(new MemberInfo(MemberName.IntermediateFormatVersionMinor, Token.Int32));
			list.Add(new MemberInfo(MemberName.IntermediateFormatVersionBuild, Token.Int32));
			return new Declaration(ObjectType.IntermediateFormatVersion, ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(IntermediateFormatVersion.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IntermediateFormatVersionMajor:
					writer.Write(this.m_major);
					break;
				case MemberName.IntermediateFormatVersionMinor:
					writer.Write(this.m_minor);
					break;
				case MemberName.IntermediateFormatVersionBuild:
					writer.Write(this.m_build);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(IntermediateFormatVersion.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IntermediateFormatVersionMajor:
					this.m_major = reader.ReadInt32();
					break;
				case MemberName.IntermediateFormatVersionMinor:
					this.m_minor = reader.ReadInt32();
					break;
				case MemberName.IntermediateFormatVersionBuild:
					this.m_build = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.IntermediateFormatVersion;
		}
	}
}
