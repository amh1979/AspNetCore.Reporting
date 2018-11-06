using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Sorting : IPersistable
	{
		private List<ExpressionInfo> m_sortExpressions;

		private List<bool> m_sortDirections;

		private List<bool> m_naturalSortFlags;

		private bool m_naturalSort;

		private List<bool> m_deferredSortFlags;

		private bool m_deferredSort;

		[NonSerialized]
		private SortExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Sorting.GetDeclaration();

		internal List<ExpressionInfo> SortExpressions
		{
			get
			{
				return this.m_sortExpressions;
			}
			set
			{
				this.m_sortExpressions = value;
			}
		}

		internal List<bool> SortDirections
		{
			get
			{
				return this.m_sortDirections;
			}
			set
			{
				this.m_sortDirections = value;
			}
		}

		internal List<bool> NaturalSortFlags
		{
			get
			{
				return this.m_naturalSortFlags;
			}
			set
			{
				this.m_naturalSortFlags = value;
			}
		}

		internal List<bool> DeferredSortFlags
		{
			get
			{
				return this.m_deferredSortFlags;
			}
			set
			{
				this.m_deferredSortFlags = value;
			}
		}

		internal SortExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal bool NaturalSort
		{
			get
			{
				return this.m_naturalSort;
			}
			set
			{
				this.m_naturalSort = value;
			}
		}

		internal bool DeferredSort
		{
			get
			{
				return this.m_deferredSort;
			}
		}

		internal bool ShouldApplySorting
		{
			get
			{
				if (!this.m_naturalSort && !this.m_deferredSort && this.m_sortDirections != null)
				{
					return this.m_sortDirections.Count > 0;
				}
				return false;
			}
		}

		internal Sorting(ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				this.m_sortExpressions = new List<ExpressionInfo>();
				this.m_sortDirections = new List<bool>();
				this.m_naturalSortFlags = new List<bool>();
				this.m_deferredSortFlags = new List<bool>();
			}
		}

		internal void ValidateNaturalSortFlags(PublishingContextStruct context)
		{
			this.m_naturalSort = Sorting.ValidateExclusiveSortFlag(context, this.m_naturalSortFlags, "NaturalSort");
		}

		internal void ValidateDeferredSortFlags(PublishingContextStruct context)
		{
			this.m_deferredSort = Sorting.ValidateExclusiveSortFlag(context, this.m_deferredSortFlags, "DeferredSort");
		}

		private static bool ValidateExclusiveSortFlag(PublishingContextStruct context, List<bool> flags, string propertyName)
		{
			if (flags != null && flags.Count != 0)
			{
				int count = flags.Count;
				bool flag = flags[0];
				for (int i = 1; i < count; i++)
				{
					if (flag != flags[i])
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSortFlagCombination, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
						return false;
					}
				}
				return flag;
			}
			return false;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.SortStart();
			if (this.m_sortExpressions != null)
			{
				for (int i = 0; i < this.m_sortExpressions.Count; i++)
				{
					ExpressionInfo expressionInfo = this.m_sortExpressions[i];
					expressionInfo.Initialize("SortExpression", context);
					context.ExprHostBuilder.SortExpression(expressionInfo);
				}
			}
			context.ExprHostBuilder.SortEnd();
		}

		internal void SetExprHost(SortExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			Sorting sorting = (Sorting)base.MemberwiseClone();
			if (this.m_sortExpressions != null)
			{
				sorting.m_sortExpressions = new List<ExpressionInfo>(this.m_sortExpressions.Count);
				foreach (ExpressionInfo sortExpression in this.m_sortExpressions)
				{
					sorting.m_sortExpressions.Add((ExpressionInfo)sortExpression.PublishClone(context));
				}
			}
			if (this.m_sortDirections != null)
			{
				sorting.m_sortDirections = new List<bool>(this.m_sortDirections.Count);
				{
					foreach (bool sortDirection in this.m_sortDirections)
					{
						sorting.m_sortDirections.Add(sortDirection);
					}
					return sorting;
				}
			}
			return sorting;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.SortExpressions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SortDirections, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Boolean));
			list.Add(new MemberInfo(MemberName.NaturalSortFlags, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Boolean));
			list.Add(new MemberInfo(MemberName.NaturalSort, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DeferredSortFlags, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Boolean, Lifetime.AddedIn(100)));
			list.Add(new MemberInfo(MemberName.DeferredSort, Token.Boolean, Lifetime.AddedIn(100)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sorting, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Sorting.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.SortExpressions:
					writer.Write(this.m_sortExpressions);
					break;
				case MemberName.SortDirections:
					writer.WriteListOfPrimitives(this.m_sortDirections);
					break;
				case MemberName.NaturalSortFlags:
					writer.WriteListOfPrimitives(this.m_naturalSortFlags);
					break;
				case MemberName.NaturalSort:
					writer.Write(this.m_naturalSort);
					break;
				case MemberName.DeferredSortFlags:
					writer.WriteListOfPrimitives(this.m_deferredSortFlags);
					break;
				case MemberName.DeferredSort:
					writer.Write(this.m_deferredSort);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Sorting.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.SortExpressions:
					this.m_sortExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.SortDirections:
					this.m_sortDirections = reader.ReadListOfPrimitives<bool>();
					break;
				case MemberName.NaturalSortFlags:
					this.m_naturalSortFlags = reader.ReadListOfPrimitives<bool>();
					break;
				case MemberName.NaturalSort:
					this.m_naturalSort = reader.ReadBoolean();
					break;
				case MemberName.DeferredSortFlags:
					this.m_deferredSortFlags = reader.ReadListOfPrimitives<bool>();
					break;
				case MemberName.DeferredSort:
					this.m_deferredSort = reader.ReadBoolean();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sorting;
		}
	}
}
