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
	internal sealed class MapMarkerImage : IPersistable
	{
		[NonSerialized]
		private MapMarkerImageExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapMarkerImage.GetDeclaration();

		private ExpressionInfo m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_mIMEType;

		private ExpressionInfo m_transparentColor;

		private ExpressionInfo m_resizeMode;

		internal ExpressionInfo Source
		{
			get
			{
				return this.m_source;
			}
			set
			{
				this.m_source = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal ExpressionInfo MIMEType
		{
			get
			{
				return this.m_mIMEType;
			}
			set
			{
				this.m_mIMEType = value;
			}
		}

		internal ExpressionInfo TransparentColor
		{
			get
			{
				return this.m_transparentColor;
			}
			set
			{
				this.m_transparentColor = value;
			}
		}

		internal ExpressionInfo ResizeMode
		{
			get
			{
				return this.m_resizeMode;
			}
			set
			{
				this.m_resizeMode = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapMarkerImageExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal MapMarkerImage()
		{
		}

		internal MapMarkerImage(Map map)
		{
			this.m_map = map;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapMarkerImageStart();
			if (this.m_source != null)
			{
				this.m_source.Initialize("Source", context);
				context.ExprHostBuilder.MapMarkerImageSource(this.m_source);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.MapMarkerImageValue(this.m_value);
			}
			if (this.m_mIMEType != null)
			{
				this.m_mIMEType.Initialize("MIMEType", context);
				context.ExprHostBuilder.MapMarkerImageMIMEType(this.m_mIMEType);
			}
			if (this.m_transparentColor != null)
			{
				this.m_transparentColor.Initialize("TransparentColor", context);
				context.ExprHostBuilder.MapMarkerImageTransparentColor(this.m_transparentColor);
			}
			if (this.m_resizeMode != null)
			{
				this.m_resizeMode.Initialize("ResizeMode", context);
				context.ExprHostBuilder.MapMarkerImageResizeMode(this.m_resizeMode);
			}
			context.ExprHostBuilder.MapMarkerImageEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapMarkerImage mapMarkerImage = (MapMarkerImage)base.MemberwiseClone();
			mapMarkerImage.m_map = context.CurrentMapClone;
			if (this.m_source != null)
			{
				mapMarkerImage.m_source = (ExpressionInfo)this.m_source.PublishClone(context);
			}
			if (this.m_value != null)
			{
				mapMarkerImage.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			if (this.m_mIMEType != null)
			{
				mapMarkerImage.m_mIMEType = (ExpressionInfo)this.m_mIMEType.PublishClone(context);
			}
			if (this.m_transparentColor != null)
			{
				mapMarkerImage.m_transparentColor = (ExpressionInfo)this.m_transparentColor.PublishClone(context);
			}
			if (this.m_resizeMode != null)
			{
				mapMarkerImage.m_resizeMode = (ExpressionInfo)this.m_resizeMode.PublishClone(context);
			}
			return mapMarkerImage;
		}

		internal void SetExprHost(MapMarkerImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Source, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MIMEType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransparentColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ResizeMode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapMarkerImage.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.Source:
					writer.Write(this.m_source);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.MIMEType:
					writer.Write(this.m_mIMEType);
					break;
				case MemberName.TransparentColor:
					writer.Write(this.m_transparentColor);
					break;
				case MemberName.ResizeMode:
					writer.Write(this.m_resizeMode);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapMarkerImage.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.Source:
					this.m_source = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MIMEType:
					this.m_mIMEType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TransparentColor:
					this.m_transparentColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ResizeMode:
					this.m_resizeMode = (ExpressionInfo)reader.ReadRIFObject();
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
			if (memberReferencesCollection.TryGetValue(MapMarkerImage.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerImage;
		}

		internal AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType EvaluateSource(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return EnumTranslator.TranslateImageSourceType(context.ReportRuntime.EvaluateMapMarkerImageSourceExpression(this, this.m_map.Name), context.ReportRuntime);
		}

		internal string EvaluateStringValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context, out bool errorOccurred)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapMarkerImageStringValueExpression(this, this.m_map.Name, out errorOccurred);
		}

		internal byte[] EvaluateBinaryValue(IReportScopeInstance romInstance, OnDemandProcessingContext context, out bool errOccurred)
		{
			context.SetupContext(this.m_map, romInstance);
			return context.ReportRuntime.EvaluateMapMarkerImageBinaryValueExpression(this, this.m_map.Name, out errOccurred);
		}

		internal string EvaluateMIMEType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapMarkerImageMIMETypeExpression(this, this.m_map.Name);
		}

		internal string EvaluateTransparentColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapMarkerImageTransparentColorExpression(this, this.m_map.Name);
		}

		internal MapResizeMode EvaluateResizeMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapResizeMode(context.ReportRuntime.EvaluateMapMarkerImageResizeModeExpression(this, this.m_map.Name), context.ReportRuntime);
		}
	}
}
