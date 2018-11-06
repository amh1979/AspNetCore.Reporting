using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class DataAggregateInfo : IPersistable, IStaticReferenceable
	{
		internal enum AggregateTypes
		{
			First,
			Last,
			Sum,
			Avg,
			Max,
			Min,
			CountDistinct,
			CountRows,
			Count,
			StDev,
			Var,
			StDevP,
			VarP,
			Aggregate,
			Previous,
			Union
		}

		internal class PublishingValidationInfo
		{
			private AspNetCore.ReportingServices.ReportProcessing.ObjectType m_objectType;

			private string m_objectName;

			private string m_propertyName;

			private IRIFDataScope m_evaluationScope;

			private List<DataAggregateInfo> m_nestedAggregates;

			private string m_scope;

			private bool m_hasScope;

			private bool m_recursive;

			private int m_aggregateOfAggregatesLevel = -1;

			private bool m_hasAnyFieldReferences;

			internal AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
			{
				get
				{
					return this.m_objectType;
				}
				set
				{
					this.m_objectType = value;
				}
			}

			internal string ObjectName
			{
				get
				{
					return this.m_objectName;
				}
				set
				{
					this.m_objectName = value;
				}
			}

			internal string PropertyName
			{
				get
				{
					return this.m_propertyName;
				}
				set
				{
					this.m_propertyName = value;
				}
			}

			internal IRIFDataScope EvaluationScope
			{
				get
				{
					return this.m_evaluationScope;
				}
				set
				{
					this.m_evaluationScope = value;
				}
			}

			internal List<DataAggregateInfo> NestedAggregates
			{
				get
				{
					return this.m_nestedAggregates;
				}
				set
				{
					this.m_nestedAggregates = value;
				}
			}

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

			internal bool HasScope
			{
				get
				{
					return this.m_hasScope;
				}
				set
				{
					this.m_hasScope = value;
				}
			}

			internal bool Recursive
			{
				get
				{
					return this.m_recursive;
				}
				set
				{
					this.m_recursive = value;
				}
			}

			internal int AggregateOfAggregatesLevel
			{
				get
				{
					return this.m_aggregateOfAggregatesLevel;
				}
				set
				{
					this.m_aggregateOfAggregatesLevel = value;
				}
			}

			internal bool HasAnyFieldReferences
			{
				get
				{
					return this.m_hasAnyFieldReferences;
				}
				set
				{
					this.m_hasAnyFieldReferences = value;
				}
			}

			internal PublishingValidationInfo PublishClone()
			{
				return (PublishingValidationInfo)base.MemberwiseClone();
			}
		}

		private string m_name;

		private AggregateTypes m_aggregateType;

		private ExpressionInfo[] m_expressions;

		private List<string> m_duplicateNames;

		private int m_dataSetIndexInCollection = -1;

		private int m_updateScopeID = -1;

		private int m_updateScopeDepth = -1;

		private bool m_updatesAtRowScope;

		[NonSerialized]
		private PublishingValidationInfo m_publishingInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataAggregateInfo.GetDeclaration();

		[NonSerialized]
		private AggregateParamExprHost[] m_expressionHosts;

		[NonSerialized]
		private bool m_exprHostInitialized;

		[NonSerialized]
		private ObjectModelImpl m_exprHostReportObjectModel;

		[NonSerialized]
		private bool m_hasCachedFieldReferences;

		[NonSerialized]
		private int m_staticId = -2147483648;

		internal virtual bool MustCopyAggregateResult
		{
			get
			{
				return false;
			}
		}

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal string EvaluationScopeName
		{
			get
			{
				return this.PublishingInfo.Scope;
			}
			set
			{
				this.PublishingInfo.Scope = value;
			}
		}

		internal IRIFDataScope EvaluationScope
		{
			get
			{
				return this.PublishingInfo.EvaluationScope;
			}
			set
			{
				this.PublishingInfo.EvaluationScope = value;
			}
		}

		internal AggregateTypes AggregateType
		{
			get
			{
				return this.m_aggregateType;
			}
			set
			{
				this.m_aggregateType = value;
			}
		}

		internal ExpressionInfo[] Expressions
		{
			get
			{
				return this.m_expressions;
			}
			set
			{
				this.m_expressions = value;
			}
		}

		internal List<string> DuplicateNames
		{
			get
			{
				return this.m_duplicateNames;
			}
			set
			{
				this.m_duplicateNames = value;
			}
		}

		internal int DataSetIndexInCollection
		{
			get
			{
				return this.m_dataSetIndexInCollection;
			}
			set
			{
				this.m_dataSetIndexInCollection = value;
			}
		}

		internal string ExpressionText
		{
			get
			{
				if (this.m_expressions != null && 1 == this.m_expressions.Length)
				{
					return this.m_expressions[0].OriginalText;
				}
				return string.Empty;
			}
		}

		internal string ExpressionTextForCompaction
		{
			get
			{
				if (this.PublishingInfo.Recursive)
				{
					return this.ExpressionText + "$Recursive";
				}
				return this.ExpressionText;
			}
		}

		internal AggregateParamExprHost[] ExpressionHosts
		{
			get
			{
				return this.m_expressionHosts;
			}
		}

		internal bool ExprHostInitialized
		{
			get
			{
				return this.m_exprHostInitialized;
			}
			set
			{
				this.m_exprHostInitialized = value;
			}
		}

		internal bool Recursive
		{
			get
			{
				return this.PublishingInfo.Recursive;
			}
			set
			{
				this.PublishingInfo.Recursive = value;
			}
		}

		internal bool IsAggregateOfAggregate
		{
			get
			{
				if (this.PublishingInfo.NestedAggregates != null)
				{
					return this.PublishingInfo.NestedAggregates.Count > 0;
				}
				return false;
			}
		}

		internal int UpdateScopeID
		{
			get
			{
				return this.m_updateScopeID;
			}
			set
			{
				this.m_updateScopeID = value;
			}
		}

		internal int UpdateScopeDepth
		{
			get
			{
				return this.m_updateScopeDepth;
			}
			set
			{
				this.m_updateScopeDepth = value;
			}
		}

		internal bool UpdatesAtRowScope
		{
			get
			{
				return this.m_updatesAtRowScope;
			}
			set
			{
				this.m_updatesAtRowScope = value;
			}
		}

		internal PublishingValidationInfo PublishingInfo
		{
			get
			{
				if (this.m_publishingInfo == null)
				{
					this.m_publishingInfo = new PublishingValidationInfo();
				}
				return this.m_publishingInfo;
			}
		}

		public int ID
		{
			get
			{
				return this.m_staticId;
			}
		}

		internal void AddNestedAggregate(DataAggregateInfo agg)
		{
			if (AggregateTypes.Previous != this.m_aggregateType)
			{
				int num = agg.IsAggregateOfAggregate ? (agg.PublishingInfo.AggregateOfAggregatesLevel + 1) : 0;
				if (num > this.PublishingInfo.AggregateOfAggregatesLevel)
				{
					this.PublishingInfo.AggregateOfAggregatesLevel = num;
				}
				if (this.PublishingInfo.NestedAggregates == null)
				{
					this.PublishingInfo.NestedAggregates = new List<DataAggregateInfo>();
				}
				this.PublishingInfo.NestedAggregates.Add(agg);
			}
		}

		internal bool ShouldRecordFieldReferences()
		{
			return !this.m_hasCachedFieldReferences;
		}

		internal void StoreFieldReferences(OnDemandProcessingContext odpContext, List<string> dataFieldNames)
		{
			this.m_hasCachedFieldReferences = true;
			odpContext.OdpMetadata.ReportSnapshot.AggregateFieldReferences[this.m_name] = dataFieldNames;
		}

		public virtual object PublishClone(AutomaticSubtotalContext context)
		{
			DataAggregateInfo dataAggregateInfo = (DataAggregateInfo)base.MemberwiseClone();
			if (dataAggregateInfo.m_publishingInfo != null)
			{
				dataAggregateInfo.m_publishingInfo = this.m_publishingInfo.PublishClone();
				dataAggregateInfo.m_publishingInfo.NestedAggregates = null;
			}
			dataAggregateInfo.m_name = context.CreateAggregateID(this.m_name);
			bool flag = false;
			if (context.OuterAggregate != null)
			{
				flag = true;
				context.OuterAggregate.AddNestedAggregate(dataAggregateInfo);
			}
			if (this.IsAggregateOfAggregate)
			{
				context.OuterAggregate = dataAggregateInfo;
			}
			if (this.PublishingInfo.HasScope)
			{
				if (flag)
				{
					dataAggregateInfo.SetScope(context.GetNewScopeNameForInnerOrOuterAggregate(this));
				}
				else
				{
					dataAggregateInfo.SetScope(context.GetNewScopeName(this.PublishingInfo.Scope));
				}
			}
			if (this.m_expressions != null)
			{
				dataAggregateInfo.m_expressions = new ExpressionInfo[this.m_expressions.Length];
				for (int i = 0; i < this.m_expressions.Length; i++)
				{
					dataAggregateInfo.m_expressions[i] = (ExpressionInfo)this.m_expressions[i].PublishClone(context);
				}
			}
			return dataAggregateInfo;
		}

		internal virtual string GetAsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_aggregateType.ToString());
			stringBuilder.Append("(");
			if (this.m_expressions != null)
			{
				for (int i = 0; i < this.m_expressions.Length; i++)
				{
					stringBuilder.Append(this.m_expressions[i].OriginalText);
				}
			}
			if (this.PublishingInfo.HasScope)
			{
				if (this.m_expressions != null)
				{
					stringBuilder.Append(", \"");
				}
				stringBuilder.Append(this.PublishingInfo.Scope);
				stringBuilder.Append("\"");
			}
			if (this.PublishingInfo.Recursive)
			{
				if (this.m_expressions != null || this.PublishingInfo.HasScope)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("Recursive");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		internal void SetScope(string scope)
		{
			this.PublishingInfo.HasScope = true;
			this.PublishingInfo.Scope = scope;
		}

		internal bool GetScope(out string scope)
		{
			scope = this.PublishingInfo.Scope;
			return this.PublishingInfo.HasScope;
		}

		internal void SetExprHosts(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
			if (!this.m_exprHostInitialized)
			{
				for (int i = 0; i < this.m_expressions.Length; i++)
				{
					ExpressionInfo expressionInfo = this.m_expressions[i];
					if (expressionInfo.ExprHostID >= 0)
					{
						if (this.m_expressionHosts == null)
						{
							this.m_expressionHosts = new AggregateParamExprHost[this.m_expressions.Length];
						}
						AggregateParamExprHost aggregateParamExprHost = reportExprHost.AggregateParamHostsRemotable[expressionInfo.ExprHostID];
						aggregateParamExprHost.SetReportObjectModel(reportObjectModel);
						this.m_expressionHosts[i] = aggregateParamExprHost;
					}
				}
				this.m_exprHostInitialized = true;
				this.m_exprHostReportObjectModel = reportObjectModel;
			}
			else if (this.m_exprHostReportObjectModel != reportObjectModel && this.m_expressionHosts != null)
			{
				for (int j = 0; j < this.m_expressionHosts.Length; j++)
				{
					if (this.m_expressionHosts[j] != null)
					{
						this.m_expressionHosts[j].SetReportObjectModel(reportObjectModel);
					}
				}
				this.m_exprHostReportObjectModel = reportObjectModel;
			}
		}

		internal bool IsPostSortAggregate()
		{
			if (this.m_aggregateType != 0 && AggregateTypes.Last != this.m_aggregateType && AggregateTypes.Previous != this.m_aggregateType)
			{
				return false;
			}
			return true;
		}

		internal virtual bool IsRunningValue()
		{
			return false;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.AggregateType, Token.Enum));
			list.Add(new MemberInfo(MemberName.Expressions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DuplicateNames, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
			list.Add(new MemberInfo(MemberName.DataSetIndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.UpdateScopeID, Token.Int32));
			list.Add(new MemberInfo(MemberName.UpdateScopeDepth, Token.Int32));
			list.Add(new MemberInfo(MemberName.UpdatesAtRowScope, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DataAggregateInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.AggregateType:
					writer.WriteEnum((int)this.m_aggregateType);
					break;
				case MemberName.Expressions:
					writer.Write(this.m_expressions);
					break;
				case MemberName.DuplicateNames:
					writer.WriteListOfPrimitives(this.m_duplicateNames);
					break;
				case MemberName.DataSetIndexInCollection:
					writer.Write(this.m_dataSetIndexInCollection);
					break;
				case MemberName.UpdateScopeID:
					writer.Write(this.m_updateScopeID);
					break;
				case MemberName.UpdateScopeDepth:
					writer.Write(this.m_updateScopeDepth);
					break;
				case MemberName.UpdatesAtRowScope:
					writer.Write(this.m_updatesAtRowScope);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DataAggregateInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.AggregateType:
					this.m_aggregateType = (AggregateTypes)reader.ReadEnum();
					break;
				case MemberName.Expressions:
					this.m_expressions = reader.ReadArrayOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.DuplicateNames:
					this.m_duplicateNames = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.DataSetIndexInCollection:
					this.m_dataSetIndexInCollection = reader.ReadInt32();
					break;
				case MemberName.UpdateScopeID:
					this.m_updateScopeID = reader.ReadInt32();
					break;
				case MemberName.UpdateScopeDepth:
					this.m_updateScopeDepth = reader.ReadInt32();
					break;
				case MemberName.UpdatesAtRowScope:
					this.m_updatesAtRowScope = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo;
		}

		public void SetID(int id)
		{
			this.m_staticId = id;
		}
	}
}
