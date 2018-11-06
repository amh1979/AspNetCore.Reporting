using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
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
	internal abstract class ReportItem : IDOwner, IStyleContainer, IComparable, IPersistable, ICustomPropertiesHolder, IVisibilityOwner, IReferenceable, IStaticReferenceable
	{
		internal enum DataElementStyles
		{
			Attribute,
			Element,
			Auto
		}

		private const string ZeroSize = "0mm";

		private const int OverlapDetectionRounding = 1;

		protected string m_name;

		protected Style m_styleClass;

		protected string m_top;

		protected double m_topValue;

		protected string m_left;

		protected double m_leftValue;

		protected string m_height;

		protected double m_heightValue;

		protected string m_width;

		protected double m_widthValue;

		protected int m_zIndex;

		protected ExpressionInfo m_toolTip;

		protected Visibility m_visibility;

		protected ExpressionInfo m_documentMapLabel;

		protected ExpressionInfo m_bookmark;

		protected bool m_repeatedSibling;

		protected bool m_isFullSize;

		private int m_exprHostID = -1;

		protected string m_dataElementName;

		protected DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.Auto;

		protected DataValueList m_customProperties;

		protected bool m_computed;

		protected string m_repeatWith;

		[Reference]
		private IVisibilityOwner m_containingDynamicVisibility;

		[Reference]
		private IVisibilityOwner m_containingDynamicRowVisibility;

		[Reference]
		private IVisibilityOwner m_containingDynamicColumnVisibility;

		[NonSerialized]
		protected ReportItem m_parent;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ReportItem.GetDeclaration();

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		[NonSerialized]
		protected bool m_softPageBreak;

		[NonSerialized]
		protected bool m_shareMyLastPage = true;

		[NonSerialized]
		protected bool m_startHidden;

		[NonSerialized]
		protected double m_topInPage;

		[NonSerialized]
		protected double m_bottomInPage;

		[NonSerialized]
		private AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.PageTextboxes m_repeatedSiblingTextboxes;

		[NonSerialized]
		private int m_staticRefId = -2147483648;

		[NonSerialized]
		private IReportScopeInstance m_romScopeInstance;

		[NonSerialized]
		private bool m_cachedHiddenValue;

		[NonSerialized]
		private bool m_cachedDeepHiddenValue;

		[NonSerialized]
		private bool m_cachedStartHiddenValue;

		[NonSerialized]
		private bool m_hasCachedHiddenValue;

		[NonSerialized]
		private bool m_hasCachedDeepHiddenValue;

		[NonSerialized]
		private bool m_hasCachedStartHiddenValue;

		[NonSerialized]
		private List<InstancePathItem> m_visibilityCacheLastInstancePath;

		[NonSerialized]
		protected StyleProperties m_sharedStyleProperties;

		[NonSerialized]
		protected bool m_noNonSharedStyleProps;

		[NonSerialized]
		protected ReportSize m_heightForRendering;

		[NonSerialized]
		protected ReportSize m_widthForRendering;

		[NonSerialized]
		protected ReportSize m_topForRendering;

		[NonSerialized]
		protected ReportSize m_leftForRendering;

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		public Style StyleClass
		{
			get
			{
				return this.m_styleClass;
			}
			set
			{
				this.m_styleClass = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath
		{
			get
			{
				return this;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IStyleContainer.ObjectType
		{
			get
			{
				return this.ObjectType;
			}
		}

		string IStyleContainer.Name
		{
			get
			{
				return this.Name;
			}
		}

		internal string Top
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

		internal double TopValue
		{
			get
			{
				return this.m_topValue;
			}
			set
			{
				this.m_topValue = value;
			}
		}

		internal string Left
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

		internal double LeftValue
		{
			get
			{
				return this.m_leftValue;
			}
			set
			{
				this.m_leftValue = value;
			}
		}

		internal string Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		internal double HeightValue
		{
			get
			{
				return this.m_heightValue;
			}
			set
			{
				this.m_heightValue = value;
			}
		}

		internal string Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		internal double WidthValue
		{
			get
			{
				return this.m_widthValue;
			}
			set
			{
				this.m_widthValue = value;
			}
		}

		internal double AbsoluteTopValue
		{
			get
			{
				if (this.m_heightValue < 0.0)
				{
					return ReportItem.RoundSize(this.m_topValue + this.m_heightValue);
				}
				return ReportItem.RoundSize(this.m_topValue);
			}
		}

		internal double AbsoluteLeftValue
		{
			get
			{
				if (this.m_widthValue < 0.0)
				{
					return ReportItem.RoundSize(this.m_leftValue + this.m_widthValue);
				}
				return ReportItem.RoundSize(this.m_leftValue);
			}
		}

		internal double AbsoluteBottomValue
		{
			get
			{
				if (this.m_heightValue < 0.0)
				{
					return ReportItem.RoundSize(this.m_topValue);
				}
				return ReportItem.RoundSize(this.m_topValue + this.m_heightValue);
			}
		}

		internal double AbsoluteRightValue
		{
			get
			{
				if (this.m_widthValue < 0.0)
				{
					return ReportItem.RoundSize(this.m_leftValue);
				}
				return ReportItem.RoundSize(this.m_leftValue + this.m_widthValue);
			}
		}

		internal int ZIndex
		{
			get
			{
				return this.m_zIndex;
			}
			set
			{
				this.m_zIndex = value;
			}
		}

		internal ExpressionInfo ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
			set
			{
				this.m_toolTip = value;
			}
		}

		public Visibility Visibility
		{
			get
			{
				return this.m_visibility;
			}
			set
			{
				this.m_visibility = value;
			}
		}

		internal ExpressionInfo DocumentMapLabel
		{
			get
			{
				return this.m_documentMapLabel;
			}
			set
			{
				this.m_documentMapLabel = value;
			}
		}

		internal ExpressionInfo Bookmark
		{
			get
			{
				return this.m_bookmark;
			}
			set
			{
				this.m_bookmark = value;
			}
		}

		internal bool RepeatedSibling
		{
			get
			{
				return this.m_repeatedSibling;
			}
			set
			{
				this.m_repeatedSibling = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

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

		internal virtual string DataElementNameDefault
		{
			get
			{
				return this.m_name;
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

		internal ReportItem Parent
		{
			get
			{
				return this.m_parent;
			}
			set
			{
				this.m_parent = value;
			}
		}

		internal bool Computed
		{
			get
			{
				return this.m_computed;
			}
			set
			{
				this.m_computed = value;
			}
		}

		internal virtual bool IsDataRegion
		{
			get
			{
				return false;
			}
		}

		internal string RepeatWith
		{
			get
			{
				return this.m_repeatWith;
			}
			set
			{
				this.m_repeatWith = value;
			}
		}

		internal ReportItemExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal virtual bool SoftPageBreak
		{
			get
			{
				return this.m_softPageBreak;
			}
			set
			{
				this.m_softPageBreak = value;
			}
		}

		internal virtual bool ShareMyLastPage
		{
			get
			{
				return this.m_shareMyLastPage;
			}
			set
			{
				this.m_shareMyLastPage = value;
			}
		}

		internal bool StartHidden
		{
			get
			{
				return this.m_startHidden;
			}
			set
			{
				this.m_startHidden = value;
			}
		}

		internal StyleProperties SharedStyleProperties
		{
			get
			{
				return this.m_sharedStyleProperties;
			}
			set
			{
				this.m_sharedStyleProperties = value;
			}
		}

		internal bool NoNonSharedStyleProps
		{
			get
			{
				return this.m_noNonSharedStyleProps;
			}
			set
			{
				this.m_noNonSharedStyleProps = value;
			}
		}

		internal ReportSize HeightForRendering
		{
			get
			{
				return this.m_heightForRendering;
			}
			set
			{
				this.m_heightForRendering = value;
			}
		}

		internal ReportSize WidthForRendering
		{
			get
			{
				return this.m_widthForRendering;
			}
			set
			{
				this.m_widthForRendering = value;
			}
		}

		internal ReportSize TopForRendering
		{
			get
			{
				return this.m_topForRendering;
			}
			set
			{
				this.m_topForRendering = value;
			}
		}

		internal ReportSize LeftForRendering
		{
			get
			{
				return this.m_leftForRendering;
			}
			set
			{
				this.m_leftForRendering = value;
			}
		}

		internal virtual DataElementOutputTypes DataElementOutputDefault
		{
			get
			{
				return DataElementOutputTypes.Output;
			}
		}

		internal double TopInStartPage
		{
			get
			{
				return this.m_topInPage;
			}
			set
			{
				this.m_topInPage = value;
			}
		}

		internal double BottomInEndPage
		{
			get
			{
				return this.m_bottomInPage;
			}
			set
			{
				this.m_bottomInPage = value;
			}
		}

		DataValueList ICustomPropertiesHolder.CustomProperties
		{
			get
			{
				return this.m_customProperties;
			}
		}

		IInstancePath ICustomPropertiesHolder.InstancePath
		{
			get
			{
				return this;
			}
		}

		internal DataValueList CustomProperties
		{
			get
			{
				return this.m_customProperties;
			}
			set
			{
				this.m_customProperties = value;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.PageTextboxes RepeatedSiblingTextboxes
		{
			get
			{
				return this.m_repeatedSiblingTextboxes;
			}
			set
			{
				this.m_repeatedSiblingTextboxes = value;
			}
		}

		internal abstract AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get;
		}

		public IReportScopeInstance ROMScopeInstance
		{
			get
			{
				return this.m_romScopeInstance;
			}
			set
			{
				this.m_romScopeInstance = value;
				if (this.IsVisibilityCacheInstancePathInvalid())
				{
					this.ResetVisibilityComputationCache();
				}
			}
		}

		public IVisibilityOwner ContainingDynamicVisibility
		{
			get
			{
				return this.m_containingDynamicVisibility;
			}
			set
			{
				this.m_containingDynamicVisibility = value;
			}
		}

		public IVisibilityOwner ContainingDynamicColumnVisibility
		{
			get
			{
				return this.m_containingDynamicColumnVisibility;
			}
			set
			{
				this.m_containingDynamicColumnVisibility = value;
			}
		}

		public IVisibilityOwner ContainingDynamicRowVisibility
		{
			get
			{
				return this.m_containingDynamicRowVisibility;
			}
			set
			{
				this.m_containingDynamicRowVisibility = value;
			}
		}

		public string SenderUniqueName
		{
			get
			{
				if (this.m_visibility != null)
				{
					TextBox toggleSender = this.m_visibility.ToggleSender;
					if (toggleSender != null)
					{
						return toggleSender.UniqueName;
					}
				}
				return null;
			}
		}

		int IStaticReferenceable.ID
		{
			get
			{
				return this.m_staticRefId;
			}
		}

		protected ReportItem(int id, ReportItem parent)
			: base(id)
		{
			this.m_parent = parent;
		}

		protected ReportItem(ReportItem parent)
		{
			this.m_parent = parent;
		}

		internal bool IsOrContainsDataRegionOrSubReport()
		{
			if (this.IsDataRegion)
			{
				return true;
			}
			if (this.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map)
			{
				return ((Map)this).ContainsMapDataRegion();
			}
			if (this.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Subreport)
			{
				return true;
			}
			if (this.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle)
			{
				return ((Rectangle)this).ContainsDataRegionOrSubReport();
			}
			return false;
		}

		public bool ComputeHidden(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction)
		{
			if (!this.CanUseCachedVisibilityData(this.m_hasCachedHiddenValue))
			{
				this.UpdateVisibilityDataCacheFlag(ref this.m_hasCachedHiddenValue);
				bool flag = default(bool);
				this.m_cachedHiddenValue = Visibility.ComputeHidden((IVisibilityOwner)this, renderingContext, direction, out flag);
				if (flag)
				{
					this.m_hasCachedDeepHiddenValue = true;
					this.m_cachedDeepHiddenValue = this.m_cachedHiddenValue;
				}
			}
			return this.m_cachedHiddenValue;
		}

		public bool ComputeDeepHidden(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction)
		{
			if (!this.CanUseCachedVisibilityData(this.m_hasCachedDeepHiddenValue))
			{
				bool hidden = this.ComputeHidden(renderingContext, direction);
				if (!this.m_hasCachedDeepHiddenValue)
				{
					this.m_hasCachedDeepHiddenValue = true;
					this.m_cachedDeepHiddenValue = Visibility.ComputeDeepHidden(hidden, this, direction, renderingContext);
				}
			}
			return this.m_cachedDeepHiddenValue;
		}

		public bool ComputeStartHidden(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			if (!this.CanUseCachedVisibilityData(this.m_hasCachedStartHiddenValue))
			{
				this.UpdateVisibilityDataCacheFlag(ref this.m_hasCachedStartHiddenValue);
				if (this.m_visibility == null || this.m_visibility.Hidden == null)
				{
					this.m_cachedStartHiddenValue = false;
				}
				else if (!this.m_visibility.Hidden.IsExpression)
				{
					this.m_cachedStartHiddenValue = this.m_visibility.Hidden.BoolValue;
				}
				else
				{
					this.m_cachedStartHiddenValue = this.EvaluateStartHidden(this.m_romScopeInstance, renderingContext.OdpContext);
				}
			}
			return this.m_cachedStartHiddenValue;
		}

		private bool CanUseCachedVisibilityData(bool cacheHasValue)
		{
			if (!cacheHasValue)
			{
				return false;
			}
			if ((this.m_romScopeInstance == null || this.m_romScopeInstance.IsNewContext) && this.IsVisibilityCacheInstancePathInvalid())
			{
				this.ResetVisibilityComputationCache();
				return false;
			}
			return true;
		}

		private bool IsVisibilityCacheInstancePathInvalid()
		{
			if (this.m_visibilityCacheLastInstancePath != null)
			{
				if (this.m_visibilityCacheLastInstancePath.Count > 0)
				{
					return !InstancePathItem.IsSamePath(this.InstancePath, this.m_visibilityCacheLastInstancePath);
				}
				return false;
			}
			return true;
		}

		private void UpdateVisibilityDataCacheFlag(ref bool cacheHasValue)
		{
			cacheHasValue = true;
			if (this.m_romScopeInstance != null && !this.m_romScopeInstance.IsNewContext)
			{
				return;
			}
			InstancePathItem.DeepCopyPath(this.InstancePath, ref this.m_visibilityCacheLastInstancePath);
		}

		internal void ResetVisibilityComputationCache()
		{
			this.m_hasCachedHiddenValue = false;
			this.m_hasCachedDeepHiddenValue = false;
			this.m_hasCachedStartHiddenValue = false;
		}

		internal virtual bool Initialize(InitializationContext context)
		{
			if (this.m_top == null)
			{
				this.m_top = "0mm";
				this.m_topValue = 0.0;
			}
			else
			{
				this.m_topValue = context.ValidateSize(ref this.m_top, "Top");
			}
			if (this.m_left == null)
			{
				this.m_left = "0mm";
				this.m_leftValue = 0.0;
			}
			else
			{
				this.m_leftValue = context.ValidateSize(ref this.m_left, "Left");
			}
			this.ValidateItemSizeAndBoundaries(context);
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
			if (this.m_documentMapLabel != null)
			{
				this.m_documentMapLabel.Initialize("Label", context);
				context.ExprHostBuilder.GenericLabel(this.m_documentMapLabel);
			}
			if (this.m_bookmark != null)
			{
				this.m_bookmark.Initialize("Bookmark", context);
				context.ExprHostBuilder.ReportItemBookmark(this.m_bookmark);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ReportItemToolTip(this.m_toolTip);
			}
			if (this.m_customProperties != null)
			{
				this.m_customProperties.Initialize(null, context);
			}
			this.DataRendererInitialize(context);
			return false;
		}

		internal virtual void TraverseScopes(IRIFScopeVisitor visitor)
		{
		}

		private void ValidateItemSizeAndBoundaries(InitializationContext context)
		{
			if (context.PublishingContext.PublishingContextKind != PublishingContextKind.DataShape)
			{
				if (this.m_parent != null)
				{
					bool flag = true;
					if (this.m_width == null)
					{
						if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablix) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
						{
							if (AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix == context.ObjectType)
							{
								this.m_width = "0mm";
								this.m_widthValue = 0.0;
								flag = false;
							}
							else if (AspNetCore.ReportingServices.ReportProcessing.ObjectType.PageHeader == context.ObjectType || AspNetCore.ReportingServices.ReportProcessing.ObjectType.PageFooter == context.ObjectType)
							{
								ReportSection reportSection = this.m_parent as ReportSection;
								this.m_widthValue = reportSection.PageSectionWidth;
								this.m_width = AspNetCore.ReportingServices.ReportPublishing.Converter.ConvertSize(this.m_widthValue);
							}
							else
							{
								this.m_widthValue = Math.Round(this.m_parent.m_widthValue - this.m_leftValue, 10);
								this.m_width = AspNetCore.ReportingServices.ReportPublishing.Converter.ConvertSize(this.m_widthValue);
							}
						}
						else
						{
							flag = false;
						}
					}
					if (flag)
					{
						this.m_widthValue = context.ValidateSize(this.m_width, "Width");
					}
					flag = true;
					if (this.m_height == null)
					{
						if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablix) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
						{
							if (AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix == context.ObjectType)
							{
								this.m_height = "0mm";
								this.m_heightValue = 0.0;
								flag = false;
							}
							else
							{
								this.m_heightValue = Math.Round(this.m_parent.m_heightValue - this.m_topValue, 10);
								this.m_height = AspNetCore.ReportingServices.ReportPublishing.Converter.ConvertSize(this.m_heightValue);
							}
						}
						else
						{
							flag = false;
						}
					}
					if (flag)
					{
						this.m_heightValue = context.ValidateSize(this.m_height, "Height");
					}
				}
				else
				{
					this.m_widthValue = context.ValidateSize(ref this.m_width, "Width");
					this.m_heightValue = context.ValidateSize(ref this.m_height, "Height");
				}
				if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablix) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
				{
					this.ValidateParentBoundaries(context, context.ObjectType, context.ObjectName);
				}
			}
		}

		private void ValidateParentBoundaries(InitializationContext context, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (this.m_parent != null && !(this.m_parent is Report) && !(this.m_parent is ReportSection))
			{
				if (objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Line)
				{
					if (this.AbsoluteTopValue < 0.0)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Top".ToLowerInvariant());
					}
					if (this.AbsoluteLeftValue < 0.0)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Left".ToLowerInvariant());
					}
				}
				if (this.AbsoluteBottomValue > ReportItem.RoundSize(this.m_parent.HeightValue))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Bottom".ToLowerInvariant());
				}
				if (this.AbsoluteRightValue > ReportItem.RoundSize(this.m_parent.WidthValue))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Right".ToLowerInvariant());
				}
			}
		}

		protected static double RoundSize(double size)
		{
			return Math.Round(Math.Round(size, 4), 1);
		}

		protected virtual void DataRendererInitialize(InitializationContext context)
		{
			AspNetCore.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref this.m_dataElementName, this.DataElementNameDefault, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
			if (this.m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				if (this.m_visibility != null && this.m_visibility.Hidden != null && this.m_visibility.Hidden.Type == ExpressionInfo.Types.Constant && this.m_visibility.Hidden.BoolValue && this.m_visibility.Toggle == null)
				{
					goto IL_008b;
				}
				if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InNonToggleableHiddenStaticTablixMember) != 0)
				{
					goto IL_008b;
				}
				this.m_dataElementOutput = this.DataElementOutputDefault;
			}
			return;
			IL_008b:
			this.m_dataElementOutput = DataElementOutputTypes.NoOutput;
		}

		internal virtual void CalculateSizes(double width, double height, InitializationContext context, bool overwrite)
		{
			if (overwrite)
			{
				this.m_top = "0mm";
				this.m_topValue = 0.0;
				this.m_left = "0mm";
				this.m_leftValue = 0.0;
			}
			if (this.m_width == null || (overwrite && this.m_widthValue != width))
			{
				this.m_width = width.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				this.m_widthValue = context.ValidateSize(ref this.m_width, "Width");
			}
			if (this.m_height == null || (overwrite && this.m_heightValue != height))
			{
				this.m_height = height.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				this.m_heightValue = context.ValidateSize(ref this.m_height, "Height");
			}
			this.ValidateParentBoundaries(context, this.ObjectType, this.Name);
		}

		internal void CalculateSizes(InitializationContext context, bool overwrite)
		{
			double width = this.m_widthValue;
			double height = this.m_heightValue;
			if (this.m_width == null)
			{
				width = Math.Round(Math.Max(0.0, this.m_parent.m_widthValue - this.m_leftValue), 10);
			}
			if (this.m_height == null)
			{
				height = Math.Round(Math.Max(0.0, this.m_parent.m_heightValue - this.m_topValue), 10);
			}
			this.CalculateSizes(width, height, context, overwrite);
		}

		internal virtual void InitializeRVDirectionDependentItems(InitializationContext context)
		{
		}

		internal virtual void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is ReportItem))
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
			ReportItem reportItem = (ReportItem)obj;
			if (this.m_topValue < reportItem.m_topValue)
			{
				return -1;
			}
			if (this.m_topValue > reportItem.m_topValue)
			{
				return 1;
			}
			if (this.m_leftValue < reportItem.m_leftValue)
			{
				return -1;
			}
			if (this.m_leftValue > reportItem.m_leftValue)
			{
				return 1;
			}
			return 0;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ReportItem reportItem = (ReportItem)base.PublishClone(context);
			reportItem.m_name = context.CreateUniqueReportItemName(this.m_name, base.m_isClone);
			if (this.m_styleClass != null)
			{
				reportItem.m_styleClass = (Style)this.m_styleClass.PublishClone(context);
			}
			if (this.m_top != null)
			{
				reportItem.m_top = (string)this.m_top.Clone();
			}
			if (this.m_left != null)
			{
				reportItem.m_left = (string)this.m_left.Clone();
			}
			if (this.m_height != null)
			{
				reportItem.m_height = (string)this.m_height.Clone();
			}
			if (this.m_width != null)
			{
				reportItem.m_width = (string)this.m_width.Clone();
			}
			if (this.m_toolTip != null)
			{
				reportItem.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_visibility != null)
			{
				reportItem.m_visibility = (Visibility)this.m_visibility.PublishClone(context, false);
			}
			reportItem.m_documentMapLabel = null;
			reportItem.m_bookmark = null;
			if (this.m_dataElementName != null)
			{
				reportItem.m_dataElementName = (string)this.m_dataElementName.Clone();
			}
			if (this.m_repeatWith != null)
			{
				context.AddReportItemWithRepeatWithToUpdate(reportItem);
				reportItem.m_repeatWith = (string)this.m_repeatWith.Clone();
			}
			if (this.m_customProperties != null)
			{
				reportItem.m_customProperties = new DataValueList(this.m_customProperties.Count);
				{
					foreach (DataValue customProperty in this.m_customProperties)
					{
						reportItem.m_customProperties.Add((DataValue)customProperty.PublishClone(context));
					}
					return reportItem;
				}
			}
			return reportItem;
		}

		internal override void SetupCriRenderItemDef(ReportItem reportItem)
		{
			base.SetupCriRenderItemDef(reportItem);
			reportItem.Name = this.Name + "." + reportItem.Name;
			reportItem.DataElementName = reportItem.Name;
			reportItem.DataElementOutput = this.DataElementOutput;
			reportItem.RepeatWith = this.RepeatWith;
			reportItem.RepeatedSibling = this.RepeatedSibling;
			reportItem.Top = this.Top;
			reportItem.TopValue = this.TopValue;
			reportItem.Left = this.Left;
			reportItem.LeftValue = this.LeftValue;
			reportItem.Height = this.Height;
			reportItem.HeightValue = this.HeightValue;
			reportItem.Width = this.Width;
			reportItem.WidthValue = this.WidthValue;
			reportItem.ZIndex = this.ZIndex;
			reportItem.Visibility = this.Visibility;
			reportItem.Computed = true;
		}

		internal void UpdateRepeatWithReference(AutomaticSubtotalContext context)
		{
			if (this.m_repeatWith != null)
			{
				this.m_repeatWith = context.GetNewReportItemName(this.m_repeatWith);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.Top, Token.String));
			list.Add(new MemberInfo(MemberName.TopValue, Token.Double));
			list.Add(new MemberInfo(MemberName.Left, Token.String));
			list.Add(new MemberInfo(MemberName.LeftValue, Token.Double));
			list.Add(new MemberInfo(MemberName.Height, Token.String));
			list.Add(new MemberInfo(MemberName.HeightValue, Token.Double));
			list.Add(new MemberInfo(MemberName.Width, Token.String));
			list.Add(new MemberInfo(MemberName.WidthValue, Token.Double));
			list.Add(new MemberInfo(MemberName.ZIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.Visibility, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Visibility));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Bookmark, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RepeatedSibling, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsFullSize, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.CustomProperties, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.Computed, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ContainingDynamicVisibility, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ContainingDynamicRowVisibility, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ContainingDynamicColumnVisibility, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.RepeatWith, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ReportItem.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.StyleClass:
					writer.Write(this.m_styleClass);
					break;
				case MemberName.Top:
					writer.Write(this.m_top);
					break;
				case MemberName.TopValue:
					writer.Write(this.m_topValue);
					break;
				case MemberName.Left:
					writer.Write(this.m_left);
					break;
				case MemberName.LeftValue:
					writer.Write(this.m_leftValue);
					break;
				case MemberName.Height:
					writer.Write(this.m_height);
					break;
				case MemberName.HeightValue:
					writer.Write(this.m_heightValue);
					break;
				case MemberName.Width:
					writer.Write(this.m_width);
					break;
				case MemberName.WidthValue:
					writer.Write(this.m_widthValue);
					break;
				case MemberName.ZIndex:
					writer.Write(this.m_zIndex);
					break;
				case MemberName.Visibility:
					writer.Write(this.m_visibility);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.Label:
					writer.Write(this.m_documentMapLabel);
					break;
				case MemberName.Bookmark:
					writer.Write(this.m_bookmark);
					break;
				case MemberName.RepeatedSibling:
					writer.Write(this.m_repeatedSibling);
					break;
				case MemberName.IsFullSize:
					writer.Write(this.m_isFullSize);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.DataElementName:
					writer.Write(this.m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)this.m_dataElementOutput);
					break;
				case MemberName.CustomProperties:
					writer.Write(this.m_customProperties);
					break;
				case MemberName.Computed:
					writer.Write(this.m_computed);
					break;
				case MemberName.ContainingDynamicVisibility:
					writer.WriteReference(this.m_containingDynamicVisibility);
					break;
				case MemberName.ContainingDynamicRowVisibility:
					writer.WriteReference(this.m_containingDynamicRowVisibility);
					break;
				case MemberName.ContainingDynamicColumnVisibility:
					writer.WriteReference(this.m_containingDynamicColumnVisibility);
					break;
				case MemberName.RepeatWith:
					writer.Write(this.m_repeatWith);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ReportItem.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.StyleClass:
					this.m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.Top:
					this.m_top = reader.ReadString();
					break;
				case MemberName.TopValue:
					this.m_topValue = reader.ReadDouble();
					break;
				case MemberName.Left:
					this.m_left = reader.ReadString();
					break;
				case MemberName.LeftValue:
					this.m_leftValue = reader.ReadDouble();
					break;
				case MemberName.Height:
					this.m_height = reader.ReadString();
					break;
				case MemberName.HeightValue:
					this.m_heightValue = reader.ReadDouble();
					break;
				case MemberName.Width:
					this.m_width = reader.ReadString();
					break;
				case MemberName.WidthValue:
					this.m_widthValue = reader.ReadDouble();
					break;
				case MemberName.ZIndex:
					this.m_zIndex = reader.ReadInt32();
					break;
				case MemberName.Visibility:
					this.m_visibility = (Visibility)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					this.m_documentMapLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Bookmark:
					this.m_bookmark = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RepeatedSibling:
					this.m_repeatedSibling = reader.ReadBoolean();
					break;
				case MemberName.IsFullSize:
					this.m_isFullSize = reader.ReadBoolean();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.DataElementName:
					this.m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					this.m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.CustomProperties:
					this.m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.Computed:
					this.m_computed = reader.ReadBoolean();
					break;
				case MemberName.ContainingDynamicVisibility:
					this.m_containingDynamicVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.ContainingDynamicRowVisibility:
					this.m_containingDynamicRowVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.ContainingDynamicColumnVisibility:
					this.m_containingDynamicColumnVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.RepeatWith:
					this.m_repeatWith = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ReportItem.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.ContainingDynamicVisibility:
					{
						IReferenceable referenceable2 = default(IReferenceable);
						if (referenceableItems.TryGetValue(item.RefID, out referenceable2))
						{
							this.m_containingDynamicVisibility = (referenceable2 as IVisibilityOwner);
						}
						break;
					}
					case MemberName.ContainingDynamicRowVisibility:
					{
						IReferenceable referenceable3 = default(IReferenceable);
						if (referenceableItems.TryGetValue(item.RefID, out referenceable3))
						{
							this.m_containingDynamicRowVisibility = (referenceable3 as IVisibilityOwner);
						}
						break;
					}
					case MemberName.ContainingDynamicColumnVisibility:
					{
						IReferenceable referenceable = default(IReferenceable);
						if (referenceableItems.TryGetValue(item.RefID, out referenceable))
						{
							this.m_containingDynamicColumnVisibility = (referenceable as IVisibilityOwner);
						}
						break;
					}
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem;
		}

		internal abstract void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel);

		protected void ReportItemSetExprHost(ReportItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_styleClass != null)
			{
				this.m_styleClass.SetStyleExprHost(this.m_exprHost);
			}
			if (this.m_exprHost.CustomPropertyHostsRemotable != null)
			{
				Global.Tracer.Assert(null != this.m_customProperties, "(null != m_customProperties)");
				this.m_customProperties.SetExprHost(this.m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal bool EvaluateStartHidden(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateStartHiddenExpression(this.Visibility, this.m_exprHost, this.ObjectType, this.m_name);
		}

		internal string EvaluateBookmark(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateReportItemBookmarkExpression(this);
		}

		internal string EvaluateDocumentMapLabel(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateReportItemDocumentMapLabelExpression(this);
		}

		internal string EvaluateToolTip(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateReportItemToolTipExpression(this);
		}

		void IStaticReferenceable.SetID(int id)
		{
			this.m_staticRefId = id;
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return this.GetObjectType();
		}
	}
}
