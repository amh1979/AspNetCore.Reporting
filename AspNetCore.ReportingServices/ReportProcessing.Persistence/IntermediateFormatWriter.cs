using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class IntermediateFormatWriter
	{
		private sealed class ReportServerBinaryWriter
		{
			private sealed class BinaryWriterWrapper : BinaryWriter
			{
				internal BinaryWriterWrapper(Stream stream)
					: base(stream, Encoding.Unicode)
				{
				}

				internal new void Write7BitEncodedInt(int int32Value)
				{
					base.Write7BitEncodedInt(int32Value);
				}
			}

			private BinaryWriterWrapper m_binaryWriter;

			internal ReportServerBinaryWriter(Stream stream)
			{
				this.m_binaryWriter = new BinaryWriterWrapper(stream);
			}

			internal void WriteGuid(Guid guid)
			{
				byte[] array = guid.ToByteArray();
				IntermediateFormatWriter.Assert(null != array);
				IntermediateFormatWriter.Assert(16 == array.Length);
				this.m_binaryWriter.Write((byte)239);
				this.m_binaryWriter.Write(array);
			}

			internal void WriteString(string stringValue)
			{
				if (stringValue == null)
				{
					this.WriteNull();
				}
				else
				{
					this.m_binaryWriter.Write((byte)240);
					this.m_binaryWriter.Write(stringValue);
				}
			}

			internal void WriteChar(char charValue)
			{
				this.m_binaryWriter.Write((byte)243);
				this.m_binaryWriter.Write(charValue);
			}

			internal void WriteBoolean(bool booleanValue)
			{
				this.m_binaryWriter.Write((byte)244);
				this.m_binaryWriter.Write(booleanValue);
			}

			internal void WriteInt16(short int16Value)
			{
				this.m_binaryWriter.Write((byte)245);
				this.m_binaryWriter.Write(int16Value);
			}

			internal void WriteInt32(int int32Value)
			{
				this.m_binaryWriter.Write((byte)246);
				this.m_binaryWriter.Write(int32Value);
			}

			internal void WriteInt64(long int64Value)
			{
				this.m_binaryWriter.Write((byte)247);
				this.m_binaryWriter.Write(int64Value);
			}

			internal void WriteUInt16(ushort uint16Value)
			{
				this.m_binaryWriter.Write((byte)248);
				this.m_binaryWriter.Write(uint16Value);
			}

			internal void WriteUInt32(uint uint32Value)
			{
				this.m_binaryWriter.Write((byte)249);
				this.m_binaryWriter.Write(uint32Value);
			}

			internal void WriteUInt64(ulong uint64Value)
			{
				this.m_binaryWriter.Write((byte)250);
				this.m_binaryWriter.Write(uint64Value);
			}

			internal void WriteByte(byte byteValue)
			{
				this.m_binaryWriter.Write((byte)251);
				this.m_binaryWriter.Write(byteValue);
			}

			internal void WriteSByte(sbyte sbyteValue)
			{
				this.m_binaryWriter.Write((byte)252);
				this.m_binaryWriter.Write(sbyteValue);
			}

			internal void WriteSingle(float singleValue)
			{
				this.m_binaryWriter.Write((byte)253);
				this.m_binaryWriter.Write(singleValue);
			}

			internal void WriteDouble(double doubleValue)
			{
				this.m_binaryWriter.Write((byte)254);
				this.m_binaryWriter.Write(doubleValue);
			}

			internal void WriteDecimal(decimal decimalValue)
			{
				this.m_binaryWriter.Write((byte)255);
				this.m_binaryWriter.Write(decimalValue);
			}

			internal void WriteDateTime(DateTime dateTimeValue)
			{
				this.m_binaryWriter.Write((byte)241);
				this.m_binaryWriter.Write(dateTimeValue.Ticks);
			}

			internal void WriteTimeSpan(TimeSpan timeSpanValue)
			{
				this.m_binaryWriter.Write((byte)242);
				this.m_binaryWriter.Write(timeSpanValue.Ticks);
			}

			internal void WriteBytes(byte[] bytesValue)
			{
				if (bytesValue == null)
				{
					this.WriteNull();
				}
				else
				{
					this.m_binaryWriter.Write((byte)5);
					this.m_binaryWriter.Write((byte)251);
					this.m_binaryWriter.Write7BitEncodedInt(bytesValue.Length);
					this.m_binaryWriter.Write(bytesValue);
				}
			}

			internal void WriteInt32s(int[] int32Values)
			{
				if (int32Values == null)
				{
					this.WriteNull();
				}
				else
				{
					this.m_binaryWriter.Write((byte)5);
					this.m_binaryWriter.Write((byte)246);
					this.m_binaryWriter.Write7BitEncodedInt(int32Values.Length);
					for (int i = 0; i < int32Values.Length; i++)
					{
						this.m_binaryWriter.Write(int32Values[i]);
					}
				}
			}

			internal void WriteFloatArray(float[] values)
			{
				if (values == null)
				{
					this.WriteNull();
				}
				else
				{
					this.m_binaryWriter.Write((byte)5);
					this.m_binaryWriter.Write((byte)253);
					this.m_binaryWriter.Write7BitEncodedInt(values.Length);
					for (int i = 0; i < values.Length; i++)
					{
						this.m_binaryWriter.Write(values[i]);
					}
				}
			}

			internal void WriteChars(char[] charsValue)
			{
				if (charsValue == null)
				{
					this.WriteNull();
				}
				else
				{
					this.m_binaryWriter.Write((byte)5);
					this.m_binaryWriter.Write((byte)243);
					this.m_binaryWriter.Write7BitEncodedInt(charsValue.Length);
					this.m_binaryWriter.Write(charsValue);
				}
			}

			internal void StartObject(ObjectType objectType)
			{
				this.m_binaryWriter.Write((byte)1);
				this.m_binaryWriter.Write7BitEncodedInt((int)objectType);
			}

			internal void EndObject()
			{
				this.m_binaryWriter.Write((byte)2);
			}

			internal void WriteNull()
			{
				this.m_binaryWriter.Write((byte)0);
			}

			internal void WriteReference(ObjectType objectType, int referenceValue)
			{
				this.m_binaryWriter.Write((byte)3);
				this.m_binaryWriter.Write7BitEncodedInt((int)objectType);
				this.m_binaryWriter.Write(referenceValue);
			}

			internal void WriteNoTypeReference(int referenceValue)
			{
				this.m_binaryWriter.Write((byte)3);
				this.m_binaryWriter.Write(referenceValue);
			}

			internal void WriteEnum(int enumValue)
			{
				this.m_binaryWriter.Write((byte)4);
				this.m_binaryWriter.Write7BitEncodedInt(enumValue);
			}

			internal void WriteDataFieldStatus(DataFieldStatus status)
			{
				this.m_binaryWriter.Write((byte)8);
				this.m_binaryWriter.Write7BitEncodedInt((int)status);
			}

			internal void StartArray(int count)
			{
				this.m_binaryWriter.Write((byte)6);
				this.m_binaryWriter.Write7BitEncodedInt(count);
			}

			internal void EndArray()
			{
			}

			internal void DeclareType(ObjectType objectType, Declaration declaration)
			{
				IntermediateFormatWriter.Assert(null != declaration);
				IntermediateFormatWriter.Assert(null != declaration.Members);
				this.m_binaryWriter.Write((byte)7);
				this.m_binaryWriter.Write7BitEncodedInt((int)objectType);
				this.m_binaryWriter.Write7BitEncodedInt((int)declaration.BaseType);
				this.m_binaryWriter.Write7BitEncodedInt(declaration.Members.Count);
				for (int i = 0; i < declaration.Members.Count; i++)
				{
					IntermediateFormatWriter.Assert(null != declaration.Members[i]);
					this.m_binaryWriter.Write7BitEncodedInt((int)declaration.Members[i].MemberName);
					this.m_binaryWriter.Write((byte)declaration.Members[i].Token);
					this.m_binaryWriter.Write7BitEncodedInt((int)declaration.Members[i].ObjectType);
				}
			}
		}

		private ReportServerBinaryWriter m_writer;

		private bool m_writeDeclarations;

		private bool[] m_declarationsWritten;

		private bool m_writeUniqueName = true;

		internal bool[] DeclarationsToWrite
		{
			get
			{
				IntermediateFormatWriter.Assert(!this.m_writeDeclarations);
				return this.m_declarationsWritten;
			}
		}

		internal IntermediateFormatWriter(Stream stream, bool[] firstPageDeclarationsToWrite, bool[] otherPageDeclarationsToWrite)
		{
			this.Initialize(stream, true, firstPageDeclarationsToWrite, otherPageDeclarationsToWrite);
		}

		internal IntermediateFormatWriter(Stream stream, bool writeDeclarations)
		{
			this.Initialize(stream, writeDeclarations, null, null);
		}

		private void Initialize(Stream stream, bool writeDeclarations, bool[] firstPageDeclarationsToWrite, bool[] otherPageDeclarationsToWrite)
		{
			IntermediateFormatWriter.Assert(null != stream);
			this.m_writer = new ReportServerBinaryWriter(stream);
			this.m_writer.WriteBytes(VersionStamp.GetBytes());
			this.m_writeDeclarations = writeDeclarations;
			this.m_declarationsWritten = new bool[DeclarationList.Current.Count];
			this.WriteDeclarations(firstPageDeclarationsToWrite);
			this.WriteDeclarations(otherPageDeclarationsToWrite);
		}

		private void WriteDeclarations(bool[] declarationsToWrite)
		{
			if (declarationsToWrite != null)
			{
				IntermediateFormatWriter.Assert(this.m_declarationsWritten.Length == declarationsToWrite.Length);
				for (int i = 0; i < declarationsToWrite.Length; i++)
				{
					if (declarationsToWrite[i])
					{
						this.DeclareType((ObjectType)i);
					}
				}
			}
		}

		internal void WriteIntermediateFormatVersion(IntermediateFormatVersion version)
		{
			if (version == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.IntermediateFormatVersion;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteInt32(version.Major);
				this.m_writer.WriteInt32(version.Minor);
				this.m_writer.WriteInt32(version.Build);
				this.m_writer.EndObject();
			}
		}

		internal void WriteReport(Report report)
		{
			if (report == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Report;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				report.IntermediateFormatVersion.SetCurrent();
				this.WriteIntermediateFormatVersion(report.IntermediateFormatVersion);
				this.m_writer.WriteGuid(report.ReportVersion);
				this.WriteReportItemBase(report);
				this.m_writer.WriteString(report.Author);
				this.m_writer.WriteInt32(report.AutoRefresh);
				this.WriteEmbeddedImageHashtable(report.EmbeddedImages);
				this.WritePageSection(report.PageHeader);
				this.WritePageSection(report.PageFooter);
				this.WriteReportItemCollection(report.ReportItems);
				this.WriteDataSourceList(report.DataSources);
				this.m_writer.WriteString(report.PageHeight);
				this.m_writer.WriteDouble(report.PageHeightValue);
				this.m_writer.WriteString(report.PageWidth);
				this.m_writer.WriteDouble(report.PageWidthValue);
				this.m_writer.WriteString(report.LeftMargin);
				this.m_writer.WriteDouble(report.LeftMarginValue);
				this.m_writer.WriteString(report.RightMargin);
				this.m_writer.WriteDouble(report.RightMarginValue);
				this.m_writer.WriteString(report.TopMargin);
				this.m_writer.WriteDouble(report.TopMarginValue);
				this.m_writer.WriteString(report.BottomMargin);
				this.m_writer.WriteDouble(report.BottomMarginValue);
				this.m_writer.WriteInt32(report.Columns);
				this.m_writer.WriteString(report.ColumnSpacing);
				this.m_writer.WriteDouble(report.ColumnSpacingValue);
				this.WriteDataAggregateInfoList(report.PageAggregates);
				this.m_writer.WriteBytes(report.CompiledCode);
				this.m_writer.WriteBoolean(report.MergeOnePass);
				this.m_writer.WriteBoolean(report.PageMergeOnePass);
				this.m_writer.WriteBoolean(report.SubReportMergeTransactions);
				this.m_writer.WriteBoolean(report.NeedPostGroupProcessing);
				this.m_writer.WriteBoolean(report.HasPostSortAggregates);
				this.m_writer.WriteBoolean(report.HasReportItemReferences);
				this.WriteShowHideTypes(report.ShowHideType);
				this.WriteImageStreamNames(report.ImageStreamNames);
				this.m_writer.WriteInt32(report.LastID);
				this.m_writer.WriteInt32(report.BodyID);
				this.WriteSubReportList(report.SubReports);
				this.m_writer.WriteBoolean(report.HasImageStreams);
				this.m_writer.WriteBoolean(report.HasLabels);
				this.m_writer.WriteBoolean(report.HasBookmarks);
				this.m_writer.WriteBoolean(report.ParametersNotUsedInQuery);
				this.WriteParameterDefList(report.Parameters);
				this.m_writer.WriteString(report.OneDataSetName);
				this.WriteStringList(report.CodeModules);
				this.WriteCodeClassList(report.CodeClasses);
				this.m_writer.WriteBoolean(report.HasSpecialRecursiveAggregates);
				this.WriteExpressionInfo(report.Language);
				this.m_writer.WriteString(report.DataTransform);
				this.m_writer.WriteString(report.DataSchema);
				this.m_writer.WriteBoolean(report.DataElementStyleAttribute);
				this.m_writer.WriteString(report.Code);
				this.m_writer.WriteBoolean(report.HasUserSortFilter);
				this.m_writer.WriteBoolean(report.CompiledCodeGeneratedWithRefusedPermissions);
				this.m_writer.WriteString(report.InteractiveHeight);
				this.m_writer.WriteDouble(report.InteractiveHeightValue);
				this.m_writer.WriteString(report.InteractiveWidth);
				this.m_writer.WriteDouble(report.InteractiveWidthValue);
				this.WriteInScopeSortFilterHashtable(report.NonDetailSortFiltersInScope);
				this.WriteInScopeSortFilterHashtable(report.DetailSortFiltersInScope);
				this.m_writer.EndObject();
			}
		}

		internal void WriteReportSnapshot(ReportSnapshot reportSnapshot)
		{
			if (reportSnapshot == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportSnapshot;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteDateTime(reportSnapshot.ExecutionTime);
				this.WriteReport(reportSnapshot.Report);
				this.WriteParameterInfoCollection(reportSnapshot.Parameters);
				this.WriteReportInstance(reportSnapshot.ReportInstance);
				this.m_writer.WriteBoolean(reportSnapshot.HasDocumentMap);
				this.m_writer.WriteBoolean(reportSnapshot.HasShowHide);
				this.m_writer.WriteBoolean(reportSnapshot.HasBookmarks);
				this.m_writer.WriteBoolean(reportSnapshot.HasImageStreams);
				this.m_writer.WriteString(reportSnapshot.RequestUserName);
				this.m_writer.WriteString(reportSnapshot.ReportServerUrl);
				this.m_writer.WriteString(reportSnapshot.ReportFolder);
				this.m_writer.WriteString(reportSnapshot.Language);
				this.WriteProcessingMessageList(reportSnapshot.Warnings);
				this.WriteInt64List(reportSnapshot.PageSectionOffsets);
				this.m_writer.EndObject();
			}
		}

		internal void WriteDocumentMapNode(DocumentMapNode documentMapNode)
		{
			if (documentMapNode == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DocumentMapNode;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(documentMapNode);
				this.m_writer.WriteString(documentMapNode.Id);
				this.m_writer.WriteString(documentMapNode.Label);
				this.m_writer.WriteInt32(documentMapNode.Page);
				this.WriteDocumentMapNodes(documentMapNode.Children);
				this.m_writer.EndObject();
			}
		}

		internal void WriteBookmarksHashtable(BookmarksHashtable bookmarks)
		{
			if (bookmarks == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.BookmarksHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(bookmarks.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = bookmarks.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string stringValue = (string)enumerator.Key;
					this.m_writer.WriteString(stringValue);
					this.WriteBookmarkInformation((BookmarkInformation)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == bookmarks.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		internal void WriteDrillthroughInfo(ReportDrillthroughInfo reportDrillthroughInfo)
		{
			if (reportDrillthroughInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportDrillthroughInfo;
				this.m_writer.StartObject(objectType);
				this.WriteTokensHashtable(reportDrillthroughInfo.RewrittenCommands);
				this.WriteDrillthroughHashtable(reportDrillthroughInfo.DrillthroughInformation);
				this.m_writer.EndObject();
			}
		}

		internal void WriteTokensHashtable(TokensHashtable tokens)
		{
			if (tokens == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TokensHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(tokens.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = tokens.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int int32Value = (int)enumerator.Key;
					this.m_writer.WriteInt32(int32Value);
					this.WriteVariant(enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == tokens.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		internal void WriteDrillthroughHashtable(DrillthroughHashtable drillthrough)
		{
			if (drillthrough == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DrillthroughHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(drillthrough.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = drillthrough.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string stringValue = (string)enumerator.Key;
					this.m_writer.WriteString(stringValue);
					this.WriteDrillthroughInformation((DrillthroughInformation)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == drillthrough.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		internal void WriteSenderInformationHashtable(SenderInformationHashtable senders)
		{
			if (senders == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.SenderInformationHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(senders.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = senders.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int int32Value = (int)enumerator.Key;
					this.m_writer.WriteInt32(int32Value);
					this.WriteSenderInformation((SenderInformation)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == senders.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		internal void WriteReceiverInformationHashtable(ReceiverInformationHashtable receivers)
		{
			if (receivers == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReceiverInformationHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(receivers.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = receivers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int int32Value = (int)enumerator.Key;
					this.m_writer.WriteInt32(int32Value);
					this.WriteReceiverInformation((ReceiverInformation)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == receivers.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		internal void WriteQuickFindHashtable(QuickFindHashtable quickFind)
		{
			if (quickFind == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.QuickFindHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(quickFind.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = quickFind.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int int32Value = (int)enumerator.Key;
					this.m_writer.WriteInt32(int32Value);
					this.WriteReportItemInstanceReference((ReportItemInstance)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == quickFind.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		internal void WriteSortFilterEventInfoHashtable(SortFilterEventInfoHashtable sortFilterEventInfo)
		{
			if (sortFilterEventInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.SortFilterEventInfoHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(sortFilterEventInfo.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = sortFilterEventInfo.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int int32Value = (int)enumerator.Key;
					this.m_writer.WriteInt32(int32Value);
					this.WriteSortFilterEventInfo((SortFilterEventInfo)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == sortFilterEventInfo.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		internal Int64List WritePageSections(Stream stream, List<PageSectionInstance> pageSections)
		{
			Int64List int64List = null;
			if (pageSections == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.PageSectionInstanceList;
				this.m_writer.StartObject(objectType);
				int count = pageSections.Count;
				IntermediateFormatWriter.Assert(0 == count % 2);
				this.m_writer.StartArray(count);
				int64List = new Int64List(count / 2);
				for (int i = 0; i < count; i++)
				{
					if (i % 2 == 0)
					{
						int64List.Add(stream.Position);
					}
					this.WritePageSectionInstance(pageSections[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
			return int64List;
		}

		internal void WriteSortFilterEventInfo(SortFilterEventInfo sortFilterEventInfo)
		{
			if (sortFilterEventInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.SortFilterEventInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemReference(sortFilterEventInfo.EventSource);
				this.WriteVariantLists(sortFilterEventInfo.EventSourceScopeInfo, true);
				this.m_writer.EndObject();
			}
		}

		internal void WriteInstanceInfo(InstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else if (instanceInfo is ReportItemInstanceInfo)
			{
				this.WriteReportItemInstanceInfo((ReportItemInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is SimpleTextBoxInstanceInfo)
			{
				this.WriteSimpleTextBoxInstanceInfo((SimpleTextBoxInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is ReportItemColInstanceInfo)
			{
				this.WriteReportItemColInstanceInfo((ReportItemColInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is ListContentInstanceInfo)
			{
				this.WriteListContentInstanceInfo((ListContentInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is TableRowInstanceInfo)
			{
				this.WriteTableRowInstanceInfo((TableRowInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is TableGroupInstanceInfo)
			{
				this.WriteTableGroupInstanceInfo((TableGroupInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is MatrixCellInstanceInfo)
			{
				this.WriteMatrixCellInstanceInfo((MatrixCellInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is MatrixSubtotalHeadingInstanceInfo)
			{
				this.WriteMatrixSubtotalHeadingInstanceInfo((MatrixSubtotalHeadingInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is TableDetailInstanceInfo)
			{
				this.WriteTableDetailInstanceInfo((TableDetailInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is ChartInstanceInfo)
			{
				this.WriteChartInstanceInfo((ChartInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is ChartHeadingInstanceInfo)
			{
				this.WriteChartHeadingInstanceInfo((ChartHeadingInstanceInfo)instanceInfo);
			}
			else if (instanceInfo is ChartDataPointInstanceInfo)
			{
				this.WriteChartDataPointInstanceInfo((ChartDataPointInstanceInfo)instanceInfo);
			}
			else
			{
				IntermediateFormatWriter.Assert(instanceInfo is MatrixHeadingInstanceInfo);
				this.WriteMatrixHeadingInstanceInfo((MatrixHeadingInstanceInfo)instanceInfo);
			}
		}

		internal void WriteRecordSetInfo(RecordSetInfo recordSetInfo)
		{
			if (recordSetInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RecordSetInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteBoolean(recordSetInfo.ReaderExtensionsSupported);
				this.WriteRecordSetPropertyNamesList(recordSetInfo.FieldPropertyNames);
				this.m_writer.WriteEnum((int)recordSetInfo.CompareOptions);
				this.m_writer.EndObject();
			}
		}

		internal void WriteRecordSetPropertyNamesList(RecordSetPropertyNamesList list)
		{
			if (list == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RecordSetPropertyNamesList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					this.WriteRecordSetPropertyNames(list[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		internal void WriteRecordSetPropertyNames(RecordSetPropertyNames field)
		{
			if (field == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RecordSetPropertyNames;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteStringList(field.PropertyNames);
				this.m_writer.EndObject();
			}
		}

		internal bool WriteRecordRow(RecordRow recordRow, RecordSetPropertyNamesList aliasPropertyNames)
		{
			bool result = true;
			if (recordRow == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RecordRow;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				result = this.WriteRecordFields(recordRow.RecordFields, aliasPropertyNames);
				this.m_writer.WriteBoolean(recordRow.IsAggregateRow);
				this.m_writer.WriteInt32(recordRow.AggregationFieldCount);
				this.m_writer.EndObject();
			}
			return result;
		}

		private static void Assert(bool condition)
		{
			Global.Tracer.Assert(condition);
		}

		private void DeclareType(ObjectType objectType)
		{
			if (!this.m_declarationsWritten[(int)objectType])
			{
				if (this.m_writeDeclarations)
				{
					this.m_writer.DeclareType(objectType, DeclarationList.Current[objectType]);
				}
				this.m_declarationsWritten[(int)objectType] = true;
			}
		}

		private void WriteValidValueList(ValidValueList parameters)
		{
			if (parameters == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ValidValueList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(parameters.Count);
				for (int i = 0; i < parameters.Count; i++)
				{
					this.WriteValidValue(parameters[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteParameterDefList(ParameterDefList parameters)
		{
			if (parameters == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ParameterDefList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(parameters.Count);
				for (int i = 0; i < parameters.Count; i++)
				{
					this.WriteParameterDef(parameters[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteParameterDefRefList(ParameterDefList parameters)
		{
			if (parameters == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ParameterDefList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(parameters.Count);
				for (int i = 0; i < parameters.Count; i++)
				{
					this.m_writer.WriteString(parameters[i].Name);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteParameterInfoCollection(ParameterInfoCollection parameters)
		{
			if (parameters == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ParameterInfoCollection;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(parameters.Count);
				for (int i = 0; i < parameters.Count; i++)
				{
					this.WriteParameterInfo(parameters[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteParameterInfoRefCollection(ParameterInfoCollection parameters)
		{
			if (parameters == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ParameterInfoCollection;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(parameters.Count);
				for (int i = 0; i < parameters.Count; i++)
				{
					this.m_writer.WriteString(parameters[i].Name);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteFilterList(FilterList filters)
		{
			if (filters == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.FilterList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(filters.Count);
				for (int i = 0; i < filters.Count; i++)
				{
					this.WriteFilter(filters[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDataSourceList(DataSourceList dataSources)
		{
			if (dataSources == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataSourceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(dataSources.Count);
				for (int i = 0; i < dataSources.Count; i++)
				{
					this.WriteDataSource(dataSources[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDataAggregateInfoList(DataAggregateInfoList aggregates)
		{
			if (aggregates == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataAggregateInfoList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(aggregates.Count);
				for (int i = 0; i < aggregates.Count; i++)
				{
					this.WriteDataAggregateInfo(aggregates[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteReportItemIDList(ReportItemList reportItems)
		{
			if (reportItems == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportItemList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(reportItems.Count);
				for (int i = 0; i < reportItems.Count; i++)
				{
					this.WriteReportItemID(reportItems[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteReportItemID(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				IntermediateFormatWriter.Assert(reportItem is TextBox);
				this.m_writer.WriteReference(ObjectType.TextBox, reportItem.ID);
			}
		}

		private void WriteReportItemList(ReportItemList reportItems)
		{
			if (reportItems == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportItemList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(reportItems.Count);
				for (int i = 0; i < reportItems.Count; i++)
				{
					this.WriteReportItem(reportItems[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteReportItemIndexerList(ReportItemIndexerList reportItemIndexers)
		{
			if (reportItemIndexers == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportItemIndexerList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(reportItemIndexers.Count);
				for (int i = 0; i < reportItemIndexers.Count; i++)
				{
					this.WriteReportItemIndexer(reportItemIndexers[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteRunningValueInfoList(RunningValueInfoList runningValues)
		{
			if (runningValues == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RunningValueInfoList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(runningValues.Count);
				for (int i = 0; i < runningValues.Count; i++)
				{
					this.WriteRunningValueInfo(runningValues[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteStyleAttributeHashtable(StyleAttributeHashtable styleAttributes)
		{
			if (styleAttributes == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.StyleAttributeHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(styleAttributes.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = styleAttributes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string text = (string)enumerator.Key;
					IntermediateFormatWriter.Assert(null != text);
					this.m_writer.WriteString(text);
					this.WriteAttributeInfo((AttributeInfo)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == styleAttributes.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteImageInfo(ImageInfo imageInfo)
		{
			if (imageInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ImageInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(imageInfo.StreamName);
				this.m_writer.WriteString(imageInfo.MimeType);
				this.m_writer.EndObject();
			}
		}

		private void WriteDrillthroughParameters(DrillthroughParameters parameters)
		{
			if (parameters == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DrillthroughParameters;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(parameters.Count);
				for (int i = 0; i < parameters.Count; i++)
				{
					string key = parameters.GetKey(i);
					object value = parameters.GetValue(i);
					object[] array = null;
					IntermediateFormatWriter.Assert(null != key);
					this.m_writer.WriteString(key);
					if (value != null)
					{
						array = (value as object[]);
					}
					if (array != null)
					{
						this.WriteVariants(array, false);
					}
					else
					{
						this.WriteVariant(value);
					}
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteImageStreamNames(ImageStreamNames streamNames)
		{
			if (streamNames == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ImageStreamNames;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(streamNames.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = streamNames.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string text = (string)enumerator.Key;
					IntermediateFormatWriter.Assert(null != text);
					this.m_writer.WriteString(text);
					this.WriteImageInfo((ImageInfo)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == streamNames.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteEmbeddedImageHashtable(EmbeddedImageHashtable embeddedImages)
		{
			if (embeddedImages == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.EmbeddedImageHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(embeddedImages.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = embeddedImages.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string text = (string)enumerator.Key;
					IntermediateFormatWriter.Assert(null != text);
					this.m_writer.WriteString(text);
					this.WriteImageInfo((ImageInfo)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == embeddedImages.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteExpressionInfoList(ExpressionInfoList expressions)
		{
			if (expressions == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ExpressionInfoList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(expressions.Count);
				for (int i = 0; i < expressions.Count; i++)
				{
					this.WriteExpressionInfo(expressions[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDataSetList(DataSetList dataSets)
		{
			if (dataSets == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataSetList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(dataSets.Count);
				for (int i = 0; i < dataSets.Count; i++)
				{
					this.WriteDataSet(dataSets[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteExpressionInfos(ExpressionInfo[] expressions)
		{
			if (expressions == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(expressions.Length);
				for (int i = 0; i < expressions.Length; i++)
				{
					this.WriteExpressionInfo(expressions[i]);
				}
				this.m_writer.EndArray();
			}
		}

		private void WriteStringList(StringList strings)
		{
			if (strings == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.StringList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(strings.Count);
				for (int i = 0; i < strings.Count; i++)
				{
					this.m_writer.WriteString(strings[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDataFieldList(DataFieldList fields)
		{
			if (fields == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataFieldList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(fields.Count);
				for (int i = 0; i < fields.Count; i++)
				{
					this.WriteDataField(fields[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDataRegionList(DataRegionList dataRegions)
		{
			if (dataRegions == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataRegionList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(dataRegions.Count);
				for (int i = 0; i < dataRegions.Count; i++)
				{
					this.WriteDataRegionReference(dataRegions[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteParameterValueList(ParameterValueList parameters)
		{
			if (parameters == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ParameterValueList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(parameters.Count);
				for (int i = 0; i < parameters.Count; i++)
				{
					this.WriteParameterValue(parameters[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteCodeClassList(CodeClassList classes)
		{
			if (classes == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CodeClassList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(classes.Count);
				for (int i = 0; i < classes.Count; i++)
				{
					this.WriteCodeClass(classes[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteIntList(IntList ints)
		{
			if (ints == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.IntList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(ints.Count);
				for (int i = 0; i < ints.Count; i++)
				{
					this.m_writer.WriteInt32(ints[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteInt64List(Int64List longs)
		{
			if (longs == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Int64List;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(longs.Count);
				for (int i = 0; i < longs.Count; i++)
				{
					this.m_writer.WriteInt64(longs[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteBoolList(BoolList bools)
		{
			if (bools == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.BoolList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(bools.Count);
				for (int i = 0; i < bools.Count; i++)
				{
					this.m_writer.WriteBoolean(bools[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixRowList(MatrixRowList rows)
		{
			if (rows == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixRowList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(rows.Count);
				for (int i = 0; i < rows.Count; i++)
				{
					this.WriteMatrixRow(rows[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixColumnList(MatrixColumnList columns)
		{
			if (columns == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixColumnList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(columns.Count);
				for (int i = 0; i < columns.Count; i++)
				{
					this.WriteMatrixColumn(columns[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteTableColumnList(TableColumnList columns)
		{
			if (columns == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableColumnList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(columns.Count);
				for (int i = 0; i < columns.Count; i++)
				{
					this.WriteTableColumn(columns[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteTableRowList(TableRowList rows)
		{
			if (rows == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableRowList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(rows.Count);
				for (int i = 0; i < rows.Count; i++)
				{
					this.WriteTableRow(rows[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteChartColumnList(ChartColumnList columns)
		{
			if (columns == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartColumnList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(columns.Count);
				for (int i = 0; i < columns.Count; i++)
				{
					this.WriteChartColumn(columns[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteCustomReportItemHeadingList(CustomReportItemHeadingList headings)
		{
			if (headings == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItemHeadingList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(headings.Count);
				for (int i = 0; i < headings.Count; i++)
				{
					this.WriteCustomReportItemHeading(headings[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDataCellsList(DataCellsList rows)
		{
			if (rows == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataCellsList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(rows.Count);
				for (int i = 0; i < rows.Count; i++)
				{
					this.WriteDataCellList(rows[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDataCellList(DataCellList cells)
		{
			if (cells == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataCellList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(cells.Count);
				for (int i = 0; i < cells.Count; i++)
				{
					this.WriteDataValueCRIList(cells[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDataValueCRIList(DataValueCRIList values)
		{
			if (values == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataValueCRIList;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteDataValueList(values);
				this.m_writer.WriteInt32(values.RDLRowIndex);
				this.m_writer.WriteInt32(values.RDLColumnIndex);
				this.m_writer.EndObject();
			}
		}

		private void WriteDataValueList(DataValueList values)
		{
			if (values == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataValueList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(values.Count);
				for (int i = 0; i < values.Count; i++)
				{
					this.WriteDataValue(values[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDataValueInstanceList(DataValueInstanceList instances)
		{
			if (instances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataValueInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(instances.Count);
				for (int i = 0; i < instances.Count; i++)
				{
					this.WriteDataValueInstance(instances[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteImageMapAreaInstanceList(ImageMapAreaInstanceList mapAreas)
		{
			if (mapAreas == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ImageMapAreaInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(mapAreas.Count);
				for (int i = 0; i < mapAreas.Count; i++)
				{
					this.WriteImageMapAreaInstance(mapAreas[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteSubReportList(SubReportList subReports)
		{
			if (subReports == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.SubReportList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(subReports.Count);
				for (int i = 0; i < subReports.Count; i++)
				{
					this.WriteSubReportReference(subReports[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteSubReportReference(SubReport subReport)
		{
			if (subReport == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteReference(ObjectType.SubReport, subReport.ID);
			}
		}

		private void WriteNonComputedUniqueNamess(NonComputedUniqueNames[] names)
		{
			if (names == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(names.Length);
				for (int i = 0; i < names.Length; i++)
				{
					this.WriteNonComputedUniqueNames(names[i]);
				}
				this.m_writer.EndArray();
			}
		}

		private void WriteReportItemInstanceList(ReportItemInstanceList reportItemInstances)
		{
			if (reportItemInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportItemInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(reportItemInstances.Count);
				for (int i = 0; i < reportItemInstances.Count; i++)
				{
					this.WriteReportItemInstance(reportItemInstances[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteRenderingPagesRangesList(RenderingPagesRangesList renderingPagesRanges)
		{
			if (renderingPagesRanges == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RenderingPagesRangesList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(renderingPagesRanges.Count);
				for (int i = 0; i < renderingPagesRanges.Count; i++)
				{
					this.WriteRenderingPagesRanges(renderingPagesRanges[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteListContentInstanceList(ListContentInstanceList listContents)
		{
			if (listContents == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ListContentInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(listContents.Count);
				for (int i = 0; i < listContents.Count; i++)
				{
					this.WriteListContentInstance(listContents[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixHeadingInstanceList(MatrixHeadingInstanceList matrixheadingInstances)
		{
			if (matrixheadingInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixHeadingInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(matrixheadingInstances.Count);
				for (int i = 0; i < matrixheadingInstances.Count; i++)
				{
					this.WriteMatrixHeadingInstance(matrixheadingInstances[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixCellInstancesList(MatrixCellInstancesList matrixCellInstancesList)
		{
			if (matrixCellInstancesList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixCellInstancesList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(matrixCellInstancesList.Count);
				for (int i = 0; i < matrixCellInstancesList.Count; i++)
				{
					this.WriteMatrixCellInstanceList(matrixCellInstancesList[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixCellInstanceList(MatrixCellInstanceList matrixCellInstances)
		{
			if (matrixCellInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixCellInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(matrixCellInstances.Count);
				for (int i = 0; i < matrixCellInstances.Count; i++)
				{
					MatrixSubtotalCellInstance matrixSubtotalCellInstance = matrixCellInstances[i] as MatrixSubtotalCellInstance;
					if (matrixSubtotalCellInstance != null)
					{
						this.WriteMatrixSubtotalCellInstance(matrixSubtotalCellInstance);
					}
					else
					{
						this.WriteMatrixCellInstance(matrixCellInstances[i]);
					}
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteMultiChartInstanceList(MultiChartInstanceList multichartInstances)
		{
			if (multichartInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MultiChartInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(multichartInstances.Count);
				for (int i = 0; i < multichartInstances.Count; i++)
				{
					this.WriteMultiChartInstance(multichartInstances[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteChartHeadingInstanceList(ChartHeadingInstanceList chartheadingInstances)
		{
			if (chartheadingInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartHeadingInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(chartheadingInstances.Count);
				for (int i = 0; i < chartheadingInstances.Count; i++)
				{
					this.WriteChartHeadingInstance(chartheadingInstances[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteChartDataPointInstancesList(ChartDataPointInstancesList chartDataPointInstancesList)
		{
			if (chartDataPointInstancesList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartDataPointInstancesList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(chartDataPointInstancesList.Count);
				for (int i = 0; i < chartDataPointInstancesList.Count; i++)
				{
					this.WriteChartDataPointInstanceList(chartDataPointInstancesList[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteChartDataPointInstanceList(ChartDataPointInstanceList chartDataPointInstanceList)
		{
			if (chartDataPointInstanceList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartDataPointInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(chartDataPointInstanceList.Count);
				for (int i = 0; i < chartDataPointInstanceList.Count; i++)
				{
					this.WriteChartDataPointInstance(chartDataPointInstanceList[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteTableRowInstances(TableRowInstance[] tableRowInstances)
		{
			if (tableRowInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(tableRowInstances.Length);
				for (int i = 0; i < tableRowInstances.Length; i++)
				{
					this.WriteTableRowInstance(tableRowInstances[i]);
				}
				this.m_writer.EndArray();
			}
		}

		private void WriteTableDetailInstanceList(TableDetailInstanceList tableDetailInstanceList)
		{
			if (tableDetailInstanceList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableDetailInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(tableDetailInstanceList.Count);
				for (int i = 0; i < tableDetailInstanceList.Count; i++)
				{
					this.WriteTableDetailInstance(tableDetailInstanceList[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteTableGroupInstanceList(TableGroupInstanceList tableGroupInstances)
		{
			if (tableGroupInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableGroupInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(tableGroupInstances.Count);
				for (int i = 0; i < tableGroupInstances.Count; i++)
				{
					this.WriteTableGroupInstance(tableGroupInstances[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteTableColumnInstances(TableColumnInstance[] tableColumnInstances)
		{
			if (tableColumnInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(tableColumnInstances.Length);
				for (int i = 0; i < tableColumnInstances.Length; i++)
				{
					this.WriteTableColumnInstance(tableColumnInstances[i]);
				}
				this.m_writer.EndArray();
			}
		}

		private void WriteCustomReportItemHeadingInstanceList(CustomReportItemHeadingInstanceList headingInstances)
		{
			if (headingInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItemHeadingInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(headingInstances.Count);
				for (int i = 0; i < headingInstances.Count; i++)
				{
					this.WriteCustomReportItemHeadingInstance(headingInstances[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteCustomReportItemCellInstancesList(CustomReportItemCellInstancesList cellInstancesList)
		{
			if (cellInstancesList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItemCellInstancesList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(cellInstancesList.Count);
				for (int i = 0; i < cellInstancesList.Count; i++)
				{
					this.WriteCustomReportItemCellInstanceList(cellInstancesList[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteCustomReportItemCellInstanceList(CustomReportItemCellInstanceList cellInstances)
		{
			if (cellInstances == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItemCellInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(cellInstances.Count);
				for (int i = 0; i < cellInstances.Count; i++)
				{
					this.WriteCustomReportItemCellInstance(cellInstances[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteDocumentMapNodes(DocumentMapNode[] nodes)
		{
			if (nodes == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(nodes.Length);
				for (int i = 0; i < nodes.Length; i++)
				{
					this.WriteDocumentMapNode(nodes[i]);
				}
				this.m_writer.EndArray();
			}
		}

		private void WriteStrings(string[] strings)
		{
			if (strings == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(strings.Length);
				for (int i = 0; i < strings.Length; i++)
				{
					this.m_writer.WriteString(strings[i]);
				}
				this.m_writer.EndArray();
			}
		}

		private void WriteVariants(object[] variants)
		{
			this.WriteVariants(variants, false);
		}

		private void WriteVariants(object[] variants, bool isMultiValue)
		{
			if (variants == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(variants.Length);
				for (int i = 0; i < variants.Length; i++)
				{
					if (isMultiValue && variants[i] is object[])
					{
						this.WriteVariants(variants[i] as object[], false);
					}
					else
					{
						this.WriteVariant(variants[i]);
					}
				}
				this.m_writer.EndArray();
			}
		}

		private void WriteVariantList(VariantList variants, bool convertDBNull)
		{
			if (variants == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.VariantList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(variants.Count);
				for (int i = 0; i < variants.Count; i++)
				{
					this.WriteVariant(((ArrayList)variants)[i], convertDBNull);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteVariantLists(VariantList[] variantLists, bool convertDBNull)
		{
			if (variantLists == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(variantLists.Length);
				for (int i = 0; i < variantLists.Length; i++)
				{
					this.WriteVariantList(variantLists[i], convertDBNull);
				}
				this.m_writer.EndArray();
			}
		}

		private void WriteProcessingMessageList(ProcessingMessageList messages)
		{
			if (messages == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ProcessingMessageList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(messages.Count);
				for (int i = 0; i < messages.Count; i++)
				{
					this.WriteProcessingMessage(messages[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteIDOwnerBase(IDOwner idOwner)
		{
			IntermediateFormatWriter.Assert(null != idOwner);
			ObjectType objectType = ObjectType.IDOwner;
			this.DeclareType(objectType);
			this.m_writer.WriteInt32(idOwner.ID);
		}

		private void WriteReportItem(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				this.m_writer.WriteNull();
			}
			else if (reportItem is Line)
			{
				this.WriteLine((Line)reportItem);
			}
			else if (reportItem is Rectangle)
			{
				this.WriteRectangle((Rectangle)reportItem);
			}
			else if (reportItem is Image)
			{
				this.WriteImage((Image)reportItem);
			}
			else if (reportItem is CheckBox)
			{
				this.WriteCheckBox((CheckBox)reportItem);
			}
			else if (reportItem is TextBox)
			{
				this.WriteTextBox((TextBox)reportItem);
			}
			else if (reportItem is SubReport)
			{
				this.WriteSubReport((SubReport)reportItem);
			}
			else if (reportItem is ActiveXControl)
			{
				this.WriteActiveXControl((ActiveXControl)reportItem);
			}
			else
			{
				IntermediateFormatWriter.Assert(reportItem is DataRegion);
				this.WriteDataRegion((DataRegion)reportItem);
			}
		}

		private void WriteReportItemBase(ReportItem reportItem)
		{
			IntermediateFormatWriter.Assert(null != reportItem);
			ObjectType objectType = ObjectType.ReportItem;
			this.DeclareType(objectType);
			this.WriteIDOwnerBase(reportItem);
			this.m_writer.WriteString(reportItem.Name);
			this.WriteStyle(reportItem.StyleClass);
			this.m_writer.WriteString(reportItem.Top);
			this.m_writer.WriteDouble(reportItem.TopValue);
			this.m_writer.WriteString(reportItem.Left);
			this.m_writer.WriteDouble(reportItem.LeftValue);
			this.m_writer.WriteString(reportItem.Height);
			this.m_writer.WriteDouble(reportItem.HeightValue);
			this.m_writer.WriteString(reportItem.Width);
			this.m_writer.WriteDouble(reportItem.WidthValue);
			this.m_writer.WriteInt32(reportItem.ZIndex);
			this.WriteVisibility(reportItem.Visibility);
			this.WriteExpressionInfo(reportItem.ToolTip);
			this.WriteExpressionInfo(reportItem.Label);
			this.WriteExpressionInfo(reportItem.Bookmark);
			this.m_writer.WriteString(reportItem.Custom);
			this.m_writer.WriteBoolean(reportItem.RepeatedSibling);
			this.m_writer.WriteBoolean(reportItem.IsFullSize);
			this.m_writer.WriteInt32(reportItem.ExprHostID);
			this.m_writer.WriteString(reportItem.DataElementName);
			this.WriteDataElementOutputType(reportItem.DataElementOutput);
			this.m_writer.WriteInt32(reportItem.DistanceFromReportTop);
			this.m_writer.WriteInt32(reportItem.DistanceBeforeTop);
			this.WriteIntList(reportItem.SiblingAboveMe);
			this.WriteDataValueList(reportItem.CustomProperties);
		}

		private void WriteReportItemReference(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteNoTypeReference(reportItem.ID);
			}
		}

		private void WritePageSection(PageSection pageSection)
		{
			if (pageSection == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.PageSection;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemBase(pageSection);
				this.m_writer.WriteBoolean(pageSection.PrintOnFirstPage);
				this.m_writer.WriteBoolean(pageSection.PrintOnLastPage);
				this.WriteReportItemCollection(pageSection.ReportItems);
				this.m_writer.WriteBoolean(pageSection.PostProcessEvaluate);
				this.m_writer.EndObject();
			}
		}

		private void WriteReportItemCollection(ReportItemCollection reportItems)
		{
			if (reportItems == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportItemCollection;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteIDOwnerBase(reportItems);
				this.WriteReportItemList(reportItems.NonComputedReportItems);
				this.WriteReportItemList(reportItems.ComputedReportItems);
				this.WriteReportItemIndexerList(reportItems.SortedReportItems);
				this.WriteRunningValueInfoList(reportItems.RunningValues);
				this.m_writer.EndObject();
			}
		}

		private void WriteShowHideTypes(Report.ShowHideTypes showHideType)
		{
			this.m_writer.WriteEnum((int)showHideType);
		}

		private void WriteDataElementOutputType(DataElementOutputTypes element)
		{
			this.m_writer.WriteEnum((int)element);
		}

		private void WriteStyle(Style style)
		{
			if (style == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Style;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteStyleAttributeHashtable(style.StyleAttributes);
				this.WriteExpressionInfoList(style.ExpressionList);
				this.m_writer.EndObject();
			}
		}

		private void WriteVisibility(Visibility visibility)
		{
			if (visibility == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Visibility;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteExpressionInfo(visibility.Hidden);
				this.m_writer.WriteString(visibility.Toggle);
				this.m_writer.WriteBoolean(visibility.RecursiveReceiver);
				this.m_writer.EndObject();
			}
		}

		private void WriteFilter(Filter filter)
		{
			if (filter == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Filter;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteExpressionInfo(filter.Expression);
				this.WriteOperators(filter.Operator);
				this.WriteExpressionInfoList(filter.Values);
				this.m_writer.WriteInt32(filter.ExprHostID);
				this.m_writer.EndObject();
			}
		}

		private void WriteOperators(Filter.Operators operators)
		{
			this.m_writer.WriteEnum((int)operators);
		}

		private void WriteDataSource(DataSource dataSource)
		{
			if (dataSource == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataSource;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(dataSource.Name);
				this.m_writer.WriteBoolean(dataSource.Transaction);
				this.m_writer.WriteString(dataSource.Type);
				this.WriteExpressionInfo(dataSource.ConnectStringExpression);
				this.m_writer.WriteBoolean(dataSource.IntegratedSecurity);
				this.m_writer.WriteString(dataSource.Prompt);
				this.m_writer.WriteString(dataSource.DataSourceReference);
				this.WriteDataSetList(dataSource.DataSets);
				this.m_writer.WriteGuid(dataSource.ID);
				this.m_writer.WriteInt32(dataSource.ExprHostID);
				this.m_writer.WriteString(dataSource.SharedDataSourceReferencePath);
				this.m_writer.EndObject();
			}
		}

		private void WriteDataAggregateInfo(DataAggregateInfo aggregate)
		{
			if (aggregate == null)
			{
				this.m_writer.WriteNull();
			}
			else if (aggregate is RunningValueInfo)
			{
				this.WriteRunningValueInfo((RunningValueInfo)aggregate);
			}
			else
			{
				ObjectType objectType = ObjectType.DataAggregateInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteDataAggregateInfoBase(aggregate);
				this.m_writer.EndObject();
			}
		}

		private void WriteDataAggregateInfoBase(DataAggregateInfo aggregate)
		{
			IntermediateFormatWriter.Assert(null != aggregate);
			ObjectType objectType = ObjectType.DataAggregateInfo;
			this.DeclareType(objectType);
			this.m_writer.WriteString(aggregate.Name);
			this.WriteAggregateTypes(aggregate.AggregateType);
			this.WriteExpressionInfos(aggregate.Expressions);
			this.WriteStringList(aggregate.DuplicateNames);
		}

		private void WriteExpressionInfo(ExpressionInfo expression)
		{
			if (expression == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ExpressionInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteTypes(expression.Type);
				this.m_writer.WriteString(expression.Value);
				this.m_writer.WriteBoolean(expression.BoolValue);
				this.m_writer.WriteInt32(expression.IntValue);
				this.m_writer.WriteInt32(expression.ExprHostID);
				this.m_writer.WriteString(expression.OriginalText);
				this.m_writer.EndObject();
			}
		}

		private void WriteAggregateTypes(DataAggregateInfo.AggregateTypes aggregateType)
		{
			this.m_writer.WriteEnum((int)aggregateType);
		}

		private void WriteTypes(ExpressionInfo.Types type)
		{
			this.m_writer.WriteEnum((int)type);
		}

		private void WriteReportItemIndexer(ReportItemIndexer reportItemIndexer)
		{
			ObjectType objectType = ObjectType.ReportItemIndexer;
			this.DeclareType(objectType);
			this.m_writer.StartObject(objectType);
			this.m_writer.WriteBoolean(reportItemIndexer.IsComputed);
			this.m_writer.WriteInt32(reportItemIndexer.Index);
			this.m_writer.EndObject();
		}

		private void WriteRenderingPagesRanges(RenderingPagesRanges renderingPagesRanges)
		{
			ObjectType objectType = ObjectType.RenderingPagesRanges;
			this.DeclareType(objectType);
			this.m_writer.StartObject(objectType);
			this.m_writer.WriteInt32(renderingPagesRanges.StartPage);
			this.m_writer.WriteInt32(renderingPagesRanges.EndPage);
			this.m_writer.EndObject();
		}

		private void WriteRunningValueInfo(RunningValueInfo runningValue)
		{
			if (runningValue == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RunningValueInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteDataAggregateInfoBase(runningValue);
				this.m_writer.WriteString(runningValue.Scope);
				this.m_writer.EndObject();
			}
		}

		private void WriteAttributeInfo(AttributeInfo attributeInfo)
		{
			if (attributeInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.AttributeInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteBoolean(attributeInfo.IsExpression);
				this.m_writer.WriteString(attributeInfo.Value);
				this.m_writer.WriteBoolean(attributeInfo.BoolValue);
				this.m_writer.WriteInt32(attributeInfo.IntValue);
				this.m_writer.EndObject();
			}
		}

		private void WriteDataSet(DataSet dataSet)
		{
			if (dataSet == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataSet;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				Global.Tracer.Assert(dataSet.ID > 0);
				this.WriteIDOwnerBase(dataSet);
				this.m_writer.WriteString(dataSet.Name);
				dataSet.PopulateReferencedFieldProperties();
				this.WriteDataFieldList(dataSet.Fields);
				this.WriteReportQuery(dataSet.Query);
				this.WriteSensitivity(dataSet.CaseSensitivity);
				this.m_writer.WriteString(dataSet.Collation);
				this.WriteSensitivity(dataSet.AccentSensitivity);
				this.WriteSensitivity(dataSet.KanatypeSensitivity);
				this.WriteSensitivity(dataSet.WidthSensitivity);
				this.WriteDataRegionList(dataSet.DataRegions);
				this.WriteDataAggregateInfoList(dataSet.Aggregates);
				this.WriteFilterList(dataSet.Filters);
				this.m_writer.WriteInt32(dataSet.RecordSetSize);
				this.m_writer.WriteBoolean(dataSet.UsedOnlyInParameters);
				this.m_writer.WriteInt32(dataSet.NonCalculatedFieldCount);
				this.m_writer.WriteInt32(dataSet.ExprHostID);
				this.WriteDataAggregateInfoList(dataSet.PostSortAggregates);
				this.m_writer.WriteInt32((int)dataSet.LCID);
				this.m_writer.WriteBoolean(dataSet.HasDetailUserSortFilter);
				this.WriteExpressionInfoList(dataSet.UserSortExpressions);
				this.m_writer.WriteBoolean(dataSet.DynamicFieldReferences);
				if (dataSet.InterpretSubtotalsAsDetailsIsAuto)
				{
					dataSet.InterpretSubtotalsAsDetails = true;
				}
				this.m_writer.WriteBoolean(dataSet.InterpretSubtotalsAsDetails);
				this.m_writer.EndObject();
			}
		}

		private void WriteReportQuery(ReportQuery query)
		{
			if (query == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportQuery;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteCommandType(query.CommandType);
				this.WriteExpressionInfo(query.CommandText);
				this.WriteParameterValueList(query.Parameters);
				this.m_writer.WriteInt32(query.TimeOut);
				this.m_writer.WriteString(query.CommandTextValue);
				this.m_writer.WriteString(query.RewrittenCommandText);
				this.m_writer.EndObject();
			}
		}

		private void WriteSensitivity(DataSet.Sensitivity sensitivity)
		{
			this.m_writer.WriteEnum((int)sensitivity);
		}

		private void WriteCommandType(CommandType commandType)
		{
			this.m_writer.WriteEnum((int)commandType);
		}

		private void WriteDataField(Field field)
		{
			if (field == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Field;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(field.Name);
				this.m_writer.WriteString(field.DataField);
				this.WriteExpressionInfo(field.Value);
				this.m_writer.WriteInt32(field.ExprHostID);
				this.m_writer.WriteBoolean(field.DynamicPropertyReferences);
				this.WriteFieldPropertyHashtable(field.ReferencedProperties);
				this.m_writer.EndObject();
			}
		}

		internal void WriteFieldPropertyHashtable(FieldPropertyHashtable properties)
		{
			if (properties == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.FieldPropertyHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(properties.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = properties.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string stringValue = enumerator.Key as string;
					this.m_writer.WriteString(stringValue);
					num++;
				}
				IntermediateFormatWriter.Assert(num == properties.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteParameterValue(ParameterValue parameter)
		{
			ObjectType objectType = ObjectType.ParameterValue;
			this.DeclareType(objectType);
			this.m_writer.StartObject(objectType);
			this.m_writer.WriteString(parameter.Name);
			this.WriteExpressionInfo(parameter.Value);
			this.m_writer.WriteInt32(parameter.ExprHostID);
			this.WriteExpressionInfo(parameter.Omit);
			this.m_writer.EndObject();
		}

		private void WriteCodeClass(CodeClass codeClass)
		{
			ObjectType objectType = ObjectType.CodeClass;
			this.DeclareType(objectType);
			this.m_writer.StartObject(objectType);
			this.m_writer.WriteString(codeClass.ClassName);
			this.m_writer.WriteString(codeClass.InstanceName);
			this.m_writer.EndObject();
		}

		private void WriteAction(Action actionInfo)
		{
			if (actionInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Action;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteActionItemList(actionInfo.ActionItems);
				this.WriteStyle(actionInfo.StyleClass);
				this.m_writer.WriteInt32(actionInfo.ComputedActionItemsCount);
				this.m_writer.EndObject();
			}
		}

		private void WriteActionItemList(ActionItemList actionItemList)
		{
			if (actionItemList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ActionItemList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(actionItemList.Count);
				for (int i = 0; i < actionItemList.Count; i++)
				{
					this.WriteActionItem(actionItemList[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteActionItem(ActionItem actionItem)
		{
			if (actionItem == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ActionItem;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteExpressionInfo(actionItem.HyperLinkURL);
				this.WriteExpressionInfo(actionItem.DrillthroughReportName);
				this.WriteParameterValueList(actionItem.DrillthroughParameters);
				this.WriteExpressionInfo(actionItem.DrillthroughBookmarkLink);
				this.WriteExpressionInfo(actionItem.BookmarkLink);
				this.WriteExpressionInfo(actionItem.Label);
				this.m_writer.WriteInt32(actionItem.ExprHostID);
				this.m_writer.WriteInt32(actionItem.ComputedIndex);
				this.m_writer.EndObject();
			}
		}

		private void WriteLine(Line line)
		{
			if (line == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Line;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemBase(line);
				this.m_writer.WriteBoolean(line.LineSlant);
				this.m_writer.EndObject();
			}
		}

		private void WriteRectangle(Rectangle rectangle)
		{
			if (rectangle == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Rectangle;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemBase(rectangle);
				this.WriteReportItemCollection(rectangle.ReportItems);
				this.m_writer.WriteBoolean(rectangle.PageBreakAtEnd);
				this.m_writer.WriteBoolean(rectangle.PageBreakAtStart);
				this.m_writer.WriteInt32(rectangle.LinkToChild);
				this.m_writer.EndObject();
			}
		}

		private void WriteImage(Image image)
		{
			if (image == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Image;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemBase(image);
				this.WriteAction(image.Action);
				this.WriteSourceType(image.Source);
				this.WriteExpressionInfo(image.Value);
				this.WriteExpressionInfo(image.MIMEType);
				this.WriteSizings(image.Sizing);
				this.m_writer.EndObject();
			}
		}

		private void WriteImageMapAreaInstance(ImageMapAreaInstance mapArea)
		{
			if (mapArea == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ImageMapAreaInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(mapArea.ID);
				this.WriteImageMapAreaShape(mapArea.Shape);
				this.m_writer.WriteFloatArray(mapArea.Coordinates);
				this.WriteAction(mapArea.Action);
				this.WriteActionInstance(mapArea.ActionInstance);
				this.m_writer.WriteInt32(mapArea.UniqueName);
				this.m_writer.EndObject();
			}
		}

		private void WriteImageMapAreaShape(ImageMapArea.ImageMapAreaShape sourceType)
		{
			this.m_writer.WriteEnum((int)sourceType);
		}

		private void WriteSourceType(Image.SourceType sourceType)
		{
			this.m_writer.WriteEnum((int)sourceType);
		}

		private void WriteSizings(Image.Sizings sizing)
		{
			this.m_writer.WriteEnum((int)sizing);
		}

		private void WriteCheckBox(CheckBox checkBox)
		{
			if (checkBox == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CheckBox;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemBase(checkBox);
				this.WriteExpressionInfo(checkBox.Value);
				this.m_writer.WriteString(checkBox.HideDuplicates);
				this.m_writer.EndObject();
			}
		}

		private void WriteTextBox(TextBox textBox)
		{
			if (textBox == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TextBox;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemBase(textBox);
				this.WriteExpressionInfo(textBox.Value);
				this.m_writer.WriteBoolean(textBox.CanGrow);
				this.m_writer.WriteBoolean(textBox.CanShrink);
				this.m_writer.WriteString(textBox.HideDuplicates);
				this.WriteAction(textBox.Action);
				this.m_writer.WriteBoolean(textBox.IsToggle);
				this.WriteExpressionInfo(textBox.InitialToggleState);
				this.WriteTypeCode(textBox.ValueType);
				this.m_writer.WriteString(textBox.Formula);
				this.m_writer.WriteBoolean(textBox.ValueReferenced);
				this.m_writer.WriteBoolean(textBox.RecursiveSender);
				this.m_writer.WriteBoolean(textBox.DataElementStyleAttribute);
				this.WriteGroupingReferenceList(textBox.ContainingScopes);
				this.WriteEndUserSort(textBox.UserSort);
				this.m_writer.WriteBoolean(textBox.IsMatrixCellScope);
				this.m_writer.WriteBoolean(textBox.IsSubReportTopLevelScope);
				this.m_writer.EndObject();
			}
		}

		private void WriteEndUserSort(EndUserSort userSort)
		{
			if (userSort == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.EndUserSort;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteInt32(userSort.DataSetID);
				this.WriteSortFilterScopeReference(userSort.SortExpressionScope);
				this.WriteGroupingReferenceList(userSort.GroupsInSortTarget);
				this.WriteSortFilterScopeReference(userSort.SortTarget);
				this.m_writer.WriteInt32(userSort.SortExpressionIndex);
				this.WriteSubReportList(userSort.DetailScopeSubReports);
				this.m_writer.EndObject();
			}
		}

		private void WriteSortFilterScopeReference(ISortFilterScope sortFilterScope)
		{
			if (sortFilterScope == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteReference(ObjectType.ISortFilterScope, sortFilterScope.ID);
			}
		}

		private void WriteGroupingReferenceList(GroupingList groups)
		{
			if (groups == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.GroupingList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(groups.Count);
				for (int i = 0; i < groups.Count; i++)
				{
					if (groups[i] == null)
					{
						this.m_writer.WriteNull();
					}
					else
					{
						this.m_writer.WriteReference(ObjectType.Grouping, groups[i].Owner.ID);
					}
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteTypeCode(TypeCode typeCode)
		{
			this.m_writer.WriteEnum((int)typeCode);
		}

		private void WriteSubReport(SubReport subReport)
		{
			if (subReport == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.SubReport;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemBase(subReport);
				this.m_writer.WriteString(subReport.ReportPath);
				this.WriteParameterValueList(subReport.Parameters);
				this.WriteExpressionInfo(subReport.NoRows);
				this.m_writer.WriteBoolean(subReport.MergeTransactions);
				this.WriteGroupingReferenceList(subReport.ContainingScopes);
				this.m_writer.WriteBoolean(subReport.IsMatrixCellScope);
				this.WriteScopeLookupTable(subReport.DataSetUniqueNameMap);
				this.WriteStatus(subReport.RetrievalStatus);
				this.m_writer.WriteString(subReport.ReportName);
				this.m_writer.WriteString(subReport.Description);
				this.WriteReport(subReport.Report);
				this.m_writer.WriteString(subReport.StringUri);
				this.WriteParameterInfoCollection(subReport.ParametersFromCatalog);
				this.m_writer.EndObject();
			}
		}

		private void WriteScopeLookupTable(ScopeLookupTable scopeTable)
		{
			if (scopeTable == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ScopeLookupTable;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteScopeTableValues(scopeTable.LookupTable);
				this.m_writer.EndObject();
			}
		}

		private void WriteScopeTableValues(object value)
		{
			if (value is int)
			{
				this.m_writer.WriteInt32((int)value);
			}
			else
			{
				Global.Tracer.Assert(value is Hashtable);
				Hashtable hashtable = (Hashtable)value;
				this.m_writer.StartArray(hashtable.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					this.WriteVariant(enumerator.Key, true);
					this.WriteScopeTableValues(enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == hashtable.Count);
				this.m_writer.EndArray();
			}
		}

		private void WriteStatus(SubReport.Status status)
		{
			this.m_writer.WriteEnum((int)status);
		}

		private void WriteActiveXControl(ActiveXControl control)
		{
			if (control == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ActiveXControl;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemBase(control);
				this.m_writer.WriteString(control.ClassID);
				this.m_writer.WriteString(control.CodeBase);
				this.WriteParameterValueList(control.Parameters);
				this.m_writer.EndObject();
			}
		}

		private void WriteParameterBase(ParameterBase parameter)
		{
			IntermediateFormatWriter.Assert(null != parameter);
			ObjectType objectType = ObjectType.ParameterBase;
			this.DeclareType(objectType);
			this.m_writer.WriteString(parameter.Name);
			this.WriteDataType(parameter.DataType);
			this.m_writer.WriteBoolean(parameter.Nullable);
			this.m_writer.WriteString(parameter.Prompt);
			this.m_writer.WriteBoolean(parameter.UsedInQuery);
			this.m_writer.WriteBoolean(parameter.AllowBlank);
			this.m_writer.WriteBoolean(parameter.MultiValue);
			this.WriteVariants(parameter.DefaultValues);
			this.m_writer.WriteBoolean(parameter.PromptUser);
		}

		private void WriteParameterDef(ParameterDef parameter)
		{
			if (parameter == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ParameterDef;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteParameterBase(parameter);
				this.WriteParameterDataSource(parameter.ValidValuesDataSource);
				this.WriteExpressionInfoList(parameter.ValidValuesValueExpressions);
				this.WriteExpressionInfoList(parameter.ValidValuesLabelExpressions);
				this.WriteParameterDataSource(parameter.DefaultDataSource);
				this.WriteExpressionInfoList(parameter.DefaultExpressions);
				this.WriteParameterDefRefList(parameter.DependencyList);
				this.m_writer.WriteInt32(parameter.ExprHostID);
				this.m_writer.EndObject();
			}
		}

		private void WriteParameterDataSource(ParameterDataSource paramDataSource)
		{
			if (paramDataSource == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ParameterDataSource;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteInt32(paramDataSource.DataSourceIndex);
				this.m_writer.WriteInt32(paramDataSource.DataSetIndex);
				this.m_writer.WriteInt32(paramDataSource.ValueFieldIndex);
				this.m_writer.WriteInt32(paramDataSource.LabelFieldIndex);
				this.m_writer.EndObject();
			}
		}

		private void WriteValidValue(ValidValue validValue)
		{
			if (validValue == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ValidValue;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(validValue.LabelRaw);
				this.WriteVariant(validValue.Value);
				this.m_writer.EndObject();
			}
		}

		private void WriteDataRegion(DataRegion dataRegion)
		{
			if (dataRegion == null)
			{
				this.m_writer.WriteNull();
			}
			else if (dataRegion is List)
			{
				this.WriteList((List)dataRegion);
			}
			else if (dataRegion is Matrix)
			{
				this.WriteMatrix((Matrix)dataRegion);
			}
			else if (dataRegion is Table)
			{
				this.WriteTable((Table)dataRegion);
			}
			else if (dataRegion is Chart)
			{
				this.WriteChart((Chart)dataRegion);
			}
			else if (dataRegion is CustomReportItem)
			{
				this.WriteCustomReportItem((CustomReportItem)dataRegion);
			}
			else
			{
				IntermediateFormatWriter.Assert(dataRegion is OWCChart);
				this.WriteOWCChart((OWCChart)dataRegion);
			}
		}

		private void WriteDataRegionBase(DataRegion dataRegion)
		{
			IntermediateFormatWriter.Assert(null != dataRegion);
			ObjectType objectType = ObjectType.DataRegion;
			this.DeclareType(objectType);
			this.WriteReportItemBase(dataRegion);
			this.m_writer.WriteString(dataRegion.DataSetName);
			this.WriteExpressionInfo(dataRegion.NoRows);
			this.m_writer.WriteBoolean(dataRegion.PageBreakAtEnd);
			this.m_writer.WriteBoolean(dataRegion.PageBreakAtStart);
			this.m_writer.WriteBoolean(dataRegion.KeepTogether);
			this.WriteIntList(dataRegion.RepeatSiblings);
			this.WriteFilterList(dataRegion.Filters);
			this.WriteDataAggregateInfoList(dataRegion.Aggregates);
			this.WriteDataAggregateInfoList(dataRegion.PostSortAggregates);
			this.WriteExpressionInfoList(dataRegion.UserSortExpressions);
			this.WriteInScopeSortFilterHashtable(dataRegion.DetailSortFiltersInScope);
		}

		private void WriteDataRegionReference(DataRegion dataRegion)
		{
			if (dataRegion == null)
			{
				this.m_writer.WriteNull();
			}
			else if (dataRegion is List)
			{
				this.m_writer.WriteReference(ObjectType.List, dataRegion.ID);
			}
			else if (dataRegion is Table)
			{
				this.m_writer.WriteReference(ObjectType.Table, dataRegion.ID);
			}
			else if (dataRegion is Matrix)
			{
				this.m_writer.WriteReference(ObjectType.Matrix, dataRegion.ID);
			}
			else if (dataRegion is Chart)
			{
				this.m_writer.WriteReference(ObjectType.Chart, dataRegion.ID);
			}
			else if (dataRegion is CustomReportItem)
			{
				this.m_writer.WriteReference(ObjectType.CustomReportItem, dataRegion.ID);
			}
			else
			{
				IntermediateFormatWriter.Assert(dataRegion is OWCChart);
				this.m_writer.WriteReference(ObjectType.OWCChart, dataRegion.ID);
			}
		}

		private void WriteReportHierarchyNode(ReportHierarchyNode node)
		{
			if (node == null)
			{
				this.m_writer.WriteNull();
			}
			else if (node is TableGroup)
			{
				this.WriteTableGroup((TableGroup)node);
			}
			else if (node is MatrixHeading)
			{
				this.WriteMatrixHeading((MatrixHeading)node);
			}
			else if (node is ChartHeading)
			{
				this.WriteChartHeading((ChartHeading)node);
			}
			else if (node is MultiChart)
			{
				this.WriteMultiChart((MultiChart)node);
			}
			else if (node is CustomReportItemHeading)
			{
				this.WriteCustomReportItemHeading((CustomReportItemHeading)node);
			}
			else
			{
				ObjectType objectType = ObjectType.ReportHierarchyNode;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportHierarchyNodeBase(node);
				this.m_writer.EndObject();
			}
		}

		private void WriteReportHierarchyNodeBase(ReportHierarchyNode node)
		{
			IntermediateFormatWriter.Assert(null != node);
			ObjectType objectType = ObjectType.ReportHierarchyNode;
			this.DeclareType(objectType);
			this.WriteIDOwnerBase(node);
			this.WriteGrouping(node.Grouping);
			this.WriteSorting(node.Sorting);
			this.WriteReportHierarchyNode(node.InnerHierarchy);
			this.WriteDataRegionReference(node.DataRegionDef);
		}

		private void WriteGrouping(Grouping grouping)
		{
			if (grouping == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Grouping;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(grouping.Name);
				this.WriteExpressionInfoList(grouping.GroupExpressions);
				this.WriteExpressionInfo(grouping.GroupLabel);
				this.WriteBoolList(grouping.SortDirections);
				this.m_writer.WriteBoolean(grouping.PageBreakAtEnd);
				this.m_writer.WriteBoolean(grouping.PageBreakAtStart);
				this.m_writer.WriteString(grouping.Custom);
				this.WriteDataAggregateInfoList(grouping.Aggregates);
				this.m_writer.WriteBoolean(grouping.GroupAndSort);
				this.WriteFilterList(grouping.Filters);
				this.WriteReportItemIDList(grouping.ReportItemsWithHideDuplicates);
				this.WriteExpressionInfoList(grouping.Parent);
				this.WriteDataAggregateInfoList(grouping.RecursiveAggregates);
				this.WriteDataAggregateInfoList(grouping.PostSortAggregates);
				this.m_writer.WriteString(grouping.DataElementName);
				this.m_writer.WriteString(grouping.DataCollectionName);
				this.WriteDataElementOutputType(grouping.DataElementOutput);
				this.WriteDataValueList(grouping.CustomProperties);
				this.m_writer.WriteBoolean(grouping.SaveGroupExprValues);
				this.WriteExpressionInfoList(grouping.UserSortExpressions);
				this.WriteInScopeSortFilterHashtable(grouping.NonDetailSortFiltersInScope);
				this.WriteInScopeSortFilterHashtable(grouping.DetailSortFiltersInScope);
				this.m_writer.EndObject();
			}
		}

		private void WriteInScopeSortFilterHashtable(InScopeSortFilterHashtable inScopeSortFilters)
		{
			if (inScopeSortFilters == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.InScopeSortFilterHashtable;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(inScopeSortFilters.Count);
				int num = 0;
				IDictionaryEnumerator enumerator = inScopeSortFilters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int int32Value = (int)enumerator.Key;
					this.m_writer.WriteInt32(int32Value);
					this.WriteIntList((IntList)enumerator.Value);
					num++;
				}
				IntermediateFormatWriter.Assert(num == inScopeSortFilters.Count);
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteSorting(Sorting sorting)
		{
			if (sorting == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Sorting;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteExpressionInfoList(sorting.SortExpressions);
				this.WriteBoolList(sorting.SortDirections);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableGroup(TableGroup tableGroup)
		{
			if (tableGroup == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableGroup;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportHierarchyNodeBase(tableGroup);
				this.WriteTableRowList(tableGroup.HeaderRows);
				this.m_writer.WriteBoolean(tableGroup.HeaderRepeatOnNewPage);
				this.WriteTableRowList(tableGroup.FooterRows);
				this.m_writer.WriteBoolean(tableGroup.FooterRepeatOnNewPage);
				this.WriteVisibility(tableGroup.Visibility);
				this.m_writer.WriteBoolean(tableGroup.PropagatedPageBreakAtStart);
				this.m_writer.WriteBoolean(tableGroup.PropagatedPageBreakAtEnd);
				this.WriteRunningValueInfoList(tableGroup.RunningValues);
				this.m_writer.WriteBoolean(tableGroup.HasExprHost);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableGroupReference(TableGroup tableGroup)
		{
			if (tableGroup == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteReference(ObjectType.TableGroup, tableGroup.ID);
			}
		}

		private void WriteTableDetail(TableDetail tableDetail)
		{
			if (tableDetail == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableDetail;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteIDOwnerBase(tableDetail);
				this.WriteTableRowList(tableDetail.DetailRows);
				this.WriteSorting(tableDetail.Sorting);
				this.WriteVisibility(tableDetail.Visibility);
				this.WriteRunningValueInfoList(tableDetail.RunningValues);
				this.m_writer.WriteBoolean(tableDetail.HasExprHost);
				this.m_writer.WriteBoolean(tableDetail.SimpleDetailRows);
				this.m_writer.EndObject();
			}
		}

		private void WritePivotHeadingBase(PivotHeading pivotHeading)
		{
			if (pivotHeading == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.PivotHeading;
				this.DeclareType(objectType);
				this.WriteReportHierarchyNodeBase(pivotHeading);
				this.WriteVisibility(pivotHeading.Visibility);
				this.WriteSubtotal(pivotHeading.Subtotal);
				this.m_writer.WriteInt32(pivotHeading.Level);
				this.m_writer.WriteBoolean(pivotHeading.IsColumn);
				this.m_writer.WriteBoolean(pivotHeading.HasExprHost);
				this.m_writer.WriteInt32(pivotHeading.SubtotalSpan);
				this.WriteIntList(pivotHeading.IDs);
			}
		}

		private void WriteMatrixHeading(MatrixHeading matrixHeading)
		{
			if (matrixHeading == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixHeading;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WritePivotHeadingBase(matrixHeading);
				this.m_writer.WriteString(matrixHeading.Size);
				this.m_writer.WriteDouble(matrixHeading.SizeValue);
				this.WriteReportItemCollection(matrixHeading.ReportItems);
				this.m_writer.WriteBoolean(matrixHeading.OwcGroupExpression);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixHeadingReference(MatrixHeading matrixHeading)
		{
			if (matrixHeading == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteReference(ObjectType.MatrixHeading, matrixHeading.ID);
			}
		}

		private void WriteTablixHeadingBase(TablixHeading tablixHeading)
		{
			if (tablixHeading == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TablixHeading;
				this.DeclareType(objectType);
				this.WriteReportHierarchyNodeBase(tablixHeading);
				this.m_writer.WriteBoolean(tablixHeading.Subtotal);
				this.m_writer.WriteBoolean(tablixHeading.IsColumn);
				this.m_writer.WriteInt32(tablixHeading.Level);
				this.m_writer.WriteBoolean(tablixHeading.HasExprHost);
				this.m_writer.WriteInt32(tablixHeading.HeadingSpan);
			}
		}

		private void WriteCustomReportItemHeading(CustomReportItemHeading heading)
		{
			if (heading == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItemHeading;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteTablixHeadingBase(heading);
				this.m_writer.WriteBoolean(heading.Static);
				this.WriteCustomReportItemHeadingList(heading.InnerHeadings);
				this.WriteDataValueList(heading.CustomProperties);
				this.m_writer.WriteInt32(heading.ExprHostID);
				this.WriteRunningValueInfoList(heading.RunningValues);
				this.m_writer.EndObject();
			}
		}

		private void WriteCustomReportItemHeadingReference(CustomReportItemHeading heading)
		{
			if (heading == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteReference(ObjectType.CustomReportItemHeading, heading.ID);
			}
		}

		private void WriteTableRow(TableRow tableRow)
		{
			if (tableRow == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableRow;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteIDOwnerBase(tableRow);
				this.WriteReportItemCollection(tableRow.ReportItems);
				this.WriteIntList(tableRow.IDs);
				this.WriteIntList(tableRow.ColSpans);
				this.m_writer.WriteString(tableRow.Height);
				this.m_writer.WriteDouble(tableRow.HeightValue);
				this.WriteVisibility(tableRow.Visibility);
				this.m_writer.EndObject();
			}
		}

		private void WriteSubtotal(Subtotal subtotal)
		{
			if (subtotal == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Subtotal;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteIDOwnerBase(subtotal);
				this.m_writer.WriteBoolean(subtotal.AutoDerived);
				this.WriteReportItemCollection(subtotal.ReportItems);
				this.WriteStyle(subtotal.StyleClass);
				this.WritePositionType(subtotal.Position);
				this.m_writer.WriteString(subtotal.DataElementName);
				this.WriteDataElementOutputType(subtotal.DataElementOutput);
				this.m_writer.EndObject();
			}
		}

		private void WritePositionType(Subtotal.PositionType positionType)
		{
			this.m_writer.WriteEnum((int)positionType);
		}

		private void WriteList(List list)
		{
			if (list == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.List;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteDataRegionBase(list);
				this.WriteReportHierarchyNode(list.HierarchyDef);
				this.WriteReportItemCollection(list.ReportItems);
				this.m_writer.WriteBoolean(list.FillPage);
				this.m_writer.WriteString(list.DataInstanceName);
				this.WriteDataElementOutputType(list.DataInstanceElementOutput);
				this.m_writer.WriteBoolean(list.IsListMostInner);
				this.m_writer.EndObject();
			}
		}

		private void WritePivotBase(Pivot pivot)
		{
			IntermediateFormatWriter.Assert(null != pivot);
			ObjectType objectType = ObjectType.Pivot;
			this.DeclareType(objectType);
			this.WriteDataRegionBase(pivot);
			this.m_writer.WriteInt32(pivot.ColumnCount);
			this.m_writer.WriteInt32(pivot.RowCount);
			this.WriteDataAggregateInfoList(pivot.CellAggregates);
			this.WriteProcessingInnerGrouping(pivot.ProcessingInnerGrouping);
			this.WriteRunningValueInfoList(pivot.RunningValues);
			this.WriteDataAggregateInfoList(pivot.CellPostSortAggregates);
			this.WriteDataElementOutputType(pivot.CellDataElementOutput);
		}

		private void WriteTablixBase(Tablix tablix)
		{
			IntermediateFormatWriter.Assert(null != tablix);
			ObjectType objectType = ObjectType.Tablix;
			this.DeclareType(objectType);
			this.WriteDataRegionBase(tablix);
			this.m_writer.WriteInt32(tablix.ColumnCount);
			this.m_writer.WriteInt32(tablix.RowCount);
			this.WriteDataAggregateInfoList(tablix.CellAggregates);
			this.WriteProcessingInnerGrouping(tablix.ProcessingInnerGrouping);
			this.WriteRunningValueInfoList(tablix.RunningValues);
			this.WriteDataAggregateInfoList(tablix.CellPostSortAggregates);
		}

		private void WriteCustomReportItem(CustomReportItem custom)
		{
			if (custom == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItem;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteTablixBase(custom);
				this.m_writer.WriteString(custom.Type);
				this.WriteReportItemCollection(custom.AltReportItem);
				this.WriteCustomReportItemHeadingList(custom.Columns);
				this.WriteCustomReportItemHeadingList(custom.Rows);
				this.WriteDataCellsList(custom.DataRowCells);
				this.WriteRunningValueInfoList(custom.CellRunningValues);
				this.WriteIntList(custom.CellExprHostIDs);
				this.WriteReportItemCollection(custom.RenderReportItem);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartDataPointList(ChartDataPointList datapoints)
		{
			if (datapoints == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartDataPointList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(datapoints.Count);
				for (int i = 0; i < datapoints.Count; i++)
				{
					this.WriteChartDataPoint(datapoints[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteChartDataPoint(ChartDataPoint datapoint)
		{
			if (datapoint == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartDataPoint;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteExpressionInfoList(datapoint.DataValues);
				this.WriteChartDataLabel(datapoint.DataLabel);
				this.WriteAction(datapoint.Action);
				this.WriteStyle(datapoint.StyleClass);
				this.m_writer.WriteEnum((int)datapoint.MarkerType);
				this.m_writer.WriteString(datapoint.MarkerSize);
				this.WriteStyle(datapoint.MarkerStyleClass);
				this.m_writer.WriteString(datapoint.DataElementName);
				this.WriteDataElementOutputType(datapoint.DataElementOutput);
				this.m_writer.WriteInt32(datapoint.ExprHostID);
				this.WriteDataValueList(datapoint.CustomProperties);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartDataLabel(ChartDataLabel label)
		{
			if (label == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartDataLabel;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteBoolean(label.Visible);
				this.WriteExpressionInfo(label.Value);
				this.WriteStyle(label.StyleClass);
				this.m_writer.WriteEnum((int)label.Position);
				this.m_writer.WriteInt32(label.Rotation);
				this.m_writer.EndObject();
			}
		}

		private void WriteMultiChart(MultiChart multiChart)
		{
			if (multiChart == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MultiChart;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportHierarchyNodeBase(multiChart);
				this.m_writer.WriteEnum((int)multiChart.Layout);
				this.m_writer.WriteInt32(multiChart.MaxCount);
				this.m_writer.WriteBoolean(multiChart.SyncScale);
				this.m_writer.EndObject();
			}
		}

		private void WriteLegend(Legend legend)
		{
			if (legend == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Legend;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteBoolean(legend.Visible);
				this.WriteStyle(legend.StyleClass);
				this.m_writer.WriteEnum((int)legend.Position);
				this.m_writer.WriteEnum((int)legend.Layout);
				this.m_writer.WriteBoolean(legend.InsidePlotArea);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartHeading(ChartHeading chartHeading)
		{
			if (chartHeading == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartHeading;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WritePivotHeadingBase(chartHeading);
				this.WriteExpressionInfoList(chartHeading.Labels);
				this.WriteRunningValueInfoList(chartHeading.RunningValues);
				this.m_writer.WriteBoolean(chartHeading.ChartGroupExpression);
				this.WriteBoolList(chartHeading.PlotTypesLine);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartHeadingReference(ChartHeading chartHeading)
		{
			if (chartHeading == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteReference(ObjectType.ChartHeading, chartHeading.ID);
			}
		}

		private void WriteAxis(Axis axis)
		{
			if (axis == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Axis;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteBoolean(axis.Visible);
				this.WriteStyle(axis.StyleClass);
				this.WriteChartTitle(axis.Title);
				this.m_writer.WriteBoolean(axis.Margin);
				this.m_writer.WriteEnum((int)axis.MajorTickMarks);
				this.m_writer.WriteEnum((int)axis.MinorTickMarks);
				this.WriteGridLines(axis.MajorGridLines);
				this.WriteGridLines(axis.MinorGridLines);
				this.WriteExpressionInfo(axis.MajorInterval);
				this.WriteExpressionInfo(axis.MinorInterval);
				this.m_writer.WriteBoolean(axis.Reverse);
				this.WriteExpressionInfo(axis.CrossAt);
				this.m_writer.WriteBoolean(axis.AutoCrossAt);
				this.m_writer.WriteBoolean(axis.Interlaced);
				this.m_writer.WriteBoolean(axis.Scalar);
				this.WriteExpressionInfo(axis.Min);
				this.WriteExpressionInfo(axis.Max);
				this.m_writer.WriteBoolean(axis.AutoScaleMin);
				this.m_writer.WriteBoolean(axis.AutoScaleMax);
				this.m_writer.WriteBoolean(axis.LogScale);
				this.WriteDataValueList(axis.CustomProperties);
				this.m_writer.EndObject();
			}
		}

		private void WriteGridLines(GridLines gridLines)
		{
			if (gridLines == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.GridLines;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteBoolean(gridLines.ShowGridLines);
				this.WriteStyle(gridLines.StyleClass);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartTitle(ChartTitle chartTitle)
		{
			if (chartTitle == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartTitle;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteExpressionInfo(chartTitle.Caption);
				this.WriteStyle(chartTitle.StyleClass);
				this.m_writer.WriteEnum((int)chartTitle.Position);
				this.m_writer.EndObject();
			}
		}

		private void WriteThreeDProperties(ThreeDProperties properties)
		{
			if (properties == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ThreeDProperties;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteBoolean(properties.Enabled);
				this.m_writer.WriteBoolean(properties.PerspectiveProjectionMode);
				this.m_writer.WriteInt32(properties.Rotation);
				this.m_writer.WriteInt32(properties.Inclination);
				this.m_writer.WriteInt32(properties.Perspective);
				this.m_writer.WriteInt32(properties.HeightRatio);
				this.m_writer.WriteInt32(properties.DepthRatio);
				this.m_writer.WriteEnum((int)properties.Shading);
				this.m_writer.WriteInt32(properties.GapDepth);
				this.m_writer.WriteInt32(properties.WallThickness);
				this.m_writer.WriteBoolean(properties.DrawingStyleCube);
				this.m_writer.WriteBoolean(properties.Clustered);
				this.m_writer.EndObject();
			}
		}

		private void WritePlotArea(PlotArea plotArea)
		{
			if (plotArea == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.PlotArea;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteEnum((int)plotArea.Origin);
				this.WriteStyle(plotArea.StyleClass);
				this.m_writer.EndObject();
			}
		}

		private void WriteChart(Chart chart)
		{
			if (chart == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Chart;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WritePivotBase(chart);
				this.WriteChartHeading(chart.Columns);
				this.WriteChartHeading(chart.Rows);
				this.WriteChartDataPointList(chart.ChartDataPoints);
				this.WriteRunningValueInfoList(chart.CellRunningValues);
				this.WriteMultiChart(chart.MultiChart);
				this.WriteLegend(chart.Legend);
				this.WriteAxis(chart.CategoryAxis);
				this.WriteAxis(chart.ValueAxis);
				this.WriteChartHeadingReference(chart.StaticColumns);
				this.WriteChartHeadingReference(chart.StaticRows);
				this.m_writer.WriteEnum((int)chart.Type);
				this.m_writer.WriteEnum((int)chart.SubType);
				this.m_writer.WriteEnum((int)chart.Palette);
				this.WriteChartTitle(chart.Title);
				this.m_writer.WriteInt32(chart.PointWidth);
				this.WriteThreeDProperties(chart.ThreeDProperties);
				this.WritePlotArea(chart.PlotArea);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrix(Matrix matrix)
		{
			if (matrix == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Matrix;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WritePivotBase(matrix);
				this.WriteReportItemCollection(matrix.CornerReportItems);
				this.WriteMatrixHeading(matrix.Columns);
				this.WriteMatrixHeading(matrix.Rows);
				this.WriteReportItemCollection(matrix.CellReportItems);
				this.WriteIntList(matrix.CellIDs);
				this.m_writer.WriteBoolean(matrix.PropagatedPageBreakAtStart);
				this.m_writer.WriteBoolean(matrix.PropagatedPageBreakAtEnd);
				this.m_writer.WriteInt32(matrix.InnerRowLevelWithPageBreak);
				this.WriteMatrixRowList(matrix.MatrixRows);
				this.WriteMatrixColumnList(matrix.MatrixColumns);
				this.m_writer.WriteInt32(matrix.GroupsBeforeRowHeaders);
				this.m_writer.WriteBoolean(matrix.LayoutDirection);
				this.WriteMatrixHeadingReference(matrix.StaticColumns);
				this.WriteMatrixHeadingReference(matrix.StaticRows);
				this.m_writer.WriteBoolean(matrix.UseOWC);
				this.WriteStringList(matrix.OwcCellNames);
				this.m_writer.WriteString(matrix.CellDataElementName);
				this.m_writer.WriteBoolean(matrix.ColumnGroupingFixedHeader);
				this.m_writer.WriteBoolean(matrix.RowGroupingFixedHeader);
				this.m_writer.EndObject();
			}
		}

		private void WriteProcessingInnerGrouping(Pivot.ProcessingInnerGroupings directions)
		{
			this.m_writer.WriteEnum((int)directions);
		}

		private void WriteMatrixRow(MatrixRow row)
		{
			if (row == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixRow;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(row.Height);
				this.m_writer.WriteDouble(row.HeightValue);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixColumn(MatrixColumn column)
		{
			if (column == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixColumn;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(column.Width);
				this.m_writer.WriteDouble(column.WidthValue);
				this.m_writer.EndObject();
			}
		}

		private void WriteTable(Table table)
		{
			if (table == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.Table;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteDataRegionBase(table);
				this.WriteTableColumnList(table.TableColumns);
				this.WriteTableRowList(table.HeaderRows);
				this.m_writer.WriteBoolean(table.HeaderRepeatOnNewPage);
				this.WriteTableGroup(table.TableGroups);
				this.WriteTableDetail(table.TableDetail);
				this.WriteTableGroupReference(table.DetailGroup);
				this.WriteTableRowList(table.FooterRows);
				this.m_writer.WriteBoolean(table.FooterRepeatOnNewPage);
				this.m_writer.WriteBoolean(table.PropagatedPageBreakAtStart);
				this.m_writer.WriteBoolean(table.GroupBreakAtStart);
				this.m_writer.WriteBoolean(table.PropagatedPageBreakAtEnd);
				this.m_writer.WriteBoolean(table.GroupBreakAtEnd);
				this.m_writer.WriteBoolean(table.FillPage);
				this.m_writer.WriteBoolean(table.UseOWC);
				this.m_writer.WriteBoolean(table.OWCNonSharedStyles);
				this.WriteRunningValueInfoList(table.RunningValues);
				this.m_writer.WriteString(table.DetailDataElementName);
				this.m_writer.WriteString(table.DetailDataCollectionName);
				this.WriteDataElementOutputType(table.DetailDataElementOutput);
				this.m_writer.WriteBoolean(table.FixedHeader);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableColumn(TableColumn column)
		{
			if (column == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableColumn;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(column.Width);
				this.m_writer.WriteDouble(column.WidthValue);
				this.WriteVisibility(column.Visibility);
				this.m_writer.WriteBoolean(column.FixedHeader);
				this.m_writer.EndObject();
			}
		}

		private void WriteOWCChart(OWCChart chart)
		{
			if (chart == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.OWCChart;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteDataRegionBase(chart);
				this.WriteChartColumnList(chart.ChartData);
				this.m_writer.WriteString(chart.ChartDefinition);
				this.WriteRunningValueInfoList(chart.DetailRunningValues);
				this.WriteRunningValueInfoList(chart.RunningValues);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartColumn(ChartColumn column)
		{
			if (column == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartColumn;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(column.Name);
				this.WriteExpressionInfo(column.Value);
				this.m_writer.EndObject();
			}
		}

		private void WriteDataValue(DataValue value)
		{
			if (value == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataValue;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteExpressionInfo(value.Name);
				this.WriteExpressionInfo(value.Value);
				this.m_writer.WriteInt32(value.ExprHostID);
				this.m_writer.EndObject();
			}
		}

		private void WriteParameterInfo(ParameterInfo parameter)
		{
			if (parameter == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ParameterInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteParameterBase(parameter);
				this.m_writer.WriteBoolean(parameter.IsUserSupplied);
				this.WriteVariants(parameter.Values);
				this.m_writer.WriteBoolean(parameter.DynamicValidValues);
				this.m_writer.WriteBoolean(parameter.DynamicDefaultValue);
				this.WriteParameterInfoRefCollection(parameter.DependencyList);
				this.WriteValidValueList(parameter.ValidValues);
				this.WriteStrings(parameter.Labels);
				this.m_writer.EndObject();
			}
		}

		private void WriteProcessingMessage(ProcessingMessage message)
		{
			if (message == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ProcessingMessage;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteEnum((int)message.Code);
				this.m_writer.WriteEnum((int)message.Severity);
				this.m_writer.WriteEnum((int)message.ObjectType);
				this.m_writer.WriteString(message.ObjectName);
				this.m_writer.WriteString(message.PropertyName);
				this.m_writer.WriteString(message.Message);
				this.WriteProcessingMessageList(message.ProcessingMessages);
				this.m_writer.WriteEnum((int)message.CommonCode);
				this.m_writer.EndObject();
			}
		}

		private void WriteDataValueInstance(DataValueInstance instance)
		{
			if (instance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DataValueInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(instance.Name);
				this.WriteVariant(instance.Value);
				this.m_writer.EndObject();
			}
		}

		private void WriteDataType(DataType dataType)
		{
			this.m_writer.WriteEnum((int)dataType);
		}

		private void WriteBookmarkInformation(BookmarkInformation bookmark)
		{
			if (bookmark == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.BookmarkInformation;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(bookmark.Id);
				this.m_writer.WriteInt32(bookmark.Page);
				this.m_writer.EndObject();
			}
		}

		private void WriteDrillthroughInformation(DrillthroughInformation drillthroughInfo)
		{
			if (drillthroughInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.DrillthroughInformation;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(drillthroughInfo.ReportName);
				this.WriteDrillthroughParameters(drillthroughInfo.ReportParameters);
				this.WriteIntList(drillthroughInfo.DataSetTokenIDs);
				this.m_writer.EndObject();
			}
		}

		private void WriteSenderInformation(SenderInformation sender)
		{
			if (sender == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.SenderInformation;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteBoolean(sender.StartHidden);
				this.WriteIntList(sender.ReceiverUniqueNames);
				this.m_writer.WriteInt32s(sender.ContainerUniqueNames);
				this.m_writer.EndObject();
			}
		}

		private void WriteReceiverInformation(ReceiverInformation receiver)
		{
			if (receiver == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReceiverInformation;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteBoolean(receiver.StartHidden);
				this.m_writer.WriteInt32(receiver.SenderUniqueName);
				this.m_writer.EndObject();
			}
		}

		private void WriteInfoBaseBase(InfoBase infoBase)
		{
			IntermediateFormatWriter.Assert(null != infoBase);
			ObjectType objectType = ObjectType.InfoBase;
			this.DeclareType(objectType);
		}

		private void WriteSimpleOffsetInfo(OffsetInfo offsetInfo)
		{
			this.m_writer.WriteInt64(offsetInfo.Offset);
		}

		private void WriteActionInstance(ActionInstance actionInstance)
		{
			if (actionInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ActionInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteActionItemInstanceList(actionInstance.ActionItemsValues);
				this.WriteVariants(actionInstance.StyleAttributeValues);
				this.m_writer.WriteInt32(actionInstance.UniqueName);
				this.m_writer.EndObject();
			}
		}

		private void WriteActionItemInstanceList(ActionItemInstanceList actionItemInstanceList)
		{
			if (actionItemInstanceList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ActionItemInstanceList;
				this.m_writer.StartObject(objectType);
				this.m_writer.StartArray(actionItemInstanceList.Count);
				for (int i = 0; i < actionItemInstanceList.Count; i++)
				{
					this.WriteActionItemInstance(actionItemInstanceList[i]);
				}
				this.m_writer.EndArray();
				this.m_writer.EndObject();
			}
		}

		private void WriteActionItemInstance(ActionItemInstance actionItemInstance)
		{
			if (actionItemInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ActionItemInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteString(actionItemInstance.HyperLinkURL);
				this.m_writer.WriteString(actionItemInstance.BookmarkLink);
				this.m_writer.WriteString(actionItemInstance.DrillthroughReportName);
				this.WriteDrillthroughVariants(actionItemInstance.DrillthroughParametersValues, actionItemInstance.DataSetTokenIDs);
				this.WriteBoolList(actionItemInstance.DrillthroughParametersOmits);
				this.m_writer.WriteString(actionItemInstance.Label);
				this.m_writer.EndObject();
			}
		}

		private void WriteDrillthroughVariants(object[] variants, IntList tokenIDs)
		{
			if (variants == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(variants.Length);
				object obj = null;
				for (int i = 0; i < variants.Length; i++)
				{
					obj = null;
					if (tokenIDs == null || tokenIDs[i] < 0)
					{
						obj = variants[i];
					}
					if (obj is object[])
					{
						this.WriteVariants(obj as object[], false);
					}
					else
					{
						this.WriteVariant(obj);
					}
				}
				this.m_writer.EndArray();
			}
		}

		private void WriteReportInstanceInfo(ReportInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.WriteParameterInfoCollection(instanceInfo.Parameters);
				this.m_writer.WriteString(instanceInfo.ReportName);
				this.m_writer.WriteBoolean(instanceInfo.NoRows);
				this.m_writer.WriteInt32(instanceInfo.BodyUniqueName);
				this.m_writer.EndObject();
			}
		}

		private void WriteReportItemColInstanceInfo(ReportItemColInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportItemColInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.WriteNonComputedUniqueNamess(instanceInfo.ChildrenNonComputedUniqueNames);
				this.m_writer.EndObject();
			}
		}

		private void WriteReportItemInstanceInfo(ReportItemInstanceInfo reportItemInstanceInfo)
		{
			if (reportItemInstanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else if (reportItemInstanceInfo is LineInstanceInfo)
			{
				this.WriteLineInstanceInfo((LineInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is RectangleInstanceInfo)
			{
				this.WriteRectangleInstanceInfo((RectangleInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is ImageInstanceInfo)
			{
				this.WriteImageInstanceInfo((ImageInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is CheckBoxInstanceInfo)
			{
				this.WriteCheckBoxInstanceInfo((CheckBoxInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is TextBoxInstanceInfo)
			{
				this.WriteTextBoxInstanceInfo((TextBoxInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is SubReportInstanceInfo)
			{
				this.WriteSubReportInstanceInfo((SubReportInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is ActiveXControlInstanceInfo)
			{
				this.WriteActiveXControlInstanceInfo((ActiveXControlInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is ListInstanceInfo)
			{
				this.WriteListInstanceInfo((ListInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is MatrixInstanceInfo)
			{
				this.WriteMatrixInstanceInfo((MatrixInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is TableInstanceInfo)
			{
				this.WriteTableInstanceInfo((TableInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is OWCChartInstanceInfo)
			{
				this.WriteOWCChartInstanceInfo((OWCChartInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is ChartInstanceInfo)
			{
				this.WriteChartInstanceInfo((ChartInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is CustomReportItemInstanceInfo)
			{
				this.WriteCustomReportItemInstanceInfo((CustomReportItemInstanceInfo)reportItemInstanceInfo);
			}
			else if (reportItemInstanceInfo is PageSectionInstanceInfo)
			{
				this.WritePageSectionInstanceInfo((PageSectionInstanceInfo)reportItemInstanceInfo);
			}
			else
			{
				IntermediateFormatWriter.Assert(reportItemInstanceInfo is ReportInstanceInfo);
				this.WriteReportInstanceInfo((ReportInstanceInfo)reportItemInstanceInfo);
			}
		}

		private void WriteLineInstanceInfo(LineInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.LineInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.m_writer.EndObject();
			}
		}

		private void WriteTextBoxInstanceInfo(TextBoxInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TextBoxInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.m_writer.WriteString(instanceInfo.FormattedValue);
				this.WriteVariant(instanceInfo.OriginalValue);
				TextBox textBox = (TextBox)instanceInfo.ReportItemDef;
				if (textBox.HideDuplicates != null)
				{
					this.m_writer.WriteBoolean(instanceInfo.Duplicate);
				}
				if (textBox.Action != null)
				{
					this.WriteActionInstance(instanceInfo.Action);
				}
				if (textBox.InitialToggleState != null)
				{
					this.m_writer.WriteBoolean(instanceInfo.InitialToggleState);
				}
				this.m_writer.EndObject();
			}
		}

		private void WriteSimpleTextBoxInstanceInfo(SimpleTextBoxInstanceInfo simpleTextBoxInstanceInfo)
		{
			if (simpleTextBoxInstanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.SimpleTextBoxInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(simpleTextBoxInstanceInfo);
				this.m_writer.WriteString(simpleTextBoxInstanceInfo.FormattedValue);
				this.WriteVariant(simpleTextBoxInstanceInfo.OriginalValue);
				this.m_writer.EndObject();
			}
		}

		private void WriteRectangleInstanceInfo(RectangleInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RectangleInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.m_writer.EndObject();
			}
		}

		private void WriteCheckBoxInstanceInfo(CheckBoxInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CheckBoxInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.m_writer.WriteBoolean(instanceInfo.Value);
				this.m_writer.WriteBoolean(instanceInfo.Duplicate);
				this.m_writer.EndObject();
			}
		}

		private void WriteImageInstanceInfo(ImageInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ImageInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.m_writer.WriteString(instanceInfo.ImageValue);
				this.WriteActionInstance(instanceInfo.Action);
				this.m_writer.WriteBoolean(instanceInfo.BrokenImage);
				this.WriteImageMapAreaInstanceList(instanceInfo.ImageMapAreas);
				this.m_writer.EndObject();
			}
		}

		private void WriteSubReportInstanceInfo(SubReportInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.SubReportInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.m_writer.WriteString(instanceInfo.NoRows);
				this.m_writer.EndObject();
			}
		}

		private void WriteActiveXControlInstanceInfo(ActiveXControlInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ActiveXControlInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.WriteVariants(instanceInfo.ParameterValues);
				this.m_writer.EndObject();
			}
		}

		private void WriteListInstanceInfo(ListInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ListInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.m_writer.WriteString(instanceInfo.NoRows);
				this.m_writer.EndObject();
			}
		}

		private void WriteListContentInstanceInfo(ListContentInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ListContentInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.m_writer.WriteBoolean(instanceInfo.StartHidden);
				this.m_writer.WriteString(instanceInfo.Label);
				this.WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixInstanceInfo(MatrixInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.WriteNonComputedUniqueNames(instanceInfo.CornerNonComputedNames);
				this.m_writer.WriteString(instanceInfo.NoRows);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixHeadingInstanceInfo(MatrixHeadingInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixHeadingInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.WriteNonComputedUniqueNames(instanceInfo.ContentUniqueNames);
				this.m_writer.WriteBoolean(instanceInfo.StartHidden);
				this.m_writer.WriteInt32(instanceInfo.HeadingCellIndex);
				this.m_writer.WriteInt32(instanceInfo.HeadingSpan);
				this.WriteVariant(instanceInfo.GroupExpressionValue);
				this.m_writer.WriteString(instanceInfo.Label);
				this.WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixSubtotalHeadingInstanceInfo(MatrixSubtotalHeadingInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixSubtotalHeadingInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.WriteMatrixHeadingInstanceInfo(instanceInfo);
				this.WriteVariants(instanceInfo.StyleAttributeValues);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixCellInstanceInfo(MatrixCellInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixCellInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.WriteNonComputedUniqueNames(instanceInfo.ContentUniqueNames);
				this.m_writer.WriteInt32(instanceInfo.RowIndex);
				this.m_writer.WriteInt32(instanceInfo.ColumnIndex);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartInstanceInfo(ChartInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.WriteAxisInstance(instanceInfo.CategoryAxis);
				this.WriteAxisInstance(instanceInfo.ValueAxis);
				this.WriteChartTitleInstance(instanceInfo.Title);
				this.WriteVariants(instanceInfo.PlotAreaStyleAttributeValues);
				this.WriteVariants(instanceInfo.LegendStyleAttributeValues);
				this.m_writer.WriteString(instanceInfo.CultureName);
				this.m_writer.WriteString(instanceInfo.NoRows);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartHeadingInstanceInfo(ChartHeadingInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartHeadingInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.WriteVariant(instanceInfo.HeadingLabel);
				this.m_writer.WriteInt32(instanceInfo.HeadingCellIndex);
				this.m_writer.WriteInt32(instanceInfo.HeadingSpan);
				this.WriteVariant(instanceInfo.GroupExpressionValue);
				this.m_writer.WriteInt32(instanceInfo.StaticGroupingIndex);
				this.WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartDataPointInstanceInfo(ChartDataPointInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartDataPointInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.m_writer.WriteInt32(instanceInfo.DataPointIndex);
				this.WriteVariants(instanceInfo.DataValues);
				this.m_writer.WriteString(instanceInfo.DataLabelValue);
				this.WriteVariants(instanceInfo.DataLabelStyleAttributeValues);
				this.WriteActionInstance(instanceInfo.Action);
				this.WriteVariants(instanceInfo.StyleAttributeValues);
				this.WriteVariants(instanceInfo.MarkerStyleAttributeValues);
				this.WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableInstanceInfo(TableInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.WriteTableColumnInstances(instanceInfo.ColumnInstances);
				this.m_writer.WriteString(instanceInfo.NoRows);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableGroupInstanceInfo(TableGroupInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableGroupInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.m_writer.WriteBoolean(instanceInfo.StartHidden);
				this.m_writer.WriteString(instanceInfo.Label);
				this.WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableDetailInstanceInfo(TableDetailInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableDetailInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.m_writer.WriteBoolean(instanceInfo.StartHidden);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableRowInstanceInfo(TableRowInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableRowInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoBase(instanceInfo);
				this.m_writer.WriteBoolean(instanceInfo.StartHidden);
				this.m_writer.EndObject();
			}
		}

		private void WriteOWCChartInstanceInfo(OWCChartInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.OWCChartInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.WriteVariantLists(instanceInfo.ChartData, false);
				this.m_writer.WriteString(instanceInfo.NoRows);
				this.m_writer.EndObject();
			}
		}

		private void WriteCustomReportItemInstanceInfo(CustomReportItemInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItemInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.m_writer.EndObject();
			}
		}

		private void WritePageSectionInstanceInfo(PageSectionInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.PageSectionInstanceInfo;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceInfoBase(instanceInfo);
				this.m_writer.EndObject();
			}
		}

		private void WriteInstanceInfoBase(InstanceInfo instanceInfo)
		{
			IntermediateFormatWriter.Assert(null != instanceInfo);
			ObjectType objectType = ObjectType.InstanceInfo;
			this.DeclareType(objectType);
			this.WriteInfoBaseBase(instanceInfo);
		}

		private void WriteReportItemInstanceInfoBase(ReportItemInstanceInfo instanceInfo)
		{
			IntermediateFormatWriter.Assert(null != instanceInfo);
			ObjectType objectType = ObjectType.ReportItemInstanceInfo;
			this.DeclareType(objectType);
			this.WriteInstanceInfoBase(instanceInfo);
			ReportItem reportItemDef = instanceInfo.ReportItemDef;
			if (reportItemDef.StyleClass != null && reportItemDef.StyleClass.ExpressionList != null)
			{
				this.WriteVariants(instanceInfo.StyleAttributeValues);
			}
			if (reportItemDef.Visibility != null)
			{
				this.m_writer.WriteBoolean(instanceInfo.StartHidden);
			}
			if (reportItemDef.Label != null)
			{
				this.m_writer.WriteString(instanceInfo.Label);
			}
			if (reportItemDef.Bookmark != null)
			{
				this.m_writer.WriteString(instanceInfo.Bookmark);
			}
			if (reportItemDef.ToolTip != null)
			{
				this.m_writer.WriteString(instanceInfo.ToolTip);
			}
			if (reportItemDef.CustomProperties != null)
			{
				this.WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
			}
		}

		private void WriteNonComputedUniqueNames(NonComputedUniqueNames names)
		{
			if (names == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.NonComputedUniqueNames;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteInt32(names.UniqueName);
				this.WriteNonComputedUniqueNamess(names.ChildrenUniqueNames);
				this.m_writer.EndObject();
			}
		}

		private void WriteInstanceInfoOwnerBase(InstanceInfoOwner owner)
		{
			IntermediateFormatWriter.Assert(null != owner);
			ObjectType objectType = ObjectType.InstanceInfoOwner;
			this.DeclareType(objectType);
			this.WriteSimpleOffsetInfo(owner.OffsetInfo);
		}

		private void WriteReportItemInstance(ReportItemInstance reportItemInstance)
		{
			if (reportItemInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else if (reportItemInstance is LineInstance)
			{
				this.WriteLineInstance((LineInstance)reportItemInstance);
			}
			else if (reportItemInstance is RectangleInstance)
			{
				this.WriteRectangleInstance((RectangleInstance)reportItemInstance);
			}
			else if (reportItemInstance is ImageInstance)
			{
				this.WriteImageInstance((ImageInstance)reportItemInstance);
			}
			else if (reportItemInstance is CheckBoxInstance)
			{
				this.WriteCheckBoxInstance((CheckBoxInstance)reportItemInstance);
			}
			else if (reportItemInstance is TextBoxInstance)
			{
				this.WriteTextBoxInstance((TextBoxInstance)reportItemInstance);
			}
			else if (reportItemInstance is SubReportInstance)
			{
				this.WriteSubReportInstance((SubReportInstance)reportItemInstance);
			}
			else if (reportItemInstance is ActiveXControlInstance)
			{
				this.WriteActiveXControlInstance((ActiveXControlInstance)reportItemInstance);
			}
			else if (reportItemInstance is ListInstance)
			{
				this.WriteListInstance((ListInstance)reportItemInstance);
			}
			else if (reportItemInstance is MatrixInstance)
			{
				this.WriteMatrixInstance((MatrixInstance)reportItemInstance);
			}
			else if (reportItemInstance is TableInstance)
			{
				this.WriteTableInstance((TableInstance)reportItemInstance);
			}
			else if (reportItemInstance is ChartInstance)
			{
				this.WriteChartInstance((ChartInstance)reportItemInstance);
			}
			else if (reportItemInstance is CustomReportItemInstance)
			{
				this.WriteCustomReportItemInstance((CustomReportItemInstance)reportItemInstance);
			}
			else
			{
				IntermediateFormatWriter.Assert(reportItemInstance is OWCChartInstance);
				this.WriteOWCChartInstance((OWCChartInstance)reportItemInstance);
			}
		}

		private void WriteReportItemInstanceBase(ReportItemInstance reportItemInstance)
		{
			IntermediateFormatWriter.Assert(null != reportItemInstance);
			ObjectType objectType = ObjectType.ReportItemInstance;
			this.DeclareType(objectType);
			this.WriteInstanceInfoOwnerBase(reportItemInstance);
			if (this.m_writeUniqueName)
			{
				this.m_writer.WriteInt32(reportItemInstance.UniqueName);
			}
		}

		private void WriteReportItemInstanceReference(ReportItemInstance reportItemInstance)
		{
			if (reportItemInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				IntermediateFormatWriter.Assert(reportItemInstance is OWCChartInstance || reportItemInstance is ChartInstance);
				ObjectType objectType = (ObjectType)((reportItemInstance is OWCChartInstance) ? 122 : 167);
				this.m_writer.WriteReference(objectType, reportItemInstance.UniqueName);
			}
		}

		private void WriteReportInstance(ReportInstance reportInstance)
		{
			if (reportInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(reportInstance);
				this.WriteReportItemColInstance(reportInstance.ReportItemColInstance);
				this.m_writer.WriteString(reportInstance.Language);
				this.m_writer.WriteInt32(reportInstance.NumberOfPages);
				this.m_writer.EndObject();
			}
		}

		private void WriteReportItemColInstance(ReportItemColInstance reportItemColInstance)
		{
			if (reportItemColInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ReportItemColInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoOwnerBase(reportItemColInstance);
				this.WriteReportItemInstanceList(reportItemColInstance.ReportItemInstances);
				this.WriteRenderingPagesRangesList(reportItemColInstance.ChildrenStartAndEndPages);
				this.m_writer.EndObject();
			}
		}

		private void WriteLineInstance(LineInstance lineInstance)
		{
			if (lineInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.LineInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(lineInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteTextBoxInstance(TextBoxInstance textBoxInstance)
		{
			if (textBoxInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TextBoxInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(textBoxInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteRectangleInstance(RectangleInstance rectangleInstance)
		{
			if (rectangleInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RectangleInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(rectangleInstance);
				this.WriteReportItemColInstance(rectangleInstance.ReportItemColInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteCheckBoxInstance(CheckBoxInstance checkBoxInstance)
		{
			if (checkBoxInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CheckBoxInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(checkBoxInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteImageInstance(ImageInstance imageInstance)
		{
			if (imageInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ImageInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(imageInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteSubReportInstance(SubReportInstance subReportInstance)
		{
			if (subReportInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.SubReportInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(subReportInstance);
				this.WriteReportInstance(subReportInstance.ReportInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteActiveXControlInstance(ActiveXControlInstance activeXControlInstance)
		{
			if (activeXControlInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ActiveXControlInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(activeXControlInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteListInstance(ListInstance listInstance)
		{
			if (listInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ListInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(listInstance);
				this.WriteListContentInstanceList(listInstance.ListContents);
				this.WriteRenderingPagesRangesList(listInstance.ChildrenStartAndEndPages);
				this.m_writer.EndObject();
			}
		}

		private void WriteListContentInstance(ListContentInstance listContentInstance)
		{
			if (listContentInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ListContentInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoOwnerBase(listContentInstance);
				this.m_writer.WriteInt32(listContentInstance.UniqueName);
				this.WriteReportItemColInstance(listContentInstance.ReportItemColInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixInstance(MatrixInstance matrixInstance)
		{
			if (matrixInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(matrixInstance);
				this.WriteReportItemInstance(matrixInstance.CornerContent);
				this.WriteMatrixHeadingInstanceList(matrixInstance.ColumnInstances);
				this.WriteMatrixHeadingInstanceList(matrixInstance.RowInstances);
				this.WriteMatrixCellInstancesList(matrixInstance.Cells);
				this.m_writer.WriteInt32(matrixInstance.InstanceCountOfInnerRowWithPageBreak);
				this.WriteRenderingPagesRangesList(matrixInstance.ChildrenStartAndEndPages);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixHeadingInstance(MatrixHeadingInstance matrixHeadingInstance)
		{
			if (matrixHeadingInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixHeadingInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoOwnerBase(matrixHeadingInstance);
				this.m_writer.WriteInt32(matrixHeadingInstance.UniqueName);
				this.WriteReportItemInstance(matrixHeadingInstance.Content);
				this.WriteMatrixHeadingInstanceList(matrixHeadingInstance.SubHeadingInstances);
				this.m_writer.WriteBoolean(matrixHeadingInstance.IsSubtotal);
				this.WriteRenderingPagesRangesList(matrixHeadingInstance.ChildrenStartAndEndPages);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixCellInstance(MatrixCellInstance matrixCellInstance)
		{
			if (matrixCellInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MatrixCellInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoOwnerBase(matrixCellInstance);
				ReportItemInstance content = matrixCellInstance.Content;
				ReportItem reportItem = (content == null) ? null : content.ReportItemDef;
				this.WriteReportItemReference(reportItem);
				this.WriteReportItemInstance(matrixCellInstance.Content);
				this.m_writer.EndObject();
			}
		}

		private void WriteMatrixSubtotalCellInstance(MatrixSubtotalCellInstance matrixSubtotalCellInstance)
		{
			Global.Tracer.Assert(null != matrixSubtotalCellInstance);
			ObjectType objectType = ObjectType.MatrixSubtotalCellInstance;
			this.DeclareType(objectType);
			this.m_writer.StartObject(objectType);
			this.WriteInstanceInfoOwnerBase(matrixSubtotalCellInstance);
			Global.Tracer.Assert(null != matrixSubtotalCellInstance.SubtotalHeadingInstance, "(null != matrixSubtotalCellInstance.SubtotalHeadingInstance)");
			this.WriteMatrixCellInstance(matrixSubtotalCellInstance);
			this.m_writer.WriteInt32(matrixSubtotalCellInstance.SubtotalHeadingInstance.UniqueName);
			this.m_writer.EndObject();
		}

		private void WriteChartInstance(ChartInstance chartInstance)
		{
			if (chartInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(chartInstance);
				this.WriteMultiChartInstanceList(chartInstance.MultiCharts);
				this.m_writer.EndObject();
			}
		}

		private void WriteMultiChartInstance(MultiChartInstance multiChartInstance)
		{
			if (multiChartInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.MultiChartInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteChartHeadingInstanceList(multiChartInstance.ColumnInstances);
				this.WriteChartHeadingInstanceList(multiChartInstance.RowInstances);
				this.WriteChartDataPointInstancesList(multiChartInstance.DataPoints);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartHeadingInstance(ChartHeadingInstance chartHeadingInstance)
		{
			if (chartHeadingInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartHeadingInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoOwnerBase(chartHeadingInstance);
				this.m_writer.WriteInt32(chartHeadingInstance.UniqueName);
				this.WriteChartHeadingInstanceList(chartHeadingInstance.SubHeadingInstances);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartDataPointInstance(ChartDataPointInstance dataPointInstance)
		{
			if (dataPointInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartDataPointInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoOwnerBase(dataPointInstance);
				this.m_writer.WriteInt32(dataPointInstance.UniqueName);
				this.m_writer.EndObject();
			}
		}

		private void WriteAxisInstance(AxisInstance axisInstance)
		{
			if (axisInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.AxisInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteInt32(axisInstance.UniqueName);
				this.WriteChartTitleInstance(axisInstance.Title);
				this.WriteVariants(axisInstance.StyleAttributeValues);
				this.WriteVariants(axisInstance.MajorGridLinesStyleAttributeValues);
				this.WriteVariants(axisInstance.MinorGridLinesStyleAttributeValues);
				this.WriteVariant(axisInstance.MinValue);
				this.WriteVariant(axisInstance.MaxValue);
				this.WriteVariant(axisInstance.CrossAtValue);
				this.WriteVariant(axisInstance.MajorIntervalValue);
				this.WriteVariant(axisInstance.MinorIntervalValue);
				this.WriteDataValueInstanceList(axisInstance.CustomPropertyInstances);
				this.m_writer.EndObject();
			}
		}

		private void WriteChartTitleInstance(ChartTitleInstance chartTitleInstance)
		{
			if (chartTitleInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.ChartTitleInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteInt32(chartTitleInstance.UniqueName);
				this.m_writer.WriteString(chartTitleInstance.Caption);
				this.WriteVariants(chartTitleInstance.StyleAttributeValues);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableInstance(TableInstance tableInstance)
		{
			if (tableInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				Table table = (Table)tableInstance.ReportItemDef;
				ObjectType objectType = ObjectType.TableInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(tableInstance);
				if (table.HeaderRows != null)
				{
					this.WriteTableRowInstances(tableInstance.HeaderRowInstances);
				}
				if (table.TableGroups != null)
				{
					this.WriteTableGroupInstanceList(tableInstance.TableGroupInstances);
				}
				else if (table.TableDetail != null)
				{
					if (table.TableDetail.SimpleDetailRows)
					{
						int int32Value = -1;
						if (tableInstance.TableDetailInstances != null && 0 < tableInstance.TableDetailInstances.Count)
						{
							int32Value = tableInstance.TableDetailInstances[0].UniqueName;
						}
						this.m_writer.WriteInt32(int32Value);
					}
					this.WriteTableDetailInstanceList(tableInstance.TableDetailInstances);
				}
				if (table.FooterRows != null)
				{
					this.WriteTableRowInstances(tableInstance.FooterRowInstances);
				}
				this.WriteRenderingPagesRangesList(tableInstance.ChildrenStartAndEndPages);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableGroupInstance(TableGroupInstance tableGroupInstance)
		{
			if (tableGroupInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				TableGroup tableGroupDef = tableGroupInstance.TableGroupDef;
				Table table = (Table)tableGroupDef.DataRegionDef;
				ObjectType objectType = ObjectType.TableGroupInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoOwnerBase(tableGroupInstance);
				this.m_writer.WriteInt32(tableGroupInstance.UniqueName);
				if (tableGroupDef.HeaderRows != null)
				{
					this.WriteTableRowInstances(tableGroupInstance.HeaderRowInstances);
				}
				if (tableGroupDef.FooterRows != null)
				{
					this.WriteTableRowInstances(tableGroupInstance.FooterRowInstances);
				}
				if (tableGroupDef.InnerHierarchy != null)
				{
					this.WriteTableGroupInstanceList(tableGroupInstance.SubGroupInstances);
				}
				else if (table.TableDetail != null)
				{
					if (table.TableDetail.SimpleDetailRows)
					{
						int int32Value = -1;
						if (tableGroupInstance.TableDetailInstances != null && 0 < tableGroupInstance.TableDetailInstances.Count)
						{
							int32Value = tableGroupInstance.TableDetailInstances[0].UniqueName;
						}
						this.m_writer.WriteInt32(int32Value);
					}
					this.WriteTableDetailInstanceList(tableGroupInstance.TableDetailInstances);
				}
				this.WriteRenderingPagesRangesList(tableGroupInstance.ChildrenStartAndEndPages);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableDetailInstance(TableDetailInstance tableDetailInstance)
		{
			if (tableDetailInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableDetailInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoOwnerBase(tableDetailInstance);
				bool simpleDetailRows = tableDetailInstance.TableDetailDef.SimpleDetailRows;
				if (simpleDetailRows)
				{
					this.m_writeUniqueName = false;
				}
				else
				{
					this.m_writer.WriteInt32(tableDetailInstance.UniqueName);
				}
				this.WriteTableRowInstances(tableDetailInstance.DetailRowInstances);
				if (simpleDetailRows)
				{
					this.m_writeUniqueName = true;
				}
				this.m_writer.EndObject();
			}
		}

		private void WriteTableRowInstance(TableRowInstance tableRowInstance)
		{
			if (tableRowInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableRowInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteInstanceInfoOwnerBase(tableRowInstance);
				if (this.m_writeUniqueName)
				{
					this.m_writer.WriteInt32(tableRowInstance.UniqueName);
				}
				this.WriteReportItemColInstance(tableRowInstance.TableRowReportItemColInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteTableColumnInstance(TableColumnInstance tableColumnInstance)
		{
			if (tableColumnInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.TableColumnInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteInt32(tableColumnInstance.UniqueName);
				this.m_writer.WriteBoolean(tableColumnInstance.StartHidden);
				this.m_writer.EndObject();
			}
		}

		private void WriteOWCChartInstance(OWCChartInstance chartInstance)
		{
			if (chartInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.OWCChartInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(chartInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteCustomReportItemInstance(CustomReportItemInstance instance)
		{
			if (instance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItemInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(instance);
				this.WriteReportItemColInstance(instance.AltReportItemColInstance);
				this.WriteCustomReportItemHeadingInstanceList(instance.ColumnInstances);
				this.WriteCustomReportItemHeadingInstanceList(instance.RowInstances);
				this.WriteCustomReportItemCellInstancesList(instance.Cells);
				this.m_writer.EndObject();
			}
		}

		private void WriteCustomReportItemHeadingInstance(CustomReportItemHeadingInstance headingInstance)
		{
			if (headingInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItemHeadingInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteCustomReportItemHeadingInstanceList(headingInstance.SubHeadingInstances);
				this.WriteCustomReportItemHeadingReference(headingInstance.HeadingDefinition);
				this.m_writer.WriteInt32(headingInstance.HeadingCellIndex);
				this.m_writer.WriteInt32(headingInstance.HeadingSpan);
				this.WriteDataValueInstanceList(headingInstance.CustomPropertyInstances);
				this.m_writer.WriteString(headingInstance.Label);
				this.WriteVariantList(headingInstance.GroupExpressionValues, false);
				this.m_writer.EndObject();
			}
		}

		private void WriteCustomReportItemCellInstance(CustomReportItemCellInstance cellInstance)
		{
			if (cellInstance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.CustomReportItemCellInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.m_writer.WriteInt32(cellInstance.RowIndex);
				this.m_writer.WriteInt32(cellInstance.ColumnIndex);
				this.WriteDataValueInstanceList(cellInstance.DataValueInstances);
				this.m_writer.EndObject();
			}
		}

		private void WritePageSectionInstance(PageSectionInstance instance)
		{
			if (instance == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.PageSectionInstance;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				this.WriteReportItemInstanceBase(instance);
				this.m_writer.WriteInt32(instance.PageNumber);
				this.WriteReportItemColInstance(instance.ReportItemColInstance);
				this.m_writer.EndObject();
			}
		}

		private void WriteVariant(object variant)
		{
			this.WriteVariant(variant, false);
		}

		private void WriteVariant(object variant, bool convertDBNull)
		{
			if (variant == null)
			{
				Global.Tracer.Assert(!convertDBNull);
				this.m_writer.WriteNull();
			}
			else if (DBNull.Value == variant)
			{
				Global.Tracer.Assert(convertDBNull, "(convertDBNull)");
				this.m_writer.WriteNull();
			}
			else if (variant is string)
			{
				this.m_writer.WriteString((string)variant);
			}
			else if (variant is char)
			{
				this.m_writer.WriteChar((char)variant);
			}
			else if (variant is bool)
			{
				this.m_writer.WriteBoolean((bool)variant);
			}
			else if (variant is short)
			{
				this.m_writer.WriteInt16((short)variant);
			}
			else if (variant is int)
			{
				this.m_writer.WriteInt32((int)variant);
			}
			else if (variant is long)
			{
				this.m_writer.WriteInt64((long)variant);
			}
			else if (variant is ushort)
			{
				this.m_writer.WriteUInt16((ushort)variant);
			}
			else if (variant is uint)
			{
				this.m_writer.WriteUInt32((uint)variant);
			}
			else if (variant is ulong)
			{
				this.m_writer.WriteUInt64((ulong)variant);
			}
			else if (variant is byte)
			{
				this.m_writer.WriteByte((byte)variant);
			}
			else if (variant is sbyte)
			{
				this.m_writer.WriteSByte((sbyte)variant);
			}
			else if (variant is float)
			{
				this.m_writer.WriteSingle((float)variant);
			}
			else if (variant is double)
			{
				this.m_writer.WriteDouble((double)variant);
			}
			else if (variant is decimal)
			{
				this.m_writer.WriteDecimal((decimal)variant);
			}
			else if (variant is DateTime)
			{
				this.m_writer.WriteDateTime((DateTime)variant);
			}
			else
			{
				IntermediateFormatWriter.Assert(variant is TimeSpan);
				this.m_writer.WriteTimeSpan((TimeSpan)variant);
			}
		}

		private bool WriteRecordFields(RecordField[] recordFields, RecordSetPropertyNamesList aliasPropertyNames)
		{
			bool result = true;
			if (recordFields == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.StartArray(recordFields.Length);
				for (int i = 0; i < recordFields.Length; i++)
				{
					if (aliasPropertyNames != null && aliasPropertyNames[i] != null)
					{
						recordFields[i].PopulateFieldPropertyValues(aliasPropertyNames[i].PropertyNames);
					}
					if (!this.WriteRecordField(recordFields[i]))
					{
						result = false;
					}
				}
				this.m_writer.EndArray();
			}
			return result;
		}

		private bool WriteRecordField(RecordField recordField)
		{
			bool result = true;
			if (recordField == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RecordField;
				this.DeclareType(objectType);
				this.m_writer.StartObject(objectType);
				if (recordField.IsOverflow)
				{
					this.m_writer.WriteDataFieldStatus(DataFieldStatus.Overflow);
				}
				else if (recordField.IsError)
				{
					this.m_writer.WriteDataFieldStatus(DataFieldStatus.IsError);
				}
				else if (recordField.IsUnSupportedDataType || !this.WriteFieldValue(recordField.FieldValue))
				{
					this.m_writer.WriteDataFieldStatus(DataFieldStatus.UnSupportedDataType);
					result = false;
				}
				this.m_writer.WriteBoolean(recordField.IsAggregationField);
				this.WriteVariantList(recordField.FieldPropertyValues, false);
				this.m_writer.EndObject();
			}
			return result;
		}

		private bool WriteFieldValue(object variant)
		{
			if (variant == null)
			{
				this.m_writer.WriteNull();
				goto IL_0258;
			}
			if (variant is string)
			{
				this.m_writer.WriteString((string)variant);
				goto IL_0258;
			}
			if (variant is char)
			{
				this.m_writer.WriteChar((char)variant);
				goto IL_0258;
			}
			if (variant is char[])
			{
				this.m_writer.WriteChars((char[])variant);
				goto IL_0258;
			}
			if (variant is bool)
			{
				this.m_writer.WriteBoolean((bool)variant);
				goto IL_0258;
			}
			if (variant is short)
			{
				this.m_writer.WriteInt16((short)variant);
				goto IL_0258;
			}
			if (variant is int)
			{
				this.m_writer.WriteInt32((int)variant);
				goto IL_0258;
			}
			if (variant is long)
			{
				this.m_writer.WriteInt64((long)variant);
				goto IL_0258;
			}
			if (variant is ushort)
			{
				this.m_writer.WriteUInt16((ushort)variant);
				goto IL_0258;
			}
			if (variant is uint)
			{
				this.m_writer.WriteUInt32((uint)variant);
				goto IL_0258;
			}
			if (variant is ulong)
			{
				this.m_writer.WriteUInt64((ulong)variant);
				goto IL_0258;
			}
			if (variant is byte)
			{
				this.m_writer.WriteByte((byte)variant);
				goto IL_0258;
			}
			if (variant is byte[])
			{
				this.m_writer.WriteBytes((byte[])variant);
				goto IL_0258;
			}
			if (variant is sbyte)
			{
				this.m_writer.WriteSByte((sbyte)variant);
				goto IL_0258;
			}
			if (variant is float)
			{
				this.m_writer.WriteSingle((float)variant);
				goto IL_0258;
			}
			if (variant is double)
			{
				this.m_writer.WriteDouble((double)variant);
				goto IL_0258;
			}
			if (variant is decimal)
			{
				this.m_writer.WriteDecimal((decimal)variant);
				goto IL_0258;
			}
			if (variant is DateTime)
			{
				this.m_writer.WriteDateTime((DateTime)variant);
				goto IL_0258;
			}
			if (variant is TimeSpan)
			{
				this.m_writer.WriteTimeSpan((TimeSpan)variant);
				goto IL_0258;
			}
			if (variant is Guid)
			{
				this.m_writer.WriteGuid((Guid)variant);
				goto IL_0258;
			}
			if (variant is DBNull)
			{
				this.m_writer.WriteNull();
				goto IL_0258;
			}
			return false;
			IL_0258:
			return true;
		}
	}
}
