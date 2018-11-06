using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ReportItemCollection : IDOwner, IPersistable, IStaticReferenceable, IEnumerable<ReportItem>, IEnumerable
	{
		private List<ReportItem> m_nonComputedReportItems;

		private List<ReportItem> m_computedReportItems;

		private List<ReportItemIndexer> m_sortedReportItemList;

		private List<int> m_romIndexMap;

		[NonSerialized]
		private bool m_normal;

		[NonSerialized]
		private bool m_unpopulated;

		[NonSerialized]
		private List<ReportItem> m_entries;

		[NonSerialized]
		private string m_linkToChildName;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ReportItemCollection.GetDeclaration();

		[NonSerialized]
		private bool m_firstInstance = true;

		[NonSerialized]
		private int m_staticRefId = -2147483648;

		internal ReportItem this[int index]
		{
			get
			{
				if (this.m_unpopulated)
				{
					Global.Tracer.Assert(null != this.m_entries, "(null != m_entries)");
					return this.m_entries[index];
				}
				bool flag = default(bool);
				int num = default(int);
				ReportItem result = default(ReportItem);
				this.GetReportItem(index, out flag, out num, out result);
				return result;
			}
		}

		internal int Count
		{
			get
			{
				if (this.m_unpopulated)
				{
					Global.Tracer.Assert(null != this.m_entries, "(null != m_entries)");
					return this.m_entries.Count;
				}
				if (this.m_sortedReportItemList == null)
				{
					return 0;
				}
				return this.m_sortedReportItemList.Count;
			}
		}

		internal List<ReportItem> ComputedReportItems
		{
			get
			{
				Global.Tracer.Assert(!this.m_unpopulated, "(!m_unpopulated)");
				return this.m_computedReportItems;
			}
			set
			{
				this.m_computedReportItems = value;
			}
		}

		internal List<ReportItem> NonComputedReportItems
		{
			get
			{
				Global.Tracer.Assert(!this.m_unpopulated, "(!m_unpopulated)");
				return this.m_nonComputedReportItems;
			}
			set
			{
				this.m_nonComputedReportItems = value;
			}
		}

		internal List<ReportItemIndexer> SortedReportItems
		{
			get
			{
				Global.Tracer.Assert(!this.m_unpopulated, "(!m_unpopulated)");
				return this.m_sortedReportItemList;
			}
			set
			{
				this.m_sortedReportItemList = value;
			}
		}

		internal List<int> ROMIndexMap
		{
			get
			{
				return this.m_romIndexMap;
			}
		}

		internal bool FirstInstance
		{
			get
			{
				return this.m_firstInstance;
			}
			set
			{
				this.m_firstInstance = value;
			}
		}

		internal string LinkToChild
		{
			set
			{
				this.m_linkToChildName = value;
			}
		}

		int IStaticReferenceable.ID
		{
			get
			{
				return this.m_staticRefId;
			}
		}

		internal ReportItemCollection()
		{
		}

		internal ReportItemCollection(int id, bool normal)
			: base(id)
		{
			this.m_normal = normal;
			this.m_unpopulated = true;
			this.m_entries = new List<ReportItem>();
		}

		public IEnumerator<ReportItem> GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++)
			{
				yield return this[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal void AddReportItem(ReportItem reportItem)
		{
			Global.Tracer.Assert(this.m_unpopulated, "(m_unpopulated)");
			Global.Tracer.Assert(null != reportItem, "(null != reportItem)");
			Global.Tracer.Assert(null != this.m_entries, "(null != m_entries)");
			this.m_entries.Add(reportItem);
		}

		internal void AddCustomRenderItem(ReportItem reportItem)
		{
			Global.Tracer.Assert(null != reportItem, "(null != reportItem)");
			this.m_unpopulated = false;
			if (this.m_sortedReportItemList == null)
			{
				this.m_nonComputedReportItems = new List<ReportItem>();
				this.m_computedReportItems = new List<ReportItem>();
				this.m_sortedReportItemList = new List<ReportItemIndexer>();
			}
			ReportItemIndexer item = default(ReportItemIndexer);
			if (reportItem.Computed)
			{
				this.m_computedReportItems.Add(reportItem);
				item.Index = this.m_computedReportItems.Count - 1;
			}
			else
			{
				this.m_nonComputedReportItems.Add(reportItem);
				item.Index = this.m_nonComputedReportItems.Count - 1;
			}
			item.IsComputed = reportItem.Computed;
			this.m_sortedReportItemList.Add(item);
		}

		internal bool Initialize(InitializationContext context)
		{
			Global.Tracer.Assert(this.m_unpopulated, "(m_unpopulated)");
			if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				context.RegisterPeerScopes(this);
			}
			Global.Tracer.Assert(null != this.m_entries, "(null != m_entries)");
			int count = this.m_entries.Count;
			bool flag = true;
			bool flag2 = false;
			SortedReportItemIndexList sortedReportItemIndexList = new SortedReportItemIndexList(count);
			bool result = true;
			for (int i = 0; i < count; i++)
			{
				ReportItem reportItem = this.m_entries[i];
				Global.Tracer.Assert(null != reportItem, "(null != item)");
				if (!reportItem.Initialize(context))
				{
					result = false;
				}
				if (i == 0 && reportItem.Parent != null)
				{
					if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablix) != 0)
					{
						flag2 = true;
					}
					if (reportItem.Parent.HeightValue < reportItem.Parent.WidthValue)
					{
						flag = false;
					}
				}
				sortedReportItemIndexList.Add(this.m_entries, i, flag);
			}
			if (count > 1 && !flag2 && !context.PublishingContext.IsRdlx)
			{
				this.RegisterOverlappingItems(context, count, sortedReportItemIndexList, flag);
			}
			return result;
		}

		internal void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			for (int i = 0; i < this.m_entries.Count; i++)
			{
				ReportItem reportItem = this.m_entries[i];
				reportItem.InitializeRVDirectionDependentItems(context);
			}
		}

		internal void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			for (int i = 0; i < this.m_entries.Count; i++)
			{
				ReportItem reportItem = this.m_entries[i];
				reportItem.DetermineGroupingExprValueCount(context, groupingExprCount);
			}
		}

		private void RegisterOverlappingItems(InitializationContext context, int count, SortedReportItemIndexList sortedTop, bool isSortedVertically)
		{
			Hashtable hashtable = new Hashtable(count);
			for (int i = 0; i < count - 1; i++)
			{
				int num = sortedTop[i];
				double num2 = isSortedVertically ? this.m_entries[num].AbsoluteBottomValue : this.m_entries[num].AbsoluteRightValue;
				bool flag = true;
				for (int j = i + 1; j < count; j++)
				{
					if (!flag)
					{
						break;
					}
					int num3 = sortedTop[j];
					Global.Tracer.Assert(num != num3, "(currentIndex != peerIndex)");
					double num4 = isSortedVertically ? this.m_entries[num3].AbsoluteTopValue : this.m_entries[num3].AbsoluteLeftValue;
					if (num2 > num4)
					{
						int num5 = Math.Min(num, num3);
						int item = Math.Max(num, num3);
						List<int> list = hashtable[num5] as List<int>;
						if (list == null)
						{
							list = new List<int>();
							hashtable[num5] = list;
						}
						list.Add(item);
					}
					else
					{
						flag = false;
					}
				}
			}
			foreach (int key in hashtable.Keys)
			{
				List<int> list2 = hashtable[key] as List<int>;
				double num7 = isSortedVertically ? this.m_entries[key].AbsoluteLeftValue : this.m_entries[key].AbsoluteTopValue;
				double num8 = isSortedVertically ? this.m_entries[key].AbsoluteRightValue : this.m_entries[key].AbsoluteBottomValue;
				for (int k = 0; k < list2.Count; k++)
				{
					int index = list2[k];
					double num9 = isSortedVertically ? this.m_entries[index].AbsoluteLeftValue : this.m_entries[index].AbsoluteTopValue;
					double num10 = isSortedVertically ? this.m_entries[index].AbsoluteRightValue : this.m_entries[index].AbsoluteBottomValue;
					if (num9 > num7 && num9 < num8)
					{
						goto IL_023a;
					}
					if (num10 > num7 && num10 < num8)
					{
						goto IL_023a;
					}
					if (num9 <= num7 && num8 <= num10)
					{
						goto IL_023a;
					}
					if (num7 <= num9 && num10 <= num8)
					{
						goto IL_023a;
					}
					continue;
					IL_023a:
					if ((this.m_entries[key].ObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem || ((CustomReportItem)this.m_entries[key]).AltReportItem != this.m_entries[index]) && (this.m_entries[index].ObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem || ((CustomReportItem)this.m_entries[index]).AltReportItem != this.m_entries[key]))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsOverlappingReportItems, Severity.Warning, this.m_entries[key].ObjectType, this.m_entries[key].Name, null, ErrorContext.GetLocalizedObjectTypeString(this.m_entries[index].ObjectType), this.m_entries[index].Name);
					}
				}
			}
		}

		internal void CalculateSizes(InitializationContext context, bool overwrite)
		{
			Global.Tracer.Assert(this.m_unpopulated, "(m_unpopulated)");
			Global.Tracer.Assert(null != this.m_entries, "(null != m_entries)");
			for (int i = 0; i < this.m_entries.Count; i++)
			{
				ReportItem reportItem = this.m_entries[i];
				Global.Tracer.Assert(null != reportItem, "(null != item)");
				reportItem.CalculateSizes(context, overwrite);
			}
		}

		internal void MarkChildrenComputed()
		{
			Global.Tracer.Assert(this.m_unpopulated, "(m_unpopulated)");
			Global.Tracer.Assert(null != this.m_entries, "(null != m_entries)");
			for (int i = 0; i < this.m_entries.Count; i++)
			{
				ReportItem reportItem = this.m_entries[i];
				Global.Tracer.Assert(null != reportItem, "(null != item)");
				if (reportItem is TextBox)
				{
					reportItem.Computed = true;
				}
			}
		}

		internal void Populate(ErrorContext errorContext)
		{
			Global.Tracer.Assert(this.m_unpopulated, "(m_unpopulated)");
			Global.Tracer.Assert(null != this.m_entries, "(null != m_entries)");
			Hashtable hashtable = new Hashtable();
			int num = -1;
			if (0 < this.m_entries.Count)
			{
				if (this.m_normal)
				{
					this.m_entries.Sort();
				}
				this.m_nonComputedReportItems = new List<ReportItem>();
				this.m_computedReportItems = new List<ReportItem>();
				this.m_sortedReportItemList = new List<ReportItemIndexer>();
				List<CustomReportItem> list = new List<CustomReportItem>();
				for (int i = 0; i < this.m_entries.Count; i++)
				{
					ReportItem reportItem = this.m_entries[i];
					Global.Tracer.Assert(null != reportItem, "(null != item)");
					if (reportItem.IsDataRegion)
					{
						hashtable[reportItem.Name] = reportItem;
					}
					if (reportItem.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem)
					{
						list.Add((CustomReportItem)reportItem);
					}
					ReportItemIndexer item = default(ReportItemIndexer);
					if (reportItem.Computed)
					{
						this.m_computedReportItems.Add(reportItem);
						item.Index = this.m_computedReportItems.Count - 1;
					}
					else
					{
						this.m_nonComputedReportItems.Add(reportItem);
						item.Index = this.m_nonComputedReportItems.Count - 1;
					}
					item.IsComputed = reportItem.Computed;
					this.m_sortedReportItemList.Add(item);
				}
				if (list.Count > 0)
				{
					bool[] array = new bool[this.m_sortedReportItemList.Count];
					foreach (CustomReportItem item2 in list)
					{
						int num3 = item2.AltReportItemIndexInParentCollectionDef = this.m_entries.IndexOf(item2.AltReportItem);
						array[num3] = true;
					}
					this.m_romIndexMap = new List<int>(this.m_sortedReportItemList.Count - list.Count);
					for (int j = 0; j < this.m_sortedReportItemList.Count; j++)
					{
						if (!array[j])
						{
							this.m_romIndexMap.Add(j);
						}
					}
					Global.Tracer.Assert(this.m_romIndexMap.Count + list.Count == this.m_sortedReportItemList.Count);
				}
			}
			this.m_unpopulated = false;
			this.m_entries = null;
			for (int k = 0; k < this.Count; k++)
			{
				ReportItem reportItem2 = this[k];
				Global.Tracer.Assert(null != reportItem2, "(null != item)");
				if (reportItem2.RepeatWith != null)
				{
					if (reportItem2.IsDataRegion || reportItem2 is SubReport || (reportItem2 is Rectangle && ((Rectangle)reportItem2).ContainsDataRegionOrSubReport()))
					{
						errorContext.Register(ProcessingErrorCode.rsInvalidRepeatWith, Severity.Error, reportItem2.ObjectType, reportItem2.Name, "RepeatWith");
					}
					if (!this.m_normal || !hashtable.ContainsKey(reportItem2.RepeatWith))
					{
						errorContext.Register(ProcessingErrorCode.rsRepeatWithNotPeerDataRegion, Severity.Error, reportItem2.ObjectType, reportItem2.Name, "RepeatWith", reportItem2.RepeatWith);
					}
					DataRegion dataRegion = (DataRegion)hashtable[reportItem2.RepeatWith];
					if (dataRegion != null)
					{
						if (dataRegion.RepeatSiblings == null)
						{
							dataRegion.RepeatSiblings = new List<int>();
						}
						dataRegion.RepeatSiblings.Add((this.m_romIndexMap == null) ? k : this.m_romIndexMap.IndexOf(k));
					}
				}
				if (this.m_linkToChildName != null && num < 0 && reportItem2.Name.Equals(this.m_linkToChildName, StringComparison.Ordinal))
				{
					num = k;
					((Rectangle)reportItem2.Parent).LinkToChild = k;
				}
			}
		}

		internal bool IsReportItemComputed(int index)
		{
			Global.Tracer.Assert(!this.m_unpopulated, "(!m_unpopulated)");
			Global.Tracer.Assert(0 <= index, "(0 <= index)");
			return this.m_sortedReportItemList[index].IsComputed;
		}

		internal ReportItem GetUnsortedReportItem(int index, bool computed)
		{
			Global.Tracer.Assert(!this.m_unpopulated, "(!m_unpopulated)");
			Global.Tracer.Assert(0 <= index, "(0 <= index)");
			return this.InternalGet(index, computed);
		}

		internal void GetReportItem(int index, out bool computed, out int internalIndex, out ReportItem reportItem)
		{
			Global.Tracer.Assert(!this.m_unpopulated, "(!m_unpopulated)");
			computed = false;
			internalIndex = -1;
			reportItem = null;
			if (this.m_sortedReportItemList != null && 0 <= index && index < this.m_sortedReportItemList.Count)
			{
				ReportItemIndexer reportItemIndexer = this.m_sortedReportItemList[index];
				if (0 <= reportItemIndexer.Index)
				{
					computed = reportItemIndexer.IsComputed;
					internalIndex = reportItemIndexer.Index;
					reportItem = this.InternalGet(internalIndex, computed);
				}
			}
		}

		private ReportItem InternalGet(int index, bool computed)
		{
			Global.Tracer.Assert(null != this.m_computedReportItems, "(null != m_computedReportItems)");
			Global.Tracer.Assert(null != this.m_nonComputedReportItems, "(null != m_nonComputedReportItems)");
			if (computed)
			{
				return this.m_computedReportItems[index];
			}
			return this.m_nonComputedReportItems[index];
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ReportItemCollection reportItemCollection = (ReportItemCollection)base.PublishClone(context);
			context.AddReportItemCollection(reportItemCollection);
			if (this.m_entries != null)
			{
				CustomReportItem customReportItem = null;
				reportItemCollection.m_entries = new List<ReportItem>();
				foreach (ReportItem entry in this.m_entries)
				{
					ReportItem reportItem = (ReportItem)entry.PublishClone(context);
					reportItemCollection.m_entries.Add(reportItem);
					if (reportItem is CustomReportItem)
					{
						Global.Tracer.Assert(customReportItem == null, "(lastCriPublishClone == null)");
						customReportItem = (CustomReportItem)reportItem;
					}
					else if (customReportItem != null)
					{
						customReportItem.AltReportItem = reportItem;
						customReportItem = null;
					}
				}
				Global.Tracer.Assert(customReportItem == null, "(lastCriPublishClone == null)");
			}
			if (this.m_linkToChildName != null)
			{
				reportItemCollection.m_linkToChildName = context.GetNewReportItemName(this.m_linkToChildName);
			}
			return reportItemCollection;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.NonComputedReportItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.ComputedReportItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.SortedReportItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemIndexer));
			list.Add(new MemberInfo(MemberName.ROMIndexMap, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ReportItemCollection.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NonComputedReportItems:
					writer.Write(this.m_nonComputedReportItems);
					break;
				case MemberName.ComputedReportItems:
					writer.Write(this.m_computedReportItems);
					break;
				case MemberName.SortedReportItems:
					writer.Write(this.m_sortedReportItemList);
					break;
				case MemberName.ROMIndexMap:
					writer.WriteListOfPrimitives(this.m_romIndexMap);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ReportItemCollection.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NonComputedReportItems:
					this.m_nonComputedReportItems = reader.ReadGenericListOfRIFObjects<ReportItem>();
					break;
				case MemberName.ComputedReportItems:
					this.m_computedReportItems = reader.ReadGenericListOfRIFObjects<ReportItem>();
					break;
				case MemberName.SortedReportItems:
					this.m_sortedReportItemList = reader.ReadGenericListOfRIFObjects<ReportItemIndexer>();
					break;
				case MemberName.ROMIndexMap:
					this.m_romIndexMap = reader.ReadListOfPrimitives<int>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection;
		}

		void IStaticReferenceable.SetID(int id)
		{
			this.m_staticRefId = id;
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return this.GetObjectType();
		}
	}
}
