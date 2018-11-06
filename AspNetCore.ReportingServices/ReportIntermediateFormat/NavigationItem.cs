using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class NavigationItem : IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = NavigationItem.GetDeclaration();

		private string m_reportItemReference;

		private BandNavigationCell m_bandNavigationCell;

		internal string ReportItemReference
		{
			get
			{
				return this.m_reportItemReference;
			}
			set
			{
				this.m_reportItemReference = value;
			}
		}

		internal BandNavigationCell BandNavigationCell
		{
			get
			{
				return this.m_bandNavigationCell;
			}
			set
			{
				this.m_bandNavigationCell = value;
			}
		}

		internal void Initialize(Tablix tablix, InitializationContext context, string NavigationType)
		{
			if (this.ReportItemReference != null && !tablix.ValidateBandReportItemReference(this.ReportItemReference))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandNavigationReference, Severity.Error, context.ObjectType, context.ObjectName, NavigationType, this.ReportItemReference);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReportItemReference, Token.String));
			list.Add(new MemberInfo(MemberName.BandNavigationCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BandNavigationCell));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NavigationItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(NavigationItem.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReportItemReference:
					writer.Write(this.m_reportItemReference);
					break;
				case MemberName.BandNavigationCell:
					writer.Write(this.m_bandNavigationCell);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(NavigationItem.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ReportItemReference:
					this.m_reportItemReference = reader.ReadString();
					break;
				case MemberName.BandNavigationCell:
					this.m_bandNavigationCell = (BandNavigationCell)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NavigationItem;
		}
	}
}
