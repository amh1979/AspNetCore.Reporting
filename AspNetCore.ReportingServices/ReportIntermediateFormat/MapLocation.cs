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
	internal sealed class MapLocation : IPersistable
	{
		[NonSerialized]
		private MapLocationExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapLocation.GetDeclaration();

		private ExpressionInfo m_left;

		private ExpressionInfo m_top;

		private ExpressionInfo m_unit;

		internal ExpressionInfo Left
		{
			get
			{
				return this.m_left;
			}
			set
			{
				this.m_left = value;
			}
		}

		internal ExpressionInfo Top
		{
			get
			{
				return this.m_top;
			}
			set
			{
				this.m_top = value;
			}
		}

		internal ExpressionInfo Unit
		{
			get
			{
				return this.m_unit;
			}
			set
			{
				this.m_unit = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapLocationExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal MapLocation()
		{
		}

		internal MapLocation(Map map)
		{
			this.m_map = map;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLocationStart();
			if (this.m_left != null)
			{
				this.m_left.Initialize("Left", context);
				context.ExprHostBuilder.MapLocationLeft(this.m_left);
			}
			if (this.m_top != null)
			{
				this.m_top.Initialize("Top", context);
				context.ExprHostBuilder.MapLocationTop(this.m_top);
			}
			if (this.m_unit != null)
			{
				this.m_unit.Initialize("Unit", context);
				context.ExprHostBuilder.MapLocationUnit(this.m_unit);
			}
			context.ExprHostBuilder.MapLocationEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapLocation mapLocation = (MapLocation)base.MemberwiseClone();
			mapLocation.m_map = context.CurrentMapClone;
			if (this.m_left != null)
			{
				mapLocation.m_left = (ExpressionInfo)this.m_left.PublishClone(context);
			}
			if (this.m_top != null)
			{
				mapLocation.m_top = (ExpressionInfo)this.m_top.PublishClone(context);
			}
			if (this.m_unit != null)
			{
				mapLocation.m_unit = (ExpressionInfo)this.m_unit.PublishClone(context);
			}
			return mapLocation;
		}

		internal void SetExprHost(MapLocationExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Left, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Top, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Unit, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLocation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapLocation.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.Left:
					writer.Write(this.m_left);
					break;
				case MemberName.Top:
					writer.Write(this.m_top);
					break;
				case MemberName.Unit:
					writer.Write(this.m_unit);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapLocation.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.Left:
					this.m_left = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Top:
					this.m_top = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Unit:
					this.m_unit = (ExpressionInfo)reader.ReadRIFObject();
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
			if (memberReferencesCollection.TryGetValue(MapLocation.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLocation;
		}

		internal double EvaluateLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLocationLeftExpression(this, this.m_map.Name);
		}

		internal double EvaluateTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLocationTopExpression(this, this.m_map.Name);
		}

		internal Unit EvaluateUnit(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return EnumTranslator.TranslateUnit(context.ReportRuntime.EvaluateMapLocationUnitExpression(this, this.m_map.Name), context.ReportRuntime);
		}
	}
}
