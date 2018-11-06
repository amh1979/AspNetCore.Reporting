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

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class GaugePanel : DataRegion, IPersistable
	{
		private List<LinearGauge> m_linearGauges;

		private List<RadialGauge> m_radialGauges;

		private List<NumericIndicator> m_numericIndicators;

		private List<StateIndicator> m_stateIndicators;

		private List<GaugeImage> m_gaugeImages;

		private List<GaugeLabel> m_gaugeLabels;

		private ExpressionInfo m_antiAliasing;

		private ExpressionInfo m_autoLayout;

		private BackFrame m_backFrame;

		private ExpressionInfo m_shadowIntensity;

		private ExpressionInfo m_textAntiAliasingQuality;

		private TopImage m_topImage;

		private GaugeMemberList m_columnMembers;

		private GaugeMemberList m_rowMembers;

		private GaugeRowList m_rows;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugePanel.GetDeclaration();

		[NonSerialized]
		private GaugePanelExprHost m_exprHost;

		[NonSerialized]
		private int m_actionOwnerCounter;

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel;
			}
		}

		internal override HierarchyNodeList ColumnMembers
		{
			get
			{
				return this.m_columnMembers;
			}
		}

		internal override HierarchyNodeList RowMembers
		{
			get
			{
				return this.m_rowMembers;
			}
		}

		internal override RowList Rows
		{
			get
			{
				return this.m_rows;
			}
		}

		internal GaugeMember GaugeMember
		{
			get
			{
				if (this.m_columnMembers != null && this.m_columnMembers.Count > 0)
				{
					return this.m_columnMembers[0];
				}
				return null;
			}
			set
			{
				if (this.m_columnMembers == null)
				{
					this.m_columnMembers = new GaugeMemberList();
				}
				else
				{
					this.m_columnMembers.Clear();
				}
				this.m_columnMembers.Add(value);
			}
		}

		internal GaugeMember GaugeRowMember
		{
			get
			{
				if (this.m_rowMembers != null && this.m_rowMembers.Count == 1)
				{
					return this.m_rowMembers[0];
				}
				return null;
			}
			set
			{
				if (this.m_rowMembers == null)
				{
					this.m_rowMembers = new GaugeMemberList();
				}
				else
				{
					this.m_rowMembers.Clear();
				}
				this.m_rowMembers.Add(value);
			}
		}

		internal GaugeRow GaugeRow
		{
			get
			{
				if (this.m_rows != null && this.m_rows.Count > 0)
				{
					return this.m_rows[0];
				}
				return null;
			}
			set
			{
				if (this.m_rows == null)
				{
					this.m_rows = new GaugeRowList();
				}
				else
				{
					this.m_rows.Clear();
				}
				this.m_rows.Add(value);
			}
		}

		internal List<LinearGauge> LinearGauges
		{
			get
			{
				return this.m_linearGauges;
			}
			set
			{
				this.m_linearGauges = value;
			}
		}

		internal List<RadialGauge> RadialGauges
		{
			get
			{
				return this.m_radialGauges;
			}
			set
			{
				this.m_radialGauges = value;
			}
		}

		internal List<NumericIndicator> NumericIndicators
		{
			get
			{
				return this.m_numericIndicators;
			}
			set
			{
				this.m_numericIndicators = value;
			}
		}

		internal List<StateIndicator> StateIndicators
		{
			get
			{
				return this.m_stateIndicators;
			}
			set
			{
				this.m_stateIndicators = value;
			}
		}

		internal List<GaugeImage> GaugeImages
		{
			get
			{
				return this.m_gaugeImages;
			}
			set
			{
				this.m_gaugeImages = value;
			}
		}

		internal List<GaugeLabel> GaugeLabels
		{
			get
			{
				return this.m_gaugeLabels;
			}
			set
			{
				this.m_gaugeLabels = value;
			}
		}

		internal ExpressionInfo AntiAliasing
		{
			get
			{
				return this.m_antiAliasing;
			}
			set
			{
				this.m_antiAliasing = value;
			}
		}

		internal ExpressionInfo AutoLayout
		{
			get
			{
				return this.m_autoLayout;
			}
			set
			{
				this.m_autoLayout = value;
			}
		}

		internal BackFrame BackFrame
		{
			get
			{
				return this.m_backFrame;
			}
			set
			{
				this.m_backFrame = value;
			}
		}

		internal ExpressionInfo ShadowIntensity
		{
			get
			{
				return this.m_shadowIntensity;
			}
			set
			{
				this.m_shadowIntensity = value;
			}
		}

		internal ExpressionInfo TextAntiAliasingQuality
		{
			get
			{
				return this.m_textAntiAliasingQuality;
			}
			set
			{
				this.m_textAntiAliasingQuality = value;
			}
		}

		internal TopImage TopImage
		{
			get
			{
				return this.m_topImage;
			}
			set
			{
				this.m_topImage = value;
			}
		}

		internal GaugePanelExprHost GaugePanelExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (this.m_exprHost == null)
				{
					return null;
				}
				return this.m_exprHost.UserSortExpressionsHost;
			}
		}

		internal GaugePanel(ReportItem parent)
			: base(parent)
		{
		}

		internal GaugePanel(int id, ReportItem parent)
			: base(id, parent)
		{
			base.RowCount = 1;
			base.ColumnCount = 1;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0 && (context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInDetailList, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			else
			{
				if (!context.RegisterDataRegion(this))
				{
					return false;
				}
				context.Location |= (AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet | AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
				context.ExprHostBuilder.DataRegionStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel, base.m_name);
				base.Initialize(context);
				base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel);
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		protected override void InitializeCorner(InitializationContext context)
		{
			if (this.GaugeRow != null)
			{
				this.GaugeRow.Initialize(context);
			}
			if (this.m_linearGauges != null)
			{
				for (int i = 0; i < this.m_linearGauges.Count; i++)
				{
					this.m_linearGauges[i].Initialize(context);
				}
			}
			if (this.m_radialGauges != null)
			{
				for (int j = 0; j < this.m_radialGauges.Count; j++)
				{
					this.m_radialGauges[j].Initialize(context);
				}
			}
			if (this.m_numericIndicators != null)
			{
				for (int k = 0; k < this.m_numericIndicators.Count; k++)
				{
					this.m_numericIndicators[k].Initialize(context);
				}
			}
			if (this.m_stateIndicators != null)
			{
				for (int l = 0; l < this.m_stateIndicators.Count; l++)
				{
					this.m_stateIndicators[l].Initialize(context);
				}
			}
			if (this.m_gaugeImages != null)
			{
				for (int m = 0; m < this.m_gaugeImages.Count; m++)
				{
					this.m_gaugeImages[m].Initialize(context);
				}
			}
			if (this.m_gaugeLabels != null)
			{
				for (int n = 0; n < this.m_gaugeLabels.Count; n++)
				{
					this.m_gaugeLabels[n].Initialize(context);
				}
			}
			if (this.m_antiAliasing != null)
			{
				this.m_antiAliasing.Initialize("AntiAliasing", context);
				context.ExprHostBuilder.GaugePanelAntiAliasing(this.m_antiAliasing);
			}
			if (this.m_autoLayout != null)
			{
				this.m_autoLayout.Initialize("AutoLayout", context);
				context.ExprHostBuilder.GaugePanelAutoLayout(this.m_autoLayout);
			}
			if (this.m_backFrame != null)
			{
				this.m_backFrame.Initialize(context);
			}
			if (this.m_shadowIntensity != null)
			{
				this.m_shadowIntensity.Initialize("ShadowIntensity", context);
				context.ExprHostBuilder.GaugePanelShadowIntensity(this.m_shadowIntensity);
			}
			if (this.m_textAntiAliasingQuality != null)
			{
				this.m_textAntiAliasingQuality.Initialize("TextAntiAliasingQuality", context);
				context.ExprHostBuilder.GaugePanelTextAntiAliasingQuality(this.m_textAntiAliasingQuality);
			}
			if (this.m_topImage != null)
			{
				this.m_topImage.Initialize(context);
			}
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			return true;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugePanel gaugePanel = (GaugePanel)(context.CurrentDataRegionClone = (GaugePanel)base.PublishClone(context));
			gaugePanel.m_rows = new GaugeRowList();
			gaugePanel.m_rowMembers = new GaugeMemberList();
			gaugePanel.m_columnMembers = new GaugeMemberList();
			if (this.GaugeMember != null)
			{
				gaugePanel.GaugeMember = (GaugeMember)this.GaugeMember.PublishClone(context, gaugePanel);
			}
			if (this.GaugeRowMember != null)
			{
				gaugePanel.GaugeRowMember = (GaugeMember)this.GaugeRowMember.PublishClone(context);
			}
			if (this.GaugeRow != null)
			{
				gaugePanel.GaugeRow = (GaugeRow)this.GaugeRow.PublishClone(context);
			}
			if (this.m_linearGauges != null)
			{
				gaugePanel.m_linearGauges = new List<LinearGauge>(this.m_linearGauges.Count);
				foreach (LinearGauge linearGauge in this.m_linearGauges)
				{
					gaugePanel.m_linearGauges.Add((LinearGauge)linearGauge.PublishClone(context));
				}
			}
			if (this.m_radialGauges != null)
			{
				gaugePanel.m_radialGauges = new List<RadialGauge>(this.m_radialGauges.Count);
				foreach (RadialGauge radialGauge in this.m_radialGauges)
				{
					gaugePanel.m_radialGauges.Add((RadialGauge)radialGauge.PublishClone(context));
				}
			}
			if (this.m_numericIndicators != null)
			{
				gaugePanel.m_numericIndicators = new List<NumericIndicator>(this.m_numericIndicators.Count);
				foreach (NumericIndicator numericIndicator in this.m_numericIndicators)
				{
					gaugePanel.m_numericIndicators.Add((NumericIndicator)numericIndicator.PublishClone(context));
				}
			}
			if (this.m_stateIndicators != null)
			{
				gaugePanel.m_stateIndicators = new List<StateIndicator>(this.m_stateIndicators.Count);
				foreach (StateIndicator stateIndicator in this.m_stateIndicators)
				{
					gaugePanel.m_stateIndicators.Add((StateIndicator)stateIndicator.PublishClone(context));
				}
			}
			if (this.m_gaugeImages != null)
			{
				gaugePanel.m_gaugeImages = new List<GaugeImage>(this.m_gaugeImages.Count);
				foreach (GaugeImage gaugeImage in this.m_gaugeImages)
				{
					gaugePanel.m_gaugeImages.Add((GaugeImage)gaugeImage.PublishClone(context));
				}
			}
			if (this.m_gaugeLabels != null)
			{
				gaugePanel.m_gaugeLabels = new List<GaugeLabel>(this.m_gaugeLabels.Count);
				foreach (GaugeLabel gaugeLabel in this.m_gaugeLabels)
				{
					gaugePanel.m_gaugeLabels.Add((GaugeLabel)gaugeLabel.PublishClone(context));
				}
			}
			if (this.m_antiAliasing != null)
			{
				gaugePanel.m_antiAliasing = (ExpressionInfo)this.m_antiAliasing.PublishClone(context);
			}
			if (this.m_autoLayout != null)
			{
				gaugePanel.m_autoLayout = (ExpressionInfo)this.m_autoLayout.PublishClone(context);
			}
			if (this.m_backFrame != null)
			{
				gaugePanel.m_backFrame = (BackFrame)this.m_backFrame.PublishClone(context);
			}
			if (this.m_shadowIntensity != null)
			{
				gaugePanel.m_shadowIntensity = (ExpressionInfo)this.m_shadowIntensity.PublishClone(context);
			}
			if (this.m_textAntiAliasingQuality != null)
			{
				gaugePanel.m_textAntiAliasingQuality = (ExpressionInfo)this.m_textAntiAliasingQuality.PublishClone(context);
			}
			if (this.m_topImage != null)
			{
				gaugePanel.m_topImage = (TopImage)this.m_topImage.PublishClone(context);
			}
			return gaugePanel;
		}

		internal GaugeAntiAliasings EvaluateAntiAliasing(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateGaugeAntiAliasings(context.ReportRuntime.EvaluateGaugePanelAntiAliasingExpression(this, base.Name), context.ReportRuntime);
		}

		internal bool EvaluateAutoLayout(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelAutoLayoutExpression(this, base.Name);
		}

		internal double EvaluateShadowIntensity(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelShadowIntensityExpression(this, base.Name);
		}

		internal TextAntiAliasingQualities EvaluateTextAntiAliasingQuality(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateTextAntiAliasingQualities(context.ReportRuntime.EvaluateGaugePanelTextAntiAliasingQualityExpression(this, base.Name), context.ReportRuntime);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeRowMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeRow));
			list.Add(new MemberInfo(MemberName.LinearGauges, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearGauge));
			list.Add(new MemberInfo(MemberName.RadialGauges, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialGauge));
			list.Add(new MemberInfo(MemberName.NumericIndicators, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicator));
			list.Add(new MemberInfo(MemberName.StateIndicators, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StateIndicator));
			list.Add(new MemberInfo(MemberName.GaugeImages, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeImage));
			list.Add(new MemberInfo(MemberName.GaugeLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeLabel));
			list.Add(new MemberInfo(MemberName.AntiAliasing, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AutoLayout, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BackFrame, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BackFrame));
			list.Add(new MemberInfo(MemberName.ShadowIntensity, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextAntiAliasingQuality, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TopImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage));
			list.Add(new MemberInfo(MemberName.ColumnMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new MemberInfo(MemberName.RowMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new MemberInfo(MemberName.Rows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeRow));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
		}

		internal List<GaugeInputValue> GetGaugeInputValues()
		{
			List<GaugeInputValue> list = new List<GaugeInputValue>();
			if (this.RadialGauges != null)
			{
				foreach (RadialGauge radialGauge in this.RadialGauges)
				{
					if (radialGauge.GaugeScales != null)
					{
						foreach (RadialScale gaugeScale in radialGauge.GaugeScales)
						{
							if (gaugeScale.MaximumValue != null)
							{
								list.Add(gaugeScale.MaximumValue);
							}
							if (gaugeScale.MinimumValue != null)
							{
								list.Add(gaugeScale.MinimumValue);
							}
							if (gaugeScale.GaugePointers != null)
							{
								foreach (RadialPointer gaugePointer in gaugeScale.GaugePointers)
								{
									if (gaugePointer.GaugeInputValue != null)
									{
										list.Add(gaugePointer.GaugeInputValue);
									}
								}
							}
							if (gaugeScale.ScaleRanges != null)
							{
								foreach (ScaleRange scaleRange in gaugeScale.ScaleRanges)
								{
									if (scaleRange.StartValue != null)
									{
										list.Add(scaleRange.StartValue);
									}
									if (scaleRange.EndValue != null)
									{
										list.Add(scaleRange.EndValue);
									}
								}
							}
						}
					}
				}
			}
			if (this.LinearGauges != null)
			{
				foreach (LinearGauge linearGauge in this.LinearGauges)
				{
					if (linearGauge.GaugeScales != null)
					{
						foreach (LinearScale gaugeScale2 in linearGauge.GaugeScales)
						{
							if (gaugeScale2.MaximumValue != null)
							{
								list.Add(gaugeScale2.MaximumValue);
							}
							if (gaugeScale2.MinimumValue != null)
							{
								list.Add(gaugeScale2.MinimumValue);
							}
							if (gaugeScale2.GaugePointers != null)
							{
								foreach (LinearPointer gaugePointer2 in gaugeScale2.GaugePointers)
								{
									if (gaugePointer2.GaugeInputValue != null)
									{
										list.Add(gaugePointer2.GaugeInputValue);
									}
								}
							}
							if (gaugeScale2.ScaleRanges != null)
							{
								foreach (ScaleRange scaleRange2 in gaugeScale2.ScaleRanges)
								{
									if (scaleRange2.StartValue != null)
									{
										list.Add(scaleRange2.StartValue);
									}
									if (scaleRange2.EndValue != null)
									{
										list.Add(scaleRange2.EndValue);
									}
								}
							}
						}
					}
				}
			}
			if (this.NumericIndicators != null)
			{
				foreach (NumericIndicator numericIndicator in this.NumericIndicators)
				{
					if (numericIndicator.GaugeInputValue != null)
					{
						list.Add(numericIndicator.GaugeInputValue);
					}
					if (numericIndicator.MaximumValue != null)
					{
						list.Add(numericIndicator.MaximumValue);
					}
					if (numericIndicator.MinimumValue != null)
					{
						list.Add(numericIndicator.MinimumValue);
					}
					if (numericIndicator.NumericIndicatorRanges != null)
					{
						foreach (NumericIndicatorRange numericIndicatorRange in numericIndicator.NumericIndicatorRanges)
						{
							if (numericIndicatorRange.StartValue != null)
							{
								list.Add(numericIndicatorRange.StartValue);
							}
							if (numericIndicatorRange.EndValue != null)
							{
								list.Add(numericIndicatorRange.EndValue);
							}
						}
					}
				}
			}
			if (this.StateIndicators != null)
			{
				{
					foreach (StateIndicator stateIndicator in this.StateIndicators)
					{
						if (stateIndicator.GaugeInputValue != null)
						{
							list.Add(stateIndicator.GaugeInputValue);
						}
						if (stateIndicator.MinimumValue != null)
						{
							list.Add(stateIndicator.MinimumValue);
						}
						if (stateIndicator.MaximumValue != null)
						{
							list.Add(stateIndicator.MaximumValue);
						}
						if (stateIndicator.IndicatorStates != null)
						{
							foreach (IndicatorState indicatorState in stateIndicator.IndicatorStates)
							{
								if (indicatorState.StartValue != null)
								{
									list.Add(indicatorState.StartValue);
								}
								if (indicatorState.EndValue != null)
								{
									list.Add(indicatorState.EndValue);
								}
							}
						}
					}
					return list;
				}
			}
			return list;
		}

		internal int GenerateActionOwnerID()
		{
			return ++this.m_actionOwnerCounter;
		}

		public override void CreateDomainScopeMember(ReportHierarchyNode parentNode, Grouping grouping, AutomaticSubtotalContext context)
		{
			GaugeMember gaugeMember = new GaugeMember(context.GenerateID(), this);
			gaugeMember.Grouping = grouping.CloneForDomainScope(context, gaugeMember);
			HierarchyNodeList hierarchyNodeList = (parentNode != null) ? parentNode.InnerHierarchy : this.ColumnMembers;
			if (hierarchyNodeList != null)
			{
				hierarchyNodeList.Add(gaugeMember);
				gaugeMember.IsColumn = true;
				this.GaugeRow.Cells.Insert(this.ColumnMembers.GetMemberIndex(gaugeMember), new GaugeCell(context.GenerateID(), this));
				base.ColumnCount++;
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugePanel.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LinearGauges:
					writer.Write(this.m_linearGauges);
					break;
				case MemberName.RadialGauges:
					writer.Write(this.m_radialGauges);
					break;
				case MemberName.NumericIndicators:
					writer.Write(this.m_numericIndicators);
					break;
				case MemberName.StateIndicators:
					writer.Write(this.m_stateIndicators);
					break;
				case MemberName.GaugeImages:
					writer.Write(this.m_gaugeImages);
					break;
				case MemberName.GaugeLabels:
					writer.Write(this.m_gaugeLabels);
					break;
				case MemberName.AntiAliasing:
					writer.Write(this.m_antiAliasing);
					break;
				case MemberName.AutoLayout:
					writer.Write(this.m_autoLayout);
					break;
				case MemberName.BackFrame:
					writer.Write(this.m_backFrame);
					break;
				case MemberName.ShadowIntensity:
					writer.Write(this.m_shadowIntensity);
					break;
				case MemberName.TextAntiAliasingQuality:
					writer.Write(this.m_textAntiAliasingQuality);
					break;
				case MemberName.TopImage:
					writer.Write(this.m_topImage);
					break;
				case MemberName.ColumnMembers:
					writer.Write(this.m_columnMembers);
					break;
				case MemberName.RowMembers:
					writer.Write(this.m_rowMembers);
					break;
				case MemberName.Rows:
					writer.Write(this.m_rows);
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
			reader.RegisterDeclaration(GaugePanel.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeMember:
					this.GaugeMember = (GaugeMember)reader.ReadRIFObject();
					break;
				case MemberName.GaugeRowMember:
					this.GaugeRowMember = (GaugeMember)reader.ReadRIFObject();
					break;
				case MemberName.GaugeRow:
					this.GaugeRow = (GaugeRow)reader.ReadRIFObject();
					break;
				case MemberName.LinearGauges:
					this.m_linearGauges = reader.ReadGenericListOfRIFObjects<LinearGauge>();
					break;
				case MemberName.RadialGauges:
					this.m_radialGauges = reader.ReadGenericListOfRIFObjects<RadialGauge>();
					break;
				case MemberName.NumericIndicators:
					this.m_numericIndicators = reader.ReadGenericListOfRIFObjects<NumericIndicator>();
					break;
				case MemberName.StateIndicators:
					this.m_stateIndicators = reader.ReadGenericListOfRIFObjects<StateIndicator>();
					break;
				case MemberName.GaugeImages:
					this.m_gaugeImages = reader.ReadGenericListOfRIFObjects<GaugeImage>();
					break;
				case MemberName.GaugeLabels:
					this.m_gaugeLabels = reader.ReadGenericListOfRIFObjects<GaugeLabel>();
					break;
				case MemberName.AntiAliasing:
					this.m_antiAliasing = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AutoLayout:
					this.m_autoLayout = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BackFrame:
					this.m_backFrame = (BackFrame)reader.ReadRIFObject();
					break;
				case MemberName.ShadowIntensity:
					this.m_shadowIntensity = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextAntiAliasingQuality:
					this.m_textAntiAliasingQuality = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TopImage:
					this.m_topImage = (TopImage)reader.ReadRIFObject();
					break;
				case MemberName.ColumnMembers:
					this.m_columnMembers = reader.ReadListOfRIFObjects<GaugeMemberList>();
					break;
				case MemberName.RowMembers:
					this.m_rowMembers = reader.ReadListOfRIFObjects<GaugeMemberList>();
					break;
				case MemberName.Rows:
					this.m_rows = reader.ReadListOfRIFObjects<GaugeRowList>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.GaugePanelHostsRemotable[base.ExprHostID];
				base.DataRegionSetExprHost(this.m_exprHost, this.m_exprHost.SortHost, this.m_exprHost.FilterHostsRemotable, this.m_exprHost.UserSortExpressionsHost, this.m_exprHost.PageBreakExprHost, this.m_exprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (this.m_exprHost != null)
			{
				IList<LinearGaugeExprHost> linearGaugesHostsRemotable = this.m_exprHost.LinearGaugesHostsRemotable;
				if (this.m_linearGauges != null && linearGaugesHostsRemotable != null)
				{
					for (int i = 0; i < this.m_linearGauges.Count; i++)
					{
						LinearGauge linearGauge = this.m_linearGauges[i];
						if (linearGauge != null && linearGauge.ExpressionHostID > -1)
						{
							linearGauge.SetExprHost(linearGaugesHostsRemotable[linearGauge.ExpressionHostID], reportObjectModel);
						}
					}
				}
				IList<RadialGaugeExprHost> radialGaugesHostsRemotable = this.m_exprHost.RadialGaugesHostsRemotable;
				if (this.m_radialGauges != null && radialGaugesHostsRemotable != null)
				{
					for (int j = 0; j < this.m_radialGauges.Count; j++)
					{
						RadialGauge radialGauge = this.m_radialGauges[j];
						if (radialGauge != null && radialGauge.ExpressionHostID > -1)
						{
							radialGauge.SetExprHost(radialGaugesHostsRemotable[radialGauge.ExpressionHostID], reportObjectModel);
						}
					}
				}
				IList<NumericIndicatorExprHost> numericIndicatorsHostsRemotable = this.m_exprHost.NumericIndicatorsHostsRemotable;
				if (this.m_numericIndicators != null && numericIndicatorsHostsRemotable != null)
				{
					for (int k = 0; k < this.m_numericIndicators.Count; k++)
					{
						NumericIndicator numericIndicator = this.m_numericIndicators[k];
						if (numericIndicator != null && numericIndicator.ExpressionHostID > -1)
						{
							numericIndicator.SetExprHost(numericIndicatorsHostsRemotable[numericIndicator.ExpressionHostID], reportObjectModel);
						}
					}
				}
				IList<StateIndicatorExprHost> stateIndicatorsHostsRemotable = this.m_exprHost.StateIndicatorsHostsRemotable;
				if (this.m_stateIndicators != null && stateIndicatorsHostsRemotable != null)
				{
					for (int l = 0; l < this.m_stateIndicators.Count; l++)
					{
						StateIndicator stateIndicator = this.m_stateIndicators[l];
						if (stateIndicator != null && stateIndicator.ExpressionHostID > -1)
						{
							stateIndicator.SetExprHost(stateIndicatorsHostsRemotable[stateIndicator.ExpressionHostID], reportObjectModel);
						}
					}
				}
				IList<GaugeImageExprHost> gaugeImagesHostsRemotable = this.m_exprHost.GaugeImagesHostsRemotable;
				if (this.m_gaugeImages != null && gaugeImagesHostsRemotable != null)
				{
					for (int m = 0; m < this.m_gaugeImages.Count; m++)
					{
						GaugeImage gaugeImage = this.m_gaugeImages[m];
						if (gaugeImage != null && gaugeImage.ExpressionHostID > -1)
						{
							gaugeImage.SetExprHost(gaugeImagesHostsRemotable[gaugeImage.ExpressionHostID], reportObjectModel);
						}
					}
				}
				IList<GaugeLabelExprHost> gaugeLabelsHostsRemotable = this.m_exprHost.GaugeLabelsHostsRemotable;
				if (this.m_gaugeLabels != null && gaugeLabelsHostsRemotable != null)
				{
					for (int n = 0; n < this.m_gaugeLabels.Count; n++)
					{
						GaugeLabel gaugeLabel = this.m_gaugeLabels[n];
						if (gaugeLabel != null && gaugeLabel.ExpressionHostID > -1)
						{
							gaugeLabel.SetExprHost(gaugeLabelsHostsRemotable[gaugeLabel.ExpressionHostID], reportObjectModel);
						}
					}
				}
				if (this.m_backFrame != null && this.m_exprHost.BackFrameHost != null)
				{
					this.m_backFrame.SetExprHost(this.m_exprHost.BackFrameHost, reportObjectModel);
				}
				if (this.m_topImage != null && this.m_exprHost.TopImageHost != null)
				{
					this.m_topImage.SetExprHost(this.m_exprHost.TopImageHost, reportObjectModel);
				}
				IList<GaugeCellExprHost> cellHostsRemotable = this.m_exprHost.CellHostsRemotable;
				if (cellHostsRemotable != null && this.GaugeRow != null && cellHostsRemotable.Count > 0 && this.GaugeRow.GaugeCell != null)
				{
					this.GaugeRow.GaugeCell.SetExprHost(cellHostsRemotable[0], reportObjectModel);
				}
			}
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return this.m_exprHost.NoRowsExpr;
		}
	}
}
