using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ThreeDProperties
	{
		internal enum ShadingTypes
		{
			None,
			Simple,
			Real
		}

		private bool m_enabled;

		private bool m_perspectiveProjectionMode = true;

		private int m_rotation;

		private int m_inclination;

		private int m_perspective;

		private int m_heightRatio;

		private int m_depthRatio;

		private ShadingTypes m_shading;

		private int m_gapDepth;

		private int m_wallThickness;

		private bool m_drawingStyleCube = true;

		private bool m_clustered;

		internal bool Enabled
		{
			get
			{
				return this.m_enabled;
			}
			set
			{
				this.m_enabled = value;
			}
		}

		internal bool PerspectiveProjectionMode
		{
			get
			{
				return this.m_perspectiveProjectionMode;
			}
			set
			{
				this.m_perspectiveProjectionMode = value;
			}
		}

		internal int Rotation
		{
			get
			{
				return this.m_rotation;
			}
			set
			{
				this.m_rotation = value;
			}
		}

		internal int Inclination
		{
			get
			{
				return this.m_inclination;
			}
			set
			{
				this.m_inclination = value;
			}
		}

		internal int Perspective
		{
			get
			{
				return this.m_perspective;
			}
			set
			{
				this.m_perspective = value;
			}
		}

		internal int HeightRatio
		{
			get
			{
				return this.m_heightRatio;
			}
			set
			{
				this.m_heightRatio = value;
			}
		}

		internal int DepthRatio
		{
			get
			{
				return this.m_depthRatio;
			}
			set
			{
				this.m_depthRatio = value;
			}
		}

		internal ShadingTypes Shading
		{
			get
			{
				return this.m_shading;
			}
			set
			{
				this.m_shading = value;
			}
		}

		internal int GapDepth
		{
			get
			{
				return this.m_gapDepth;
			}
			set
			{
				this.m_gapDepth = value;
			}
		}

		internal int WallThickness
		{
			get
			{
				return this.m_wallThickness;
			}
			set
			{
				this.m_wallThickness = value;
			}
		}

		internal bool DrawingStyleCube
		{
			get
			{
				return this.m_drawingStyleCube;
			}
			set
			{
				this.m_drawingStyleCube = value;
			}
		}

		internal bool Clustered
		{
			get
			{
				return this.m_clustered;
			}
			set
			{
				this.m_clustered = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Enabled, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PerspectiveProjectionMode, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Rotation, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Inclination, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Perspective, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeightRatio, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DepthRatio, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Shading, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.GapDepth, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.WallThickness, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DrawingStyleCube, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Clustered, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
