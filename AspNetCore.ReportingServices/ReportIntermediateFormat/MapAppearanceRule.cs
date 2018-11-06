using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
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
	internal class MapAppearanceRule : IPersistable
	{
		[NonSerialized]
		protected MapAppearanceRuleExprHost m_exprHost;

		[NonSerialized]
		protected MapAppearanceRuleExprHost m_exprHostMapMember;

		[Reference]
		protected Map m_map;

		[Reference]
		protected MapVectorLayer m_mapVectorLayer;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapAppearanceRule.GetDeclaration();

		private ExpressionInfo m_dataValue;

		private ExpressionInfo m_distributionType;

		private ExpressionInfo m_bucketCount;

		private ExpressionInfo m_startValue;

		private ExpressionInfo m_endValue;

		private List<MapBucket> m_mapBuckets;

		private string m_legendName;

		private ExpressionInfo m_legendText;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

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

		internal ExpressionInfo DataValue
		{
			get
			{
				return this.m_dataValue;
			}
			set
			{
				this.m_dataValue = value;
			}
		}

		internal ExpressionInfo DistributionType
		{
			get
			{
				return this.m_distributionType;
			}
			set
			{
				this.m_distributionType = value;
			}
		}

		internal ExpressionInfo BucketCount
		{
			get
			{
				return this.m_bucketCount;
			}
			set
			{
				this.m_bucketCount = value;
			}
		}

		internal ExpressionInfo StartValue
		{
			get
			{
				return this.m_startValue;
			}
			set
			{
				this.m_startValue = value;
			}
		}

		internal ExpressionInfo EndValue
		{
			get
			{
				return this.m_endValue;
			}
			set
			{
				this.m_endValue = value;
			}
		}

		internal List<MapBucket> MapBuckets
		{
			get
			{
				return this.m_mapBuckets;
			}
			set
			{
				this.m_mapBuckets = value;
			}
		}

		internal string LegendName
		{
			get
			{
				return this.m_legendName;
			}
			set
			{
				this.m_legendName = value;
			}
		}

		internal ExpressionInfo LegendText
		{
			get
			{
				return this.m_legendText;
			}
			set
			{
				this.m_legendText = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapAppearanceRuleExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal MapAppearanceRuleExprHost ExprHostMapMember
		{
			get
			{
				return this.m_exprHostMapMember;
			}
		}

		internal MapAppearanceRule()
		{
		}

		internal MapAppearanceRule(MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_map = map;
			this.m_mapVectorLayer = mapVectorLayer;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (this.m_distributionType != null)
			{
				this.m_distributionType.Initialize("DistributionType", context);
				context.ExprHostBuilder.MapAppearanceRuleDistributionType(this.m_distributionType);
			}
			if (this.m_bucketCount != null)
			{
				this.m_bucketCount.Initialize("BucketCount", context);
				context.ExprHostBuilder.MapAppearanceRuleBucketCount(this.m_bucketCount);
			}
			if (this.m_startValue != null)
			{
				this.m_startValue.Initialize("StartValue", context);
				context.ExprHostBuilder.MapAppearanceRuleStartValue(this.m_startValue);
			}
			if (this.m_endValue != null)
			{
				this.m_endValue.Initialize("EndValue", context);
				context.ExprHostBuilder.MapAppearanceRuleEndValue(this.m_endValue);
			}
			if (this.m_mapBuckets != null)
			{
				for (int i = 0; i < this.m_mapBuckets.Count; i++)
				{
					this.m_mapBuckets[i].Initialize(context, i);
				}
			}
			if (this.m_legendText != null)
			{
				this.m_legendText.Initialize("LegendText", context);
				context.ExprHostBuilder.MapAppearanceRuleLegendText(this.m_legendText);
			}
			if (this.m_mapVectorLayer.MapDataRegionName == null && this.m_dataValue != null)
			{
				this.m_dataValue.Initialize("DataValue", context);
				context.ExprHostBuilder.MapAppearanceRuleDataValue(this.m_dataValue);
			}
		}

		internal virtual void InitializeMapMember(InitializationContext context)
		{
			if (this.m_mapVectorLayer.MapDataRegionName != null && this.m_dataValue != null)
			{
				this.m_dataValue.Initialize("DataValue", context);
				context.ExprHostBuilder.MapAppearanceRuleDataValue(this.m_dataValue);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapAppearanceRule mapAppearanceRule = (MapAppearanceRule)base.MemberwiseClone();
			mapAppearanceRule.m_map = context.CurrentMapClone;
			mapAppearanceRule.m_mapVectorLayer = context.CurrentMapVectorLayerClone;
			if (this.m_dataValue != null)
			{
				mapAppearanceRule.m_dataValue = (ExpressionInfo)this.m_dataValue.PublishClone(context);
			}
			if (this.m_distributionType != null)
			{
				mapAppearanceRule.m_distributionType = (ExpressionInfo)this.m_distributionType.PublishClone(context);
			}
			if (this.m_bucketCount != null)
			{
				mapAppearanceRule.m_bucketCount = (ExpressionInfo)this.m_bucketCount.PublishClone(context);
			}
			if (this.m_startValue != null)
			{
				mapAppearanceRule.m_startValue = (ExpressionInfo)this.m_startValue.PublishClone(context);
			}
			if (this.m_endValue != null)
			{
				mapAppearanceRule.m_endValue = (ExpressionInfo)this.m_endValue.PublishClone(context);
			}
			if (this.m_mapBuckets != null)
			{
				mapAppearanceRule.m_mapBuckets = new List<MapBucket>(this.m_mapBuckets.Count);
				foreach (MapBucket mapBucket in this.m_mapBuckets)
				{
					mapAppearanceRule.m_mapBuckets.Add((MapBucket)mapBucket.PublishClone(context));
				}
			}
			if (this.m_legendText != null)
			{
				mapAppearanceRule.m_legendText = (ExpressionInfo)this.m_legendText.PublishClone(context);
			}
			return mapAppearanceRule;
		}

		internal virtual void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			IList<MapBucketExprHost> mapBucketsHostsRemotable = this.ExprHost.MapBucketsHostsRemotable;
			if (this.m_mapBuckets != null && mapBucketsHostsRemotable != null)
			{
				for (int i = 0; i < this.m_mapBuckets.Count; i++)
				{
					MapBucket mapBucket = this.m_mapBuckets[i];
					if (mapBucket != null && mapBucket.ExpressionHostID > -1)
					{
						mapBucket.SetExprHost(mapBucketsHostsRemotable[mapBucket.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal virtual void SetExprHostMapMember(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHostMapMember = exprHost;
			this.m_exprHostMapMember.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistributionType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BucketCount, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StartValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapBuckets, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBucket));
			list.Add(new MemberInfo(MemberName.LegendName, Token.String));
			list.Add(new MemberInfo(MemberName.LegendText, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.MapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Token.Reference));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapAppearanceRule, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapAppearanceRule.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.MapVectorLayer:
					writer.WriteReference(this.m_mapVectorLayer);
					break;
				case MemberName.DataValue:
					writer.Write(this.m_dataValue);
					break;
				case MemberName.DistributionType:
					writer.Write(this.m_distributionType);
					break;
				case MemberName.BucketCount:
					writer.Write(this.m_bucketCount);
					break;
				case MemberName.StartValue:
					writer.Write(this.m_startValue);
					break;
				case MemberName.EndValue:
					writer.Write(this.m_endValue);
					break;
				case MemberName.MapBuckets:
					writer.Write(this.m_mapBuckets);
					break;
				case MemberName.LegendName:
					writer.Write(this.m_legendName);
					break;
				case MemberName.LegendText:
					writer.Write(this.m_legendText);
					break;
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

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapAppearanceRule.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.MapVectorLayer:
					this.m_mapVectorLayer = reader.ReadReference<MapVectorLayer>(this);
					break;
				case MemberName.DataValue:
					this.m_dataValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DistributionType:
					this.m_distributionType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BucketCount:
					this.m_bucketCount = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StartValue:
					this.m_startValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndValue:
					this.m_endValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapBuckets:
					this.m_mapBuckets = reader.ReadGenericListOfRIFObjects<MapBucket>();
					break;
				case MemberName.LegendName:
					this.m_legendName = reader.ReadString();
					break;
				case MemberName.LegendText:
					this.m_legendText = (ExpressionInfo)reader.ReadRIFObject();
					break;
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

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(MapAppearanceRule.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.Map:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_map = (Map)referenceableItems[item.RefID];
						break;
					case MemberName.MapVectorLayer:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_mapVectorLayer = (MapVectorLayer)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapAppearanceRule;
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateDataValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_mapVectorLayer.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapAppearanceRuleDataValueExpression(this, this.m_map.Name);
		}

		internal MapRuleDistributionType EvaluateDistributionType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapRuleDistributionType(context.ReportRuntime.EvaluateMapAppearanceRuleDistributionTypeExpression(this, this.m_map.Name), context.ReportRuntime);
		}

		internal int EvaluateBucketCount(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapAppearanceRuleBucketCountExpression(this, this.m_map.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateStartValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapAppearanceRuleStartValueExpression(this, this.m_map.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateEndValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapAppearanceRuleEndValueExpression(this, this.m_map.Name);
		}

		internal string EvaluateLegendText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateMapAppearanceRuleLegendTextExpression(this, this.m_map.Name);
			return this.m_map.GetFormattedStringFromValue(ref variantResult, context);
		}
	}
}
