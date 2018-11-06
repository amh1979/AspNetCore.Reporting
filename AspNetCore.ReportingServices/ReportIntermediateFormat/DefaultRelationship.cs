using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DefaultRelationship : Relationship
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = DefaultRelationship.GetDeclaration();

		[NonSerialized]
		private string m_relatedDataSetName;

		internal string RelatedDataSetName
		{
			get
			{
				return this.m_relatedDataSetName;
			}
			set
			{
				this.m_relatedDataSetName = value;
			}
		}

		internal void Initialize(DataSet thisDataSet, InitializationContext context)
		{
			if (base.m_relatedDataSet == null)
			{
				Global.Tracer.Assert(context.ErrorContext.HasError, "Expected error not found. BindAndValidate should have been called before Initialize");
			}
			else
			{
				base.JoinConditionInitialize(base.m_relatedDataSet, context);
			}
		}

		internal void BindAndValidate(DataSet thisDataSet, ErrorContext errorContext, Report report)
		{
			if (base.m_joinConditions == null && !base.m_naturalJoin)
			{
				errorContext.Register(ProcessingErrorCode.rsMissingDefaultRelationshipJoinCondition, Severity.Error, thisDataSet.ObjectType, thisDataSet.Name, "DefaultRelationship", this.m_relatedDataSetName.MarkAsPrivate());
			}
			else if (!report.MappingNameToDataSet.TryGetValue(this.m_relatedDataSetName, out base.m_relatedDataSet))
			{
				errorContext.Register(ProcessingErrorCode.rsNonExistingRelationshipRelatedScope, Severity.Error, thisDataSet.ObjectType, thisDataSet.Name, "DefaultRelationship", "RelatedDataSet", this.m_relatedDataSetName.MarkAsPrivate());
			}
			else
			{
				if (thisDataSet.ID == base.m_relatedDataSet.ID)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidSelfJoinRelationship, Severity.Error, thisDataSet.ObjectType, thisDataSet.Name, "DefaultRelationship", "RelatedDataSet", this.m_relatedDataSetName.MarkAsPrivate());
				}
				if (!base.m_naturalJoin)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidDefaultRelationshipNotNaturalJoin, Severity.Error, thisDataSet.ObjectType, thisDataSet.Name, "DefaultRelationship", "NaturalJoin");
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DefaultRelationship, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Relationship, memberInfoList);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DefaultRelationship.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(DefaultRelationship.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DefaultRelationship;
		}
	}
}
