using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal abstract class Relationship : IPersistable
	{
		internal sealed class JoinCondition : IPersistable
		{
			private ExpressionInfo m_foreignKeyExpression;

			private ExpressionInfo m_primaryKeyExpression;

			private int m_exprHostID;

			private SortDirection m_sortDirection;

			[NonSerialized]
			private static readonly Declaration m_Declaration = JoinCondition.GetDeclaration();

			[NonSerialized]
			private JoinConditionExprHost m_exprHost;

			internal ExpressionInfo ForeignKeyExpression
			{
				get
				{
					return this.m_foreignKeyExpression;
				}
			}

			internal ExpressionInfo PrimaryKeyExpression
			{
				get
				{
					return this.m_primaryKeyExpression;
				}
			}

			internal SortDirection SortDirection
			{
				get
				{
					return this.m_sortDirection;
				}
			}

			internal JoinConditionExprHost ExprHost
			{
				get
				{
					return this.m_exprHost;
				}
			}

			internal JoinCondition()
			{
			}

			internal JoinCondition(ExpressionInfo foreignKey, ExpressionInfo primaryKey, SortDirection direction)
			{
				this.m_foreignKeyExpression = foreignKey;
				this.m_primaryKeyExpression = primaryKey;
				this.m_sortDirection = direction;
			}

			internal void Initialize(DataSet relatedDataSet, bool naturalJoin, InitializationContext context)
			{
				context.ExprHostBuilder.JoinConditionStart();
				if (this.m_foreignKeyExpression != null)
				{
					this.m_foreignKeyExpression.Initialize("ForeignKey", context);
					context.ExprHostBuilder.JoinConditionForeignKeyExpr(this.m_foreignKeyExpression);
				}
				if (this.m_primaryKeyExpression != null)
				{
					context.RegisterDataSet(relatedDataSet);
					this.m_primaryKeyExpression.Initialize("PrimaryKey", context);
					context.ExprHostBuilder.JoinConditionPrimaryKeyExpr(this.m_primaryKeyExpression);
					context.UnRegisterDataSet(relatedDataSet);
				}
				this.m_exprHostID = context.ExprHostBuilder.JoinConditionEnd();
			}

			internal void SetExprHost(IList<JoinConditionExprHost> joinConditionExprHost, ObjectModelImpl reportObjectModel)
			{
				if (this.m_exprHostID >= 0)
				{
					Global.Tracer.Assert(joinConditionExprHost != null && reportObjectModel != null, "(joinConditionExprHost != null && reportObjectModel != null)");
					this.m_exprHost = joinConditionExprHost[this.m_exprHostID];
					this.m_exprHost.SetReportObjectModel(reportObjectModel);
				}
			}

			internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateForeignKeyExpr(AspNetCore.ReportingServices.RdlExpressions.ReportRuntime runtime)
			{
				return runtime.EvaluateJoinConditionForeignKeyExpression(this);
			}

			internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluatePrimaryKeyExpr(AspNetCore.ReportingServices.RdlExpressions.ReportRuntime runtime)
			{
				return runtime.EvaluateJoinConditionPrimaryKeyExpression(this);
			}

			internal static Declaration GetDeclaration()
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ForeignKeyExpression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new MemberInfo(MemberName.PrimaryKeyExpression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
				list.Add(new MemberInfo(MemberName.SortDirection, Token.Enum));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinCondition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(JoinCondition.m_Declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ForeignKeyExpression:
						writer.Write(this.m_foreignKeyExpression);
						break;
					case MemberName.PrimaryKeyExpression:
						writer.Write(this.m_primaryKeyExpression);
						break;
					case MemberName.ExprHostID:
						writer.Write(this.m_exprHostID);
						break;
					case MemberName.SortDirection:
						writer.WriteEnum((int)this.m_sortDirection);
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(JoinCondition.m_Declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.ForeignKeyExpression:
						this.m_foreignKeyExpression = (ExpressionInfo)reader.ReadRIFObject();
						break;
					case MemberName.PrimaryKeyExpression:
						this.m_primaryKeyExpression = (ExpressionInfo)reader.ReadRIFObject();
						break;
					case MemberName.ExprHostID:
						this.m_exprHostID = reader.ReadInt32();
						break;
					case MemberName.SortDirection:
						this.m_sortDirection = (SortDirection)reader.ReadEnum();
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
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinCondition;
			}
		}

		protected List<JoinCondition> m_joinConditions;

		protected bool m_naturalJoin;

		protected DataSet m_relatedDataSet;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Relationship.GetDeclaration();

		internal bool NaturalJoin
		{
			get
			{
				return this.m_naturalJoin;
			}
			set
			{
				this.m_naturalJoin = value;
			}
		}

		internal DataSet RelatedDataSet
		{
			get
			{
				return this.m_relatedDataSet;
			}
		}

		internal bool IsCrossJoin
		{
			get
			{
				return this.JoinConditionCount == 0;
			}
		}

		internal int JoinConditionCount
		{
			get
			{
				if (this.m_joinConditions != null)
				{
					return this.m_joinConditions.Count;
				}
				return 0;
			}
		}

		internal void AddJoinCondition(ExpressionInfo foreignKey, ExpressionInfo primaryKey, SortDirection direction)
		{
			this.AddJoinCondition(new JoinCondition(foreignKey, primaryKey, direction));
		}

		internal void AddJoinCondition(JoinCondition joinCondition)
		{
			if (this.m_joinConditions == null)
			{
				this.m_joinConditions = new List<JoinCondition>();
			}
			this.m_joinConditions.Add(joinCondition);
		}

		internal void JoinConditionInitialize(DataSet relatedDataSet, InitializationContext context)
		{
			for (int i = 0; i < this.JoinConditionCount; i++)
			{
				this.m_joinConditions[i].Initialize(relatedDataSet, this.m_naturalJoin, context);
			}
		}

		internal void SetExprHost(IList<JoinConditionExprHost> joinConditionExprHost, ObjectModelImpl reportObjectModel)
		{
			for (int i = 0; i < this.JoinConditionCount; i++)
			{
				this.m_joinConditions[i].SetExprHost(joinConditionExprHost, reportObjectModel);
			}
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult[] EvaluateJoinConditionKeys(bool evaluatePrimaryKeys, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRuntime)
		{
			int joinConditionCount = this.JoinConditionCount;
			if (joinConditionCount == 0)
			{
				return null;
			}
			AspNetCore.ReportingServices.RdlExpressions.VariantResult[] array = new AspNetCore.ReportingServices.RdlExpressions.VariantResult[joinConditionCount];
			for (int i = 0; i < joinConditionCount; i++)
			{
				if (evaluatePrimaryKeys)
				{
					array[i] = this.m_joinConditions[i].EvaluatePrimaryKeyExpr(reportRuntime);
				}
				else
				{
					array[i] = this.m_joinConditions[i].EvaluateForeignKeyExpr(reportRuntime);
				}
			}
			return array;
		}

		internal ExpressionInfo[] GetForeignKeyExpressions()
		{
			int joinConditionCount = this.JoinConditionCount;
			if (joinConditionCount == 0)
			{
				return null;
			}
			ExpressionInfo[] array = new ExpressionInfo[joinConditionCount];
			for (int i = 0; i < joinConditionCount; i++)
			{
				array[i] = this.m_joinConditions[i].ForeignKeyExpression;
			}
			return array;
		}

		internal SortDirection[] GetSortDirections()
		{
			if (this.JoinConditionCount == 0)
			{
				return null;
			}
			SortDirection[] array = new SortDirection[this.JoinConditionCount];
			for (int i = 0; i < this.JoinConditionCount; i++)
			{
				array[i] = this.m_joinConditions[i].SortDirection;
			}
			return array;
		}

		internal bool TryMapFieldIndex(int primaryKeyFieldIndex, out int foreignKeyFieldIndex)
		{
			if (this.JoinConditionCount > 0)
			{
				foreach (JoinCondition joinCondition in this.m_joinConditions)
				{
					if (joinCondition.PrimaryKeyExpression.Type == ExpressionInfo.Types.Field && joinCondition.ForeignKeyExpression.Type == ExpressionInfo.Types.Field && joinCondition.PrimaryKeyExpression.FieldIndex == primaryKeyFieldIndex)
					{
						foreignKeyFieldIndex = joinCondition.ForeignKeyExpression.FieldIndex;
						return true;
					}
				}
			}
			foreignKeyFieldIndex = -1;
			return false;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.JoinConditions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinCondition));
			list.Add(new MemberInfo(MemberName.NaturalJoin, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RelatedDataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Relationship, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Relationship.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.JoinConditions:
					writer.Write(this.m_joinConditions);
					break;
				case MemberName.NaturalJoin:
					writer.Write(this.m_naturalJoin);
					break;
				case MemberName.RelatedDataSet:
					writer.WriteReference(this.m_relatedDataSet);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Relationship.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.JoinConditions:
					this.m_joinConditions = reader.ReadGenericListOfRIFObjects<JoinCondition>();
					break;
				case MemberName.NaturalJoin:
					this.m_naturalJoin = reader.ReadBoolean();
					break;
				case MemberName.RelatedDataSet:
					this.m_relatedDataSet = reader.ReadReference<DataSet>(this);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(Relationship.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.RelatedDataSet)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
						Global.Tracer.Assert(this.m_relatedDataSet != (DataSet)referenceableItems[item.RefID]);
						this.m_relatedDataSet = (DataSet)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Relationship;
		}
	}
}
