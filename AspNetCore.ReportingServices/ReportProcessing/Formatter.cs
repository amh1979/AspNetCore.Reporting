using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Globalization;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal class Formatter
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Style m_styleClass;

		private OnDemandProcessingContext m_context;

		private bool m_sharedFormatSettings;

		private bool m_calendarValidated;

		private uint m_languageInstanceId;

		private ObjectType m_objectType;

		private string m_objectName;

		private Calendar m_formattingCalendar;

		internal Formatter(AspNetCore.ReportingServices.ReportIntermediateFormat.Style styleClass, OnDemandProcessingContext context, ObjectType objectType, string objectName)
		{
			this.m_context = context;
			this.m_styleClass = styleClass;
			this.m_objectType = objectType;
			this.m_objectName = objectName;
		}

		internal static string FormatWithClientCulture(object value)
		{
			bool flag = default(bool);
			return Formatter.FormatWithSpecificCulture(value, Localization.ClientPrimaryCulture, out flag);
		}

		internal static string FormatWithInvariantCulture(object value)
		{
			bool flag = default(bool);
			return Formatter.FormatWithInvariantCulture(value, out flag);
		}

		internal static string FormatWithInvariantCulture(object value, out bool errorOccurred)
		{
			return Formatter.FormatWithSpecificCulture(value, CultureInfo.InvariantCulture, out errorOccurred);
		}

		internal static string FormatWithSpecificCulture(object value, CultureInfo culture, out bool errorOccurred)
		{
			errorOccurred = false;
			if (value == null)
			{
				return null;
			}
			string result = null;
			if (value is IFormattable)
			{
				try
				{
					result = ((IFormattable)value).ToString(null, culture);
					return result;
				}
				catch (Exception)
				{
					errorOccurred = true;
					return result;
				}
			}
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = culture;
			try
			{
				result = value.ToString();
				return result;
			}
			catch (Exception)
			{
				errorOccurred = true;
				return result;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		internal static string Format(object value, ref Formatter formatter, AspNetCore.ReportingServices.ReportIntermediateFormat.Style reportItemStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Style reportElementStyle, OnDemandProcessingContext context, ObjectType objectType, string objectName)
		{
			if (formatter == null)
			{
				formatter = new Formatter(reportItemStyle, context, objectType, objectName);
			}
			Type type = value.GetType();
			TypeCode typeCode = Type.GetTypeCode(type);
			bool flag = false;
			string formatString = "";
			if (reportElementStyle != null)
			{
				reportElementStyle.GetStyleAttribute(objectType, objectName, "Format", context, ref flag, out formatString);
			}
			return formatter.FormatValue(value, formatString, typeCode);
		}

		internal string FormatValue(object value, TypeCode typeCode)
		{
			return this.FormatValue(value, null, typeCode);
		}

		internal string FormatValue(object value, string formatString, TypeCode typeCode)
		{
			return this.FormatValue(value, formatString, typeCode, false);
		}

		internal string FormatValue(object value, string formatString, TypeCode typeCode, bool addDateTimeOffsetSuffix)
		{
			CultureInfo cultureInfo = null;
			string text = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int languageState = 0;
			bool flag4 = false;
			string text2 = null;
			Calendar calendar = null;
			bool flag5 = false;
			try
			{
				if (this.m_styleClass != null)
				{
					if (formatString == null)
					{
						this.m_styleClass.GetStyleAttribute(this.m_objectType, this.m_objectName, "Format", this.m_context, ref this.m_sharedFormatSettings, out formatString);
					}
					languageState = this.m_styleClass.GetStyleAttribute(this.m_objectType, this.m_objectName, "Language", this.m_context, ref this.m_sharedFormatSettings, out text);
					if (!this.GetCulture(text, ref cultureInfo, ref flag2, ref languageState))
					{
						text2 = RPRes.rsExpressionErrorValue;
						flag5 = true;
					}
					if (!flag5 && typeCode == TypeCode.DateTime && !this.m_calendarValidated)
					{
						this.CreateAndValidateCalendar(languageState, cultureInfo);
					}
				}
				if (!flag5 && cultureInfo != null && this.m_formattingCalendar != null)
				{
					if (flag2)
					{
						if (cultureInfo.DateTimeFormat.IsReadOnly)
						{
							cultureInfo = (CultureInfo)cultureInfo.Clone();
							flag3 = true;
						}
						else
						{
							calendar = cultureInfo.DateTimeFormat.Calendar;
						}
					}
					cultureInfo.DateTimeFormat.Calendar = this.m_formattingCalendar;
				}
				if (!flag5 && formatString != null && value is IFormattable)
				{
					try
					{
						if (cultureInfo == null)
						{
							cultureInfo = Thread.CurrentThread.CurrentCulture;
							flag2 = true;
						}
						if (ReportProcessing.CompareWithInvariantCulture(formatString, "x", true) == 0)
						{
							flag4 = true;
						}
						text2 = ((IFormattable)value).ToString(formatString, cultureInfo);
						if (addDateTimeOffsetSuffix)
						{
							text2 += " +0".ToString(cultureInfo);
						}
					}
					catch (Exception ex)
					{
						text2 = RPRes.rsExpressionErrorValue;
						this.m_context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFormatString, Severity.Warning, this.m_objectType, this.m_objectName, "Format", ex.Message);
					}
					flag5 = true;
				}
				CultureInfo cultureInfo2;
				if (!flag5)
				{
					cultureInfo2 = null;
					if (!flag2 && cultureInfo != null)
					{
						goto IL_01be;
					}
					if (flag3)
					{
						goto IL_01be;
					}
					text2 = value.ToString();
				}
				goto end_IL_001a;
				IL_01be:
				cultureInfo2 = Thread.CurrentThread.CurrentCulture;
				Thread.CurrentThread.CurrentCulture = cultureInfo;
				try
				{
					text2 = value.ToString();
				}
				finally
				{
					if (cultureInfo2 != null)
					{
						Thread.CurrentThread.CurrentCulture = cultureInfo2;
					}
				}
				end_IL_001a:;
			}
			finally
			{
				if (flag2 && calendar != null)
				{
					Global.Tracer.Assert(!Thread.CurrentThread.CurrentCulture.DateTimeFormat.IsReadOnly, "(!System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.IsReadOnly)");
					Thread.CurrentThread.CurrentCulture.DateTimeFormat.Calendar = calendar;
				}
			}
			if (!flag4 && this.m_styleClass != null)
			{
				switch (typeCode)
				{
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					flag = true;
					break;
				}
				if (flag)
				{
					int num = 1;
					this.m_styleClass.GetStyleAttribute(this.m_objectType, this.m_objectName, "NumeralVariant", this.m_context, ref this.m_sharedFormatSettings, out num);
					if (num > 2)
					{
						CultureInfo cultureInfo3 = cultureInfo;
						if (cultureInfo3 == null)
						{
							cultureInfo3 = Thread.CurrentThread.CurrentCulture;
						}
						string numberDecimalSeparator = cultureInfo3.NumberFormat.NumberDecimalSeparator;
						this.m_styleClass.GetStyleAttribute(this.m_objectType, this.m_objectName, "NumeralLanguage", this.m_context, ref this.m_sharedFormatSettings, out text);
						if (text != null)
						{
							cultureInfo = new CultureInfo(text, false);
						}
						else if (cultureInfo == null)
						{
							cultureInfo = cultureInfo3;
						}
						bool flag6 = true;
						text2 = FormatDigitReplacement.FormatNumeralVariant(text2, num, cultureInfo, numberDecimalSeparator, out flag6);
						if (!flag6)
						{
							this.m_context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariantForLanguage, Severity.Warning, this.m_objectType, this.m_objectName, "NumeralVariant", num.ToString(CultureInfo.InvariantCulture), cultureInfo.Name);
						}
					}
				}
			}
			return text2;
		}

		internal CultureInfo GetCulture(string language)
		{
			bool flag = false;
			int num = 0;
			CultureInfo result = null;
			if (this.GetCulture(language, ref result, ref flag, ref num))
			{
				return result;
			}
			return Thread.CurrentThread.CurrentCulture;
		}

		private bool GetCulture(string language, ref CultureInfo formattingCulture, ref bool isThreadCulture, ref int languageState)
		{
			if (language != null)
			{
				try
				{
					formattingCulture = new CultureInfo(language, false);
					if (formattingCulture.IsNeutralCulture)
					{
						formattingCulture = CultureInfo.CreateSpecificCulture(language);
						formattingCulture = new CultureInfo(formattingCulture.Name, false);
					}
				}
				catch (Exception)
				{
					this.m_context.ErrorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Warning, this.m_objectType, this.m_objectName, "Language", language);
					return false;
				}
			}
			else
			{
				languageState = 0;
				isThreadCulture = true;
				formattingCulture = Thread.CurrentThread.CurrentCulture;
				if (this.m_context.LanguageInstanceId != this.m_languageInstanceId)
				{
					this.m_calendarValidated = false;
					this.m_formattingCalendar = null;
					this.m_languageInstanceId = this.m_context.LanguageInstanceId;
				}
			}
			return true;
		}

		private void CreateAndValidateCalendar(int languageState, CultureInfo formattingCulture)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = null;
			Calendars calendars = Calendars.Default;
			bool flag = false;
			if (this.m_styleClass.GetAttributeInfo("Calendar", out attributeInfo))
			{
				if (attributeInfo.IsExpression)
				{
					flag = true;
					calendars = (Calendars)this.m_styleClass.EvaluateStyle(this.m_objectType, this.m_objectName, attributeInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId.Calendar, this.m_context);
					this.m_sharedFormatSettings = false;
				}
				else
				{
					calendars = StyleTranslator.TranslateCalendar(attributeInfo.Value, this.m_context.ReportRuntime);
					switch (languageState)
					{
					case 1:
						flag = true;
						break;
					default:
						if (this.m_calendarValidated)
						{
							break;
						}
						this.m_calendarValidated = true;
						this.m_formattingCalendar = ProcessingValidator.CreateCalendar(calendars);
						return;
					case 0:
						break;
					}
				}
			}
			if (!flag && this.m_calendarValidated)
			{
				return;
			}
			if (calendars != 0 && ProcessingValidator.ValidateCalendar(formattingCulture, calendars, this.m_objectType, this.m_objectName, "Calendar", this.m_context.ErrorContext))
			{
				this.m_formattingCalendar = ProcessingValidator.CreateCalendar(calendars);
			}
			if (!flag)
			{
				this.m_calendarValidated = true;
			}
		}
	}
}
