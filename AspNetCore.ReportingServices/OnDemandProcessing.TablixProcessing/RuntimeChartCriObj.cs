using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeChartCriObj : RuntimeDataTablixObj
	{
		[NonSerialized]
		private static Declaration m_declaration = RuntimeChartCriObj.GetDeclaration();

		internal RuntimeChartCriObj()
		{
		}

		internal RuntimeChartCriObj(IReference<IScope> outerScope, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(outerScope, dataRegionDef, ref dataAction, odpContext, onePassProcess, objectType)
		{
		}

		internal override RuntimeDataTablixObjReference GetNestedDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(false, "RuntimeChartCriObj cannot have nested data regions");
			throw new InvalidOperationException();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeChartCriObj.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(RuntimeChartCriObj.m_declaration);
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeChartCriObj.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj, memberInfoList);
			}
			return RuntimeChartCriObj.m_declaration;
		}
	}
}
