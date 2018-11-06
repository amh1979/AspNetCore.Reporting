using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class FormatDigitReplacement
	{
		private const int DbnumHundred = 11;

		private const int DbnumThousand = 12;

		private const int DbnumTenThousand = 13;

		private const int DbnumHundredMillion = 14;

		private const int DbnumTrillion = 15;

		private const int NUM_ASCII = 0;

		private const int NUM_ARABIC_INDIC = 1;

		private const int NUM_EXTENDED_ARABIC_INDIC = 2;

		private const int NUM_DEVANAGARI = 3;

		private const int NUM_BENGALI = 4;

		private const int NUM_GURMUKHI = 5;

		private const int NUM_GUJARATI = 6;

		private const int NUM_ORIYA = 7;

		private const int NUM_TAMIL = 8;

		private const int NUM_TELUGU = 9;

		private const int NUM_KANNADA = 10;

		private const int NUM_MALAYALAM = 11;

		private const int NUM_THAI = 12;

		private const int NUM_LAO = 13;

		private const int NUM_TIBETAN = 14;

		private const int NUM_JAPANESE1 = 15;

		private const int NUM_JAPANESE2 = 16;

		private const int NUM_JAPANESE3 = 17;

		private const int NUM_CHINESE_SIMP1 = 18;

		private const int NUM_CHINESE_SIMP2 = 19;

		private const int NUM_CHINESE_SIMP3 = 20;

		private const int NUM_CHINESE_TRAD1 = 21;

		private const int NUM_CHINESE_TRAD2 = 22;

		private const int NUM_CHINESE_TRAD3 = 23;

		private const int NUM_KOREAN1 = 24;

		private const int NUM_KOREAN2 = 25;

		private const int NUM_KOREAN3 = 26;

		private const int NUM_KOREAN4 = 27;

		internal static uint[][] DBNum_Japanese = new uint[16][]
		{
			new uint[4]
			{
				12295u,
				12295u,
				65296u,
				0u
			},
			new uint[4]
			{
				19968u,
				22769u,
				65297u,
				0u
			},
			new uint[4]
			{
				20108u,
				24336u,
				65298u,
				0u
			},
			new uint[4]
			{
				19977u,
				21442u,
				65299u,
				0u
			},
			new uint[4]
			{
				22235u,
				22235u,
				65300u,
				0u
			},
			new uint[4]
			{
				20116u,
				20237u,
				65301u,
				0u
			},
			new uint[4]
			{
				20845u,
				20845u,
				65302u,
				0u
			},
			new uint[4]
			{
				19971u,
				19971u,
				65303u,
				0u
			},
			new uint[4]
			{
				20843u,
				20843u,
				65304u,
				0u
			},
			new uint[4]
			{
				20061u,
				20061u,
				65305u,
				0u
			},
			new uint[4]
			{
				21313u,
				25342u,
				21313u,
				0u
			},
			new uint[4]
			{
				30334u,
				30334u,
				30334u,
				0u
			},
			new uint[4]
			{
				21315u,
				38433u,
				21315u,
				0u
			},
			new uint[4]
			{
				19975u,
				33836u,
				19975u,
				0u
			},
			new uint[4]
			{
				20740u,
				20740u,
				20740u,
				0u
			},
			new uint[4]
			{
				20806u,
				20806u,
				20806u,
				0u
			}
		};

		internal static uint[][] DBNum_Korean = new uint[16][]
		{
			new uint[4]
			{
				65296u,
				63922u,
				65296u,
				50689u
			},
			new uint[4]
			{
				19968u,
				22777u,
				65297u,
				51068u
			},
			new uint[4]
			{
				20108u,
				36019u,
				65298u,
				51060u
			},
			new uint[4]
			{
				19977u,
				63851u,
				65299u,
				49340u
			},
			new uint[4]
			{
				22235u,
				22235u,
				65300u,
				49324u
			},
			new uint[4]
			{
				20116u,
				20237u,
				65301u,
				50724u
			},
			new uint[4]
			{
				63953u,
				63953u,
				65302u,
				50977u
			},
			new uint[4]
			{
				19971u,
				19971u,
				65303u,
				52832u
			},
			new uint[4]
			{
				20843u,
				20843u,
				65304u,
				54036u
			},
			new uint[4]
			{
				20061u,
				20061u,
				65305u,
				44396u
			},
			new uint[4]
			{
				21313u,
				63859u,
				21313u,
				49901u
			},
			new uint[4]
			{
				30334u,
				30334u,
				30334u,
				48177u
			},
			new uint[4]
			{
				21315u,
				38433u,
				21315u,
				52380u
			},
			new uint[4]
			{
				19975u,
				33836u,
				19975u,
				47564u
			},
			new uint[4]
			{
				20740u,
				20740u,
				20740u,
				50613u
			},
			new uint[4]
			{
				20806u,
				20806u,
				20806u,
				51312u
			}
		};

		internal static uint[][] DBNum_SimplChinese = new uint[16][]
		{
			new uint[4]
			{
				9675u,
				38646u,
				65296u,
				0u
			},
			new uint[4]
			{
				19968u,
				22777u,
				65297u,
				0u
			},
			new uint[4]
			{
				20108u,
				36144u,
				65298u,
				0u
			},
			new uint[4]
			{
				19977u,
				21441u,
				65299u,
				0u
			},
			new uint[4]
			{
				22235u,
				32902u,
				65300u,
				0u
			},
			new uint[4]
			{
				20116u,
				20237u,
				65301u,
				0u
			},
			new uint[4]
			{
				20845u,
				38470u,
				65302u,
				0u
			},
			new uint[4]
			{
				19971u,
				26578u,
				65303u,
				0u
			},
			new uint[4]
			{
				20843u,
				25420u,
				65304u,
				0u
			},
			new uint[4]
			{
				20061u,
				29590u,
				65305u,
				0u
			},
			new uint[4]
			{
				21313u,
				25342u,
				21313u,
				0u
			},
			new uint[4]
			{
				30334u,
				20336u,
				30334u,
				0u
			},
			new uint[4]
			{
				21315u,
				20191u,
				21315u,
				0u
			},
			new uint[4]
			{
				19975u,
				19975u,
				19975u,
				0u
			},
			new uint[4]
			{
				20159u,
				20159u,
				20159u,
				0u
			},
			new uint[4]
			{
				20806u,
				20806u,
				20806u,
				0u
			}
		};

		internal static uint[][] DBNum_TradChinese = new uint[16][]
		{
			new uint[4]
			{
				9675u,
				38646u,
				65296u,
				0u
			},
			new uint[4]
			{
				19968u,
				22777u,
				65297u,
				0u
			},
			new uint[4]
			{
				20108u,
				36019u,
				65298u,
				0u
			},
			new uint[4]
			{
				19977u,
				21443u,
				65299u,
				0u
			},
			new uint[4]
			{
				22235u,
				32902u,
				65300u,
				0u
			},
			new uint[4]
			{
				20116u,
				20237u,
				65301u,
				0u
			},
			new uint[4]
			{
				20845u,
				38520u,
				65302u,
				0u
			},
			new uint[4]
			{
				19971u,
				26578u,
				65303u,
				0u
			},
			new uint[4]
			{
				20843u,
				25420u,
				65304u,
				0u
			},
			new uint[4]
			{
				20061u,
				29590u,
				65305u,
				0u
			},
			new uint[4]
			{
				21313u,
				25342u,
				21313u,
				0u
			},
			new uint[4]
			{
				30334u,
				20336u,
				30334u,
				0u
			},
			new uint[4]
			{
				21315u,
				20191u,
				21315u,
				0u
			},
			new uint[4]
			{
				33836u,
				33836u,
				33836u,
				0u
			},
			new uint[4]
			{
				20740u,
				20740u,
				20740u,
				0u
			},
			new uint[4]
			{
				20806u,
				20806u,
				20806u,
				0u
			}
		};

		internal static char[][] SimpleDigitMapping = new char[15][]
		{
			new char[2]
			{
				'0',
				'1'
			},
			new char[2]
			{
				'٠',
				'١'
			},
			new char[2]
			{
				'۰',
				'۱'
			},
			new char[2]
			{
				'०',
				'१'
			},
			new char[2]
			{
				'০',
				'১'
			},
			new char[2]
			{
				'੦',
				'੧'
			},
			new char[2]
			{
				'૦',
				'૧'
			},
			new char[2]
			{
				'୦',
				'୧'
			},
			new char[2]
			{
				'0',
				'௧'
			},
			new char[2]
			{
				'౦',
				'౧'
			},
			new char[2]
			{
				'೦',
				'೧'
			},
			new char[2]
			{
				'൦',
				'൧'
			},
			new char[2]
			{
				'๐',
				'๑'
			},
			new char[2]
			{
				'໐',
				'໑'
			},
			new char[2]
			{
				'༠',
				'༡'
			}
		};

		private FormatDigitReplacement()
		{
		}

		internal static char SimpleDigitFromNumeralShape(char asciiDigit, int numeralShape)
		{
			if (asciiDigit >= '0' && asciiDigit <= '9')
			{
				if (asciiDigit == '0')
				{
					return FormatDigitReplacement.SimpleDigitMapping[numeralShape][0];
				}
				return (char)(ushort)(FormatDigitReplacement.SimpleDigitMapping[numeralShape][1] + asciiDigit - 49);
			}
			return asciiDigit;
		}

		private static string SimpleTranslateNumber(string numberValue, int numeralShape, char numberDecimalSeparator)
		{
			if (numeralShape >= 0 && numeralShape <= 14)
			{
				char[] array = new char[numberValue.Length];
				for (int i = 0; i < numberValue.Length; i++)
				{
					char c = numberValue[i];
					if (c != numberDecimalSeparator)
					{
						array[i] = FormatDigitReplacement.SimpleDigitFromNumeralShape(c, numeralShape);
					}
					else
					{
						array[i] = c;
					}
				}
				return new string(array);
			}
			return numberValue;
		}

		private static void SkipNonDigits(string number, ref int index)
		{
			while (index < number.Length && (number[index] < '0' || number[index] > '9'))
			{
				index++;
			}
		}

		private static string ComplexTranslateNumber(string number, int numeralShape, char numberDecimalSeparator, int numVariant)
		{
			if (numeralShape >= 15 && numeralShape <= 27)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				char[] array = new char[2 * number.Length];
				int num4 = 0;
				int num5 = 0;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				uint[][] array2 = null;
				if (numeralShape <= 17)
				{
					array2 = FormatDigitReplacement.DBNum_Japanese;
				}
				else if (numeralShape <= 20)
				{
					flag2 = true;
					array2 = FormatDigitReplacement.DBNum_SimplChinese;
				}
				else if (numeralShape <= 23)
				{
					flag2 = true;
					array2 = FormatDigitReplacement.DBNum_TradChinese;
				}
				else
				{
					array2 = FormatDigitReplacement.DBNum_Korean;
					if (numVariant == 0 || numVariant == 3)
					{
						flag4 = true;
					}
				}
				if (numVariant == 1)
				{
					flag4 = true;
				}
				while (num < number.Length && (number[num] < '0' || number[num] > '9'))
				{
					array[num2] = number[num];
					num++;
					num2++;
				}
				for (int i = num; i < number.Length && number[i] != numberDecimalSeparator; i++)
				{
					if (number[i] >= '0' && number[i] <= '9')
					{
						num3++;
					}
				}
				if (num3 > 12)
				{
					if (num3 > 16)
					{
						while (12 < num3)
						{
							FormatDigitReplacement.SkipNonDigits(number, ref num);
							array[num2] = (char)array2[number[num] - 48][numVariant];
							num++;
							num2++;
							num3--;
						}
					}
					else
					{
						num5 = 16;
						num4 = 12;
						do
						{
							if (num5 > num3)
							{
								num5--;
								num4--;
							}
							else
							{
								FormatDigitReplacement.SkipNonDigits(number, ref num);
								if (number[num] != '0')
								{
									if (flag2 || flag4 || number[num] > '1' || num5 % 4 == 1)
									{
										if (flag2 && flag)
										{
											array[num2] = (char)array2[0][numVariant];
											num2++;
											flag = false;
										}
										array[num2] = (char)array2[number[num] - 48][numVariant];
										num2++;
									}
									if (num4 >= 10)
									{
										array[num2] = (char)array2[num4][numVariant];
										num2++;
									}
								}
								else
								{
									flag = true;
								}
								num5--;
								num4--;
								num3--;
								num++;
							}
						}
						while (num5 > 12);
					}
					array[num2] = (char)array2[15][numVariant];
					num2++;
				}
				num5 = 12;
				do
				{
					num4 = 12;
					flag3 = false;
					flag = false;
					do
					{
						if (num5 > num3)
						{
							num5--;
							num4--;
						}
						else
						{
							FormatDigitReplacement.SkipNonDigits(number, ref num);
							if (number[num] != '0' || num2 == 0)
							{
								if (flag2 || flag4 || number[num] > '1' || num5 % 4 == 1)
								{
									if (flag2 && flag)
									{
										array[num2] = (char)array2[0][numVariant];
										num2++;
										flag = false;
									}
									array[num2] = (char)array2[number[num] - 48][numVariant];
									num2++;
								}
								if (num4 >= 10)
								{
									array[num2] = (char)array2[num4][numVariant];
									num2++;
								}
								flag3 = true;
							}
							else
							{
								flag = true;
							}
							num5--;
							num4--;
							num3--;
							num++;
						}
					}
					while (num5 % 4 > 0);
					if (flag3 && num3 / 4 > 0)
					{
						switch (num3)
						{
						case 8:
							array[num2] = (char)array2[14][numVariant];
							num2++;
							break;
						case 4:
							array[num2] = (char)array2[13][numVariant];
							num2++;
							break;
						}
					}
				}
				while (num3 > 0);
				if (num < number.Length && number[num] == numberDecimalSeparator)
				{
					array[num2] = number[num];
					num++;
					num2++;
					while (num < number.Length)
					{
						if (number[num] < '0' || number[num] > '9')
						{
							array[num2] = number[num];
						}
						else
						{
							array[num2] = (char)array2[number[num] - 48][numVariant];
						}
						num++;
						num2++;
					}
				}
				string text = new string(array);
				char[] trimChars = new char[1];
				return text.TrimEnd(trimChars);
			}
			return number;
		}

		private static int GetNumeralShape(int numeralVariant, CultureInfo numeralLanguage)
		{
			if (numeralLanguage == null)
			{
				return 0;
			}
			if (numeralVariant < 3)
			{
				return 0;
			}
			int lCID = numeralLanguage.LCID;
			if (numeralVariant == 7 && (lCID & 0xFF) == 18)
			{
				return 27;
			}
			switch (numeralVariant)
			{
			case 4:
				if ((lCID & 0xFF) == 18)
				{
					return 24;
				}
				if ((lCID & 0xFF) == 17)
				{
					return 15;
				}
				if ((lCID & 0xFF) != 4)
				{
					break;
				}
				if (lCID == 31748)
				{
					return 21;
				}
				return 18;
			case 5:
				if ((lCID & 0xFF) == 18)
				{
					return 25;
				}
				if ((lCID & 0xFF) == 17)
				{
					return 16;
				}
				if ((lCID & 0xFF) != 4)
				{
					break;
				}
				if (lCID == 31748)
				{
					return 22;
				}
				return 19;
			case 6:
				if ((lCID & 0xFF) == 18)
				{
					return 26;
				}
				if ((lCID & 0xFF) == 17)
				{
					return 17;
				}
				if ((lCID & 0xFF) != 4)
				{
					break;
				}
				if (lCID == 31748)
				{
					return 23;
				}
				return 20;
			case 3:
				switch (lCID)
				{
				case 1108:
					return 13;
				case 1105:
					return 14;
				case 1096:
					return 7;
				case 1093:
					return 4;
				}
				if ((lCID & 0xFF) == 1)
				{
					return 1;
				}
				if ((lCID & 0xFF) != 32 && (lCID & 0xFF) != 41)
				{
					if ((lCID & 0xFF) != 57 && (lCID & 0xFF) != 87 && (lCID & 0xFF) != 78 && (lCID & 0xFF) != 79)
					{
						if ((lCID & 0xFF) == 70)
						{
							return 5;
						}
						if ((lCID & 0xFF) == 71)
						{
							return 6;
						}
						if ((lCID & 0xFF) == 73)
						{
							return 8;
						}
						if ((lCID & 0xFF) == 74)
						{
							return 9;
						}
						if ((lCID & 0xFF) == 75)
						{
							return 10;
						}
						if ((lCID & 0xFF) == 62)
						{
							return 11;
						}
						if ((lCID & 0xFF) != 30)
						{
							break;
						}
						return 12;
					}
					return 3;
				}
				return 2;
			}
			return 0;
		}

		internal static string FormatNumeralVariant(string number, int numeralVariant, CultureInfo numeralLanguage, string numberDecimalSeparator, out bool numberTranslated)
		{
			numberTranslated = true;
			if (number != null && !(number == string.Empty))
			{
				int numeralShape = FormatDigitReplacement.GetNumeralShape(numeralVariant, numeralLanguage);
				if (numeralShape == 0)
				{
					numberTranslated = false;
					return number;
				}
				char numberDecimalSeparator2 = '.';
				if (numberDecimalSeparator != null && numberDecimalSeparator != string.Empty)
				{
					numberDecimalSeparator2 = numberDecimalSeparator[0];
				}
				if (numeralVariant >= 4)
				{
					return FormatDigitReplacement.ComplexTranslateNumber(number, numeralShape, numberDecimalSeparator2, numeralVariant - 4);
				}
				return FormatDigitReplacement.SimpleTranslateNumber(number, numeralShape, numberDecimalSeparator2);
			}
			return number;
		}
	}
}
