using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapCell : Cell, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapCell.GetDeclaration();

		protected override bool IsDataRegionBodyCell
		{
			get
			{
				return true;
			}
		}

		public AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.MapCell;
			}
		}

		public override AspNetCore.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType
		{
			get
			{
				return this.ObjectType;
			}
		}

		protected override AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode
		{
			get
			{
				return AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion;
			}
		}

		internal MapCell()
		{
		}

		internal MapCell(int id, MapDataRegion mapDataRegion)
			: base(id, mapDataRegion)
		{
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, memberInfoList);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCell;
		}
	}
}
