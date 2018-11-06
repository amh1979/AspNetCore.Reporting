using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.Common
{
	internal sealed class CommonDataComparer : IDataComparer, IEqualityComparer, IEqualityComparer<object>, IComparer, IComparer<object>
	{
		private const bool DefaultThrowExceptionOnComparisonFailure = false;

		private readonly CompareInfo m_compareInfo;

		private readonly CompareOptions m_compareOptions;

		private readonly CultureInfo m_cultureInfo;

		private readonly bool m_nullsAsBlanks;

		public CompareInfo CompareInfo
		{
			get
			{
				return this.m_compareInfo;
			}
		}

		public CompareOptions CompareOptions
		{
			get
			{
				return this.m_compareOptions;
			}
		}

		internal CommonDataComparer(CompareInfo compareInfo, CompareOptions compareOptions, bool nullsAsBlanks)
		{
			this.m_compareInfo = compareInfo;
			if (this.m_compareInfo == null)
			{
				throw new ArgumentNullException("compareInfo");
			}
			this.m_compareOptions = compareOptions;
			this.m_cultureInfo = new CultureInfo(this.m_compareInfo.Name);
			this.m_nullsAsBlanks = nullsAsBlanks;
		}

		bool IEqualityComparer.Equals(object x, object y)
		{
			return 0 == this.InternalCompareTo(x, y, false);
		}

		bool IEqualityComparer<object>.Equals(object x, object y)
		{
			return 0 == this.InternalCompareTo(x, y, false);
		}

		int IComparer.Compare(object x, object y)
		{
			return this.InternalCompareTo(x, y, false);
		}

		int IComparer<object>.Compare(object x, object y)
		{
			return this.InternalCompareTo(x, y, false);
		}

		int IDataComparer.Compare(object x, object y, bool extendedTypeComparisons)
		{
			return this.InternalCompareTo(x, y, false);
		}

		int IDataComparer.Compare(object x, object y, bool throwExceptionOnComparisonFailure, bool extendedTypeComparisons, out bool validComparisonResult)
		{
			validComparisonResult = true;
			return this.InternalCompareTo(x, y, throwExceptionOnComparisonFailure);
		}

		public int GetHashCode(object obj)
		{
			if (object.ReferenceEquals(obj, null))
			{
				return 0;
			}
			if (obj is string)
			{
				string text = (string)obj;
				if ((CompareOptions.IgnoreCase & this.m_compareOptions) != 0)
				{
					text = text.ToUpper(this.m_cultureInfo);
				}
				return text.GetHashCode();
			}
			ICustomComparable customComparable = obj as ICustomComparable;
			if (customComparable != null)
			{
				return customComparable.GetHashCode(this);
			}
			return obj.GetHashCode();
		}

		private int InternalCompareTo(object x, object y, bool throwExceptionOnComparisonFailure)
		{
			string text = x as string;
			string text2 = y as string;
			if (text != null && text2 != null)
			{
				return this.m_compareInfo.Compare(text, text2, this.m_compareOptions);
			}
			DataTypeCode dataTypeCode = ObjectSerializer.GetDataTypeCode(x);
			DataTypeCode dataTypeCode2 = ObjectSerializer.GetDataTypeCode(y);
			if (dataTypeCode == DataTypeCode.Empty && dataTypeCode2 == DataTypeCode.Empty)
			{
				return 0;
			}
			if (dataTypeCode == DataTypeCode.Empty)
			{
				if (this.m_nullsAsBlanks && ComparerUtility.IsNumericLessThanZero(y))
				{
					return 1;
				}
				return -1;
			}
			if (dataTypeCode2 == DataTypeCode.Empty)
			{
				if (this.m_nullsAsBlanks && ComparerUtility.IsNumericLessThanZero(x))
				{
					return -1;
				}
				return 1;
			}
			if (dataTypeCode != dataTypeCode2)
			{
				switch (ComparerUtility.GetCommonVariantConversionType(dataTypeCode, dataTypeCode2))
				{
				case DataTypeCode.Double:
				{
					double num3 = 0.0;
					double num4 = 0.0;
					if (dataTypeCode == DataTypeCode.DateTime)
					{
						num3 = ((DateTime)x).ToOADate();
						num4 = Convert.ToDouble(y, this.m_cultureInfo);
					}
					else if (dataTypeCode2 == DataTypeCode.DateTime)
					{
						num4 = ((DateTime)y).ToOADate();
						num3 = Convert.ToDouble(x, this.m_cultureInfo);
					}
					else
					{
						num3 = Convert.ToDouble(x, this.m_cultureInfo);
						num4 = Convert.ToDouble(y, this.m_cultureInfo);
					}
					int num5 = num3.CompareTo(num4);
					if (num5 == 0)
					{
						return CommonDataComparer.CompareNumericDateVariantTypes(dataTypeCode, dataTypeCode2, throwExceptionOnComparisonFailure);
					}
					return num5;
				}
				case DataTypeCode.Decimal:
				{
					decimal num = Convert.ToDecimal(x, this.m_cultureInfo);
					decimal value = Convert.ToDecimal(y, this.m_cultureInfo);
					int num2 = num.CompareTo(value);
					if (num2 == 0)
					{
						return CommonDataComparer.CompareNumericDateVariantTypes(dataTypeCode, dataTypeCode2, throwExceptionOnComparisonFailure);
					}
					return num2;
				}
				case DataTypeCode.Int64:
				{
					long num6 = Convert.ToInt64(x, this.m_cultureInfo);
					long value2 = Convert.ToInt64(y, this.m_cultureInfo);
					int num7 = num6.CompareTo(value2);
					if (num7 == 0)
					{
						return CommonDataComparer.CompareNumericDateVariantTypes(dataTypeCode, dataTypeCode2, throwExceptionOnComparisonFailure);
					}
					return num7;
				}
				case DataTypeCode.Unknown:
					if (!ComparerUtility.IsNonNumericVariant(dataTypeCode) && !ComparerUtility.IsNonNumericVariant(dataTypeCode2))
					{
						break;
					}
					return CommonDataComparer.CompareToNonNumericVariantTypes(dataTypeCode, dataTypeCode2, x, y, throwExceptionOnComparisonFailure);
				}
			}
			ICustomComparable customComparable = x as ICustomComparable;
			ICustomComparable customComparable2 = y as ICustomComparable;
			if (customComparable != null && customComparable2 != null)
			{
				return customComparable.CompareTo(customComparable2, this);
			}
			IComparable left = (IComparable)x;
			IComparable right = (IComparable)y;
			return this.Compare(left, right, throwExceptionOnComparisonFailure);
		}

		private int Compare(IComparable left, IComparable right, bool throwExceptionOnComparisonFailure)
		{
			if (left == right)
			{
				return 0;
			}
			try
			{
				return left.CompareTo(right);
			}
			catch (ArgumentException)
			{
				if (throwExceptionOnComparisonFailure)
				{
					throw new CommonDataComparerException(left.GetType().ToString(), right.GetType().ToString());
				}
				return -1;
			}
		}

		private static int CompareNumericDateVariantTypes(DataTypeCode x, DataTypeCode y, bool throwExceptionOnComparisonFailure)
		{
			switch (x)
			{
			case DataTypeCode.DateTime:
				return 1;
			case DataTypeCode.Double:
				if (ComparerUtility.IsLessThanReal(y))
				{
					return 1;
				}
				return -1;
			case DataTypeCode.Decimal:
				if (ComparerUtility.IsLessThanCurrency(y))
				{
					return 1;
				}
				return -1;
			case DataTypeCode.Int64:
				if (ComparerUtility.IsLessThanInt64(y))
				{
					return 1;
				}
				return -1;
			case DataTypeCode.Int32:
				return -1;
			default:
				if (throwExceptionOnComparisonFailure)
				{
					throw new CommonDataComparerException(x.ToString(), y.ToString());
				}
				return -1;
			}
		}

		private static int CompareToNonNumericVariantTypes(DataTypeCode xDataType, DataTypeCode yDataType, object x, object y, bool throwExceptionOnComparisonFailure)
		{
			if (ComparerUtility.IsNumericDateVariant(xDataType) && ComparerUtility.IsNonNumericVariant(yDataType))
			{
				return -1;
			}
			if (ComparerUtility.IsNonNumericVariant(xDataType) && ComparerUtility.IsNumericDateVariant(yDataType))
			{
				return 1;
			}
			if (xDataType == DataTypeCode.String && yDataType == DataTypeCode.Boolean)
			{
				return -1;
			}
			if (xDataType == DataTypeCode.Boolean && yDataType == DataTypeCode.String)
			{
				return 1;
			}
			if (throwExceptionOnComparisonFailure)
			{
				throw new CommonDataComparerException(x.ToString(), y.ToString());
			}
			return -1;
		}
	}
}
