using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Diagnostics;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ReportItem : ReportElement
	{
		protected bool m_isListContentsRectangle;

		protected bool m_inSubtotal;

		protected string m_definitionPath;

		protected ReportStringProperty m_toolTip;

		protected ReportStringProperty m_bookmark;

		protected ReportStringProperty m_documentMapLabel;

		protected ReportItemInstance m_instance;

		private bool m_criGeneratedInstanceEvaluated;

		protected ReportBoolProperty m_startHidden;

		private Visibility m_visibility;

		private CustomPropertyCollection m_customProperties;

		private bool m_customPropertiesReady;

		private ReportSize m_cachedTop;

		private ReportSize m_cachedLeft;

		protected ReportSize m_cachedHeight;

		protected ReportSize m_cachedWidth;

		public override string DefinitionPath
		{
			get
			{
				return this.m_definitionPath;
			}
		}

		public virtual string Name
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return base.m_renderReportItem.Name;
				}
				return base.ReportItemDef.Name;
			}
		}

		public override string ID
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (!this.m_inSubtotal && !this.m_isListContentsRectangle)
					{
						return base.m_renderReportItem.ID;
					}
					return this.DefinitionPath;
				}
				return base.ReportItemDef.RenderingModelID;
			}
		}

		public override Style Style
		{
			get
			{
				if (base.m_isOldSnapshot && this.m_isListContentsRectangle)
				{
					return new Style(this, base.m_renderingContext);
				}
				return base.Style;
			}
		}

		public virtual int LinkToChild
		{
			get
			{
				return -1;
			}
		}

		public virtual ReportSize Height
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (this.m_cachedHeight == null)
					{
						this.m_cachedHeight = new ReportSize(this.RenderReportItem.Height);
					}
					return this.m_cachedHeight;
				}
				if (base.ReportItemDef.HeightForRendering == null)
				{
					base.ReportItemDef.HeightForRendering = new ReportSize(base.ReportItemDef.Height, base.ReportItemDef.HeightValue);
				}
				return base.ReportItemDef.HeightForRendering;
			}
		}

		public virtual ReportSize Width
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (this.m_cachedWidth == null)
					{
						this.m_cachedWidth = new ReportSize(this.RenderReportItem.Width);
					}
					return this.m_cachedWidth;
				}
				if (base.ReportItemDef.WidthForRendering == null)
				{
					base.ReportItemDef.WidthForRendering = new ReportSize(base.ReportItemDef.Width, base.ReportItemDef.WidthValue);
				}
				return base.ReportItemDef.WidthForRendering;
			}
		}

		public virtual ReportSize Top
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (this.m_cachedTop == null)
					{
						this.m_cachedTop = new ReportSize(this.RenderReportItem.Top);
					}
					return this.m_cachedTop;
				}
				if (base.ReportItemDef.TopForRendering == null)
				{
					base.ReportItemDef.TopForRendering = new ReportSize(base.ReportItemDef.Top, base.ReportItemDef.TopValue);
				}
				return base.ReportItemDef.TopForRendering;
			}
		}

		public virtual ReportSize Left
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (this.m_cachedLeft == null)
					{
						this.m_cachedLeft = new ReportSize(this.RenderReportItem.Left);
					}
					return this.m_cachedLeft;
				}
				if (base.ReportItemDef.LeftForRendering == null)
				{
					base.ReportItemDef.LeftForRendering = new ReportSize(base.ReportItemDef.Left, base.ReportItemDef.LeftValue);
				}
				return base.ReportItemDef.LeftForRendering;
			}
		}

		public int ZIndex
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (this.m_isListContentsRectangle)
					{
						return 0;
					}
					return this.RenderReportItem.ZIndex;
				}
				return base.ReportItemDef.ZIndex;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_toolTip = new ReportStringProperty(this.RenderReportItem.ReportItemDef.ToolTip);
					}
					else
					{
						this.m_toolTip = new ReportStringProperty(base.m_reportItemDef.ToolTip);
					}
				}
				return this.m_toolTip;
			}
		}

		public ReportStringProperty Bookmark
		{
			get
			{
				if (this.m_bookmark == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_bookmark = new ReportStringProperty(this.RenderReportItem.ReportItemDef.Bookmark);
					}
					else
					{
						this.m_bookmark = new ReportStringProperty(base.m_reportItemDef.Bookmark);
					}
				}
				return this.m_bookmark;
			}
		}

		public ReportStringProperty DocumentMapLabel
		{
			get
			{
				if (this.m_documentMapLabel == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_documentMapLabel = new ReportStringProperty(this.RenderReportItem.ReportItemDef.Label);
					}
					else
					{
						this.m_documentMapLabel = new ReportStringProperty(base.m_reportItemDef.DocumentMapLabel);
					}
				}
				return this.m_documentMapLabel;
			}
		}

		public string RepeatWith
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.RenderReportItem.ReportItemDef.RepeatWith;
				}
				return base.ReportItemDef.RepeatWith;
			}
		}

		public bool RepeatedSibling
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.RenderReportItem.RepeatedSibling;
				}
				return base.ReportItemDef.RepeatedSibling;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				this.PrepareCustomProperties();
				return this.m_customProperties;
			}
		}

		public string DataElementName
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.RenderReportItem.DataElementName;
				}
				return base.ReportItemDef.DataElementName;
			}
		}

		public virtual DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return (DataElementOutputTypes)this.RenderReportItem.DataElementOutput;
				}
				return base.ReportItemDef.DataElementOutput;
			}
		}

		public virtual Visibility Visibility
		{
			get
			{
				if (this.m_visibility == null)
				{
					if (base.m_isOldSnapshot && this.RenderReportItem.ReportItemDef.Visibility == null)
					{
						return null;
					}
					if (!base.m_isOldSnapshot && base.m_reportItemDef.Visibility == null)
					{
						return null;
					}
					this.m_visibility = new ReportItemVisibility(this);
				}
				return this.m_visibility;
			}
		}

		internal bool InSubtotal
		{
			get
			{
				return this.m_inSubtotal;
			}
		}

		internal override ReportElementInstance ReportElementInstance
		{
			get
			{
				return this.Instance;
			}
		}

		public new ReportItemInstance Instance
		{
			get
			{
				if (base.m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				ReportItemInstance orCreateInstance = this.GetOrCreateInstance();
				this.CriEvaluateInstance();
				return orCreateInstance;
			}
		}

		internal override string InstanceUniqueName
		{
			get
			{
				if (this.Instance != null)
				{
					return this.Instance.UniqueName;
				}
				return null;
			}
		}

		internal ReportItem(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, reportItemDef, renderingContext)
		{
			this.m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(parentDefinitionPath, indexIntoParentCollectionDef);
			base.m_reportItemDef.ROMScopeInstance = this.ReportScope.ReportScopeInstance;
		}

		internal ReportItem(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext)
			: base(parentDefinitionPath, renderReportItem, renderingContext)
		{
			this.m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(parentDefinitionPath, indexIntoParentCollectionDef);
			this.m_inSubtotal = inSubtotal;
		}

		internal ReportItem(IDefinitionPath parentDefinitionPath, bool inSubtotal, RenderingContext renderingContext)
			: base(parentDefinitionPath, renderingContext)
		{
			this.m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(parentDefinitionPath, 0);
			this.m_inSubtotal = inSubtotal;
			this.m_isListContentsRectangle = true;
		}

		internal abstract ReportItemInstance GetOrCreateInstance();

		internal void SetCachedWidth(double sizeDelta)
		{
			if (base.m_isOldSnapshot)
			{
				double sizeInMM = this.RenderReportItem.Width.ToMillimeters() + sizeDelta;
				string size = sizeInMM.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				this.m_cachedWidth = new ReportSize(size, sizeInMM);
			}
		}

		internal void SetCachedHeight(double sizeDelta)
		{
			if (base.m_isOldSnapshot)
			{
				double sizeInMM = this.RenderReportItem.Height.ToMillimeters() + sizeDelta;
				string size = sizeInMM.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				this.m_cachedHeight = new ReportSize(size, sizeInMM);
			}
		}

		internal override void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			base.SetNewContext();
			this.m_criGeneratedInstanceEvaluated = false;
			if (base.m_reportItemDef != null)
			{
				base.m_reportItemDef.ResetVisibilityComputationCache();
			}
			this.m_customPropertiesReady = false;
		}

		internal override void SetNewContextChildren()
		{
		}

		internal void CriEvaluateInstance()
		{
			if (base.CriOwner != null && base.CriGenerationPhase == CriGenerationPhases.None && !this.m_criGeneratedInstanceEvaluated)
			{
				this.m_criGeneratedInstanceEvaluated = true;
				base.CriOwner.EvaluateGeneratedReportItemInstance();
			}
		}

		internal virtual void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (!base.m_isOldSnapshot)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			this.SetNewContext();
			if (renderReportItem != null)
			{
				base.m_renderReportItem = renderReportItem;
				if (this.m_customProperties != null)
				{
					this.m_customProperties.UpdateCustomProperties(renderReportItem.CustomProperties);
				}
				if (base.m_style != null && !this.m_isListContentsRectangle)
				{
					base.m_style.UpdateStyleCache(renderReportItem);
				}
			}
		}

		public CustomProperty CreateCustomProperty()
		{
			if (base.CriGenerationPhase != CriGenerationPhases.Definition)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			this.PrepareCustomProperties();
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue dataValue = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue();
			dataValue.Name = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			dataValue.Value = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			if (base.m_reportItemDef.CustomProperties == null)
			{
				base.m_reportItemDef.CustomProperties = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList();
			}
			base.m_reportItemDef.CustomProperties.Add(dataValue);
			return this.CustomProperties.Add(base.RenderingContext, dataValue.Name, dataValue.Value);
		}

		internal virtual ReportItem ExposeAs(RenderingContext renderingContext)
		{
			return this;
		}

		internal virtual void ConstructReportItemDefinition()
		{
			Global.Tracer.Assert(false, "ConstructReportElementDefinition is not implemented on this type of report item: " + base.m_reportItemDef.ObjectType.ToString());
		}

		internal virtual void CompleteCriGeneratedInstanceEvaluation()
		{
			Global.Tracer.Assert(false, "CompleteCriGeneratedInstanceEvaluation is not implemented on this type of report item: " + base.m_reportItemDef.ObjectType.ToString());
		}

		internal void ConstructReportItemDefinitionImpl()
		{
			base.ConstructReportElementDefinitionImpl();
			ReportItemInstance instance = this.Instance;
			Global.Tracer.Assert(instance != null, "(instance != null)");
			if (instance.ToolTip != null)
			{
				base.ReportItemDef.ToolTip = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.ToolTip);
			}
			else
			{
				base.ReportItemDef.ToolTip = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			this.m_toolTip = null;
			if (instance.Bookmark != null)
			{
				base.ReportItemDef.Bookmark = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.Bookmark);
			}
			else
			{
				base.ReportItemDef.Bookmark = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			this.m_bookmark = null;
			if (instance.DocumentMapLabel != null)
			{
				base.ReportItemDef.DocumentMapLabel = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.DocumentMapLabel);
			}
			else
			{
				base.ReportItemDef.DocumentMapLabel = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			this.m_documentMapLabel = null;
			if (this.m_customProperties != null)
			{
				if (this.m_customProperties.Count == 0)
				{
					base.m_reportItemDef.CustomProperties = null;
					this.m_customProperties = null;
				}
				else
				{
					this.m_customProperties.ConstructCustomPropertyDefinitions(base.m_reportItemDef.CustomProperties);
				}
			}
		}

		private void PrepareCustomProperties()
		{
			if (base.m_isOldSnapshot)
			{
				if (this.m_customProperties == null && this.RenderReportItem.CustomProperties != null)
				{
					this.m_customProperties = new CustomPropertyCollection(base.m_renderingContext, this.RenderReportItem.CustomProperties);
				}
			}
			else
			{
				if (this.m_customProperties == null)
				{
					this.m_customProperties = new CustomPropertyCollection(this.ReportScope.ReportScopeInstance, base.m_renderingContext, this, base.m_reportItemDef, base.m_reportItemDef.ObjectType, base.m_reportItemDef.Name);
				}
				else if (!this.m_customPropertiesReady)
				{
					this.m_customProperties.UpdateCustomProperties(this.ReportScope.ReportScopeInstance, base.m_reportItemDef, base.m_renderingContext.OdpContext, base.m_reportItemDef.ObjectType, base.m_reportItemDef.Name);
					this.CriEvaluateInstance();
				}
				this.m_customPropertiesReady = true;
			}
		}

		internal static int StringToInt(string intAsString)
		{
			int result = -1;
			if (int.TryParse(intAsString, NumberStyles.None, (IFormatProvider)CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return -1;
		}

		internal static ReportItem CreateItem(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef, RenderingContext renderingContext)
		{
			ReportItem reportItem = null;
			switch (reportItemDef.ObjectType)
			{
			case ObjectType.Textbox:
				reportItem = new TextBox(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox)reportItemDef, renderingContext);
				break;
			case ObjectType.Rectangle:
				reportItem = new Rectangle(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)reportItemDef, renderingContext);
				break;
			case ObjectType.Image:
				reportItem = new Image(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.Image)reportItemDef, renderingContext);
				break;
			case ObjectType.Line:
				reportItem = new Line(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.Line)reportItemDef, renderingContext);
				break;
			case ObjectType.Subreport:
				reportItem = new SubReport(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport)reportItemDef, renderingContext);
				break;
			case ObjectType.Tablix:
				reportItem = new Tablix(parentDefinitionPath, indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix)reportItemDef, renderingContext);
				break;
			case ObjectType.Chart:
				reportItem = new Chart(parentDefinitionPath, indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.Chart)reportItemDef, renderingContext);
				break;
			case ObjectType.GaugePanel:
				reportItem = new GaugePanel(parentDefinitionPath, indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel)reportItemDef, renderingContext);
				break;
			case ObjectType.CustomReportItem:
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem customReportItem = (AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem)reportItemDef;
				reportItem = new CustomReportItem(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, customReportItem, renderingContext);
				if (!((CustomReportItem)reportItem).Initialize(renderingContext))
				{
					reportItem = ReportItem.CreateItem(reportScope, parentDefinitionPath, customReportItem.AltReportItemIndexInParentCollectionDef, customReportItem.AltReportItem, renderingContext);
					reportItem.ReportItemDef.RepeatedSibling = customReportItem.RepeatedSibling;
					reportItem.ReportItemDef.RepeatWith = customReportItem.RepeatWith;
					ReportItem.ProcessAlternateCustomReportItem(customReportItem, reportItem, renderingContext);
				}
				break;
			}
			case ObjectType.Map:
				reportItem = new Map(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.Map)reportItemDef, renderingContext);
				break;
			}
			return reportItem;
		}

		internal static ReportItem CreateShim(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext)
		{
			ReportItem result = null;
			if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.TextBox)
			{
				result = new TextBox(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.TextBox)renderReportItem, renderingContext);
			}
			else if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.Rectangle)
			{
				result = new Rectangle(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.Rectangle)renderReportItem, renderingContext);
			}
			else if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.Image)
			{
				result = new Image(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.Image)renderReportItem, renderingContext);
			}
			else if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.List)
			{
				result = new Tablix(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.List)renderReportItem, renderingContext);
			}
			else if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.Table)
			{
				result = new Tablix(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.Table)renderReportItem, renderingContext);
			}
			else if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.Matrix)
			{
				result = new Tablix(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.Matrix)renderReportItem, renderingContext);
			}
			else if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.Chart)
			{
				result = new Chart(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.Chart)renderReportItem, renderingContext);
			}
			else if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.CustomReportItem)
			{
				result = new CustomReportItem(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.CustomReportItem)renderReportItem, renderingContext);
			}
			else if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.SubReport)
			{
				result = new SubReport(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.SubReport)renderReportItem, renderingContext);
			}
			else if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.Line)
			{
				result = new Line(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (AspNetCore.ReportingServices.ReportRendering.Line)renderReportItem, renderingContext);
			}
			return result;
		}

		private static void ProcessAlternateCustomReportItem(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem criDef, ReportItem reportItem, RenderingContext renderingContext)
		{
			if (!criDef.ExplicitlyDefinedAltReportItem)
			{
				string text = null;
				Global.Tracer.Assert(renderingContext.OdpContext.ExtFactory != null, "ExtFactory != null.");
				if (!renderingContext.OdpContext.ExtFactory.IsRegisteredCustomReportItemExtension(criDef.Type))
				{
					renderingContext.OdpContext.TopLevelContext.ErrorContext.Register(ProcessingErrorCode.rsCRIControlNotInstalled, Severity.Warning, ObjectType.CustomReportItem, criDef.Name, criDef.Type);
					text = "The '{1}.{0}' extension is not present in the configuration file: The element '{2}' will render the AltReportItem, which is not defined. Therefore, it shows an empty space.";
				}
				else
				{
					renderingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIControlFailedToLoad, Severity.Warning, ObjectType.CustomReportItem, criDef.Name, criDef.Type);
					text = "The '{1}.{0}' extension failed to load: The element '{2}' will render the AltReportItem, which is not defined. Therefore, it shows an empty space.";
				}
				Global.Tracer.Trace(TraceLevel.Verbose, text, criDef.Name, criDef.Type, reportItem.Name);
			}
		}
	}
}
