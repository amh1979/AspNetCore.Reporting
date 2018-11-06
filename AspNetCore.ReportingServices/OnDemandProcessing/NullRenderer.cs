using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class NullRenderer
	{
		private OnDemandProcessingContext m_odpContext;

		private DocumentMapWriter m_docMapWriter;

		private Stream m_docMapStream;

		private bool m_generateDocMap;

		private bool m_createSnapshot;

		private AspNetCore.ReportingServices.OnDemandReportRendering.Report m_report;

		internal Stream DocumentMapStream
		{
			get
			{
				return this.m_docMapStream;
			}
		}

		internal NullRenderer()
		{
		}

		internal void Process(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, OnDemandProcessingContext odpContext, bool generateDocumentMap, bool createSnapshot)
		{
			this.m_odpContext = odpContext;
			this.m_report = report;
			this.m_generateDocMap = (generateDocumentMap && this.m_report.HasDocumentMap);
			this.m_createSnapshot = createSnapshot;
			if (this.m_generateDocMap)
			{
				odpContext.HasRenderFormatDependencyInDocumentMap = false;
			}
			if (this.m_generateDocMap || this.m_createSnapshot)
			{
				foreach (AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection in report.ReportSections)
				{
					this.Visit(reportSection);
				}
			}
			if (this.m_generateDocMap && this.m_docMapWriter != null)
			{
				this.m_docMapWriter.WriteEndContainer();
				this.m_docMapWriter.Close();
				this.m_docMapWriter = null;
				if (odpContext.HasRenderFormatDependencyInDocumentMap)
				{
					odpContext.OdpMetadata.ReportSnapshot.SetRenderFormatDependencyInDocumentMap(odpContext);
				}
			}
		}

		private void Visit(AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection section)
		{
			this.Visit(section.Body.ReportItemCollection);
			this.VisitStyle(section.Body.Style);
			this.VisitStyle(section.Page.Style);
		}

		private void Visit(AspNetCore.ReportingServices.OnDemandReportRendering.ReportItemCollection itemCollection)
		{
			for (int i = 0; i < itemCollection.Count; i++)
			{
				this.Visit(((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportItem>)itemCollection)[i]);
			}
		}

		private void Visit(AspNetCore.ReportingServices.OnDemandReportRendering.ReportItem item)
		{
			if (item != null && item.Instance != null)
			{
				bool generateDocMap = this.m_generateDocMap;
				if (this.ProcessVisibilityAndContinue(item.Visibility, item.Instance.Visibility, null))
				{
					if (item is AspNetCore.ReportingServices.OnDemandReportRendering.Line || item is AspNetCore.ReportingServices.OnDemandReportRendering.Chart || item is AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel || item is AspNetCore.ReportingServices.OnDemandReportRendering.Map)
					{
						this.GenerateSimpleReportItemDocumentMap(item);
					}
					else if (item is AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)
					{
						this.GenerateSimpleReportItemDocumentMap(item);
						this.VisitStyle(item.Style);
					}
					else if (item is AspNetCore.ReportingServices.OnDemandReportRendering.Image)
					{
						this.GenerateSimpleReportItemDocumentMap(item);
						AspNetCore.ReportingServices.OnDemandReportRendering.Image image = item as AspNetCore.ReportingServices.OnDemandReportRendering.Image;
						AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType source = image.Source;
						if (this.m_createSnapshot && (source == AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.External || source == AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Database))
						{
							AspNetCore.ReportingServices.OnDemandReportRendering.ImageInstance imageInstance = image.Instance as AspNetCore.ReportingServices.OnDemandReportRendering.ImageInstance;
							if (imageInstance != null)
							{
								byte[] imageDatum = imageInstance.ImageData;
							}
						}
					}
					else if (item is AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)
					{
						this.VisitRectangle(item as AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle);
						this.VisitStyle(item.Style);
					}
					else if (!(item is AspNetCore.ReportingServices.OnDemandReportRendering.CustomReportItem))
					{
						bool flag = false;
						if (this.m_generateDocMap)
						{
							string documentMapLabel = item.Instance.DocumentMapLabel;
							if (documentMapLabel != null)
							{
								flag = true;
								this.WriteDocumentMapBeginContainer(documentMapLabel, item.Instance.UniqueName);
							}
						}
						if (item is AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)
						{
							this.VisitTablix(item as AspNetCore.ReportingServices.OnDemandReportRendering.Tablix);
							this.VisitStyle(item.Style);
						}
						else if (item is AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)
						{
							this.VisitSubReport(item as AspNetCore.ReportingServices.OnDemandReportRendering.SubReport);
						}
						else
						{
							Global.Tracer.Assert(false);
						}
						if (flag)
						{
							this.WriteDocumentMapEndContainer();
						}
					}
					this.m_generateDocMap = generateDocMap;
				}
			}
		}

		private void GenerateSimpleReportItemDocumentMap(AspNetCore.ReportingServices.OnDemandReportRendering.ReportItem item)
		{
			if (this.m_generateDocMap)
			{
				string documentMapLabel = item.Instance.DocumentMapLabel;
				if (documentMapLabel != null)
				{
					this.WriteDocumentMapNode(documentMapLabel, item.Instance.UniqueName);
				}
			}
		}

		private void VisitRectangle(AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle rectangleDef)
		{
			bool flag = false;
			if (this.m_generateDocMap)
			{
				string documentMapLabel = rectangleDef.Instance.DocumentMapLabel;
				if (documentMapLabel != null)
				{
					flag = true;
					string text = null;
					int linkToChild = rectangleDef.LinkToChild;
					if (linkToChild >= 0)
					{
						AspNetCore.ReportingServices.OnDemandReportRendering.ReportItem reportItem = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportItem>)rectangleDef.ReportItemCollection)[linkToChild];
						text = reportItem.Instance.UniqueName;
					}
					else
					{
						text = rectangleDef.Instance.UniqueName;
					}
					this.WriteDocumentMapBeginContainer(documentMapLabel, text);
				}
			}
			this.Visit(rectangleDef.ReportItemCollection);
			if (flag)
			{
				this.WriteDocumentMapEndContainer();
			}
		}

		private void VisitSubReport(AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subreportDef)
		{
			if (subreportDef.Report != null && subreportDef.Instance != null && !subreportDef.ProcessedWithError)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Report report = subreportDef.Report;
				if (!report.HasDocumentMap && !this.m_createSnapshot)
				{
					return;
				}
				foreach (AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection in report.ReportSections)
				{
					this.Visit(reportSection.Body.ReportItemCollection);
					this.VisitStyle(reportSection.Body.Style);
				}
			}
		}

		private void VisitTablix(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablixDef)
		{
			if (tablixDef.Corner != null)
			{
				TablixCornerRowCollection rowCollection = tablixDef.Corner.RowCollection;
				for (int i = 0; i < rowCollection.Count; i++)
				{
					TablixCornerRow tablixCornerRow = ((ReportElementCollectionBase<TablixCornerRow>)rowCollection)[i];
					if (tablixCornerRow != null)
					{
						for (int j = 0; j < tablixCornerRow.Count; j++)
						{
							AspNetCore.ReportingServices.OnDemandReportRendering.TablixCornerCell tablixCornerCell = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TablixCornerCell>)tablixCornerRow)[j];
							if (tablixCornerCell != null)
							{
								this.Visit(tablixCornerCell.CellContents.ReportItem);
							}
						}
					}
				}
			}
			this.VisitTablixMemberCollection(tablixDef.ColumnHierarchy.MemberCollection, -1, true);
			this.VisitTablixMemberCollection(tablixDef.RowHierarchy.MemberCollection, -1, true);
		}

		private void VisitTablixMemberCollection(TablixMemberCollection memberCollection, int rowMemberIndex, bool isTopLevel)
		{
			if (memberCollection != null)
			{
				for (int i = 0; i < memberCollection.Count; i++)
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.TablixMember tablixMember = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TablixMember>)memberCollection)[i];
					if (tablixMember.IsStatic)
					{
						this.VisitTablixMember(tablixMember, rowMemberIndex, null);
					}
					else
					{
						TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
						Stack<int> stack = new Stack<int>();
						if (isTopLevel)
						{
							tablixDynamicMemberInstance.ResetContext();
						}
						while (tablixDynamicMemberInstance.MoveNext())
						{
							this.VisitTablixMember(tablixMember, rowMemberIndex, stack);
						}
						for (int j = 0; j < stack.Count; j++)
						{
							this.WriteDocumentMapEndContainer();
						}
					}
				}
			}
		}

		private void VisitTablixMember(AspNetCore.ReportingServices.OnDemandReportRendering.TablixMember memberDef, int rowMemberIndex, Stack<int> openRecursiveLevels)
		{
			if (memberDef.Instance != null)
			{
				bool generateDocMap = this.m_generateDocMap;
				if (this.ProcessVisibilityAndContinue(memberDef.Visibility, memberDef.Instance.Visibility, memberDef))
				{
					if (!memberDef.IsStatic && rowMemberIndex == -1 && memberDef.Group != null && this.m_generateDocMap)
					{
						GroupInstance instance = memberDef.Group.Instance;
						string documentMapLabel = instance.DocumentMapLabel;
						int recursiveLevel = instance.RecursiveLevel;
						if (documentMapLabel != null)
						{
							while (openRecursiveLevels.Count > 0 && openRecursiveLevels.Peek() >= recursiveLevel)
							{
								this.WriteDocumentMapEndContainer();
								openRecursiveLevels.Pop();
							}
							this.WriteDocumentMapBeginContainer(documentMapLabel, memberDef.Group.Instance.UniqueName);
							openRecursiveLevels.Push(recursiveLevel);
						}
					}
					if (rowMemberIndex == -1 && memberDef.TablixHeader != null && memberDef.TablixHeader.CellContents != null)
					{
						this.Visit(memberDef.TablixHeader.CellContents.ReportItem);
					}
					if (memberDef.Children == null)
					{
						if (memberDef.IsColumn)
						{
							if (rowMemberIndex != -1)
							{
								AspNetCore.ReportingServices.OnDemandReportRendering.TablixCell tablixCell = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TablixCell>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TablixRow>)memberDef.OwnerTablix.Body.RowCollection)[rowMemberIndex])[memberDef.MemberCellIndex];
								if (tablixCell != null && tablixCell.CellContents != null)
								{
									this.Visit(tablixCell.CellContents.ReportItem);
								}
							}
						}
						else
						{
							this.VisitTablixMemberCollection(memberDef.OwnerTablix.ColumnHierarchy.MemberCollection, memberDef.MemberCellIndex, true);
						}
					}
					else
					{
						this.VisitTablixMemberCollection(memberDef.Children, rowMemberIndex, false);
					}
					this.m_generateDocMap = generateDocMap;
				}
			}
		}

		private void VisitStyle(AspNetCore.ReportingServices.OnDemandReportRendering.Style style)
		{
			if (style != null && this.m_createSnapshot)
			{
				BackgroundImage backgroundImage = style.BackgroundImage;
				if (backgroundImage != null && backgroundImage.Source != AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded && backgroundImage.Instance != null)
				{
					BackgroundImageInstance instance = backgroundImage.Instance;
					byte[] imageDatum = instance.ImageData;
				}
			}
		}

		private bool ProcessVisibilityAndContinue(AspNetCore.ReportingServices.OnDemandReportRendering.Visibility aVisibility, VisibilityInstance aVisibilityInstance, AspNetCore.ReportingServices.OnDemandReportRendering.TablixMember memberDef)
		{
			if (aVisibility == null)
			{
				return true;
			}
			if (aVisibilityInstance != null && this.m_createSnapshot)
			{
				bool startHidden = aVisibilityInstance.StartHidden;
			}
			switch (aVisibility.HiddenState)
			{
			case SharedHiddenState.Always:
				if (this.m_createSnapshot)
				{
					this.m_generateDocMap = false;
					return true;
				}
				return false;
			case SharedHiddenState.Sometimes:
				if (!aVisibilityInstance.CurrentlyHidden)
				{
					break;
				}
				if (aVisibility.ToggleItem != null)
				{
					break;
				}
				if (this.m_createSnapshot)
				{
					this.m_generateDocMap = false;
					return true;
				}
				return false;
			default:
				if (memberDef == null)
				{
					break;
				}
				if (!memberDef.IsTotal)
				{
					break;
				}
				if (this.m_createSnapshot)
				{
					this.m_generateDocMap = false;
					return true;
				}
				return false;
			}
			return true;
		}

		private void InitWriter()
		{
			this.m_docMapStream = this.m_odpContext.ChunkFactory.CreateChunk("DocumentMap", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Interactivity, null);
			this.m_docMapWriter = new DocumentMapWriter(this.m_docMapStream, this.m_odpContext);
			this.m_docMapWriter.WriteBeginContainer(this.m_report.Name, this.m_report.Instance.UniqueName);
		}

		private void WriteDocumentMapNode(string aLabel, string aId)
		{
			if (this.m_docMapWriter == null)
			{
				this.InitWriter();
			}
			this.m_docMapWriter.WriteNode(aLabel, aId);
		}

		private void WriteDocumentMapBeginContainer(string aLabel, string aId)
		{
			if (this.m_docMapWriter == null)
			{
				this.InitWriter();
			}
			this.m_docMapWriter.WriteBeginContainer(aLabel, aId);
		}

		private void WriteDocumentMapEndContainer()
		{
			if (this.m_docMapWriter == null)
			{
				this.InitWriter();
			}
			this.m_docMapWriter.WriteEndContainer();
		}
	}
}
