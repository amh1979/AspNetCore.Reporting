using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class EndUserSort : IPersistable
	{
		[Reference]
		private DataSet m_dataSet;

		[Reference]
		private ISortFilterScope m_sortExpressionScope;

		[Reference]
		private GroupingList m_groupsInSortTarget;

		[Reference]
		private ISortFilterScope m_sortTarget;

		private int m_sortExpressionIndex = -1;

		[NonSerialized]
		private static readonly Declaration m_Declaration = EndUserSort.GetDeclaration();

		[NonSerialized]
		private ExpressionInfo m_sortExpression;

		[NonSerialized]
		private string m_sortExpressionScopeString;

		[NonSerialized]
		private string m_sortTargetString;

		private List<SubReport> m_detailScopeSubReports;

		[NonSerialized]
		private int m_subReportDataSetGlobalId = -1;

		internal DataSet DataSet
		{
			get
			{
				return this.m_dataSet;
			}
			set
			{
				this.m_dataSet = value;
			}
		}

		internal ISortFilterScope SortExpressionScope
		{
			get
			{
				return this.m_sortExpressionScope;
			}
			set
			{
				this.m_sortExpressionScope = value;
			}
		}

		internal GroupingList GroupsInSortTarget
		{
			get
			{
				return this.m_groupsInSortTarget;
			}
			set
			{
				this.m_groupsInSortTarget = value;
			}
		}

		internal ISortFilterScope SortTarget
		{
			get
			{
				return this.m_sortTarget;
			}
			set
			{
				this.m_sortTarget = value;
			}
		}

		internal int SortExpressionIndex
		{
			get
			{
				return this.m_sortExpressionIndex;
			}
			set
			{
				this.m_sortExpressionIndex = value;
			}
		}

		internal List<SubReport> DetailScopeSubReports
		{
			get
			{
				return this.m_detailScopeSubReports;
			}
			set
			{
				this.m_detailScopeSubReports = value;
			}
		}

		internal int SubReportDataSetGlobalId
		{
			get
			{
				return this.m_subReportDataSetGlobalId;
			}
			set
			{
				this.m_subReportDataSetGlobalId = value;
			}
		}

		internal ExpressionInfo SortExpression
		{
			get
			{
				return this.m_sortExpression;
			}
			set
			{
				this.m_sortExpression = value;
			}
		}

		internal string SortExpressionScopeString
		{
			get
			{
				return this.m_sortExpressionScopeString;
			}
			set
			{
				this.m_sortExpressionScopeString = value;
			}
		}

		internal string SortTargetString
		{
			get
			{
				return this.m_sortTargetString;
			}
			set
			{
				this.m_sortTargetString = value;
			}
		}

		internal void SetSortTarget(ISortFilterScope target)
		{
			Global.Tracer.Assert(null != target);
			this.m_sortTarget = target;
			if (target.UserSortExpressions == null)
			{
				target.UserSortExpressions = new List<ExpressionInfo>();
			}
			this.m_sortExpressionIndex = target.UserSortExpressions.Count;
			target.UserSortExpressions.Add(this.m_sortExpression);
		}

		internal void SetDefaultSortTarget(ISortFilterScope target)
		{
			this.SetSortTarget(target);
			this.m_sortTargetString = target.ScopeName;
		}

		public object PublishClone(AutomaticSubtotalContext context)
		{
			EndUserSort endUserSort = (EndUserSort)base.MemberwiseClone();
			if (this.m_sortExpression != null)
			{
				endUserSort.m_sortExpression = (ExpressionInfo)this.m_sortExpression.PublishClone(context);
			}
			if (this.m_sortExpressionScopeString != null)
			{
				endUserSort.m_sortExpressionScopeString = (string)this.m_sortExpressionScopeString.Clone();
			}
			if (this.m_sortTargetString != null)
			{
				endUserSort.m_sortTargetString = (string)this.m_sortTargetString.Clone();
			}
			if (this.m_sortTargetString != null || this.m_sortExpressionScopeString != null)
			{
				context.AddEndUserSort(endUserSort);
			}
			return endUserSort;
		}

		internal void UpdateSortScopeAndTargetReference(AutomaticSubtotalContext context)
		{
			if (this.m_sortExpressionScopeString != null)
			{
				this.m_sortExpressionScopeString = context.GetNewScopeName(this.m_sortExpressionScopeString);
			}
			if (this.m_sortTargetString != null)
			{
				this.m_sortTargetString = context.GetNewScopeName(this.m_sortTargetString);
				if (this.m_sortTarget != null)
				{
					ISortFilterScope sortTarget = null;
					if (context.TryGetNewSortTarget(this.m_sortTargetString, out sortTarget))
					{
						this.SetSortTarget(sortTarget);
					}
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			list.Add(new MemberInfo(MemberName.SortExpressionScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ISortFilterScope, Token.Reference));
			list.Add(new MemberInfo(MemberName.GroupsInSortTarget, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.SortTarget, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ISortFilterScope, Token.Reference));
			list.Add(new MemberInfo(MemberName.SortExpressionIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.DetailScopeSubReports, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.EndUserSort, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(EndUserSort.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSet:
					writer.WriteReference(this.m_dataSet);
					break;
				case MemberName.SortExpressionScope:
					writer.WriteReference(this.m_sortExpressionScope);
					break;
				case MemberName.GroupsInSortTarget:
					writer.WriteListOfReferences(this.m_groupsInSortTarget);
					break;
				case MemberName.SortTarget:
					writer.WriteReference(this.m_sortTarget);
					break;
				case MemberName.SortExpressionIndex:
					writer.Write(this.m_sortExpressionIndex);
					break;
				case MemberName.DetailScopeSubReports:
					writer.WriteListOfReferences(this.m_detailScopeSubReports);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(EndUserSort.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataSet:
					this.m_dataSet = reader.ReadReference<DataSet>(this);
					break;
				case MemberName.SortExpressionScope:
					this.m_sortExpressionScope = reader.ReadReference<ISortFilterScope>(this);
					break;
				case MemberName.GroupsInSortTarget:
					this.m_groupsInSortTarget = reader.ReadListOfReferences<GroupingList, Grouping>(this);
					break;
				case MemberName.SortTarget:
					this.m_sortTarget = reader.ReadReference<ISortFilterScope>(this);
					break;
				case MemberName.SortExpressionIndex:
					this.m_sortExpressionIndex = reader.ReadInt32();
					break;
				case MemberName.DetailScopeSubReports:
					this.m_detailScopeSubReports = reader.ReadGenericListOfReferences<SubReport>(this);
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
			if (memberReferencesCollection.TryGetValue(EndUserSort.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.DataSet:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
						Global.Tracer.Assert(this.m_dataSet != (DataSet)referenceableItems[item.RefID]);
						this.m_dataSet = (DataSet)referenceableItems[item.RefID];
						break;
					case MemberName.SortTarget:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is ISortFilterScope);
						Global.Tracer.Assert(this.m_sortTarget != (ISortFilterScope)referenceableItems[item.RefID]);
						this.m_sortTarget = (ISortFilterScope)referenceableItems[item.RefID];
						break;
					case MemberName.DetailScopeSubReports:
						if (this.m_detailScopeSubReports == null)
						{
							this.m_detailScopeSubReports = new List<SubReport>();
						}
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is SubReport);
						Global.Tracer.Assert(!this.m_detailScopeSubReports.Contains((SubReport)referenceableItems[item.RefID]));
						this.m_detailScopeSubReports.Add((SubReport)referenceableItems[item.RefID]);
						break;
					case MemberName.SortExpressionScope:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is ISortFilterScope);
						Global.Tracer.Assert(this.m_sortExpressionScope != (ISortFilterScope)referenceableItems[item.RefID]);
						this.m_sortExpressionScope = (ISortFilterScope)referenceableItems[item.RefID];
						break;
					case MemberName.GroupsInSortTarget:
						if (this.m_groupsInSortTarget == null)
						{
							this.m_groupsInSortTarget = new GroupingList();
						}
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is Grouping);
						Global.Tracer.Assert(!this.m_groupsInSortTarget.Contains((Grouping)referenceableItems[item.RefID]));
						this.m_groupsInSortTarget.Add((Grouping)referenceableItems[item.RefID]);
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.EndUserSort;
		}
	}
}
