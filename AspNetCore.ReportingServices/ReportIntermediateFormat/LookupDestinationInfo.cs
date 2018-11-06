using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class LookupDestinationInfo : IPersistable
	{
		private bool m_isMultiValue;

		private ExpressionInfo m_destinationExpr;

		private int m_indexInCollection;

		private bool m_usedInSameDataSetTablixProcessing;

		private int m_exprHostID;

		[NonSerialized]
		private LookupDestExprHost m_exprHost;

		[NonSerialized]
		private string m_scope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = LookupDestinationInfo.GetDeclaration();

		internal string Scope
		{
			get
			{
				return this.m_scope;
			}
			set
			{
				this.m_scope = value;
			}
		}

		internal bool IsMultiValue
		{
			get
			{
				return this.m_isMultiValue;
			}
			set
			{
				this.m_isMultiValue = value;
			}
		}

		internal ExpressionInfo DestinationExpr
		{
			get
			{
				return this.m_destinationExpr;
			}
			set
			{
				this.m_destinationExpr = value;
			}
		}

		internal int IndexInCollection
		{
			get
			{
				return this.m_indexInCollection;
			}
			set
			{
				this.m_indexInCollection = value;
			}
		}

		internal bool UsedInSameDataSetTablixProcessing
		{
			get
			{
				return this.m_usedInSameDataSetTablixProcessing;
			}
			set
			{
				this.m_usedInSameDataSetTablixProcessing = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal LookupDestExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal LookupDestinationInfo()
		{
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			LookupDestinationInfo lookupDestinationInfo = (LookupDestinationInfo)base.MemberwiseClone();
			if (this.m_destinationExpr != null)
			{
				lookupDestinationInfo.m_destinationExpr = (ExpressionInfo)this.m_destinationExpr.PublishClone(context);
			}
			return lookupDestinationInfo;
		}

		internal void Initialize(InitializationContext context, string dataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			context.ExprHostBuilder.LookupDestStart();
			if (this.m_destinationExpr != null)
			{
				this.m_destinationExpr.LookupInitialize(dataSetName, objectType, objectName, propertyName, context);
				context.ExprHostBuilder.LookupDestExpr(this.m_destinationExpr);
			}
			this.m_exprHostID = context.ExprHostBuilder.LookupDestEnd();
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (this.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.LookupDestExprHostsRemotable[this.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateDestExpr(OnDemandProcessingContext odpContext, IErrorContext errorContext)
		{
			return odpContext.ReportRuntime.EvaluateLookupDestExpression(this, errorContext);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(LookupDestinationInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IsMultiValue:
					writer.Write(this.m_isMultiValue);
					break;
				case MemberName.DestinationExpr:
					writer.Write(this.m_destinationExpr);
					break;
				case MemberName.IndexInCollection:
					writer.Write(this.m_indexInCollection);
					break;
				case MemberName.UsedInSameDataSetTablixProcessing:
					writer.Write(this.m_usedInSameDataSetTablixProcessing);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(LookupDestinationInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IsMultiValue:
					this.m_isMultiValue = reader.ReadBoolean();
					break;
				case MemberName.DestinationExpr:
					this.m_destinationExpr = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndexInCollection:
					this.m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.UsedInSameDataSetTablixProcessing:
					this.m_usedInSameDataSetTablixProcessing = reader.ReadBoolean();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupDestinationInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (LookupDestinationInfo.m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.IsMultiValue, Token.Boolean));
				list.Add(new MemberInfo(MemberName.DestinationExpr, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
				list.Add(new MemberInfo(MemberName.UsedInSameDataSetTablixProcessing, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupDestinationInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return LookupDestinationInfo.m_Declaration;
		}
	}
}
