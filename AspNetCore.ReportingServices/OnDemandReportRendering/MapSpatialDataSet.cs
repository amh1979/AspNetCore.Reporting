using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSpatialDataSet : MapSpatialData
	{
		private ReportStringProperty m_dataSetName;

		private ReportStringProperty m_spatialField;

		private MapFieldNameCollection m_mapFieldNames;

		public ReportStringProperty DataSetName
		{
			get
			{
				if (this.m_dataSetName == null && this.MapSpatialDataSetDef.DataSetName != null)
				{
					this.m_dataSetName = new ReportStringProperty(this.MapSpatialDataSetDef.DataSetName);
				}
				return this.m_dataSetName;
			}
		}

		public ReportStringProperty SpatialField
		{
			get
			{
				if (this.m_spatialField == null && this.MapSpatialDataSetDef.SpatialField != null)
				{
					this.m_spatialField = new ReportStringProperty(this.MapSpatialDataSetDef.SpatialField);
				}
				return this.m_spatialField;
			}
		}

		public MapFieldNameCollection MapFieldNames
		{
			get
			{
				if (this.m_mapFieldNames == null && this.MapSpatialDataSetDef.MapFieldNames != null)
				{
					this.m_mapFieldNames = new MapFieldNameCollection(this.MapSpatialDataSetDef.MapFieldNames, base.m_map);
				}
				return this.m_mapFieldNames;
			}
		}

		internal DataSet DataSet
		{
			get
			{
				string text = "";
				DataSet dataSet = null;
				IDefinitionPath parentDefinitionPath = base.MapDef.ParentDefinitionPath;
				while (parentDefinitionPath.ParentDefinitionPath != null && !(parentDefinitionPath is Report))
				{
					parentDefinitionPath = parentDefinitionPath.ParentDefinitionPath;
				}
				if (parentDefinitionPath is Report)
				{
					text = this.EvaluateDataSetName();
					dataSet = ((Report)parentDefinitionPath).DataSets[text];
				}
				if (dataSet == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidDataSetName, base.MapDef.MapDef.ObjectType, base.MapDef.Name.MarkAsPrivate(), "DataSetName", text.MarkAsPrivate());
				}
				return dataSet;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet MapSpatialDataSetDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet)base.MapSpatialDataDef;
			}
		}

		public new MapSpatialDataSetInstance Instance
		{
			get
			{
				return (MapSpatialDataSetInstance)this.GetInstance();
			}
		}

		internal MapSpatialDataSet(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		private string EvaluateDataSetName()
		{
			ReportStringProperty dataSetName = this.DataSetName;
			if (!dataSetName.IsExpression)
			{
				return dataSetName.Value;
			}
			return this.Instance.DataSetName;
		}

		internal override MapSpatialDataInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapSpatialDataSetInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapFieldNames != null)
			{
				this.m_mapFieldNames.SetNewContext();
			}
		}
	}
}
