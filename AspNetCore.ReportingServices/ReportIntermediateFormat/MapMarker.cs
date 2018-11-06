using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
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
	internal sealed class MapMarker : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private MapMarkerExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapMarker.GetDeclaration();

		private ExpressionInfo m_mapMarkerStyle;

		private MapMarkerImage m_mapMarkerImage;

		internal ExpressionInfo MapMarkerStyle
		{
			get
			{
				return this.m_mapMarkerStyle;
			}
			set
			{
				this.m_mapMarkerStyle = value;
			}
		}

		internal MapMarkerImage MapMarkerImage
		{
			get
			{
				return this.m_mapMarkerImage;
			}
			set
			{
				this.m_mapMarkerImage = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapMarkerExprHost ExprHost
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

		internal MapMarker()
		{
		}

		internal MapMarker(Map map)
		{
			this.m_map = map;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapMarkerInCollectionStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			this.InnerInitialize(context);
			this.m_exprHostID = context.ExprHostBuilder.MapMarkerInCollectionEnd();
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapMarkerStart();
			this.InnerInitialize(context);
			context.ExprHostBuilder.MapMarkerEnd();
		}

		private void InnerInitialize(InitializationContext context)
		{
			if (this.m_mapMarkerStyle != null)
			{
				this.m_mapMarkerStyle.Initialize("MapMarkerStyle", context);
				context.ExprHostBuilder.MapMarkerMapMarkerStyle(this.m_mapMarkerStyle);
			}
			if (this.m_mapMarkerImage != null)
			{
				this.m_mapMarkerImage.Initialize(context);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapMarker mapMarker = (MapMarker)base.MemberwiseClone();
			mapMarker.m_map = context.CurrentMapClone;
			if (this.m_mapMarkerStyle != null)
			{
				mapMarker.m_mapMarkerStyle = (ExpressionInfo)this.m_mapMarkerStyle.PublishClone(context);
			}
			if (this.m_mapMarkerImage != null)
			{
				mapMarker.m_mapMarkerImage = (MapMarkerImage)this.m_mapMarkerImage.PublishClone(context);
			}
			return mapMarker;
		}

		internal void SetExprHost(MapMarkerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_mapMarkerImage != null && this.ExprHost.MapMarkerImageHost != null)
			{
				this.m_mapMarkerImage.SetExprHost(this.ExprHost.MapMarkerImageHost, reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapMarkerStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapMarkerlImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerImage));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarker, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapMarker.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.MapMarkerStyle:
					writer.Write(this.m_mapMarkerStyle);
					break;
				case MemberName.MapMarkerlImage:
					writer.Write(this.m_mapMarkerImage);
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
			reader.RegisterDeclaration(MapMarker.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.MapMarkerStyle:
					this.m_mapMarkerStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapMarkerlImage:
					this.m_mapMarkerImage = (MapMarkerImage)reader.ReadRIFObject();
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
			if (memberReferencesCollection.TryGetValue(MapMarker.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarker;
		}

		internal MapMarkerStyle EvaluateMapMarkerStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapMarkerStyle(context.ReportRuntime.EvaluateMapMarkerMapMarkerStyleExpression(this, this.m_map.Name), context.ReportRuntime);
		}
	}
}
