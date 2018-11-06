using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeGaugePanelObj : RuntimeChartCriObj
	{
		[NonSerialized]
		private static Declaration m_declaration = RuntimeGaugePanelObj.GetDeclaration();

		internal RuntimeGaugePanelObj()
		{
		}

		internal RuntimeGaugePanelObj(IReference<IScope> outerScope, GaugePanel gaugePanelDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess)
			: base(outerScope, (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)gaugePanelDef, ref dataAction, odpContext, onePassProcess, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel)
		{
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeGaugePanelObj.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(RuntimeGaugePanelObj.m_declaration);
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeGaugePanelObj.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObj, memberInfoList);
			}
			return RuntimeGaugePanelObj.m_declaration;
		}
	}
}
