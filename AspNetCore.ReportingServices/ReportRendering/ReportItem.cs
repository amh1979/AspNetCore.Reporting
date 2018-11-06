using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal abstract class ReportItem : IDocumentMapEntry
	{
		private string m_uniqueName;

		protected int m_intUniqueName;

		private Style m_style;

		private CustomPropertyCollection m_customProperties;

		protected bool m_canClick;

		protected bool m_canEdit;

		protected bool m_canDrag;

		protected bool m_dropTarget;

		private MemberBase m_members;

		public string Name
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.DefinitionName;
				}
				return this.ReportItemDef.Name;
			}
		}

		public string ID
		{
			get
			{
				if (this.IsCustomControl)
				{
					return null;
				}
				if (this.ReportItemDef.RenderingModelID == null)
				{
					this.ReportItemDef.RenderingModelID = this.ReportItemDef.ID.ToString(CultureInfo.InvariantCulture);
				}
				return this.ReportItemDef.RenderingModelID;
			}
		}

		public bool InDocumentMap
		{
			get
			{
				if (this.IsCustomControl)
				{
					return null != this.Processing.Label;
				}
				return null != this.Label;
			}
		}

		public bool IsFullSize
		{
			get
			{
				if (this.IsCustomControl)
				{
					return false;
				}
				return this.ReportItemDef.IsFullSize;
			}
		}

		public string Label
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.Label;
				}
				string result = null;
				if (this.ReportItemDef.Label != null)
				{
					result = ((this.ReportItemDef.Label.Type != ExpressionInfo.Types.Constant) ? ((this.ReportItemInstance != null) ? this.InstanceInfo.Label : null) : this.ReportItemDef.Label.Value);
				}
				return result;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.Label = value;
			}
		}

		public virtual int LinkToChild
		{
			get
			{
				return -1;
			}
		}

		public string Bookmark
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.Bookmark;
				}
				string result = null;
				if (this.ReportItemDef.Bookmark != null)
				{
					result = ((this.ReportItemDef.Bookmark.Type != ExpressionInfo.Types.Constant) ? ((this.ReportItemInstance != null) ? this.InstanceInfo.Bookmark : null) : this.ReportItemDef.Bookmark.Value);
				}
				return result;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.Bookmark = value;
			}
		}

		public string UniqueName
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.m_uniqueName;
				}
				string text = this.m_uniqueName;
				if (this.m_uniqueName == null && this.m_intUniqueName != 0)
				{
					text = this.m_intUniqueName.ToString(CultureInfo.InvariantCulture);
					if (this.UseCache)
					{
						this.m_uniqueName = text;
					}
				}
				return text;
			}
		}

		public ReportSize Height
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.Height;
				}
				if (this.ReportItemDef.HeightForRendering == null)
				{
					this.ReportItemDef.HeightForRendering = new ReportSize(this.ReportItemDef.Height, this.ReportItemDef.HeightValue);
				}
				return this.ReportItemDef.HeightForRendering;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.Height = value;
			}
		}

		public ReportSize Width
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.Width;
				}
				if (this.ReportItemDef.WidthForRendering == null)
				{
					this.ReportItemDef.WidthForRendering = new ReportSize(this.ReportItemDef.Width, this.ReportItemDef.WidthValue);
				}
				return this.ReportItemDef.WidthForRendering;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.Width = value;
			}
		}

		public ReportSize Top
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.Top;
				}
				if (this.ReportItemDef.TopForRendering == null)
				{
					this.ReportItemDef.TopForRendering = new ReportSize(this.ReportItemDef.Top, this.ReportItemDef.TopValue);
				}
				return this.ReportItemDef.TopForRendering;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.Top = value;
			}
		}

		public ReportSize Left
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.Left;
				}
				if (this.ReportItemDef.LeftForRendering == null)
				{
					this.ReportItemDef.LeftForRendering = new ReportSize(this.ReportItemDef.Left, this.ReportItemDef.LeftValue);
				}
				return this.ReportItemDef.LeftForRendering;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.Left = value;
			}
		}

		public int ZIndex
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.ZIndex;
				}
				return this.ReportItemDef.ZIndex;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.ZIndex = value;
			}
		}

		public Style Style
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.m_style;
				}
				Style style = this.m_style;
				if (this.m_style == null)
				{
					style = new Style(this, this.ReportItemDef, this.RenderingContext);
					if (this.UseCache)
					{
						this.m_style = style;
					}
				}
				return style;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.m_style = value;
			}
		}

		public string Custom
		{
			get
			{
				if (this.IsCustomControl)
				{
					return null;
				}
				string text = this.ReportItemDef.Custom;
				if (text == null && this.CustomProperties != null)
				{
					CustomProperty customProperty = this.CustomProperties["Custom"];
					if (customProperty != null && customProperty.Value != null)
					{
						text = DataTypeUtility.ConvertToInvariantString(customProperty.Value);
					}
				}
				return text;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.m_customProperties;
				}
				CustomPropertyCollection customPropertyCollection = this.m_customProperties;
				if (this.m_customProperties == null && this.ReportItemDef.CustomProperties != null)
				{
					customPropertyCollection = ((this.ReportItemInstance == null) ? new CustomPropertyCollection(this.ReportItemDef.CustomProperties, null) : new CustomPropertyCollection(this.ReportItemDef.CustomProperties, this.InstanceInfo.CustomPropertyInstances));
					if (this.UseCache)
					{
						this.m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.m_customProperties = value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.Tooltip;
				}
				string result = null;
				if (this.ReportItemDef.ToolTip != null)
				{
					result = ((ExpressionInfo.Types.Constant != this.ReportItemDef.ToolTip.Type) ? ((this.ReportItemInstance != null) ? this.InstanceInfo.ToolTip : null) : this.ReportItemDef.ToolTip.Value);
				}
				return result;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.Tooltip = value;
			}
		}

		public virtual bool Hidden
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.Hidden;
				}
				if (this.ReportItemInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(this.ReportItemDef.Visibility);
				}
				if (this.ReportItemDef.Visibility == null)
				{
					return false;
				}
				if (this.RenderingContext != null && this.ReportItemDef.Visibility.Toggle != null)
				{
					return this.RenderingContext.IsItemHidden(this.ReportItemInstance.UniqueName, false);
				}
				if (RenderingContext.GetDefinitionHidden(this.ReportItemDef.Visibility))
				{
					return true;
				}
				return this.InstanceInfo.StartHidden;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.Hidden = value;
			}
		}

		public bool HasToggle
		{
			get
			{
				if (this.IsCustomControl)
				{
					return false;
				}
				return Visibility.HasToggle(this.ReportItemDef.Visibility);
			}
		}

		public string ToggleItem
		{
			get
			{
				if (this.IsCustomControl)
				{
					return null;
				}
				if (this.ReportItemDef.Visibility == null)
				{
					return null;
				}
				return this.ReportItemDef.Visibility.Toggle;
			}
		}

		internal TextBox ToggleParent
		{
			get
			{
				if (!this.HasToggle)
				{
					return null;
				}
				if (this.ReportItemInstance == null)
				{
					return null;
				}
				if (this.RenderingContext == null)
				{
					return null;
				}
				return this.RenderingContext.GetToggleParent(this.ReportItemInstance.UniqueName);
			}
		}

		public SharedHiddenState SharedHidden
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.SharedHidden;
				}
				return Visibility.GetSharedHidden(this.ReportItemDef.Visibility);
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.SharedHidden = value;
			}
		}

		public bool IsToggleChild
		{
			get
			{
				if (this.IsCustomControl)
				{
					return false;
				}
				if (this.ReportItemInstance == null)
				{
					return false;
				}
				return this.RenderingContext.IsToggleChild(this.ReportItemInstance.UniqueName);
			}
		}

		public bool RepeatedSibling
		{
			get
			{
				if (this.IsCustomControl)
				{
					return false;
				}
				return this.ReportItemDef.RepeatedSibling;
			}
		}

		public virtual object SharedRenderingInfo
		{
			get
			{
				if (this.IsCustomControl)
				{
					return null;
				}
				Global.Tracer.Assert(null != this.RenderingContext);
				return this.RenderingContext.RenderingInfoManager.SharedRenderingInfo[this.ReportItemDef.ID];
			}
			set
			{
				if (this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Global.Tracer.Assert(null != this.RenderingContext);
				this.RenderingContext.RenderingInfoManager.SharedRenderingInfo[this.ReportItemDef.ID] = value;
			}
		}

		public object RenderingInfo
		{
			get
			{
				if (this.IsCustomControl)
				{
					return null;
				}
				Global.Tracer.Assert(null != this.RenderingContext);
				if (this.RenderingContext.InPageSection)
				{
					return this.RenderingContext.RenderingInfoManager.PageSectionRenderingInfo[this.m_uniqueName];
				}
				if (this.m_intUniqueName == 0)
				{
					return null;
				}
				Global.Tracer.Assert(0 != this.m_intUniqueName);
				return this.RenderingContext.RenderingInfoManager.RenderingInfo[this.m_intUniqueName];
			}
			set
			{
				if (this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Global.Tracer.Assert(null != this.RenderingContext);
				if (this.RenderingContext.InPageSection)
				{
					this.RenderingContext.RenderingInfoManager.PageSectionRenderingInfo[this.m_uniqueName] = value;
				}
				else
				{
					if (this.m_intUniqueName == 0)
					{
						throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
					}
					Global.Tracer.Assert(0 != this.m_intUniqueName);
					this.RenderingContext.RenderingInfoManager.RenderingInfo[this.m_intUniqueName] = value;
				}
			}
		}

		public string DataElementName
		{
			get
			{
				if (this.IsCustomControl)
				{
					return null;
				}
				return this.ReportItemDef.DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (this.IsCustomControl)
				{
					return DataElementOutputTypes.NoOutput;
				}
				return this.ReportItemDef.DataElementOutput;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ReportItem ReportItemDef
		{
			get
			{
				return this.Rendering.m_reportItemDef;
			}
		}

		internal ReportItemInstance ReportItemInstance
		{
			get
			{
				return this.Rendering.m_reportItemInstance;
			}
		}

		internal ReportItemInstanceInfo InstanceInfo
		{
			get
			{
				if (this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				if (this.Rendering.m_reportItemInstance == null)
				{
					return null;
				}
				if (this.Rendering.m_reportItemInstanceInfo == null)
				{
					this.Rendering.m_reportItemInstanceInfo = this.Rendering.m_reportItemInstance.GetInstanceInfo(this.RenderingContext.ChunkManager, this.RenderingContext.InPageSection);
				}
				return this.Rendering.m_reportItemInstanceInfo;
			}
		}

		internal RenderingContext RenderingContext
		{
			get
			{
				if (this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.Rendering.m_renderingContext;
			}
		}

		internal MatrixHeadingInstance HeadingInstance
		{
			get
			{
				if (this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.Rendering.m_headingInstance;
			}
		}

		private ReportItemRendering Rendering
		{
			get
			{
				try
				{
					return (ReportItemRendering)this.m_members;
				}
				catch
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
			}
		}

		internal ReportItemProcessing Processing
		{
			get
			{
				try
				{
					return (ReportItemProcessing)this.m_members;
				}
				catch
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
			}
		}

		internal bool UseCache
		{
			get
			{
				if (this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				if (this.Rendering.m_renderingContext != null)
				{
					return this.Rendering.m_renderingContext.CacheState;
				}
				return true;
			}
		}

		protected internal bool IsCustomControl
		{
			get
			{
				return this.m_members.IsCustomControl;
			}
		}

		internal bool SkipSearch
		{
			get
			{
				if (this.SharedHidden == SharedHiddenState.Always)
				{
					return true;
				}
				if (this.SharedHidden == SharedHiddenState.Sometimes && this.Hidden)
				{
					return true;
				}
				return false;
			}
		}

		protected ReportItem(string definitionName, string instanceName)
		{
			this.m_members = new ReportItemProcessing();
			this.Processing.DefinitionName = definitionName;
			this.m_uniqueName = instanceName;
		}

		protected ReportItem()
		{
			this.m_members = new ReportItemProcessing();
		}

		internal ReportItem(AspNetCore.ReportingServices.ReportProcessing.CustomReportItem criDef, CustomReportItemInstance criInstance, CustomReportItemInstanceInfo instanceInfo)
		{
			this.m_members = new ReportItemRendering();
			this.Rendering.m_reportItemDef = criDef;
			this.Rendering.m_reportItemInstance = criInstance;
			this.Rendering.m_reportItemInstanceInfo = instanceInfo;
			this.m_intUniqueName = criInstance.UniqueName;
		}

		internal ReportItem(string uniqueName, int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext)
		{
			this.m_members = new ReportItemRendering();
			this.m_uniqueName = uniqueName;
			this.m_intUniqueName = intUniqueName;
			this.Rendering.m_renderingContext = renderingContext;
			this.Rendering.m_reportItemDef = reportItemDef;
			this.Rendering.m_reportItemInstance = reportItemInstance;
			this.Rendering.m_headingInstance = renderingContext.HeadingInstance;
		}

		protected void DeepClone(ReportItem clone)
		{
			if (clone != null && this.IsCustomControl)
			{
				if (this.m_uniqueName != null)
				{
					clone.m_uniqueName = string.Copy(this.m_uniqueName);
				}
				clone.m_intUniqueName = this.m_intUniqueName;
				clone.m_canClick = this.m_canClick;
				clone.m_canEdit = this.m_canEdit;
				clone.m_canDrag = this.m_canDrag;
				clone.m_dropTarget = this.m_dropTarget;
				Global.Tracer.Assert(this.m_members is ReportItemProcessing);
				clone.m_members = ((ReportItemProcessing)this.m_members).DeepClone();
				if (this.m_style != null)
				{
					((StyleBase)this.m_style).ExtractRenderStyles(out clone.Processing.SharedStyles, out clone.Processing.NonSharedStyles);
				}
				if (this.m_customProperties != null)
				{
					clone.m_customProperties = this.m_customProperties.DeepClone();
				}
				return;
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
		}

		internal virtual bool Search(SearchContext searchContext)
		{
			return false;
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

		internal static ReportItem CreateItem(int indexIntoParentCollection, AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext, NonComputedUniqueNames nonComputedUniqueNames)
		{
			string uniqueName = null;
			if (renderingContext.InPageSection)
			{
				uniqueName = renderingContext.UniqueNamePrefix + "a" + indexIntoParentCollection.ToString(CultureInfo.InvariantCulture);
			}
			return ReportItem.CreateItem(uniqueName, reportItemDef, reportItemInstance, renderingContext, nonComputedUniqueNames);
		}

		internal static ReportItem CreateItem(string uniqueName, AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext, NonComputedUniqueNames nonComputedUniqueNames)
		{
			if (reportItemDef == null)
			{
				return null;
			}
			Global.Tracer.Assert(null != renderingContext);
			ReportItem reportItem = null;
			int intUniqueName = 0;
			NonComputedUniqueNames[] childrenNonComputedUniqueNames = null;
			if (reportItemInstance != null)
			{
				intUniqueName = reportItemInstance.UniqueName;
			}
			else if (nonComputedUniqueNames != null)
			{
				intUniqueName = nonComputedUniqueNames.UniqueName;
				childrenNonComputedUniqueNames = nonComputedUniqueNames.ChildrenUniqueNames;
			}
			if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.Line)
			{
				AspNetCore.ReportingServices.ReportProcessing.Line reportItemDef2 = (AspNetCore.ReportingServices.ReportProcessing.Line)reportItemDef;
				LineInstance reportItemInstance2 = (LineInstance)reportItemInstance;
				reportItem = new Line(uniqueName, intUniqueName, reportItemDef2, reportItemInstance2, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.CheckBox)
			{
				AspNetCore.ReportingServices.ReportProcessing.CheckBox reportItemDef3 = (AspNetCore.ReportingServices.ReportProcessing.CheckBox)reportItemDef;
				CheckBoxInstance reportItemInstance3 = (CheckBoxInstance)reportItemInstance;
				reportItem = new CheckBox(uniqueName, intUniqueName, reportItemDef3, reportItemInstance3, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.Image)
			{
				AspNetCore.ReportingServices.ReportProcessing.Image reportItemDef4 = (AspNetCore.ReportingServices.ReportProcessing.Image)reportItemDef;
				ImageInstance reportItemInstance4 = (ImageInstance)reportItemInstance;
				reportItem = new Image(uniqueName, intUniqueName, reportItemDef4, reportItemInstance4, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.TextBox)
			{
				AspNetCore.ReportingServices.ReportProcessing.TextBox reportItemDef5 = (AspNetCore.ReportingServices.ReportProcessing.TextBox)reportItemDef;
				TextBoxInstance reportItemInstance5 = (TextBoxInstance)reportItemInstance;
				reportItem = new TextBox(uniqueName, intUniqueName, reportItemDef5, reportItemInstance5, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.Rectangle)
			{
				AspNetCore.ReportingServices.ReportProcessing.Rectangle reportItemDef6 = (AspNetCore.ReportingServices.ReportProcessing.Rectangle)reportItemDef;
				RectangleInstance reportItemInstance6 = (RectangleInstance)reportItemInstance;
				reportItem = new Rectangle(uniqueName, intUniqueName, reportItemDef6, reportItemInstance6, renderingContext, childrenNonComputedUniqueNames);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.ActiveXControl)
			{
				AspNetCore.ReportingServices.ReportProcessing.ActiveXControl reportItemDef7 = (AspNetCore.ReportingServices.ReportProcessing.ActiveXControl)reportItemDef;
				ActiveXControlInstance reportItemInstance7 = (ActiveXControlInstance)reportItemInstance;
				reportItem = new ActiveXControl(uniqueName, intUniqueName, reportItemDef7, reportItemInstance7, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.SubReport)
			{
				AspNetCore.ReportingServices.ReportProcessing.SubReport subReport = (AspNetCore.ReportingServices.ReportProcessing.SubReport)reportItemDef;
				SubReportInstance subReportInstance = (SubReportInstance)reportItemInstance;
				bool processedWithError = false;
				Report innerReport;
				if (AspNetCore.ReportingServices.ReportProcessing.SubReport.Status.Retrieved != subReport.RetrievalStatus)
				{
					innerReport = null;
					processedWithError = true;
				}
				else
				{
					if (subReport.ReportContext == null && renderingContext.CurrentReportContext != null)
					{
						subReport.ReportContext = renderingContext.CurrentReportContext.GetSubreportContext(subReport.ReportPath);
					}
					ICatalogItemContext reportContext = subReport.ReportContext;
					RenderingContext renderingContext2 = new RenderingContext(renderingContext, subReport.Uri, subReport.Report.EmbeddedImages, subReport.Report.ImageStreamNames, reportContext);
					if (subReportInstance == null)
					{
						innerReport = new Report(subReport.Report, null, renderingContext2, subReport.ReportName, subReport.Description, null);
					}
					else if (subReportInstance.ReportInstance == null)
					{
						processedWithError = true;
						innerReport = new Report(subReport.Report, null, renderingContext2, subReport.ReportName, subReport.Description, null);
					}
					else
					{
						innerReport = new Report(subReport.Report, subReportInstance.ReportInstance, renderingContext2, subReport.ReportName, subReport.Description, null);
					}
				}
				reportItem = new SubReport(intUniqueName, subReport, subReportInstance, renderingContext, innerReport, processedWithError);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.List)
			{
				AspNetCore.ReportingServices.ReportProcessing.List reportItemDef8 = (AspNetCore.ReportingServices.ReportProcessing.List)reportItemDef;
				ListInstance reportItemInstance8 = (ListInstance)reportItemInstance;
				reportItem = new List(intUniqueName, reportItemDef8, reportItemInstance8, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.Matrix)
			{
				AspNetCore.ReportingServices.ReportProcessing.Matrix reportItemDef9 = (AspNetCore.ReportingServices.ReportProcessing.Matrix)reportItemDef;
				MatrixInstance reportItemInstance9 = (MatrixInstance)reportItemInstance;
				reportItem = new Matrix(intUniqueName, reportItemDef9, reportItemInstance9, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.Table)
			{
				AspNetCore.ReportingServices.ReportProcessing.Table reportItemDef10 = (AspNetCore.ReportingServices.ReportProcessing.Table)reportItemDef;
				TableInstance reportItemInstance10 = (TableInstance)reportItemInstance;
				reportItem = new Table(intUniqueName, reportItemDef10, reportItemInstance10, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.OWCChart)
			{
				AspNetCore.ReportingServices.ReportProcessing.OWCChart reportItemDef11 = (AspNetCore.ReportingServices.ReportProcessing.OWCChart)reportItemDef;
				OWCChartInstance reportItemInstance11 = (OWCChartInstance)reportItemInstance;
				reportItem = new OWCChart(intUniqueName, reportItemDef11, reportItemInstance11, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.Chart)
			{
				AspNetCore.ReportingServices.ReportProcessing.Chart reportItemDef12 = (AspNetCore.ReportingServices.ReportProcessing.Chart)reportItemDef;
				ChartInstance reportItemInstance12 = (ChartInstance)reportItemInstance;
				reportItem = new Chart(intUniqueName, reportItemDef12, reportItemInstance12, renderingContext);
			}
			else if (reportItemDef is AspNetCore.ReportingServices.ReportProcessing.CustomReportItem)
			{
				AspNetCore.ReportingServices.ReportProcessing.CustomReportItem reportItemDef13 = (AspNetCore.ReportingServices.ReportProcessing.CustomReportItem)reportItemDef;
				CustomReportItemInstance reportItemInstance13 = (CustomReportItemInstance)reportItemInstance;
				reportItem = new CustomReportItem(uniqueName, intUniqueName, reportItemDef13, reportItemInstance13, renderingContext, childrenNonComputedUniqueNames);
				if (!renderingContext.NativeAllCRITypes && (renderingContext.NativeCRITypes == null || !renderingContext.NativeCRITypes.ContainsKey(((CustomReportItem)reportItem).Type)))
				{
					reportItem = ((CustomReportItem)reportItem).AltReportItem;
				}
			}
			return reportItem;
		}
	}
}
