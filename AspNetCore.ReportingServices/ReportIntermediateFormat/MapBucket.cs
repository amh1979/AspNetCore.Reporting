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
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapBucket : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private MapBucketExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapBucket.GetDeclaration();

		private ExpressionInfo m_startValue;

		private ExpressionInfo m_endValue;

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

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapBucketExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		internal MapBucket()
		{
		}

		internal MapBucket(Map map)
		{
			this.m_map = map;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapBucketStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			if (this.m_startValue != null)
			{
				this.m_startValue.Initialize("StartValue", context);
				context.ExprHostBuilder.MapBucketStartValue(this.m_startValue);
			}
			if (this.m_endValue != null)
			{
				this.m_endValue.Initialize("EndValue", context);
				context.ExprHostBuilder.MapBucketEndValue(this.m_endValue);
			}
			this.m_exprHostID = context.ExprHostBuilder.MapBucketEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapBucket mapBucket = (MapBucket)base.MemberwiseClone();
			mapBucket.m_map = context.CurrentMapClone;
			if (this.m_startValue != null)
			{
				mapBucket.m_startValue = (ExpressionInfo)this.m_startValue.PublishClone(context);
			}
			if (this.m_endValue != null)
			{
				mapBucket.m_endValue = (ExpressionInfo)this.m_endValue.PublishClone(context);
			}
			return mapBucket;
		}

		internal void SetExprHost(MapBucketExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StartValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBucket, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapBucket.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.StartValue:
					writer.Write(this.m_startValue);
					break;
				case MemberName.EndValue:
					writer.Write(this.m_endValue);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapBucket.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.StartValue:
					this.m_startValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndValue:
					this.m_endValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
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
			if (memberReferencesCollection.TryGetValue(MapBucket.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.Map)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_map = (Map)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBucket;
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateStartValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapBucketStartValueExpression(this, this.m_map.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateEndValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapBucketEndValueExpression(this, this.m_map.Name);
		}
	}
}
