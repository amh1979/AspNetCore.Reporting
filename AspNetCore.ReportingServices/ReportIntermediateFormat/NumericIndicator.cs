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
	[SkipStaticValidation]
	internal sealed class NumericIndicator : GaugePanelItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = NumericIndicator.GetDeclaration();

		private GaugeInputValue m_gaugeInputValue;

		private List<NumericIndicatorRange> m_numericIndicatorRanges;

		private ExpressionInfo m_decimalDigitColor;

		private ExpressionInfo m_digitColor;

		private ExpressionInfo m_useFontPercent;

		private ExpressionInfo m_decimalDigits;

		private ExpressionInfo m_digits;

		private GaugeInputValue m_minimumValue;

		private GaugeInputValue m_maximumValue;

		private ExpressionInfo m_multiplier;

		private ExpressionInfo m_nonNumericString;

		private ExpressionInfo m_outOfRangeString;

		private ExpressionInfo m_resizeMode;

		private ExpressionInfo m_showDecimalPoint;

		private ExpressionInfo m_showLeadingZeros;

		private ExpressionInfo m_indicatorStyle;

		private ExpressionInfo m_showSign;

		private ExpressionInfo m_snappingEnabled;

		private ExpressionInfo m_snappingInterval;

		private ExpressionInfo m_ledDimColor;

		private ExpressionInfo m_separatorWidth;

		private ExpressionInfo m_separatorColor;

		internal GaugeInputValue GaugeInputValue
		{
			get
			{
				return this.m_gaugeInputValue;
			}
			set
			{
				this.m_gaugeInputValue = value;
			}
		}

		internal List<NumericIndicatorRange> NumericIndicatorRanges
		{
			get
			{
				return this.m_numericIndicatorRanges;
			}
			set
			{
				this.m_numericIndicatorRanges = value;
			}
		}

		internal ExpressionInfo DecimalDigitColor
		{
			get
			{
				return this.m_decimalDigitColor;
			}
			set
			{
				this.m_decimalDigitColor = value;
			}
		}

		internal ExpressionInfo DigitColor
		{
			get
			{
				return this.m_digitColor;
			}
			set
			{
				this.m_digitColor = value;
			}
		}

		internal ExpressionInfo UseFontPercent
		{
			get
			{
				return this.m_useFontPercent;
			}
			set
			{
				this.m_useFontPercent = value;
			}
		}

		internal ExpressionInfo DecimalDigits
		{
			get
			{
				return this.m_decimalDigits;
			}
			set
			{
				this.m_decimalDigits = value;
			}
		}

		internal ExpressionInfo Digits
		{
			get
			{
				return this.m_digits;
			}
			set
			{
				this.m_digits = value;
			}
		}

		internal GaugeInputValue MinimumValue
		{
			get
			{
				return this.m_minimumValue;
			}
			set
			{
				this.m_minimumValue = value;
			}
		}

		internal GaugeInputValue MaximumValue
		{
			get
			{
				return this.m_maximumValue;
			}
			set
			{
				this.m_maximumValue = value;
			}
		}

		internal ExpressionInfo Multiplier
		{
			get
			{
				return this.m_multiplier;
			}
			set
			{
				this.m_multiplier = value;
			}
		}

		internal ExpressionInfo NonNumericString
		{
			get
			{
				return this.m_nonNumericString;
			}
			set
			{
				this.m_nonNumericString = value;
			}
		}

		internal ExpressionInfo OutOfRangeString
		{
			get
			{
				return this.m_outOfRangeString;
			}
			set
			{
				this.m_outOfRangeString = value;
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

		internal ExpressionInfo ShowDecimalPoint
		{
			get
			{
				return this.m_showDecimalPoint;
			}
			set
			{
				this.m_showDecimalPoint = value;
			}
		}

		internal ExpressionInfo ShowLeadingZeros
		{
			get
			{
				return this.m_showLeadingZeros;
			}
			set
			{
				this.m_showLeadingZeros = value;
			}
		}

		internal ExpressionInfo IndicatorStyle
		{
			get
			{
				return this.m_indicatorStyle;
			}
			set
			{
				this.m_indicatorStyle = value;
			}
		}

		internal ExpressionInfo ShowSign
		{
			get
			{
				return this.m_showSign;
			}
			set
			{
				this.m_showSign = value;
			}
		}

		internal ExpressionInfo SnappingEnabled
		{
			get
			{
				return this.m_snappingEnabled;
			}
			set
			{
				this.m_snappingEnabled = value;
			}
		}

		internal ExpressionInfo SnappingInterval
		{
			get
			{
				return this.m_snappingInterval;
			}
			set
			{
				this.m_snappingInterval = value;
			}
		}

		internal ExpressionInfo LedDimColor
		{
			get
			{
				return this.m_ledDimColor;
			}
			set
			{
				this.m_ledDimColor = value;
			}
		}

		internal ExpressionInfo SeparatorWidth
		{
			get
			{
				return this.m_separatorWidth;
			}
			set
			{
				this.m_separatorWidth = value;
			}
		}

		internal ExpressionInfo SeparatorColor
		{
			get
			{
				return this.m_separatorColor;
			}
			set
			{
				this.m_separatorColor = value;
			}
		}

		internal new NumericIndicatorExprHost ExprHost
		{
			get
			{
				return (NumericIndicatorExprHost)base.m_exprHost;
			}
		}

		internal NumericIndicator()
		{
		}

		internal NumericIndicator(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.NumericIndicatorStart(base.m_name);
			base.Initialize(context);
			if (this.m_numericIndicatorRanges != null)
			{
				for (int i = 0; i < this.m_numericIndicatorRanges.Count; i++)
				{
					this.m_numericIndicatorRanges[i].Initialize(context);
				}
			}
			if (this.m_decimalDigitColor != null)
			{
				this.m_decimalDigitColor.Initialize("DecimalDigitColor", context);
				context.ExprHostBuilder.NumericIndicatorDecimalDigitColor(this.m_decimalDigitColor);
			}
			if (this.m_digitColor != null)
			{
				this.m_digitColor.Initialize("DigitColor", context);
				context.ExprHostBuilder.NumericIndicatorDigitColor(this.m_digitColor);
			}
			if (this.m_useFontPercent != null)
			{
				this.m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.NumericIndicatorUseFontPercent(this.m_useFontPercent);
			}
			if (this.m_decimalDigits != null)
			{
				this.m_decimalDigits.Initialize("DecimalDigits", context);
				context.ExprHostBuilder.NumericIndicatorDecimalDigits(this.m_decimalDigits);
			}
			if (this.m_digits != null)
			{
				this.m_digits.Initialize("Digits", context);
				context.ExprHostBuilder.NumericIndicatorDigits(this.m_digits);
			}
			if (this.m_multiplier != null)
			{
				this.m_multiplier.Initialize("Multiplier", context);
				context.ExprHostBuilder.NumericIndicatorMultiplier(this.m_multiplier);
			}
			if (this.m_nonNumericString != null)
			{
				this.m_nonNumericString.Initialize("NonNumericString", context);
				context.ExprHostBuilder.NumericIndicatorNonNumericString(this.m_nonNumericString);
			}
			if (this.m_outOfRangeString != null)
			{
				this.m_outOfRangeString.Initialize("OutOfRangeString", context);
				context.ExprHostBuilder.NumericIndicatorOutOfRangeString(this.m_outOfRangeString);
			}
			if (this.m_resizeMode != null)
			{
				this.m_resizeMode.Initialize("ResizeMode", context);
				context.ExprHostBuilder.NumericIndicatorResizeMode(this.m_resizeMode);
			}
			if (this.m_showDecimalPoint != null)
			{
				this.m_showDecimalPoint.Initialize("ShowDecimalPoint", context);
				context.ExprHostBuilder.NumericIndicatorShowDecimalPoint(this.m_showDecimalPoint);
			}
			if (this.m_showLeadingZeros != null)
			{
				this.m_showLeadingZeros.Initialize("ShowLeadingZeros", context);
				context.ExprHostBuilder.NumericIndicatorShowLeadingZeros(this.m_showLeadingZeros);
			}
			if (this.m_indicatorStyle != null)
			{
				this.m_indicatorStyle.Initialize("IndicatorStyle", context);
				context.ExprHostBuilder.NumericIndicatorIndicatorStyle(this.m_indicatorStyle);
			}
			if (this.m_showSign != null)
			{
				this.m_showSign.Initialize("ShowSign", context);
				context.ExprHostBuilder.NumericIndicatorShowSign(this.m_showSign);
			}
			if (this.m_snappingEnabled != null)
			{
				this.m_snappingEnabled.Initialize("SnappingEnabled", context);
				context.ExprHostBuilder.NumericIndicatorSnappingEnabled(this.m_snappingEnabled);
			}
			if (this.m_snappingInterval != null)
			{
				this.m_snappingInterval.Initialize("SnappingInterval", context);
				context.ExprHostBuilder.NumericIndicatorSnappingInterval(this.m_snappingInterval);
			}
			if (this.m_ledDimColor != null)
			{
				this.m_ledDimColor.Initialize("LedDimColor", context);
				context.ExprHostBuilder.NumericIndicatorLedDimColor(this.m_ledDimColor);
			}
			if (this.m_separatorWidth != null)
			{
				this.m_separatorWidth.Initialize("SeparatorWidth", context);
				context.ExprHostBuilder.NumericIndicatorSeparatorWidth(this.m_separatorWidth);
			}
			if (this.m_separatorColor != null)
			{
				this.m_separatorColor.Initialize("SeparatorColor", context);
				context.ExprHostBuilder.NumericIndicatorSeparatorColor(this.m_separatorColor);
			}
			base.m_exprHostID = context.ExprHostBuilder.NumericIndicatorEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			NumericIndicator numericIndicator = (NumericIndicator)base.PublishClone(context);
			if (this.m_gaugeInputValue != null)
			{
				numericIndicator.m_gaugeInputValue = (GaugeInputValue)this.m_gaugeInputValue.PublishClone(context);
			}
			if (this.m_numericIndicatorRanges != null)
			{
				numericIndicator.m_numericIndicatorRanges = new List<NumericIndicatorRange>(this.m_numericIndicatorRanges.Count);
				foreach (NumericIndicatorRange numericIndicatorRange in this.m_numericIndicatorRanges)
				{
					numericIndicator.m_numericIndicatorRanges.Add((NumericIndicatorRange)numericIndicatorRange.PublishClone(context));
				}
			}
			if (this.m_decimalDigitColor != null)
			{
				numericIndicator.m_decimalDigitColor = (ExpressionInfo)this.m_decimalDigitColor.PublishClone(context);
			}
			if (this.m_digitColor != null)
			{
				numericIndicator.m_digitColor = (ExpressionInfo)this.m_digitColor.PublishClone(context);
			}
			if (this.m_useFontPercent != null)
			{
				numericIndicator.m_useFontPercent = (ExpressionInfo)this.m_useFontPercent.PublishClone(context);
			}
			if (this.m_decimalDigits != null)
			{
				numericIndicator.m_decimalDigits = (ExpressionInfo)this.m_decimalDigits.PublishClone(context);
			}
			if (this.m_digits != null)
			{
				numericIndicator.m_digits = (ExpressionInfo)this.m_digits.PublishClone(context);
			}
			if (this.m_minimumValue != null)
			{
				numericIndicator.m_minimumValue = (GaugeInputValue)this.m_minimumValue.PublishClone(context);
			}
			if (this.m_maximumValue != null)
			{
				numericIndicator.m_maximumValue = (GaugeInputValue)this.m_maximumValue.PublishClone(context);
			}
			if (this.m_multiplier != null)
			{
				numericIndicator.m_multiplier = (ExpressionInfo)this.m_multiplier.PublishClone(context);
			}
			if (this.m_nonNumericString != null)
			{
				numericIndicator.m_nonNumericString = (ExpressionInfo)this.m_nonNumericString.PublishClone(context);
			}
			if (this.m_outOfRangeString != null)
			{
				numericIndicator.m_outOfRangeString = (ExpressionInfo)this.m_outOfRangeString.PublishClone(context);
			}
			if (this.m_resizeMode != null)
			{
				numericIndicator.m_resizeMode = (ExpressionInfo)this.m_resizeMode.PublishClone(context);
			}
			if (this.m_showDecimalPoint != null)
			{
				numericIndicator.m_showDecimalPoint = (ExpressionInfo)this.m_showDecimalPoint.PublishClone(context);
			}
			if (this.m_showLeadingZeros != null)
			{
				numericIndicator.m_showLeadingZeros = (ExpressionInfo)this.m_showLeadingZeros.PublishClone(context);
			}
			if (this.m_indicatorStyle != null)
			{
				numericIndicator.m_indicatorStyle = (ExpressionInfo)this.m_indicatorStyle.PublishClone(context);
			}
			if (this.m_showSign != null)
			{
				numericIndicator.m_showSign = (ExpressionInfo)this.m_showSign.PublishClone(context);
			}
			if (this.m_snappingEnabled != null)
			{
				numericIndicator.m_snappingEnabled = (ExpressionInfo)this.m_snappingEnabled.PublishClone(context);
			}
			if (this.m_snappingInterval != null)
			{
				numericIndicator.m_snappingInterval = (ExpressionInfo)this.m_snappingInterval.PublishClone(context);
			}
			if (this.m_ledDimColor != null)
			{
				numericIndicator.m_ledDimColor = (ExpressionInfo)this.m_ledDimColor.PublishClone(context);
			}
			if (this.m_separatorWidth != null)
			{
				numericIndicator.m_separatorWidth = (ExpressionInfo)this.m_separatorWidth.PublishClone(context);
			}
			if (this.m_separatorColor != null)
			{
				numericIndicator.m_separatorColor = (ExpressionInfo)this.m_separatorColor.PublishClone(context);
			}
			return numericIndicator;
		}

		internal void SetExprHost(NumericIndicatorExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_gaugeInputValue != null && this.ExprHost.GaugeInputValueHost != null)
			{
				this.m_gaugeInputValue.SetExprHost(this.ExprHost.GaugeInputValueHost, reportObjectModel);
			}
			IList<NumericIndicatorRangeExprHost> numericIndicatorRangesHostsRemotable = this.ExprHost.NumericIndicatorRangesHostsRemotable;
			if (this.m_numericIndicatorRanges != null && numericIndicatorRangesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_numericIndicatorRanges.Count; i++)
				{
					NumericIndicatorRange numericIndicatorRange = this.m_numericIndicatorRanges[i];
					if (numericIndicatorRange != null && numericIndicatorRange.ExpressionHostID > -1)
					{
						numericIndicatorRange.SetExprHost(numericIndicatorRangesHostsRemotable[numericIndicatorRange.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (this.m_minimumValue != null && this.ExprHost.MinimumValueHost != null)
			{
				this.m_minimumValue.SetExprHost(this.ExprHost.MinimumValueHost, reportObjectModel);
			}
			if (this.m_maximumValue != null && this.ExprHost.MaximumValueHost != null)
			{
				this.m_maximumValue.SetExprHost(this.ExprHost.MaximumValueHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicator, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, memberInfoList);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(NumericIndicator.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugeInputValue:
					writer.Write(this.m_gaugeInputValue);
					break;
				case MemberName.NumericIndicatorRanges:
					writer.Write(this.m_numericIndicatorRanges);
					break;
				case MemberName.DecimalDigitColor:
					writer.Write(this.m_decimalDigitColor);
					break;
				case MemberName.DigitColor:
					writer.Write(this.m_digitColor);
					break;
				case MemberName.UseFontPercent:
					writer.Write(this.m_useFontPercent);
					break;
				case MemberName.DecimalDigits:
					writer.Write(this.m_decimalDigits);
					break;
				case MemberName.Digits:
					writer.Write(this.m_digits);
					break;
				case MemberName.MinimumValue:
					writer.Write(this.m_minimumValue);
					break;
				case MemberName.MaximumValue:
					writer.Write(this.m_maximumValue);
					break;
				case MemberName.Multiplier:
					writer.Write(this.m_multiplier);
					break;
				case MemberName.NonNumericString:
					writer.Write(this.m_nonNumericString);
					break;
				case MemberName.OutOfRangeString:
					writer.Write(this.m_outOfRangeString);
					break;
				case MemberName.ResizeMode:
					writer.Write(this.m_resizeMode);
					break;
				case MemberName.ShowDecimalPoint:
					writer.Write(this.m_showDecimalPoint);
					break;
				case MemberName.ShowLeadingZeros:
					writer.Write(this.m_showLeadingZeros);
					break;
				case MemberName.IndicatorStyle:
					writer.Write(this.m_indicatorStyle);
					break;
				case MemberName.ShowSign:
					writer.Write(this.m_showSign);
					break;
				case MemberName.SnappingEnabled:
					writer.Write(this.m_snappingEnabled);
					break;
				case MemberName.SnappingInterval:
					writer.Write(this.m_snappingInterval);
					break;
				case MemberName.LedDimColor:
					writer.Write(this.m_ledDimColor);
					break;
				case MemberName.SeparatorWidth:
					writer.Write(this.m_separatorWidth);
					break;
				case MemberName.SeparatorColor:
					writer.Write(this.m_separatorColor);
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
			reader.RegisterDeclaration(NumericIndicator.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeInputValue:
					this.m_gaugeInputValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.NumericIndicatorRanges:
					this.m_numericIndicatorRanges = reader.ReadGenericListOfRIFObjects<NumericIndicatorRange>();
					break;
				case MemberName.DecimalDigitColor:
					this.m_decimalDigitColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DigitColor:
					this.m_digitColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseFontPercent:
					this.m_useFontPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DecimalDigits:
					this.m_decimalDigits = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Digits:
					this.m_digits = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinimumValue:
					this.m_minimumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.MaximumValue:
					this.m_maximumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.Multiplier:
					this.m_multiplier = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.NonNumericString:
					this.m_nonNumericString = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OutOfRangeString:
					this.m_outOfRangeString = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ResizeMode:
					this.m_resizeMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowDecimalPoint:
					this.m_showDecimalPoint = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowLeadingZeros:
					this.m_showLeadingZeros = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorStyle:
					this.m_indicatorStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowSign:
					this.m_showSign = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SnappingEnabled:
					this.m_snappingEnabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SnappingInterval:
					this.m_snappingInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LedDimColor:
					this.m_ledDimColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeparatorWidth:
					this.m_separatorWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeparatorColor:
					this.m_separatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicator;
		}

		internal string EvaluateDecimalDigitColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorDecimalDigitColorExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateDigitColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorDigitColorExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorUseFontPercentExpression(this, base.m_gaugePanel.Name);
		}

		internal int EvaluateDecimalDigits(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorDecimalDigitsExpression(this, base.m_gaugePanel.Name);
		}

		internal int EvaluateDigits(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorDigitsExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateMultiplier(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorMultiplierExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateNonNumericString(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorNonNumericStringExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateOutOfRangeString(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorOutOfRangeStringExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeResizeModes EvaluateResizeMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeResizeModes(context.ReportRuntime.EvaluateNumericIndicatorResizeModeExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateShowDecimalPoint(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorShowDecimalPointExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateShowLeadingZeros(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorShowLeadingZerosExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeIndicatorStyles EvaluateIndicatorStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeIndicatorStyles(context.ReportRuntime.EvaluateNumericIndicatorIndicatorStyleExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal GaugeShowSigns EvaluateShowSign(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeShowSigns(context.ReportRuntime.EvaluateNumericIndicatorShowSignExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateSnappingEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorSnappingEnabledExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateSnappingInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorSnappingIntervalExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateLedDimColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorLedDimColorExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateSeparatorWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorSeparatorWidthExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorSeparatorColorExpression(this, base.m_gaugePanel.Name);
		}
	}
}
