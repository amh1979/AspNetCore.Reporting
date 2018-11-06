using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataCell : Cell, IPersistable
	{
		private DataValueList m_dataValues;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataCell.GetDeclaration();

		protected override bool IsDataRegionBodyCell
		{
			get
			{
				return true;
			}
		}

		internal DataValueList DataValues
		{
			get
			{
				return this.m_dataValues;
			}
			set
			{
				this.m_dataValues = value;
			}
		}

		public override AspNetCore.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataCell;
			}
		}

		protected override AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode
		{
			get
			{
				return AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem;
			}
		}

		internal DataCell()
		{
		}

		internal DataCell(int id, DataRegion dataRegion)
			: base(id, dataRegion)
		{
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			this.m_dataValues.Initialize(null, rowindex, colIndex, false, context);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			DataCell dataCell = (DataCell)base.PublishClone(context);
			if (this.m_dataValues != null)
			{
				dataCell.m_dataValues = new DataValueList(this.m_dataValues.Count);
				{
					foreach (DataValue dataValue in this.m_dataValues)
					{
						dataCell.m_dataValues.Add(dataValue.PublishClone(context));
					}
					return dataCell;
				}
			}
			return dataCell;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DataCell.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.DataValues)
				{
					writer.Write(this.m_dataValues);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(DataCell.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.DataValues)
				{
					this.m_dataValues = reader.ReadListOfRIFObjects<DataValueList>();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCell;
		}
	}
}
