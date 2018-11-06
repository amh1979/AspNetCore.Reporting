using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class BandNavigationCell : TablixCellBase, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = BandNavigationCell.GetDeclaration();

		internal BandNavigationCell()
		{
		}

		internal BandNavigationCell(int id, DataRegion dataRegion)
			: base(id, dataRegion)
		{
		}

		internal void Initialize(InitializationContext context)
		{
			base.Initialize(0, 0, 0, 0, context);
		}

		protected override void StartExprHost(InitializationContext context)
		{
		}

		protected override void EndExprHost(InitializationContext context)
		{
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			return (BandNavigationCell)base.PublishClone(context);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BandNavigationCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCellBase, memberInfoList);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(BandNavigationCell.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(BandNavigationCell.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BandNavigationCell;
		}
	}
}
