using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class FormulaHandler
	{
		private int m_reportItemCount;

		private ArrayList m_reportItemsNames;

		private static Hashtable m_FormulaMapping;

		private static Regex m_RegexGlobalOnly;

		private static Regex m_RegexReportItemNameFormat;

		private static Regex m_RegexFunctionName;

		private static Regex m_RegexIdentifier;

		private static Regex m_RegexNonConstant;

		private static Regex m_RegexFieldDetection;

		private static Regex m_RegexReportItemsDetection;

		private static Regex m_RegexGlobalsDetection;

		private static Regex m_RegexAmpDetection;

		private static Regex m_RegexParametersDetection;

		private static Regex m_RegexUserDetection;

		private static Regex m_RegexAggregatesDetection;

		private static Regex m_RegexStringLiteralOnly;

		private static Regex m_RegexNothingOnly;

		private static Regex m_RegexReportItemName;

		private static Regex m_RegexSpecialFunction;

		private static RegexOptions m_regexOptions;

		private static ArrayList m_VBModulePropertiesSupported;

		private static ArrayList m_VBModulePropertiesUnSupported;

		static FormulaHandler()
		{
			FormulaHandler.m_RegexGlobalOnly = null;
			FormulaHandler.m_RegexReportItemNameFormat = null;
			FormulaHandler.m_RegexFunctionName = null;
			FormulaHandler.m_RegexIdentifier = null;
			FormulaHandler.m_RegexNonConstant = null;
			FormulaHandler.m_RegexFieldDetection = null;
			FormulaHandler.m_RegexReportItemsDetection = null;
			FormulaHandler.m_RegexGlobalsDetection = null;
			FormulaHandler.m_RegexAmpDetection = null;
			FormulaHandler.m_RegexParametersDetection = null;
			FormulaHandler.m_RegexUserDetection = null;
			FormulaHandler.m_RegexAggregatesDetection = null;
			FormulaHandler.m_RegexStringLiteralOnly = null;
			FormulaHandler.m_RegexNothingOnly = null;
			FormulaHandler.m_RegexReportItemName = null;
			FormulaHandler.m_RegexSpecialFunction = null;
			FormulaHandler.m_VBModulePropertiesSupported = null;
			FormulaHandler.m_VBModulePropertiesUnSupported = null;
			FormulaHandler.InitFormulaMapping();
			FormulaHandler.InitRegularExpressions();
		}

		private static void InitRegularExpressions()
		{
			FormulaHandler.m_regexOptions = (RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
			string text = "(\"((\"\")|[^\"])*\")";
			string text2 = "RunningValue";
			string text3 = "RowNumber";
			string text4 = "Fields";
			string text5 = "ReportItems";
			string text6 = "Globals";
			string text7 = "Parameters";
			string text8 = "Aggregates";
			string text9 = "User";
			string text10 = "Item";
			string text11 = "&";
			string str = Regex.Escape("()!#,.:&*+-/\\^<=>");
			string text12 = "(^|[" + str + "\\s])";
			string text13 = "($|[" + str + "\\s])";
			string text14 = Regex.Escape("!");
			string text15 = Regex.Escape(".");
			string text16 = "[" + text14 + text15 + "]";
			Regex.Escape(":");
			string text17 = Regex.Escape("(");
			string text18 = Regex.Escape(")");
			string str2 = text6 + "(" + text16 + ")?(" + text10 + ")?(" + text17 + ")?(?<GlobalParameterName>(\\s*" + text + "\\s*)|[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*)(" + text18 + ")?";
			FormulaHandler.m_RegexNonConstant = new Regex("^\\s*=", FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexFieldDetection = new Regex(text12 + "(?<detected>" + text4 + ")" + text13, FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexReportItemsDetection = new Regex(text12 + "(?<detected>" + text5 + ")" + text13, FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexGlobalsDetection = new Regex(text12 + "(?<detected>" + text6 + ")" + text13, FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexParametersDetection = new Regex(text12 + "(?<detected>" + text7 + ")" + text13, FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexAggregatesDetection = new Regex(text12 + "(?<detected>" + text8 + ")" + text13, FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexUserDetection = new Regex(text12 + "(?<detected>" + text9 + ")" + text13, FormulaHandler.m_regexOptions);
			string pattern = text5 + "(" + text15 + text10 + ")?" + text17 + "(?<ReportItemname>" + text + "|[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*)" + text18 + "(" + text15 + "value)?";
			FormulaHandler.m_RegexIdentifier = new Regex("[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*", FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexNothingOnly = new Regex("^\\s*Nothing\\s*$", FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexReportItemName = new Regex(text5 + text14 + "(?<reportitemname>[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*)(" + text15 + "(value))?", FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexReportItemNameFormat = new Regex(pattern, FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexFunctionName = new Regex("(?<prefix>" + text12 + ")(?<FunctionName>[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*)\\s*" + text17, FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexStringLiteralOnly = new Regex("^\\s*\"(?<string>((\"\")|[^\"])*)\"\\s*$", FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexNothingOnly = new Regex("^\\s*Nothing\\s*$", FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexGlobalOnly = new Regex("^\\s*" + str2 + "\\s*$", FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexAmpDetection = new Regex("\\s*" + text + "\\s*|(?<detected>" + text11 + ")", FormulaHandler.m_regexOptions);
			FormulaHandler.m_RegexSpecialFunction = new Regex("(?<prefix>" + text12 + ")(?<sfname>" + text2 + "|" + text3 + "|First|Last|Sum|Avg|Max|Min|CountDistinct|Count|StDevP|VarP|StDev|Var|Aggregate)\\s*\\(", FormulaHandler.m_regexOptions);
		}

		private static void InitFormulaMapping()
		{
			FormulaHandler.m_FormulaMapping = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			FormulaHandler.m_FormulaMapping.Add("Asc", "Code");
			FormulaHandler.m_FormulaMapping.Add("Cdate", "DateValue");
			FormulaHandler.m_FormulaMapping.Add("Chr", "Char");
			FormulaHandler.m_FormulaMapping.Add("DateSerial", "Date");
			FormulaHandler.m_FormulaMapping.Add("Iif", "If");
			FormulaHandler.m_FormulaMapping.Add("Lcase", "Lower");
			FormulaHandler.m_FormulaMapping.Add("Ucase", "Upper");
			FormulaHandler.m_FormulaMapping.Add("Abs", "Abs");
			FormulaHandler.m_FormulaMapping.Add("Atan", "Atan");
			FormulaHandler.m_FormulaMapping.Add("Choose", "Choose");
			FormulaHandler.m_FormulaMapping.Add("Cos", "Cos");
			FormulaHandler.m_FormulaMapping.Add("DateValue", "DateValue");
			FormulaHandler.m_FormulaMapping.Add("Day", "Day");
			FormulaHandler.m_FormulaMapping.Add("DDB", "DDB");
			FormulaHandler.m_FormulaMapping.Add("Exp", "Exp");
			FormulaHandler.m_FormulaMapping.Add("FV", "FV");
			FormulaHandler.m_FormulaMapping.Add("Hour", "Hour");
			FormulaHandler.m_FormulaMapping.Add("Int", "Int");
			FormulaHandler.m_FormulaMapping.Add("Ipmt", "Ipmt");
			FormulaHandler.m_FormulaMapping.Add("Irr", "Irr");
			FormulaHandler.m_FormulaMapping.Add("Left", "Left");
			FormulaHandler.m_FormulaMapping.Add("Minute", "Minute");
			FormulaHandler.m_FormulaMapping.Add("Mirr", "Mirr");
			FormulaHandler.m_FormulaMapping.Add("Month", "Month");
			FormulaHandler.m_FormulaMapping.Add("Now", "Now");
			FormulaHandler.m_FormulaMapping.Add("Nper", "Nper");
			FormulaHandler.m_FormulaMapping.Add("Npv", "Npv");
			FormulaHandler.m_FormulaMapping.Add("Pmt", "Pmt");
			FormulaHandler.m_FormulaMapping.Add("PPmt", "PPmt");
			FormulaHandler.m_FormulaMapping.Add("Pv", "Pv");
			FormulaHandler.m_FormulaMapping.Add("Rate", "Rate");
			FormulaHandler.m_FormulaMapping.Add("Right", "Right");
			FormulaHandler.m_FormulaMapping.Add("Second", "Second");
			FormulaHandler.m_FormulaMapping.Add("Sign", "Sign");
			FormulaHandler.m_FormulaMapping.Add("Sin", "Sin");
			FormulaHandler.m_FormulaMapping.Add("Sln", "Sln");
			FormulaHandler.m_FormulaMapping.Add("Sqrt", "Sqrt");
			FormulaHandler.m_FormulaMapping.Add("Syd", "Syd");
			FormulaHandler.m_FormulaMapping.Add("Tan", "Tan");
			FormulaHandler.m_FormulaMapping.Add("Today", "Today");
			FormulaHandler.m_FormulaMapping.Add("Year", "Year");
			FormulaHandler.m_VBModulePropertiesSupported = new ArrayList();
			FormulaHandler.m_VBModulePropertiesSupported.Add("Today");
			FormulaHandler.m_VBModulePropertiesSupported.Add("Now");
			FormulaHandler.m_VBModulePropertiesUnSupported = new ArrayList();
			FormulaHandler.m_VBModulePropertiesUnSupported.Add("DateString");
			FormulaHandler.m_VBModulePropertiesUnSupported.Add("TimeOfDay");
			FormulaHandler.m_VBModulePropertiesUnSupported.Add("Timer");
			FormulaHandler.m_VBModulePropertiesUnSupported.Add("TimeString");
			FormulaHandler.m_VBModulePropertiesUnSupported.Add("Hex");
			FormulaHandler.m_VBModulePropertiesUnSupported.Add("Oct");
		}

		internal FormulaHandler()
		{
			this.m_reportItemCount = 0;
		}

		internal ArrayList ProcessFormula(string formulaExpression, out string excelFormula)
		{
			this.m_reportItemsNames = new ArrayList();
			if (formulaExpression != null && formulaExpression.Length != 0)
			{
				string text = this.CheckValidityforConversion(formulaExpression);
				if (text != null && text.Length != 0)
				{
					this.m_reportItemCount = 0;
					excelFormula = Regex.Replace(text, FormulaHandler.m_RegexReportItemName.ToString(), this.MapToExcel, FormulaHandler.m_regexOptions);
					return this.m_reportItemsNames;
				}
				excelFormula = null;
				this.m_reportItemsNames = null;
				return null;
			}
			excelFormula = null;
			this.m_reportItemsNames = null;
			return this.m_reportItemsNames;
		}

		internal static string ProcessHeaderFooterFormula(string formulaExpression)
		{
			if (formulaExpression == null)
			{
				return null;
			}
			string text = null;
			StringBuilder stringBuilder = new StringBuilder();
			MatchCollection matchCollection = FormulaHandler.m_RegexAmpDetection.Matches(formulaExpression, 0);
			if (matchCollection == null || matchCollection.Count == 0)
			{
				text = formulaExpression;
			}
			else
			{
				int num = 0;
				string text2 = null;
				string text3 = null;
				for (int i = 0; i <= matchCollection.Count; i++)
				{
					text3 = ((i >= matchCollection.Count) ? formulaExpression.Substring(num, formulaExpression.Length - num) : formulaExpression.Substring(num, matchCollection[i].Index - num));
					text3 = text3.Trim();
					if (text3.Length > 0)
					{
						text = ((text != null) ? (text + "&" + text3) : text3);
					}
					if (i < matchCollection.Count)
					{
						text3 = formulaExpression.Substring(matchCollection[i].Index, matchCollection[i].Length);
						num = matchCollection[i].Index + matchCollection[i].Length;
						if (!(text3 == "&"))
						{
							text2 = text3.Trim();
							int length = text2.Length;
							if (length > 1 && text2[0] == '"' && text2[length - 1] == '"')
							{
								text2 = text2.Substring(1, length - 2);
							}
							if (text != null)
							{
								text = FormulaHandler.ExcelHeaderFooterFormula(text);
								if (text != null && !(text == string.Empty))
								{
									stringBuilder.Append(text);
									text = null;
									goto IL_0159;
								}
								return null;
							}
							goto IL_0159;
						}
					}
					continue;
					IL_0159:
					FormulaHandler.EncodeHeaderFooterString(stringBuilder, text2);
				}
			}
			if (text != null)
			{
				text = FormulaHandler.ExcelHeaderFooterFormula(text);
				if (text != null && !(text == string.Empty))
				{
					stringBuilder.Append(text);
					goto IL_0198;
				}
				return null;
			}
			goto IL_0198;
			IL_0198:
			return stringBuilder.ToString();
		}

		internal static void EncodeHeaderFooterString(StringBuilder output, string input)
		{
			if (output != null && input != null)
			{
				int i = output.Length;
				if (output.Length > 0 && char.IsDigit(output[i - 1]))
				{
					output.Append(' ');
				}
				output.Append(input);
				for (; i < output.Length; i++)
				{
					if (output[i] == '\\' || output[i] == '"')
					{
						output.Insert(i, '\\');
						i++;
					}
					else if (output[i] == '&')
					{
						output.Insert(i, '&');
						i++;
					}
				}
			}
		}

		internal static string ExcelHeaderFooterFormula(string formulaExpression)
		{
			string result = string.Empty;
			if (formulaExpression != null && formulaExpression.Length != 0)
			{
				Match match = FormulaHandler.m_RegexGlobalOnly.Match(formulaExpression);
				if (match.Success)
				{
					string text = match.Result("${GlobalParameterName}");
					text = text.Replace("\"", string.Empty);
					if (text != null && text.Length != 0)
					{
						switch (text.Trim().ToUpperInvariant())
						{
						case "PAGENUMBER":
						case "OVERALLPAGENUMBER":
							result = "&P";
							break;
						case "TOTALPAGES":
						case "OVERALLTOTALPAGES":
							result = "&N";
							break;
						case "REPORTNAME":
							result = "&F";
							break;
						default:
							result = string.Empty;
							break;
						}
					}
				}
			}
			return result;
		}

		private string CheckValidityforConversion(string formulaExpression)
		{
			if (!this.Detected(FormulaHandler.m_RegexFieldDetection, formulaExpression) && !this.Detected(FormulaHandler.m_RegexGlobalsDetection, formulaExpression) && !this.Detected(FormulaHandler.m_RegexNothingOnly, formulaExpression) && !this.Detected(FormulaHandler.m_RegexParametersDetection, formulaExpression) && !this.Detected(FormulaHandler.m_RegexAggregatesDetection, formulaExpression) && !this.Detected(FormulaHandler.m_RegexUserDetection, formulaExpression) && !this.Detected(FormulaHandler.m_RegexSpecialFunction, formulaExpression))
			{
				foreach (string item in FormulaHandler.m_VBModulePropertiesUnSupported)
				{
					Regex detectionRegex = new Regex("(?<FunctionName>" + item + ")(\\s*\\(\\s*\\))?", FormulaHandler.m_regexOptions);
					if (this.Detected(detectionRegex, formulaExpression))
					{
						return null;
					}
				}
				string text = this.FormatReportItemReference(formulaExpression);
				if (this.ValidateFunctionNames(ref text))
				{
					{
						foreach (string item2 in FormulaHandler.m_VBModulePropertiesSupported)
						{
							Regex regex = new Regex("(?<FunctionName>" + item2 + ")(\\s*\\(\\s*\\))?", FormulaHandler.m_regexOptions);
							text = Regex.Replace(text, regex.ToString(), this.MapVbModuleProperty, FormulaHandler.m_regexOptions);
						}
						return text;
					}
				}
				return null;
			}
			return null;
		}

		private bool ValidateFunctionNames(ref string transformedExpression)
		{
			string text = transformedExpression;
			int num = 0;
			ArrayList arrayList = new ArrayList();
			while (text.Length > 0)
			{
				Match match = FormulaHandler.m_RegexFunctionName.Match(text, 0);
				if (!match.Success)
				{
					return true;
				}
				string text2 = match.Result("${FunctionName}");
				if (text2 == null || text2.Length == 0)
				{
					num = match.Index + match.Length;
					text = text.Substring(num);
				}
				else
				{
					if (!FormulaHandler.m_FormulaMapping.ContainsKey(text2))
					{
						return false;
					}
					if (arrayList.IndexOf(text2) == -1)
					{
						Regex regex = new Regex("(?<FunctionName>" + text2 + ")\\s*\\(", FormulaHandler.m_regexOptions);
						transformedExpression = Regex.Replace(transformedExpression, regex.ToString(), this.MapFunction, FormulaHandler.m_regexOptions);
						arrayList.Add(text2);
					}
					num = match.Index + match.Length;
					text = text.Substring(num);
				}
			}
			return true;
		}

		private string MapVbModuleProperty(Match match)
		{
			string text = match.Result("${FunctionName}");
			if (text != null && text.Length != 0)
			{
				return text + "()";
			}
			return match.ToString();
		}

		private string MapFunction(Match match)
		{
			string empty = string.Empty;
			string text = match.Result("${FunctionName}");
			if (text != null && text.Length != 0)
			{
				empty = Regex.Replace(match.ToString(), FormulaHandler.m_RegexIdentifier.ToString(), FormulaHandler.m_FormulaMapping[text].ToString(), FormulaHandler.m_regexOptions);
				return Regex.Replace(empty, "\\s+", "");
			}
			return match.ToString();
		}

		private string FormatReportItemReference(string formulaExpression)
		{
			return Regex.Replace(formulaExpression, FormulaHandler.m_RegexReportItemNameFormat.ToString(), this.MapReportItemName, FormulaHandler.m_regexOptions);
		}

		private string MapReportItemName(Match match)
		{
			string empty = string.Empty;
			string text = match.Result("${ReportItemname}");
			if (text != null && text.Length != 0)
			{
				text = text.Replace("\"", string.Empty);
				return "ReportItems!" + text;
			}
			return match.ToString();
		}

		private string MapToExcel(Match match)
		{
			string result = string.Empty;
			string text = match.Result("${reportitemname}");
			if (text != null && text.Length != 0)
			{
				if (this.m_reportItemsNames.Contains(text))
				{
					result = "{" + this.m_reportItemsNames.IndexOf(text) + "}";
				}
				else
				{
					this.m_reportItemsNames.Add(text);
					result = "{" + this.m_reportItemCount + "}";
					this.m_reportItemCount++;
				}
			}
			return result;
		}

		private bool Detected(Regex detectionRegex, string formulaExpression)
		{
			Match match = detectionRegex.Match(formulaExpression);
			if (match.Success)
			{
				return true;
			}
			return false;
		}
	}
}
