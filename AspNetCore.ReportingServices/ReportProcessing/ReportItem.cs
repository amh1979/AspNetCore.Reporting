using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class ReportItem : IDOwner, ISearchByUniqueName, IComparable
	{
		internal enum DataElementOutputTypesRDL
		{
			Output,
			NoOutput,
			ContentsOnly,
			Auto
		}

		internal enum DataElementStylesRDL
		{
			AttributeNormal,
			ElementNormal,
			Auto
		}

		private const string ZeroSize = "0mm";

		internal const int OverlapDetectionRounding = 1;

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

		protected ExpressionInfo m_label;

		protected ExpressionInfo m_bookmark;

		protected string m_custom;

		protected bool m_repeatedSibling;

		protected bool m_isFullSize;

		private int m_exprHostID = -1;

		protected string m_dataElementName;

		protected DataElementOutputTypes m_dataElementOutput;

		protected int m_distanceFromReportTop = -1;

		protected int m_distanceBeforeTop;

		protected IntList m_siblingAboveMe;

		protected DataValueList m_customProperties;

		[NonSerialized]
		protected ReportItem m_parent;

		[NonSerialized]
		protected bool m_computed;

		[NonSerialized]
		protected string m_repeatWith;

		[NonSerialized]
		protected DataElementOutputTypesRDL m_dataElementOutputRDL = DataElementOutputTypesRDL.Auto;

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		[NonSerialized]
		protected int m_startPage = -1;

		[NonSerialized]
		protected int m_endPage = -1;

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
		private ReportProcessing.PageTextboxes m_repeatedSiblingTextboxes;

		[NonSerialized]
		protected string m_renderingModelID;

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

		internal abstract ObjectType ObjectType
		{
			get;
		}

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

		internal Style StyleClass
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
					return Math.Round(this.m_topValue + this.m_heightValue, 1);
				}
				return Math.Round(this.m_topValue, 1);
			}
		}

		internal double AbsoluteLeftValue
		{
			get
			{
				if (this.m_widthValue < 0.0)
				{
					return Math.Round(this.m_leftValue + this.m_widthValue, 1);
				}
				return Math.Round(this.m_leftValue, 1);
			}
		}

		internal double AbsoluteBottomValue
		{
			get
			{
				if (this.m_heightValue < 0.0)
				{
					return Math.Round(this.m_topValue, 1);
				}
				return Math.Round(this.m_topValue + this.m_heightValue, 1);
			}
		}

		internal double AbsoluteRightValue
		{
			get
			{
				if (this.m_widthValue < 0.0)
				{
					return Math.Round(this.m_leftValue, 1);
				}
				return Math.Round(this.m_leftValue + this.m_widthValue, 1);
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

		internal Visibility Visibility
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

		internal ExpressionInfo Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
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

		internal string Custom
		{
			get
			{
				return this.m_custom;
			}
			set
			{
				this.m_custom = value;
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

		internal bool IsFullSize
		{
			get
			{
				return this.m_isFullSize;
			}
			set
			{
				this.m_isFullSize = value;
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

		internal virtual int DistanceFromReportTop
		{
			get
			{
				return this.m_distanceFromReportTop;
			}
			set
			{
				this.m_distanceFromReportTop = value;
			}
		}

		internal int DistanceBeforeTop
		{
			get
			{
				return this.m_distanceBeforeTop;
			}
			set
			{
				this.m_distanceBeforeTop = value;
			}
		}

		internal IntList SiblingAboveMe
		{
			get
			{
				return this.m_siblingAboveMe;
			}
			set
			{
				this.m_siblingAboveMe = value;
			}
		}

		internal ReportItem Parent
		{
			get
			{
				return this.m_parent;
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

		internal DataElementOutputTypesRDL DataElementOutputRDL
		{
			get
			{
				return this.m_dataElementOutputRDL;
			}
			set
			{
				this.m_dataElementOutputRDL = value;
			}
		}

		internal ReportItemExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal virtual int StartPage
		{
			get
			{
				return this.m_startPage;
			}
			set
			{
				this.m_startPage = value;
			}
		}

		internal virtual int EndPage
		{
			get
			{
				return this.m_endPage;
			}
			set
			{
				this.m_endPage = value;
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

		internal string RenderingModelID
		{
			get
			{
				return this.m_renderingModelID;
			}
			set
			{
				this.m_renderingModelID = value;
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

		internal ReportProcessing.PageTextboxes RepeatedSiblingTextboxes
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

		protected ReportItem(int id, ReportItem parent)
			: base(id)
		{
			this.m_parent = parent;
		}

		protected ReportItem(ReportItem parent)
		{
			this.m_parent = parent;
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
			if (this.m_parent != null)
			{
				bool flag = true;
				if (this.m_width == null)
				{
					if ((context.Location & LocationFlags.InMatrixOrTable) == (LocationFlags)0)
					{
						if (ObjectType.Table == context.ObjectType || ObjectType.Matrix == context.ObjectType)
						{
							this.m_width = "0mm";
							this.m_widthValue = 0.0;
							flag = false;
						}
						else if (ObjectType.PageHeader == context.ObjectType || ObjectType.PageFooter == context.ObjectType)
						{
							Report report = this.m_parent as Report;
							this.m_widthValue = report.PageSectionWidth;
							this.m_width = Converter.ConvertSize(this.m_widthValue);
						}
						else
						{
							this.m_widthValue = Math.Round(this.m_parent.m_widthValue - this.m_leftValue, Validator.DecimalPrecision);
							this.m_width = Converter.ConvertSize(this.m_widthValue);
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
					if ((context.Location & LocationFlags.InMatrixOrTable) == (LocationFlags)0)
					{
						if (ObjectType.Table == context.ObjectType || ObjectType.Matrix == context.ObjectType)
						{
							this.m_height = "0mm";
							this.m_heightValue = 0.0;
							flag = false;
						}
						else
						{
							this.m_heightValue = Math.Round(this.m_parent.m_heightValue - this.m_topValue, Validator.DecimalPrecision);
							this.m_height = Converter.ConvertSize(this.m_heightValue);
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
			if ((context.Location & LocationFlags.InMatrixOrTable) == (LocationFlags)0)
			{
				this.ValidateParentBoundaries(context, context.ObjectType, context.ObjectName);
			}
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
			if (this.m_label != null)
			{
				this.m_label.Initialize("Label", context);
				context.ExprHostBuilder.GenericLabel(this.m_label);
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
				this.m_customProperties.Initialize(null, true, context);
			}
			this.DataRendererInitialize(context);
			return false;
		}

		private void ValidateParentBoundaries(InitializationContext context, ObjectType objectType, string objectName)
		{
			if (this.m_parent != null && !(this.m_parent is Report))
			{
				if (objectType == ObjectType.Line)
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
				if (this.AbsoluteBottomValue > Math.Round(this.m_parent.HeightValue, 1))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Bottom".ToLowerInvariant());
				}
				if (this.AbsoluteRightValue > Math.Round(this.m_parent.WidthValue, 1))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Right".ToLowerInvariant());
				}
			}
		}

		protected virtual void DataRendererInitialize(InitializationContext context)
		{
			CLSNameValidator.ValidateDataElementName(ref this.m_dataElementName, this.DataElementNameDefault, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
			switch (this.m_dataElementOutputRDL)
			{
			case DataElementOutputTypesRDL.Output:
				this.m_dataElementOutput = DataElementOutputTypes.Output;
				break;
			case DataElementOutputTypesRDL.NoOutput:
				this.m_dataElementOutput = DataElementOutputTypes.NoOutput;
				break;
			case DataElementOutputTypesRDL.ContentsOnly:
				this.m_dataElementOutput = DataElementOutputTypes.ContentsOnly;
				break;
			case DataElementOutputTypesRDL.Auto:
				if (context.TableColumnVisible && (this.m_visibility == null || this.m_visibility.Hidden == null || this.m_visibility.Toggle != null || (ExpressionInfo.Types.Constant == this.m_visibility.Hidden.Type && !this.m_visibility.Hidden.BoolValue)))
				{
					this.m_dataElementOutput = this.DataElementOutputDefault;
				}
				else
				{
					this.m_dataElementOutput = DataElementOutputTypes.NoOutput;
				}
				break;
			}
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
				width = Math.Round(this.m_parent.m_widthValue - this.m_leftValue, Validator.DecimalPrecision);
			}
			if (this.m_height == null)
			{
				height = Math.Round(this.m_parent.m_heightValue - this.m_topValue, Validator.DecimalPrecision);
			}
			this.CalculateSizes(width, height, context, overwrite);
		}

		internal virtual void RegisterReceiver(InitializationContext context)
		{
			if (this.m_visibility != null)
			{
				this.m_visibility.RegisterReceiver(context, false);
			}
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is ReportItem))
			{
				throw new ArgumentException("Argument was not a ReportItem.  Can only compare two ReportItems");
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
				Global.Tracer.Assert(null != this.m_customProperties);
				this.m_customProperties.SetExprHost(this.m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Top, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.TopValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Left, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.LeftValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Height, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.HeightValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Width, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.WidthValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.ZIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			memberInfoList.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Label, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Bookmark, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Custom, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.RepeatedSibling, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.IsFullSize, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.DistanceFromReportTop, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DistanceBeforeTop, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SiblingAboveMe, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.CustomProperties, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			if (nonCompNames == null)
			{
				return null;
			}
			if (targetUniqueName == nonCompNames.UniqueName)
			{
				return this;
			}
			Rectangle rectangle = this as Rectangle;
			if (rectangle != null)
			{
				return rectangle.SearchChildren(targetUniqueName, ref nonCompNames, chunkManager);
			}
			return null;
		}

		internal virtual void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, NonComputedUniqueNames nonCompNames)
		{
		}

		internal void ProcessNavigationAction(ReportProcessing.NavigationInfo navigationInfo, NonComputedUniqueNames nonCompNames, int startPage)
		{
			if (nonCompNames != null)
			{
				if (this.m_bookmark != null && this.m_bookmark.Value != null)
				{
					navigationInfo.ProcessBookmark(this.m_bookmark.Value, startPage, nonCompNames.UniqueName);
				}
				Rectangle rectangle = this as Rectangle;
				if (this.m_label != null && this.m_label.Value != null)
				{
					int num = -1;
					if (rectangle != null)
					{
						navigationInfo.EnterDocumentMapChildren();
						num = rectangle.ProcessNavigationChildren(navigationInfo, nonCompNames, startPage);
					}
					if (num < 0)
					{
						num = nonCompNames.UniqueName;
					}
					navigationInfo.AddToDocumentMap(num, rectangle != null, startPage, this.m_label.Value);
				}
				else if (rectangle != null)
				{
					rectangle.ProcessNavigationChildren(navigationInfo, nonCompNames, startPage);
				}
			}
		}
	}
}
