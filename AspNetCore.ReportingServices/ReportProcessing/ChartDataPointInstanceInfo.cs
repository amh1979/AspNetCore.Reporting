using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointInstanceInfo : InstanceInfo
	{
		private int m_dataPointIndex = -1;

		private object[] m_dataValues;

		private string m_dataLabelValue;

		private object[] m_dataLabelStyleAttributeValues;

		private ActionInstance m_action;

		private object[] m_styleAttributeValues;

		private object[] m_markerStyleAttributeValues;

		private DataValueInstanceList m_customPropertyInstances;

		internal int DataPointIndex
		{
			get
			{
				return this.m_dataPointIndex;
			}
			set
			{
				this.m_dataPointIndex = value;
			}
		}

		internal object[] DataValues
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

		internal string DataLabelValue
		{
			get
			{
				return this.m_dataLabelValue;
			}
			set
			{
				this.m_dataLabelValue = value;
			}
		}

		internal object[] DataLabelStyleAttributeValues
		{
			get
			{
				return this.m_dataLabelStyleAttributeValues;
			}
			set
			{
				this.m_dataLabelStyleAttributeValues = value;
			}
		}

		internal ActionInstance Action
		{
			get
			{
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		internal object[] StyleAttributeValues
		{
			get
			{
				return this.m_styleAttributeValues;
			}
			set
			{
				this.m_styleAttributeValues = value;
			}
		}

		internal object[] MarkerStyleAttributeValues
		{
			get
			{
				return this.m_markerStyleAttributeValues;
			}
			set
			{
				this.m_markerStyleAttributeValues = value;
			}
		}

		internal DataValueInstanceList CustomPropertyInstances
		{
			get
			{
				return this.m_customPropertyInstances;
			}
			set
			{
				this.m_customPropertyInstances = value;
			}
		}

		internal ChartDataPointInstanceInfo(ReportProcessing.ProcessingContext pc, Chart chart, ChartDataPoint dataPointDef, int dataPointIndex, ChartDataPointInstance owner)
		{
			this.m_dataPointIndex = dataPointIndex;
			int count = dataPointDef.DataValues.Count;
			this.m_dataValues = new object[count];
			bool flag = false;
			if (dataPointDef.Action != null)
			{
				flag = dataPointDef.Action.ResetObjectModelForDrillthroughContext(pc.ReportObjectModel, dataPointDef);
			}
			for (int i = 0; i < count; i++)
			{
				this.m_dataValues[i] = pc.ReportRuntime.EvaluateChartDataPointDataValueExpression(dataPointDef, dataPointDef.DataValues[i], chart.Name);
			}
			if (flag)
			{
				dataPointDef.Action.GetSelectedItemsForDrillthroughContext(pc.ReportObjectModel, dataPointDef);
			}
			if (dataPointDef.DataLabel != null)
			{
				this.m_dataLabelStyleAttributeValues = Chart.CreateStyle(pc, dataPointDef.DataLabel.StyleClass, chart.Name + ".DataLabel", owner.UniqueName);
				this.m_dataLabelValue = pc.ReportRuntime.EvaluateChartDataLabelValueExpression(dataPointDef, chart.Name, this.m_dataLabelStyleAttributeValues);
			}
			if (dataPointDef.Action != null)
			{
				this.m_action = ReportProcessing.RuntimeRICollection.CreateActionInstance(pc, dataPointDef, owner.UniqueName, chart.ObjectType, chart.Name + ".DataPoint");
			}
			this.m_styleAttributeValues = Chart.CreateStyle(pc, dataPointDef.StyleClass, chart.Name + ".DataPoint", owner.UniqueName);
			if (dataPointDef.MarkerStyleClass != null)
			{
				this.m_markerStyleAttributeValues = Chart.CreateStyle(pc, dataPointDef.MarkerStyleClass, chart.Name + ".DataPoint.Marker", owner.UniqueName);
			}
			if (dataPointDef.CustomProperties != null)
			{
				this.m_customPropertyInstances = dataPointDef.CustomProperties.EvaluateExpressions(chart.ObjectType, chart.Name, "DataPoint(" + (dataPointIndex + 1).ToString(CultureInfo.InvariantCulture) + ").", pc);
			}
			pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
		}

		internal ChartDataPointInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataPointIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DataValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.DataLabelValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataLabelStyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionInstance));
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MarkerStyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
