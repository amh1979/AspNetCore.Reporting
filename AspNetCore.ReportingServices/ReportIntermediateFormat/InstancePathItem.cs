using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class InstancePathItem
	{
		internal const char DefinitionInstanceDelimiter = 'i';

		internal const char InstancePathDelimiter = 'x';

		private const char m_0 = '0';

		private const char m_1 = '1';

		private const char m_2 = '2';

		private const char m_3 = '3';

		private const char m_4 = '4';

		private const char m_5 = '5';

		private const char m_6 = '6';

		private const char m_7 = '7';

		private const char m_8 = '8';

		private const char m_9 = '9';

		private int m_instanceIndex = -1;

		private int m_indexInCollection = -1;

		private InstancePathItemType m_indexType;

		private int m_hash;

		internal bool IsDynamicMember
		{
			get
			{
				if (this.m_indexType != InstancePathItemType.ColumnMemberInstanceIndex && this.m_indexType != InstancePathItemType.RowMemberInstanceIndex)
				{
					return this.m_indexType == InstancePathItemType.ColumnMemberInstanceIndexTopMost;
				}
				return true;
			}
		}

		internal bool IsScope
		{
			get
			{
				if (!this.IsDynamicMember)
				{
					if (this.m_indexType != InstancePathItemType.DataRegion)
					{
						return this.m_indexType == InstancePathItemType.SubReport;
					}
					return true;
				}
				return true;
			}
		}

		internal bool IsEmpty
		{
			get
			{
				return InstancePathItemType.None == this.m_indexType;
			}
		}

		internal int IndexInCollection
		{
			get
			{
				return this.m_indexInCollection;
			}
		}

		internal int InstanceIndex
		{
			get
			{
				return this.m_instanceIndex;
			}
		}

		internal InstancePathItemType Type
		{
			get
			{
				return this.m_indexType;
			}
		}

		internal InstancePathItem()
		{
			this.m_indexType = InstancePathItemType.None;
		}

		internal InstancePathItem(InstancePathItemType type, int id)
		{
			this.m_indexType = type;
			this.m_indexInCollection = id;
		}

		internal InstancePathItem(InstancePathItem original)
		{
			this.m_indexType = original.m_indexType;
			this.m_instanceIndex = original.m_instanceIndex;
			this.m_indexInCollection = original.m_indexInCollection;
		}

		public override int GetHashCode()
		{
			if (this.m_hash == 0)
			{
				this.m_hash = 41999;
				this.m_hash *= this.m_instanceIndex + this.m_indexInCollection + 3;
				this.m_hash = (this.m_hash << (int)(this.m_indexType + 3) ^ this.m_hash >> (int)(32 - (this.m_indexType + 3)));
			}
			return this.m_hash;
		}

		internal void ResetContext()
		{
			this.SetContext(-1);
		}

		internal void MoveNext()
		{
			this.SetContext(this.m_instanceIndex + 1);
		}

		internal void SetContext(int index)
		{
			this.m_instanceIndex = index;
			this.m_hash = 0;
		}

		internal static void DeepCopyPath(List<InstancePathItem> instancePath, ref List<InstancePathItem> copy)
		{
			if (instancePath != null)
			{
				int i = 0;
				int count = instancePath.Count;
				if (copy == null)
				{
					copy = new List<InstancePathItem>(count);
					for (; i < count; i++)
					{
						copy.Add(new InstancePathItem(instancePath[i]));
					}
				}
				else
				{
					int num = copy.Count;
					if (count == 0)
					{
						if (num > 0)
						{
							copy.Clear();
						}
					}
					else
					{
						if (num > count)
						{
							int num2 = num - count;
							copy.RemoveRange(count, num2);
							num -= num2;
						}
						for (; i < num; i++)
						{
							InstancePathItem instancePathItem = copy[i];
							InstancePathItem instancePathItem2 = instancePath[i];
							instancePathItem.m_hash = 0;
							instancePathItem.m_indexInCollection = instancePathItem2.m_indexInCollection;
							instancePathItem.m_indexType = instancePathItem2.m_indexType;
							instancePathItem.m_instanceIndex = instancePathItem2.m_instanceIndex;
						}
						for (; i < count; i++)
						{
							copy.Add(new InstancePathItem(instancePath[i]));
						}
					}
				}
			}
		}

		internal static bool IsSameScopePath(IInstancePath originalRIFObject, IInstancePath lastRIFObject)
		{
			if (null == originalRIFObject != (null == lastRIFObject))
			{
				return false;
			}
			if (originalRIFObject == null && lastRIFObject == null)
			{
				return true;
			}
			if (originalRIFObject.Equals(lastRIFObject))
			{
				return true;
			}
			List<InstancePathItem> instancePath = originalRIFObject.InstancePath;
			List<InstancePathItem> instancePath2 = lastRIFObject.InstancePath;
			bool flag = default(bool);
			int sharedPathIndex = InstancePathItem.GetSharedPathIndex(0, instancePath, instancePath2, false, out flag);
			if (flag)
			{
				return true;
			}
			if (sharedPathIndex < 0)
			{
				return false;
			}
			int count = instancePath.Count;
			int count2 = instancePath2.Count;
			int i = sharedPathIndex + 1;
			int j = sharedPathIndex + 1;
			for (; i < count && !instancePath[i].IsScope; i++)
			{
			}
			if (i + 1 == count && instancePath[i].m_indexType == InstancePathItemType.SubReport)
			{
				i = count;
			}
			for (; j < count2 && !instancePath2[j].IsScope; j++)
			{
			}
			if (i == count && j == count2)
			{
				return true;
			}
			return false;
		}

		internal static bool IsSamePath(List<InstancePathItem> path1, List<InstancePathItem> path2)
		{
			if (null == path1 != (null == path2))
			{
				return false;
			}
			if (path1 == null && path2 == null)
			{
				return true;
			}
			if (path1.Count != path2.Count)
			{
				return false;
			}
			bool result = default(bool);
			InstancePathItem.GetSharedPathIndex(0, path1, path2, false, out result);
			return result;
		}

		internal static int GetSharedPathIndex(int startIndexForNewPath, List<InstancePathItem> oldPath, List<InstancePathItem> newPath)
		{
			bool flag = default(bool);
			return InstancePathItem.GetSharedPathIndex(startIndexForNewPath, oldPath, newPath, false, out flag);
		}

		internal static int GetSharedPathIndex(int startIndexForNewPath, List<InstancePathItem> oldPath, List<InstancePathItem> newPath, bool returnPreviousIndex, out bool identicalPaths)
		{
			identicalPaths = false;
			int result = -1;
			int num = -1;
			if (null == oldPath != (null == newPath))
			{
				return num;
			}
			if (oldPath == null && newPath == null)
			{
				return num;
			}
			int count = oldPath.Count;
			int count2 = newPath.Count;
			int i = startIndexForNewPath;
			int j = startIndexForNewPath;
			if (startIndexForNewPath >= 0 && startIndexForNewPath < count && startIndexForNewPath < count2)
			{
				while (i < count && j < count2)
				{
					for (; i < count && oldPath[i].IsEmpty; i++)
					{
					}
					for (; j < count2 && newPath[j].IsEmpty; j++)
					{
					}
					if (returnPreviousIndex && i < count && j + 1 == count2)
					{
						return result;
					}
					if (i < count != j < count2)
					{
						return num;
					}
					if (i == count && j == count2)
					{
						break;
					}
					InstancePathItem instancePathItem = oldPath[i];
					InstancePathItem instancePathItem2 = newPath[j];
					if (instancePathItem.m_indexType != instancePathItem2.m_indexType || instancePathItem.m_indexInCollection != instancePathItem2.m_indexInCollection || instancePathItem.m_instanceIndex != instancePathItem2.m_instanceIndex)
					{
						return num;
					}
					result = num;
					num = j;
					i++;
					j++;
				}
				if (i == count && j == count2)
				{
					identicalPaths = true;
				}
				return num;
			}
			return num;
		}

		internal static bool IsEmptyPath(int startIndex, List<InstancePathItem> path)
		{
			if (path == null)
			{
				return true;
			}
			Global.Tracer.Assert(startIndex >= 0, "(startIndex >= 0)");
			int count = path.Count;
			if (startIndex != 0)
			{
				if (startIndex >= count)
				{
					return true;
				}
				if (path[startIndex].Type == InstancePathItemType.SubReport)
				{
					startIndex++;
					goto IL_0048;
				}
				return false;
			}
			goto IL_0048;
			IL_0048:
			while (path[startIndex].IsEmpty && startIndex < count)
			{
				startIndex++;
			}
			return startIndex == count;
		}

		internal static bool IsValidContext(List<InstancePathItem> path)
		{
			for (int i = 0; i < path.Count; i++)
			{
				InstancePathItem instancePathItem = path[i];
				if (instancePathItem.IsDynamicMember && instancePathItem.InstanceIndex < 0)
				{
					return false;
				}
			}
			return true;
		}

		internal static List<InstancePathItem> CombineRowColPath(List<InstancePathItem> rowPath, List<InstancePathItem> columnPath)
		{
			int parentDataRegionIndex = InstancePathItem.GetParentDataRegionIndex(rowPath);
			int parentDataRegionIndex2 = InstancePathItem.GetParentDataRegionIndex(columnPath);
			Global.Tracer.Assert(rowPath[parentDataRegionIndex].m_indexInCollection == columnPath[parentDataRegionIndex2].m_indexInCollection && parentDataRegionIndex == parentDataRegionIndex2);
			int num = columnPath.Count - parentDataRegionIndex2 - 1;
			List<InstancePathItem> list = new List<InstancePathItem>(rowPath.Count + num);
			list.AddRange(rowPath);
			if (0 < num)
			{
				columnPath[parentDataRegionIndex2 + 1].m_indexType = InstancePathItemType.ColumnMemberInstanceIndexTopMost;
				for (int i = 0; i < num; i++)
				{
					list.Add(columnPath[parentDataRegionIndex2 + 1 + i]);
				}
			}
			return list;
		}

		internal static int GetParentDataRegionIndex(List<InstancePathItem> instancePath)
		{
			if (instancePath != null && instancePath.Count != 0)
			{
				for (int num = instancePath.Count - 1; num >= 0; num--)
				{
					if (InstancePathItemType.DataRegion == instancePath[num].m_indexType)
					{
						return num;
					}
				}
				return -1;
			}
			return -1;
		}

		internal static int GetParentReportIndex(List<InstancePathItem> instancePath, bool isSubreport)
		{
			if (instancePath != null && instancePath.Count != 0)
			{
				int num = instancePath.Count;
				if (isSubreport && instancePath[num - 1].m_indexType == InstancePathItemType.SubReport)
				{
					num--;
				}
				for (int num2 = num - 1; num2 >= 0; num2--)
				{
					if (instancePath[num2].m_indexType == InstancePathItemType.SubReport)
					{
						return num2 + 1;
					}
				}
				return 0;
			}
			return 0;
		}

		internal static string GenerateUniqueNameString(int id, List<InstancePathItem> instancePath)
		{
			return InstancePathItem.GenerateUniqueNameString(id.ToString(CultureInfo.InvariantCulture), instancePath);
		}

		internal static string GenerateUniqueNameString(string idString, List<InstancePathItem> instancePath)
		{
			if (instancePath != null && instancePath.Count != 0)
			{
				return idString + 'i' + InstancePathItem.GenerateInstancePathString(instancePath, -1);
			}
			return idString;
		}

		internal static string GenerateUniqueNameString(int id, List<InstancePathItem> instancePath, int parentInstanceIndex)
		{
			string text = id.ToString(CultureInfo.InvariantCulture);
			if (instancePath != null && instancePath.Count != 0)
			{
				return text + 'i' + InstancePathItem.GenerateInstancePathString(instancePath, parentInstanceIndex);
			}
			return text;
		}

		internal static string GenerateInstancePathString(List<InstancePathItem> instancePath)
		{
			return InstancePathItem.GenerateInstancePathString(instancePath, -1);
		}

		private static string GenerateInstancePathString(List<InstancePathItem> instancePath, int parentInstanceIndex)
		{
			if (instancePath != null && instancePath.Count != 0)
			{
				int count = instancePath.Count;
				ReverseStringBuilder reverseStringBuilder = new ReverseStringBuilder(count * 2 + 4);
				bool flag = true;
				bool flag2 = true;
				bool flag3 = parentInstanceIndex >= 0;
				for (int num = count - 1; num >= 0; num--)
				{
					InstancePathItem instancePathItem = instancePath[num];
					switch (instancePathItem.m_indexType)
					{
					case InstancePathItemType.ColumnMemberInstanceIndexTopMost:
					case InstancePathItemType.ColumnMemberInstanceIndex:
						if (flag3)
						{
							flag3 = false;
							InstancePathItem.AppendInteger(ref reverseStringBuilder, parentInstanceIndex);
						}
						else
						{
							InstancePathItem.AppendInteger(ref reverseStringBuilder, instancePathItem.m_instanceIndex);
						}
						if (flag)
						{
							flag = false;
							reverseStringBuilder.Append('x');
							InstancePathItem.AppendInteger(ref reverseStringBuilder, instancePathItem.m_indexInCollection);
						}
						reverseStringBuilder.Append('C');
						break;
					case InstancePathItemType.RowMemberInstanceIndex:
						if (flag3)
						{
							flag3 = false;
							InstancePathItem.AppendInteger(ref reverseStringBuilder, parentInstanceIndex);
						}
						else
						{
							InstancePathItem.AppendInteger(ref reverseStringBuilder, instancePathItem.m_instanceIndex);
						}
						if (flag2)
						{
							flag2 = false;
							reverseStringBuilder.Append('x');
							InstancePathItem.AppendInteger(ref reverseStringBuilder, instancePathItem.m_indexInCollection);
						}
						reverseStringBuilder.Append('R');
						break;
					case InstancePathItemType.DataRegion:
						InstancePathItem.AppendInteger(ref reverseStringBuilder, instancePathItem.m_indexInCollection);
						reverseStringBuilder.Append('T');
						break;
					case InstancePathItemType.SubReport:
						InstancePathItem.AppendInteger(ref reverseStringBuilder, instancePathItem.m_indexInCollection);
						reverseStringBuilder.Append('S');
						break;
					}
				}
				return reverseStringBuilder.ToString();
			}
			return "";
		}

		private static void AppendInteger(ref ReverseStringBuilder builder, int value)
		{
			while (value > 9)
			{
				builder.Append(InstancePathItem.GetIntegerChar(value % 10));
				value /= 10;
			}
			builder.Append(InstancePathItem.GetIntegerChar(value));
		}

		private static char GetIntegerChar(int digit)
		{
			switch (digit)
			{
			case 0:
				return '0';
			case 1:
				return '1';
			case 2:
				return '2';
			case 3:
				return '3';
			case 4:
				return '4';
			case 5:
				return '5';
			case 6:
				return '6';
			case 7:
				return '7';
			case 8:
				return '8';
			case 9:
				return '9';
			default:
				return '0';
			}
		}
	}
}
