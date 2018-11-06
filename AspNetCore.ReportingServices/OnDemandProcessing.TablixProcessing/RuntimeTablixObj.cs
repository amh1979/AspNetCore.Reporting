using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeTablixObj : RuntimeDataTablixWithScopedItemsObj, IStorable, IPersistable
	{
		[NonSerialized]
		private static Declaration m_declaration = RuntimeTablixObj.GetDeclaration();

		internal RuntimeTablixObj()
		{
		}

		internal RuntimeTablixObj(IReference<IScope> outerScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablixDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess)
			: base(outerScope, (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)tablixDef, ref dataAction, odpContext, onePassProcess, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix)
		{
		}

		protected override List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> GetDataRegionScopedItems()
		{
			return ((AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix)base.m_dataRegionDef).DataRegionScopedItemsForDataProcessing;
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		internal new static Declaration GetDeclaration()
		{
			if (RuntimeTablixObj.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				RuntimeTablixObj.m_declaration = new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsObj, memberInfoList);
			}
			return RuntimeTablixObj.m_declaration;
		}
	}
}
