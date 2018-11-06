using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class CustomReportItem : DataRegion, ICreateSubtotals, IPersistable
	{
		private DataMemberList m_dataColumnMembers;

		private DataMemberList m_dataRowMembers;

		private CustomDataRowList m_dataRows;

		private bool m_isDataRegion;

		private string m_type;

		private ReportItem m_altReportItem;

		private int m_altReportItemIndexInParentCollectionDef = -1;

		private ReportItemCollection m_renderReportItem;

		private bool m_explicitAltReportItemDefined;

		[NonSerialized]
		private bool m_createdSubtotals;

		[NonSerialized]
		private static readonly Declaration m_Declaration = CustomReportItem.GetDeclaration();

		[NonSerialized]
		private CustomReportItemExprHost m_criExprHost;

		internal override bool IsDataRegion
		{
			get
			{
				return this.m_isDataRegion;
			}
		}

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem;
			}
		}

		internal override HierarchyNodeList ColumnMembers
		{
			get
			{
				return this.m_dataColumnMembers;
			}
		}

		internal override HierarchyNodeList RowMembers
		{
			get
			{
				return this.m_dataRowMembers;
			}
		}

		internal override RowList Rows
		{
			get
			{
				return this.m_dataRows;
			}
		}

		internal CustomReportItemExprHost CustomReportItemExprHost
		{
			get
			{
				return this.m_criExprHost;
			}
		}

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (this.m_criExprHost == null)
				{
					return null;
				}
				return this.m_criExprHost.UserSortExpressionsHost;
			}
		}

		internal DataMemberList DataColumnMembers
		{
			get
			{
				return this.m_dataColumnMembers;
			}
			set
			{
				this.m_dataColumnMembers = value;
			}
		}

		internal DataMemberList DataRowMembers
		{
			get
			{
				return this.m_dataRowMembers;
			}
			set
			{
				this.m_dataRowMembers = value;
			}
		}

		internal CustomDataRowList DataRows
		{
			get
			{
				return this.m_dataRows;
			}
			set
			{
				this.m_dataRows = value;
			}
		}

		internal string Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}

		internal ReportItem AltReportItem
		{
			get
			{
				return this.m_altReportItem;
			}
			set
			{
				this.m_altReportItem = value;
			}
		}

		internal int AltReportItemIndexInParentCollectionDef
		{
			get
			{
				return this.m_altReportItemIndexInParentCollectionDef;
			}
			set
			{
				this.m_altReportItemIndexInParentCollectionDef = value;
			}
		}

		internal ReportItemCollection RenderReportItem
		{
			get
			{
				return this.m_renderReportItem;
			}
			set
			{
				this.m_renderReportItem = value;
			}
		}

		internal bool ExplicitlyDefinedAltReportItem
		{
			get
			{
				return this.m_explicitAltReportItemDefined;
			}
			set
			{
				this.m_explicitAltReportItemDefined = value;
			}
		}

		internal CustomReportItem(ReportItem parent)
			: base(parent)
		{
		}

		internal CustomReportItem(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal void SetAsDataRegion()
		{
			this.m_isDataRegion = true;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			if (this.IsDataRegion)
			{
				if (!context.RegisterDataRegion(this))
				{
					return false;
				}
				context.Location |= (AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet | AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
			}
			context.ExprHostBuilder.DataRegionStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem, base.m_name);
			base.Initialize(context);
			base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem);
			if (this.IsDataRegion)
			{
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			if (!this.IsDataRegion)
			{
				return false;
			}
			if (this.m_dataRows != null && this.m_dataRows.Count != 0)
			{
				return true;
			}
			if (base.m_rowCount == 0 && base.m_columnCount == 0)
			{
				return false;
			}
			context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataRows, Severity.Error, context.ObjectType, context.ObjectName, base.m_rowCount.ToString(CultureInfo.InvariantCulture.NumberFormat));
			return false;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			CustomReportItem customReportItem = (CustomReportItem)(context.CurrentDataRegionClone = (CustomReportItem)base.PublishClone(context));
			if (this.m_dataColumnMembers != null)
			{
				customReportItem.m_dataColumnMembers = new DataMemberList(this.m_dataColumnMembers.Count);
				foreach (DataMember dataColumnMember in this.m_dataColumnMembers)
				{
					customReportItem.m_dataColumnMembers.Add(dataColumnMember.PublishClone(context, customReportItem));
				}
			}
			if (this.m_dataRowMembers != null)
			{
				customReportItem.m_dataRowMembers = new DataMemberList(this.m_dataRowMembers.Count);
				foreach (DataMember dataRowMember in this.m_dataRowMembers)
				{
					customReportItem.m_dataRowMembers.Add(dataRowMember.PublishClone(context, customReportItem));
				}
			}
			if (this.m_dataRows != null)
			{
				customReportItem.m_dataRows = new CustomDataRowList(this.m_dataRows.Count);
				foreach (CustomDataRow dataRow in this.m_dataRows)
				{
					customReportItem.m_dataRows.Add((CustomDataRow)dataRow.PublishClone(context));
				}
			}
			context.CreateSubtotalsDefinitions.Add(customReportItem);
			return customReportItem;
		}

		public void CreateAutomaticSubtotals(AutomaticSubtotalContext context)
		{
			if (!this.m_createdSubtotals && this.IsDataRegion && this.m_dataRows != null && base.m_rowCount == this.m_dataRows.Count)
			{
				for (int i = 0; i < this.m_dataRows.Count; i++)
				{
					if (this.m_dataRows[i].Cells == null || this.m_dataRows[i].Cells.Count != base.m_columnCount)
					{
						return;
					}
				}
				context.Location = AspNetCore.ReportingServices.ReportPublishing.LocationFlags.None;
				context.ObjectType = this.ObjectType;
				context.ObjectName = "CustomReportItem";
				context.CurrentDataRegion = this;
				context.CurrentScope = base.DataSetName;
				context.CurrentDataScope = this;
				context.CellLists = new List<CellList>(this.m_dataRows.Count);
				for (int j = 0; j < this.m_dataRows.Count; j++)
				{
					context.CellLists.Add(new CellList());
				}
				context.Rows = new RowList(this.m_dataRows.Count);
				context.StartIndex = 0;
				this.CreateAutomaticSubtotals(context, this.m_dataColumnMembers, true);
				context.StartIndex = 0;
				this.CreateAutomaticSubtotals(context, this.m_dataRowMembers, false);
				context.CurrentScope = null;
				context.CurrentDataScope = null;
				this.m_createdSubtotals = true;
			}
		}

		private int CreateAutomaticSubtotals(AutomaticSubtotalContext context, DataMemberList members, bool isColumn)
		{
			int num = 0;
			for (int i = 0; i < members.Count; i++)
			{
				DataMember dataMember = members[i];
				if (dataMember.Subtotal)
				{
					context.CurrentIndex = context.StartIndex;
					if (isColumn)
					{
						foreach (CellList cellList in context.CellLists)
						{
							cellList.Clear();
						}
					}
					else
					{
						context.Rows.Clear();
					}
					base.BuildAndSetupAxisScopeTreeForAutoSubtotals(ref context, dataMember);
					DataMember dataMember2 = (DataMember)dataMember.PublishClone(context, null, true);
					context.AdjustReferences();
					dataMember2.IsAutoSubtotal = true;
					dataMember2.Subtotal = false;
					members.Insert(i + 1, dataMember2);
					num = context.CurrentIndex - context.StartIndex;
					if (isColumn)
					{
						int num2 = 0;
						while (i < this.m_dataRows.Count)
						{
							this.m_dataRows[num2].Cells.InsertRange(context.CurrentIndex, context.CellLists[num2]);
							num2++;
						}
						base.m_columnCount += num;
					}
					else
					{
						this.m_dataRows.InsertRange(context.CurrentIndex, context.Rows);
						base.m_rowCount += num;
					}
					if (dataMember.SubMembers != null)
					{
						context.CurrentScope = dataMember.Grouping.Name;
						context.CurrentDataScope = dataMember;
						int num3 = this.CreateAutomaticSubtotals(context, dataMember.SubMembers, isColumn);
						if (isColumn)
						{
							dataMember.ColSpan += num3;
						}
						else
						{
							dataMember.RowSpan += num3;
						}
						num += num3;
					}
					else
					{
						context.StartIndex++;
					}
				}
				else if (dataMember.SubMembers != null)
				{
					if (dataMember.Grouping != null)
					{
						context.CurrentScope = dataMember.Grouping.Name;
						context.CurrentDataScope = dataMember;
					}
					int num4 = this.CreateAutomaticSubtotals(context, dataMember.SubMembers, isColumn);
					if (isColumn)
					{
						dataMember.ColSpan += num4;
					}
					else
					{
						dataMember.RowSpan += num4;
					}
					num += num4;
				}
				else
				{
					context.StartIndex++;
				}
			}
			return num;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataColumnMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember));
			list.Add(new MemberInfo(MemberName.DataRowMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember));
			list.Add(new MemberInfo(MemberName.DataRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomDataRow));
			list.Add(new MemberInfo(MemberName.Type, Token.String));
			list.Add(new MemberInfo(MemberName.AltReportItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, Token.Reference));
			list.Add(new MemberInfo(MemberName.RenderReportItemColDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
			list.Add(new MemberInfo(MemberName.AltReportItemIndexInParentCollectionDef, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExplicitAltReportItem, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsDataRegion, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomReportItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(CustomReportItem.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataColumnMembers:
					writer.Write(this.m_dataColumnMembers);
					break;
				case MemberName.DataRowMembers:
					writer.Write(this.m_dataRowMembers);
					break;
				case MemberName.DataRows:
					writer.Write(this.m_dataRows);
					break;
				case MemberName.Type:
					writer.Write(this.m_type);
					break;
				case MemberName.AltReportItem:
					writer.WriteReference(this.m_altReportItem);
					break;
				case MemberName.AltReportItemIndexInParentCollectionDef:
					writer.Write(this.m_altReportItemIndexInParentCollectionDef);
					break;
				case MemberName.RenderReportItemColDef:
					writer.Write(this.m_renderReportItem);
					break;
				case MemberName.ExplicitAltReportItem:
					writer.Write(this.m_explicitAltReportItemDefined);
					break;
				case MemberName.IsDataRegion:
					writer.Write(this.m_isDataRegion);
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
			this.m_isDataRegion = (base.m_dataSetName != null);
			reader.RegisterDeclaration(CustomReportItem.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataColumnMembers:
					this.m_dataColumnMembers = reader.ReadListOfRIFObjects<DataMemberList>();
					break;
				case MemberName.DataRowMembers:
					this.m_dataRowMembers = reader.ReadListOfRIFObjects<DataMemberList>();
					break;
				case MemberName.DataRows:
					this.m_dataRows = reader.ReadListOfRIFObjects<CustomDataRowList>();
					break;
				case MemberName.Type:
					this.m_type = reader.ReadString();
					break;
				case MemberName.AltReportItem:
					this.m_altReportItem = reader.ReadReference<ReportItem>(this);
					break;
				case MemberName.AltReportItemIndexInParentCollectionDef:
					this.m_altReportItemIndexInParentCollectionDef = reader.ReadInt32();
					break;
				case MemberName.RenderReportItemColDef:
					this.m_renderReportItem = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.ExplicitAltReportItem:
					this.m_explicitAltReportItemDefined = reader.ReadBoolean();
					break;
				case MemberName.IsDataRegion:
					this.m_isDataRegion = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(CustomReportItem.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.AltReportItem)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_altReportItem = (ReportItem)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomReportItem;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_criExprHost = reportExprHost.CustomReportItemHostsRemotable[base.ExprHostID];
				base.DataRegionSetExprHost(this.m_criExprHost, this.m_criExprHost.SortHost, this.m_criExprHost.FilterHostsRemotable, this.m_criExprHost.UserSortExpressionsHost, this.m_criExprHost.PageBreakExprHost, this.m_criExprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (this.m_dataRows != null && this.m_dataRows.Count > 0)
			{
				IList<DataCellExprHost> list = (this.m_criExprHost != null) ? this.m_criExprHost.CellHostsRemotable : null;
				if (list != null)
				{
					for (int i = 0; i < this.m_dataRows.Count; i++)
					{
						CustomDataRow customDataRow = this.m_dataRows[i];
						Global.Tracer.Assert(customDataRow != null && null != customDataRow.Cells, "(null != row && null != row.Cells)");
						for (int j = 0; j < customDataRow.DataCells.Count; j++)
						{
							DataCell dataCell = customDataRow.DataCells[j];
							Global.Tracer.Assert(dataCell != null && null != dataCell.DataValues, "(null != cell && null != cell.DataValues)");
							if (dataCell.ExpressionHostID >= 0)
							{
								dataCell.DataValues.SetExprHost(list[dataCell.ExpressionHostID].DataValueHostsRemotable, reportObjectModel);
							}
						}
					}
				}
			}
			else
			{
				Global.Tracer.Assert(this.m_criExprHost == null || this.m_criExprHost.CellHostsRemotable == null || this.m_criExprHost.CellHostsRemotable.Count == 0);
			}
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return this.m_criExprHost.NoRowsExpr;
		}

		protected override ReportHierarchyNode CreateHierarchyNode(int id)
		{
			return new DataMember(id, this);
		}

		protected override Row CreateRow(int id, int columnCount)
		{
			CustomDataRow customDataRow = new CustomDataRow(id);
			customDataRow.DataCells = new DataCellList(columnCount);
			return customDataRow;
		}

		protected override Cell CreateCell(int id, int rowIndex, int colIndex)
		{
			return new DataCell(id, this);
		}
	}
}
