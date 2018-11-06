using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Diagnostics;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class IntermediateFormatVersion
	{
		private int m_major;

		private int m_minor;

		private int m_build;

		private static readonly int m_current_major;

		private static readonly int m_current_minor;

		private static readonly int m_current_build;

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
				if (this.CompareTo(IntermediateFormatVersion.m_current_major, IntermediateFormatVersion.m_current_minor, IntermediateFormatVersion.m_current_build) < 0)
				{
					return true;
				}
				return false;
			}
		}

		internal bool IsRS2000_Beta2_orOlder
		{
			get
			{
				return this.CompareTo(8, 0, 673) <= 0;
			}
		}

		internal bool IsRS2000_WithSpecialRecursiveAggregates
		{
			get
			{
				return this.CompareTo(8, 0, 700) >= 0;
			}
		}

		internal bool IsRS2000_WithNewChartYAxis
		{
			get
			{
				return this.CompareTo(8, 0, 713) >= 0;
			}
		}

		internal bool IsRS2000_WithOtherPageChunkSplit
		{
			get
			{
				return this.CompareTo(8, 0, 716) >= 0;
			}
		}

		internal bool IsRS2000_RTM_orOlder
		{
			get
			{
				return this.CompareTo(8, 0, 743) <= 0;
			}
		}

		internal bool IsRS2000_RTM_orNewer
		{
			get
			{
				return this.CompareTo(8, 0, 743) >= 0;
			}
		}

		internal bool IsRS2000_WithUnusedFieldsOptimization
		{
			get
			{
				return this.CompareTo(8, 0, 801) >= 0;
			}
		}

		internal bool IsRS2000_WithImageInfo
		{
			get
			{
				return this.CompareTo(8, 0, 843) >= 0;
			}
		}

		internal bool IsRS2005_Beta2_orOlder
		{
			get
			{
				return this.CompareTo(9, 0, 852) <= 0;
			}
		}

		internal bool IsRS2005_WithMultipleActions
		{
			get
			{
				return this.CompareTo(9, 0, 937) >= 0;
			}
		}

		internal bool IsRS2005_WithSpecialChunkSplit
		{
			get
			{
				return this.CompareTo(9, 0, 937) >= 0;
			}
		}

		internal bool IsRS2005_IDW9_orOlder
		{
			get
			{
				return this.CompareTo(9, 0, 951) <= 0;
			}
		}

		internal bool IsRS2005_WithTableDetailFix
		{
			get
			{
				return this.CompareTo(10, 2, 0) >= 0;
			}
		}

		internal bool IsRS2005_WithPHFChunks
		{
			get
			{
				return this.CompareTo(10, 3, 0) >= 0;
			}
		}

		internal bool IsRS2005_WithTableOptimizations
		{
			get
			{
				return this.CompareTo(10, 4, 0) >= 0;
			}
		}

		internal bool IsRS2005_WithSharedDrillthroughParams
		{
			get
			{
				return this.CompareTo(10, 8, 0) >= 0;
			}
		}

		internal bool IsRS2005_WithSimpleTextBoxOptimizations
		{
			get
			{
				return this.CompareTo(10, 5, 0) >= 0;
			}
		}

		internal bool IsRS2005_WithChartHeadingInstanceFix
		{
			get
			{
				return this.CompareTo(10, 6, 0) >= 0;
			}
		}

		internal bool IsRS2005_WithXmlDataElementOutputChange
		{
			get
			{
				return this.CompareTo(10, 7, 0) >= 0;
			}
		}

		internal bool Is_WithUserSort
		{
			get
			{
				return this.CompareTo(9, 0, 970) >= 0;
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
			IntermediateFormatVersion.m_current_major = 10;
			IntermediateFormatVersion.m_current_minor = 8;
			int current_build = 0;
            //todo: can delete?
            RevertImpersonationContext.Run(delegate
			{
				current_build = IntermediateFormatVersion.EncodeFileVersion(FileVersionInfo.GetVersionInfo(typeof(IntermediateFormatVersion).Assembly.Location));
			});
            //current_build = IntermediateFormatVersion.EncodeFileVersion(FileVersionInfo.GetVersionInfo(typeof(IntermediateFormatVersion).Assembly.Location));
            IntermediateFormatVersion.m_current_build = current_build;
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
			this.m_major = IntermediateFormatVersion.m_current_major;
			this.m_minor = IntermediateFormatVersion.m_current_minor;
			this.m_build = IntermediateFormatVersion.m_current_build;
		}

		private int CompareTo(int major, int minor, int build)
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

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.IntermediateFormatVersionMajor, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.IntermediateFormatVersionMinor, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.IntermediateFormatVersionBuild, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IntermediateFormatVersion, memberInfoList);
		}

		public override string ToString()
		{
			if (this.m_major < 10)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", this.m_major, this.m_minor, this.m_build);
			}
			int num = default(int);
			int num2 = default(int);
			int num3 = default(int);
			int num4 = default(int);
			IntermediateFormatVersion.DecodeFileVersion(this.m_build, out num, out num2, out num3, out num4);
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", this.m_major, this.m_minor, num3, num4);
		}
	}
}
