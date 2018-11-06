using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Diagnostics;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomReportItem : ReportItem, IDataRegion, IReportScope
	{
		private const AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes ChunkType = AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems;

		private int m_indexIntoParentCollectionDef = -1;

		private int m_memberCellDefinitionIndex;

		private CustomData m_data;

		private ReportItem m_altReportItem;

		private ReportItem m_generatedReportItem;

		private ReportItem m_exposeAs;

		private ReportSize m_dynamicWidth;

		private ReportSize m_dynamicHeight;

		public ReportSize DynamicWidth
		{
			set
			{
				this.m_dynamicWidth = value;
			}
		}

		public ReportSize DynamicHeight
		{
			set
			{
				this.m_dynamicHeight = value;
			}
		}

		public override ReportSize Width
		{
			get
			{
				return this.m_dynamicWidth ?? base.Width;
			}
		}

		public override ReportSize Height
		{
			get
			{
				return this.m_dynamicHeight ?? base.Height;
			}
		}

		public string Type
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.RenderCri.Type;
				}
				return this.CriDef.Type;
			}
		}

		internal bool HasCustomData
		{
			get
			{
				return this.m_data != null;
			}
		}

		public CustomData CustomData
		{
			get
			{
				if (this.m_data == null)
				{
					this.m_data = new CustomData(this);
				}
				return this.m_data;
			}
		}

		public ReportItem AltReportItem
		{
			get
			{
				if (this.m_altReportItem == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_altReportItem = ReportItem.CreateShim(this, 0, base.m_inSubtotal, this.RenderCri.AltReportItem, base.m_renderingContext);
					}
					else
					{
						this.m_altReportItem = ReportItem.CreateItem(this.ParentScope, base.ParentDefinitionPath, this.CriDef.AltReportItemIndexInParentCollectionDef, this.CriDef.AltReportItem, base.m_renderingContext);
					}
				}
				return this.m_altReportItem;
			}
		}

		public ReportItem GeneratedReportItem
		{
			get
			{
				return this.m_generatedReportItem;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.CustomReportItem RenderCri
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return (AspNetCore.ReportingServices.ReportRendering.CustomReportItem)base.m_renderReportItem;
				}
				return null;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance
		{
			get
			{
				if (this.CriDef.IsDataRegion)
				{
					return this.m_data;
				}
				return this.ParentScope.ReportScopeInstance;
			}
		}

		IRIFReportScope IReportScope.RIFReportScope
		{
			get
			{
				if (this.CriDef.IsDataRegion)
				{
					return this.CriDef;
				}
				return this.ParentScope.RIFReportScope;
			}
		}

		private IReportScope ParentScope
		{
			get
			{
				return base.ReportScope;
			}
		}

		bool IDataRegion.HasDataCells
		{
			get
			{
				if (this.m_data != null)
				{
					return this.m_data.HasDataRowCollection;
				}
				return false;
			}
		}

		IDataRegionRowCollection IDataRegion.RowCollection
		{
			get
			{
				if (this.m_data != null)
				{
					return this.m_data.RowCollection;
				}
				return null;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem CriDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem)base.m_reportItemDef;
			}
		}

		internal CustomReportItem(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
			this.m_indexIntoParentCollectionDef = indexIntoParentCollectionDef;
		}

		internal CustomReportItem(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.CustomReportItem renderCri, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderCri, renderingContext)
		{
		}

		internal bool Initialize(RenderingContext renderingContext)
		{
			this.m_exposeAs = null;
			if (renderingContext.IsRenderAsNativeCri(this.CriDef))
			{
				this.m_exposeAs = this;
			}
			else
			{
				if (!this.LoadGeneratedReportItemDefinition())
				{
					this.GenerateReportItemDefinition();
				}
				this.m_exposeAs = this.m_generatedReportItem;
			}
			return this.m_exposeAs != null;
		}

		internal override ReportItem ExposeAs(RenderingContext renderingContext)
		{
			Global.Tracer.Assert(this.m_exposeAs != null, "m_exposeAs != null");
			return this.m_exposeAs;
		}

		private bool LoadGeneratedReportItemDefinition()
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = base.RenderingContext.OdpContext.OdpMetadata.ReportSnapshot;
			string chunkName = default(string);
			if (!reportSnapshot.TryGetGeneratedReportItemChunkName(this.GetGeneratedDefinitionChunkKey(), out chunkName))
			{
				return false;
			}
			IChunkFactory chunkFactory = base.RenderingContext.OdpContext.ChunkFactory;
			string text = default(string);
			Stream chunk = chunkFactory.GetChunk(chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems, ChunkMode.Open, out text);
			if (chunk == null)
			{
				return false;
			}
			using (chunk)
			{
				IntermediateFormatReader intermediateFormatReader = new IntermediateFormatReader(chunk, new ProcessingRIFObjectCreator((AspNetCore.ReportingServices.ReportIntermediateFormat.IDOwner)base.m_reportItemDef.ParentInstancePath, base.m_reportItemDef.Parent));
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem = (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem)intermediateFormatReader.ReadRIFObject();
				Global.Tracer.Assert(!intermediateFormatReader.HasReferences, "!reader.HasReferences");
				reportItem.GlobalID = -this.CriDef.GlobalID;
				if (reportItem.StyleClass != null)
				{
					reportItem.StyleClass.InitializeForCRIGeneratedReportItem();
				}
				reportItem.Visibility = base.m_reportItemDef.Visibility;
				AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType = reportItem.ObjectType;
				if (objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Image)
				{
					Image image = new Image(this.ParentScope, base.ParentDefinitionPath, this.m_indexIntoParentCollectionDef, (AspNetCore.ReportingServices.ReportIntermediateFormat.Image)reportItem, base.RenderingContext);
					image.CriOwner = this;
					image.CriGenerationPhase = CriGenerationPhases.None;
					this.m_generatedReportItem = image;
				}
				else
				{
					Global.Tracer.Assert(false, "Unexpected CRI generated report item type: " + reportItem.ObjectType);
				}
			}
			return true;
		}

		private static string CreateChunkName()
		{
			return Guid.NewGuid().ToString("N");
		}

		private void GenerateReportItemDefinition()
		{
			this.m_generatedReportItem = null;
			ICustomReportItem controlInstance = base.RenderingContext.OdpContext.CriProcessingControls.GetControlInstance(this.CriDef.Type, base.RenderingContext.OdpContext.ExtFactory);
			if (controlInstance != null)
			{
				try
				{
					controlInstance.GenerateReportItemDefinition(this);
				}
				catch (Exception ex)
				{
					base.RenderingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIProcessingError, Severity.Warning, this.Name, this.Type);
					Global.Tracer.TraceException(TraceLevel.Error, RPRes.rsCRIProcessingError(this.Name, this.Type) + " " + ex.ToString());
					return;
				}
				if (this.m_generatedReportItem == null)
				{
					base.RenderingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemNull, Severity.Warning, this.CriDef.ObjectType, this.Name, this.Type);
				}
				else
				{
					this.m_generatedReportItem.ConstructReportItemDefinition();
					this.m_generatedReportItem.CriGenerationPhase = CriGenerationPhases.None;
					string text = CustomReportItem.CreateChunkName();
					OnDemandProcessingContext odpContext = base.RenderingContext.OdpContext;
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = odpContext.OdpMetadata.ReportSnapshot;
					IChunkFactory chunkFactory = odpContext.ChunkFactory;
					using (Stream stream = chunkFactory.CreateChunk(text, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems, null))
					{
						IntermediateFormatWriter intermediateFormatWriter = new IntermediateFormatWriter(stream, odpContext.GetActiveCompatibilityVersion());
						AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility visibility = this.m_generatedReportItem.ReportItemDef.Visibility;
						this.m_generatedReportItem.ReportItemDef.Visibility = null;
						intermediateFormatWriter.Write(this.m_generatedReportItem.ReportItemDef);
						this.m_generatedReportItem.ReportItemDef.Visibility = visibility;
						stream.Flush();
					}
					reportSnapshot.AddGeneratedReportItemChunkName(this.GetGeneratedDefinitionChunkKey(), text);
				}
			}
		}

		internal void EvaluateGeneratedReportItemInstance()
		{
			Global.Tracer.Assert(this.m_generatedReportItem.CriGenerationPhase == CriGenerationPhases.None);
			this.m_generatedReportItem.CriGenerationPhase = CriGenerationPhases.Instance;
			try
			{
				if (!this.LoadGeneratedReportItemInstance())
				{
					try
					{
						ICustomReportItem controlInstance = base.RenderingContext.OdpContext.CriProcessingControls.GetControlInstance(this.CriDef.Type, base.RenderingContext.OdpContext.ExtFactory);
						Global.Tracer.Assert(null != controlInstance, "(null != control)");
						controlInstance.EvaluateReportItemInstance(this);
						this.m_generatedReportItem.CompleteCriGeneratedInstanceEvaluation();
					}
					catch (Exception innerException)
					{
						throw new RenderingObjectModelException(ErrorCode.rsCRIProcessingError, innerException, this.Name, this.Type);
					}
					goto end_IL_0024;
				}
				return;
				end_IL_0024:;
			}
			finally
			{
				this.m_generatedReportItem.CriGenerationPhase = CriGenerationPhases.None;
			}
			this.SaveGeneratedReportItemInstance();
		}

		private string GetGeneratedDefinitionChunkKey()
		{
			return this.DefinitionPath;
		}

		private string GetGeneratedInstanceChunkKey()
		{
			return this.GetGeneratedDefinitionChunkKey() + "_II_" + base.Instance.UniqueName;
		}

		private bool LoadGeneratedReportItemInstance()
		{
			Global.Tracer.Assert(this.m_generatedReportItem != null && this.m_generatedReportItem.Instance != null, "m_generatedReportItem != null && m_generatedReportItem.Instance != null");
			if (this.m_dynamicWidth == null && this.m_dynamicHeight == null)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = base.RenderingContext.OdpContext.OdpMetadata.ReportSnapshot;
				string text = default(string);
				if (this.CriDef.RepeatWith != null)
				{
					if (!reportSnapshot.TryGetImageChunkName(this.GetGeneratedInstanceChunkKey(), out text))
					{
						return false;
					}
					((ImageInstance)this.m_generatedReportItem.Instance).StreamName = text;
					return true;
				}
				if (!reportSnapshot.TryGetGeneratedReportItemChunkName(this.GetGeneratedInstanceChunkKey(), out text))
				{
					return false;
				}
				IChunkFactory chunkFactory = base.RenderingContext.OdpContext.ChunkFactory;
				string text2 = default(string);
				Stream chunk = chunkFactory.GetChunk(text, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems, ChunkMode.Open, out text2);
				if (chunk == null)
				{
					return false;
				}
				using (chunk)
				{
					ROMInstanceObjectCreator rOMInstanceObjectCreator = new ROMInstanceObjectCreator(this.m_generatedReportItem.Instance);
					IntermediateFormatReader intermediateFormatReader = new IntermediateFormatReader(chunk, rOMInstanceObjectCreator, rOMInstanceObjectCreator);
					IPersistable persistable = intermediateFormatReader.ReadRIFObject();
					Global.Tracer.Assert(persistable is ReportItemInstance, "reportItemInstance is ReportItemInstance");
					Global.Tracer.Assert(!intermediateFormatReader.HasReferences, "!reader.HasReferences");
				}
				return true;
			}
			return false;
		}

		private void SaveGeneratedReportItemInstance()
		{
			if (this.m_dynamicWidth == null && this.m_dynamicHeight == null)
			{
				OnDemandProcessingContext odpContext = base.RenderingContext.OdpContext;
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = odpContext.OdpMetadata.ReportSnapshot;
				IChunkFactory chunkFactory = odpContext.ChunkFactory;
				if (this.CriDef.RepeatWith == null)
				{
					string text = CustomReportItem.CreateChunkName();
					using (Stream stream = chunkFactory.CreateChunk(text, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems, null))
					{
						new IntermediateFormatWriter(stream, odpContext.GetActiveCompatibilityVersion()).Write(this.m_generatedReportItem.Instance);
						stream.Flush();
					}
					reportSnapshot.AddGeneratedReportItemChunkName(this.GetGeneratedInstanceChunkKey(), text);
				}
				else
				{
					ImageInstance imageInstance = (ImageInstance)this.m_generatedReportItem.Instance;
					string text = imageInstance.StreamName = ImageHelper.StoreImageDataInChunk(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, imageInstance.ImageData, imageInstance.MIMEType, base.RenderingContext.OdpContext.OdpMetadata, base.RenderingContext.OdpContext.ChunkFactory);
					reportSnapshot.AddImageChunkName(this.GetGeneratedInstanceChunkKey(), text);
				}
			}
		}

		public void CreateCriImageDefinition()
		{
			if (this.m_generatedReportItem != null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.Image image = new AspNetCore.ReportingServices.ReportIntermediateFormat.Image(-base.m_reportItemDef.ID, base.m_reportItemDef.Parent);
			image.ParentInstancePath = (AspNetCore.ReportingServices.ReportIntermediateFormat.IDOwner)base.m_reportItemDef.ParentInstancePath;
			image.GlobalID = -this.CriDef.GlobalID;
			image.Name = "Image";
			base.m_reportItemDef.SetupCriRenderItemDef(image);
			image.Source = Image.SourceType.Database;
			image.Action = new AspNetCore.ReportingServices.ReportIntermediateFormat.Action();
			Image image2 = new Image(this.ParentScope, base.ParentDefinitionPath, this.m_indexIntoParentCollectionDef, image, base.RenderingContext);
			image2.CriOwner = this;
			image2.CriGenerationPhase = CriGenerationPhases.Definition;
			this.m_generatedReportItem = image2;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new CustomReportItemInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_data != null)
			{
				this.m_data.SetNewContext();
			}
			if (this.m_altReportItem != null && base.m_isOldSnapshot)
			{
				this.m_altReportItem.SetNewContext();
			}
			if (this.m_generatedReportItem != null)
			{
				this.m_generatedReportItem.SetNewContext();
			}
		}

		internal override void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (renderReportItem != null)
			{
				this.m_altReportItem = null;
				this.m_data = null;
			}
			else
			{
				if (this.m_data != null && this.m_data.DataColumnHierarchy != null)
				{
					this.m_data.DataColumnHierarchy.ResetContext();
				}
				if (this.m_data != null && this.m_data.DataRowHierarchy != null)
				{
					this.m_data.DataRowHierarchy.ResetContext();
				}
			}
		}

		internal int GetCurrentMemberCellDefinitionIndex()
		{
			return this.m_memberCellDefinitionIndex;
		}

		internal int GetAndIncrementMemberCellDefinitionIndex()
		{
			return this.m_memberCellDefinitionIndex++;
		}

		internal void ResetMemberCellDefinitionIndex(int startIndex)
		{
			this.m_memberCellDefinitionIndex = startIndex;
		}
	}
}
