using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableColumn
	{
		private string m_width;

		private double m_widthValue;

		private Visibility m_visibility;

		private bool m_fixedHeader;

		[NonSerialized]
		private ReportSize m_widthForRendering;

		internal string Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		internal double WidthValue
		{
			get
			{
				return this.m_widthValue;
			}
			set
			{
				this.m_widthValue = value;
			}
		}

		internal Visibility Visibility
		{
			get
			{
				return this.m_visibility;
			}
			set
			{
				this.m_visibility = value;
			}
		}

		internal ReportSize WidthForRendering
		{
			get
			{
				return this.m_widthForRendering;
			}
			set
			{
				this.m_widthForRendering = value;
			}
		}

		internal bool FixedHeader
		{
			get
			{
				return this.m_fixedHeader;
			}
			set
			{
				this.m_fixedHeader = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			this.m_widthValue = context.ValidateSize(ref this.m_width, "Width");
			if (this.m_visibility != null)
			{
				this.m_visibility.Initialize(context, false, true);
			}
		}

		internal void RegisterReceiver(InitializationContext context)
		{
			if (this.m_visibility != null)
			{
				this.m_visibility.RegisterReceiver(context, false);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Width, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.WidthValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			memberInfoList.Add(new MemberInfo(MemberName.FixedHeader, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
