using AspNetCore.ReportingServices.OnDemandProcessing;
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
	[Serializable]
	internal sealed class MapLimits : IPersistable
	{
		[NonSerialized]
		private MapLimitsExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapLimits.GetDeclaration();

		private ExpressionInfo m_minimumX;

		private ExpressionInfo m_minimumY;

		private ExpressionInfo m_maximumX;

		private ExpressionInfo m_maximumY;

		private ExpressionInfo m_limitToData;

		internal ExpressionInfo MinimumX
		{
			get
			{
				return this.m_minimumX;
			}
			set
			{
				this.m_minimumX = value;
			}
		}

		internal ExpressionInfo MinimumY
		{
			get
			{
				return this.m_minimumY;
			}
			set
			{
				this.m_minimumY = value;
			}
		}

		internal ExpressionInfo MaximumX
		{
			get
			{
				return this.m_maximumX;
			}
			set
			{
				this.m_maximumX = value;
			}
		}

		internal ExpressionInfo MaximumY
		{
			get
			{
				return this.m_maximumY;
			}
			set
			{
				this.m_maximumY = value;
			}
		}

		internal ExpressionInfo LimitToData
		{
			get
			{
				return this.m_limitToData;
			}
			set
			{
				this.m_limitToData = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapLimitsExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal MapLimits()
		{
		}

		internal MapLimits(Map map)
		{
			this.m_map = map;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLimitsStart();
			if (this.m_minimumX != null)
			{
				this.m_minimumX.Initialize("MinimumX", context);
				context.ExprHostBuilder.MapLimitsMinimumX(this.m_minimumX);
			}
			if (this.m_minimumY != null)
			{
				this.m_minimumY.Initialize("MinimumY", context);
				context.ExprHostBuilder.MapLimitsMinimumY(this.m_minimumY);
			}
			if (this.m_maximumX != null)
			{
				this.m_maximumX.Initialize("MaximumX", context);
				context.ExprHostBuilder.MapLimitsMaximumX(this.m_maximumX);
			}
			if (this.m_maximumY != null)
			{
				this.m_maximumY.Initialize("MaximumY", context);
				context.ExprHostBuilder.MapLimitsMaximumY(this.m_maximumY);
			}
			if (this.m_limitToData != null)
			{
				this.m_limitToData.Initialize("LimitToData", context);
				context.ExprHostBuilder.MapLimitsLimitToData(this.m_limitToData);
			}
			context.ExprHostBuilder.MapLimitsEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapLimits mapLimits = (MapLimits)base.MemberwiseClone();
			mapLimits.m_map = context.CurrentMapClone;
			if (this.m_minimumX != null)
			{
				mapLimits.m_minimumX = (ExpressionInfo)this.m_minimumX.PublishClone(context);
			}
			if (this.m_minimumY != null)
			{
				mapLimits.m_minimumY = (ExpressionInfo)this.m_minimumY.PublishClone(context);
			}
			if (this.m_maximumX != null)
			{
				mapLimits.m_maximumX = (ExpressionInfo)this.m_maximumX.PublishClone(context);
			}
			if (this.m_maximumY != null)
			{
				mapLimits.m_maximumY = (ExpressionInfo)this.m_maximumY.PublishClone(context);
			}
			if (this.m_limitToData != null)
			{
				mapLimits.m_limitToData = (ExpressionInfo)this.m_limitToData.PublishClone(context);
			}
			return mapLimits;
		}

		internal void SetExprHost(MapLimitsExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MinimumX, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinimumY, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumX, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumY, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LimitToData, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLimits, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapLimits.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.MinimumX:
					writer.Write(this.m_minimumX);
					break;
				case MemberName.MinimumY:
					writer.Write(this.m_minimumY);
					break;
				case MemberName.MaximumX:
					writer.Write(this.m_maximumX);
					break;
				case MemberName.MaximumY:
					writer.Write(this.m_maximumY);
					break;
				case MemberName.LimitToData:
					writer.Write(this.m_limitToData);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapLimits.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.MinimumX:
					this.m_minimumX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinimumY:
					this.m_minimumY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumX:
					this.m_maximumX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumY:
					this.m_maximumY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LimitToData:
					this.m_limitToData = (ExpressionInfo)reader.ReadRIFObject();
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
			if (memberReferencesCollection.TryGetValue(MapLimits.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLimits;
		}

		internal double EvaluateMinimumX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsMinimumXExpression(this, this.m_map.Name);
		}

		internal double EvaluateMinimumY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsMinimumYExpression(this, this.m_map.Name);
		}

		internal double EvaluateMaximumX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsMaximumXExpression(this, this.m_map.Name);
		}

		internal double EvaluateMaximumY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsMaximumYExpression(this, this.m_map.Name);
		}

		internal bool EvaluateLimitToData(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsLimitToDataExpression(this, this.m_map.Name);
		}
	}
}
