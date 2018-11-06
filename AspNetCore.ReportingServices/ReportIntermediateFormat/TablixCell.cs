using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class TablixCell : TablixCellBase, IPersistable
	{
		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.Auto;

		[NonSerialized]
		private static readonly Declaration m_Declaration = TablixCell.GetDeclaration();

		[NonSerialized]
		private string m_cellIDForRendering;

		[NonSerialized]
		private ReportSize m_cellWidthForRendering;

		[NonSerialized]
		private ReportSize m_cellHeightForRendering;

		protected override bool IsDataRegionBodyCell
		{
			get
			{
				return true;
			}
		}

		internal string DataElementName
		{
			get
			{
				return this.m_dataElementName;
			}
			set
			{
				this.m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataElementOutput;
			}
			set
			{
				this.m_dataElementOutput = value;
			}
		}

		internal string CellIDForRendering
		{
			get
			{
				return this.m_cellIDForRendering;
			}
			set
			{
				this.m_cellIDForRendering = value;
			}
		}

		internal ReportSize CellWidthForRendering
		{
			get
			{
				return this.m_cellWidthForRendering;
			}
			set
			{
				this.m_cellWidthForRendering = value;
			}
		}

		internal ReportSize CellHeightForRendering
		{
			get
			{
				return this.m_cellHeightForRendering;
			}
			set
			{
				this.m_cellHeightForRendering = value;
			}
		}

		internal TablixCell()
		{
		}

		internal TablixCell(int id, DataRegion dataRegion)
			: base(id, dataRegion)
		{
		}

		internal override void DataRendererInitialize(InitializationContext context)
		{
			if (this.m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				this.m_dataElementOutput = DataElementOutputTypes.ContentsOnly;
			}
			AspNetCore.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref this.m_dataElementName, "Cell", context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			bool flag = false;
			if (context.HasUserSorts)
			{
				context.Location |= (AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem | AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixCell);
				if (context.IsDataRegionCellScope)
				{
					flag = true;
					context.RegisterIndividualCellScope(this);
				}
			}
			if (base.m_cellContents != null)
			{
				base.m_cellContents.InitializeRVDirectionDependentItems(context);
			}
			if (base.m_altCellContents != null)
			{
				base.m_altCellContents.InitializeRVDirectionDependentItems(context);
			}
			if (flag)
			{
				context.UnRegisterIndividualCellScope(this);
			}
		}

		internal void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			if (base.m_cellContents != null)
			{
				base.m_cellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
			}
			if (base.m_altCellContents != null)
			{
				base.m_altCellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TablixCell tablixCell = (TablixCell)base.PublishClone(context);
			if (this.m_dataElementName != null)
			{
				tablixCell.m_dataElementName = (string)this.m_dataElementName.Clone();
			}
			return tablixCell;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCellBase, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TablixCell.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataElementName:
					writer.Write(this.m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)this.m_dataElementOutput);
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
			reader.RegisterDeclaration(TablixCell.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataElementName:
					this.m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					this.m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCell;
		}

		internal void SetExprHost(TablixCellExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			base.BaseSetExprHost(exprHost, reportObjectModel);
		}
	}
}
