using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class BuiltInFonts
	{
		private static Ffn Times_New_Roman;

		private static Ffn Symbol;

		private static Ffn Arial;

		private static Ffn Agency_FB;

		private static Ffn Algerian;

		private static Ffn Arial_Black;

		private static Ffn Arial_Narrow;

		private static Ffn Arial_Rounded_MT_Bold;

		private static Ffn Arial_Unicode_MS;

		private static Ffn Baskerville_Old_Face;

		private static Ffn Batang;

		private static Ffn Bauhaus_93;

		private static Ffn Bell_MT;

		private static Ffn Berlin_Sans_FB;

		private static Ffn Berlin_Sans_FB_Demi;

		private static Ffn Bernard_MT_Condensed;

		private static Ffn Bitstream_Vera_Sans;

		private static Ffn Bitstream_Vera_Sans_Mono;

		private static Ffn Bitstream_Vera_Serif;

		private static Ffn Blackadder_ITC;

		private static Ffn Bodoni_MT;

		private static Ffn Bodoni_MT_Black;

		private static Ffn Bodoni_MT_Condensed;

		private static Ffn Bodoni_MT_Poster_Compressed;

		private static Ffn Book_Antiqua;

		private static Ffn Bookman_Old_Style;

		private static Ffn Bradley_Hand_ITC;

		private static Ffn Britannic_Bold;

		private static Ffn Broadway;

		private static Ffn Brush_Script_MT;

		private static Ffn Californian_FB;

		private static Ffn Calisto_MT;

		private static Ffn Castellar;

		private static Ffn Centaur;

		private static Ffn Century;

		private static Ffn Century_Gothic;

		private static Ffn Century_Schoolbook;

		private static Ffn Chiller;

		private static Ffn Colonna_MT;

		private static Ffn Comic_Sans_MS;

		private static Ffn Cooper_Black;

		private static Ffn Copperplate_Gothic_Bold;

		private static Ffn Copperplate_Gothic_Light;

		private static Ffn Courier_New;

		private static Ffn Curlz_MT;

		private static Ffn Edwardian_Script_ITC;

		private static Ffn Elephant;

		private static Ffn Engravers_MT;

		private static Ffn Eras_Bold_ITC;

		private static Ffn Eras_Demi_ITC;

		private static Ffn Eras_Light_ITC;

		private static Ffn Eras_Medium_ITC;

		private static Ffn Estrangelo_Edessa;

		private static Ffn Felix_Titling;

		private static Ffn Footlight_MT_Light;

		private static Ffn Forte;

		private static Ffn Franklin_Gothic_Book;

		private static Ffn Franklin_Gothic_Demi;

		private static Ffn Franklin_Gothic_Demi_Cond;

		private static Ffn Franklin_Gothic_Heavy;

		private static Ffn Franklin_Gothic_Medium;

		private static Ffn Franklin_Gothic_Medium_Cond;

		private static Ffn Freestyle_Script;

		private static Ffn French_Script_MT;

		private static Ffn Garamond;

		private static Ffn Gautami;

		private static Ffn Georgia;

		private static Ffn Gigi;

		private static Ffn Gill_Sans_MT;

		private static Ffn Gill_Sans_MT_Condensed;

		private static Ffn Gill_Sans_MT_Ext_Condensed_Bold;

		private static Ffn Gill_Sans_Ultra_Bold;

		private static Ffn Gill_Sans_Ultra_Bold_Condensed;

		private static Ffn Gloucester_MT_Extra_Condensed;

		private static Ffn Goudy_Stout;

		private static Ffn Haettenschweiler;

		private static Ffn Harlow_Solid_Italic;

		private static Ffn Harrington;

		private static Ffn High_Tower_Text;

		private static Ffn Impact;

		private static Ffn Imprint_MT_Shadow;

		private static Ffn Informal_Roman;

		private static Ffn Jokerman;

		private static Ffn Juice_ITC;

		private static Ffn Kristen_ITC;

		private static Ffn Kunstler_Script;

		private static Ffn Latha;

		private static Ffn Lucida_Bright;

		private static Ffn Lucida_Calligraphy;

		private static Ffn Lucida_Console;

		private static Ffn Lucida_Fax;

		private static Ffn Onyx;

		private static Ffn Lucida_Handwriting;

		private static Ffn Lucida_Sans;

		private static Ffn Lucida_Sans_Typewriter;

		private static Ffn Lucida_Sans_Unicode;

		private static Ffn Magneto;

		private static Ffn Maiandra_GD;

		private static Ffn Mangal;

		private static Ffn Matura_MT_Script_Capitals;

		private static Ffn Microsoft_Sans_Serif;

		private static Ffn Mistral;

		private static Ffn Modern_No__20;

		private static Ffn Monotype_Corsiva;

		private static Ffn MS_Mincho;

		private static Ffn MS_Reference_Sans_Serif;

		private static Ffn MV_Boli;

		private static Ffn Niagara_Engraved;

		private static Ffn Niagara_Solid;

		private static Ffn OCR_A_Extended;

		private static Ffn Old_English_Text_MT;

		private static Ffn Palace_Script_MT;

		private static Ffn Palatino_Linotype;

		private static Ffn Papyrus;

		private static Ffn Parchment;

		private static Ffn Perpetua;

		private static Ffn Perpetua_Titling_MT;

		private static Ffn Playbill;

		private static Ffn Poor_Richard;

		private static Ffn Pristina;

		private static Ffn Raavi;

		private static Ffn Rage_Italic;

		private static Ffn Ravie;

		private static Ffn Rockwell;

		private static Ffn Rockwell_Condensed;

		private static Ffn Rockwell_Extra_Bold;

		private static Ffn Script_MT_Bold;

		private static Ffn Showcard_Gothic;

		private static Ffn Shruti;

		private static Ffn SimSun;

		private static Ffn Snap_ITC;

		private static Ffn Stencil;

		private static Ffn Sylfaen;

		private static Ffn Tahoma;

		private static Ffn Tempus_Sans_ITC;

		private static Ffn Trebuchet_MS;

		private static Ffn Tunga;

		private static Ffn Tw_Cen_MT;

		private static Ffn Tw_Cen_MT_Condensed;

		private static Ffn Tw_Cen_MT_Condensed_Extra_Bold;

		private static Ffn Verdana;

		private static Ffn Viner_Hand_ITC;

		private static Ffn Vivaldi;

		private static Ffn Vladimir_Script;

		private static Ffn Wide_Latin;

		private static Hashtable m_fontMap;

		private static readonly int BaseFontSize;

		internal static Ffn GetFont(string name)
		{
			Ffn ffn = (Ffn)BuiltInFonts.m_fontMap[name];
			if (ffn == null)
			{
				char[] array = new char[name.Length + 1];
				Array.Copy(name.ToCharArray(), array, name.Length);
				ffn = new Ffn(BuiltInFonts.BaseFontSize + array.Length * 2, 38, 400, 0, 0, new byte[10]
				{
					2,
					11,
					6,
					4,
					2,
					2,
					2,
					2,
					2,
					4
				}, new byte[24]
				{
					135,
					122,
					0,
					32,
					0,
					0,
					0,
					128,
					8,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					255,
					1,
					0,
					0,
					0,
					0,
					0,
					0
				}, array);
			}
			return ffn;
		}

		static BuiltInFonts()
		{
			BuiltInFonts.Times_New_Roman = new Ffn(71, 22, 400, 0, 0, new byte[10]
			{
				2,
				2,
				6,
				3,
				5,
				4,
				5,
				2,
				3,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				32,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'T',
				'i',
				'm',
				'e',
				's',
				' ',
				'N',
				'e',
				'w',
				' ',
				'R',
				'o',
				'm',
				'a',
				'n',
				'\0'
			});
			BuiltInFonts.Symbol = new Ffn(53, 22, 400, 2, 0, new byte[10]
			{
				5,
				5,
				1,
				2,
				1,
				7,
				6,
				2,
				5,
				7
			}, new byte[24]
			{
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				128,
				0,
				0,
				0,
				0
			}, new char[7]
			{
				'S',
				'y',
				'm',
				'b',
				'o',
				'l',
				'\0'
			});
			BuiltInFonts.Arial = new Ffn(51, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				32,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				'\0'
			});
			BuiltInFonts.Agency_FB = new Ffn(59, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				3,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'A',
				'g',
				'e',
				'n',
				'c',
				'y',
				' ',
				'F',
				'B',
				'\0'
			});
			BuiltInFonts.Algerian = new Ffn(57, 86, 400, 0, 0, new byte[10]
			{
				4,
				2,
				7,
				5,
				4,
				10,
				2,
				6,
				7,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'A',
				'l',
				'g',
				'e',
				'r',
				'i',
				'a',
				'n',
				'\0'
			});
			BuiltInFonts.Arial_Black = new Ffn(63, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				10,
				4,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[12]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				' ',
				'B',
				'l',
				'a',
				'c',
				'k',
				'\0'
			});
			BuiltInFonts.Arial_Narrow = new Ffn(65, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				6,
				2,
				2,
				2,
				3,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				' ',
				'N',
				'a',
				'r',
				'r',
				'o',
				'w',
				'\0'
			});
			BuiltInFonts.Arial_Rounded_MT_Bold = new Ffn(83, 38, 400, 0, 0, new byte[10]
			{
				2,
				15,
				7,
				4,
				3,
				5,
				4,
				3,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[22]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				' ',
				'R',
				'o',
				'u',
				'n',
				'd',
				'e',
				'd',
				' ',
				'M',
				'T',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			BuiltInFonts.Arial_Unicode_MS = new Ffn(73, 38, 400, 128, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				255,
				255,
				255,
				255,
				255,
				255,
				255,
				233,
				63,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				63,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				' ',
				'U',
				'n',
				'i',
				'c',
				'o',
				'd',
				'e',
				' ',
				'M',
				'S',
				'\0'
			});
			BuiltInFonts.Baskerville_Old_Face = new Ffn(81, 22, 400, 0, 0, new byte[10]
			{
				2,
				2,
				6,
				2,
				8,
				5,
				5,
				2,
				3,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'B',
				'a',
				's',
				'k',
				'e',
				'r',
				'v',
				'i',
				'l',
				'l',
				'e',
				' ',
				'O',
				'l',
				'd',
				' ',
				'F',
				'a',
				'c',
				'e',
				'\0'
			});
			BuiltInFonts.Batang = new Ffn(59, 22, 400, 129, 7, new byte[10]
			{
				2,
				3,
				6,
				0,
				0,
				1,
				1,
				1,
				1,
				1
			}, new byte[24]
			{
				175,
				2,
				0,
				176,
				251,
				124,
				215,
				105,
				48,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				8,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'B',
				'a',
				't',
				'a',
				'n',
				'g',
				'\0',
				'?',
				'?',
				'\0'
			});
			BuiltInFonts.Bauhaus_93 = new Ffn(61, 86, 400, 0, 0, new byte[10]
			{
				4,
				3,
				9,
				5,
				2,
				11,
				2,
				2,
				12,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[11]
			{
				'B',
				'a',
				'u',
				'h',
				'a',
				'u',
				's',
				' ',
				'9',
				'3',
				'\0'
			});
			BuiltInFonts.Bell_MT = new Ffn(55, 22, 400, 0, 0, new byte[10]
			{
				2,
				2,
				5,
				3,
				6,
				3,
				5,
				2,
				3,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'B',
				'e',
				'l',
				'l',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Berlin_Sans_FB = new Ffn(69, 38, 400, 0, 0, new byte[10]
			{
				2,
				14,
				6,
				2,
				2,
				5,
				2,
				2,
				3,
				6
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'B',
				'e',
				'r',
				'l',
				'i',
				'n',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'F',
				'B',
				'\0'
			});
			BuiltInFonts.Berlin_Sans_FB_Demi = new Ffn(79, 38, 700, 0, 0, new byte[10]
			{
				2,
				14,
				8,
				2,
				2,
				5,
				2,
				2,
				3,
				6
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'B',
				'e',
				'r',
				'l',
				'i',
				'n',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'F',
				'B',
				' ',
				'D',
				'e',
				'm',
				'i',
				'\0'
			});
			BuiltInFonts.Bernard_MT_Condensed = new Ffn(81, 22, 400, 0, 0, new byte[10]
			{
				2,
				5,
				8,
				6,
				6,
				9,
				5,
				2,
				4,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'B',
				'e',
				'r',
				'n',
				'a',
				'r',
				'd',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Bitstream_Vera_Sans = new Ffn(79, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				3,
				3,
				8,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				175,
				0,
				0,
				128,
				74,
				32,
				0,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'B',
				'i',
				't',
				's',
				't',
				'r',
				'e',
				'a',
				'm',
				' ',
				'V',
				'e',
				'r',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				'\0'
			});
			BuiltInFonts.Bitstream_Vera_Sans_Mono = new Ffn(89, 53, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				9,
				3,
				8,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				175,
				0,
				0,
				128,
				74,
				32,
				0,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[25]
			{
				'B',
				'i',
				't',
				's',
				't',
				'r',
				'e',
				'a',
				'm',
				' ',
				'V',
				'e',
				'r',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'o',
				'n',
				'o',
				'\0'
			});
			BuiltInFonts.Bitstream_Vera_Serif = new Ffn(81, 22, 400, 0, 0, new byte[10]
			{
				2,
				6,
				6,
				3,
				5,
				6,
				5,
				2,
				2,
				4
			}, new byte[24]
			{
				175,
				0,
				0,
				128,
				74,
				32,
				0,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'B',
				'i',
				't',
				's',
				't',
				'r',
				'e',
				'a',
				'm',
				' ',
				'V',
				'e',
				'r',
				'a',
				' ',
				'S',
				'e',
				'r',
				'i',
				'f',
				'\0'
			});
			BuiltInFonts.Blackadder_ITC = new Ffn(69, 86, 400, 0, 0, new byte[10]
			{
				4,
				2,
				5,
				5,
				5,
				16,
				7,
				2,
				13,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'B',
				'l',
				'a',
				'c',
				'k',
				'a',
				'd',
				'd',
				'e',
				'r',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Bodoni_MT = new Ffn(59, 22, 400, 0, 0, new byte[10]
			{
				2,
				7,
				6,
				3,
				8,
				6,
				6,
				2,
				2,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'B',
				'o',
				'd',
				'o',
				'n',
				'i',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Bodoni_MT_Black = new Ffn(71, 22, 900, 0, 0, new byte[10]
			{
				2,
				7,
				10,
				3,
				8,
				6,
				6,
				2,
				2,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'B',
				'o',
				'd',
				'o',
				'n',
				'i',
				' ',
				'M',
				'T',
				' ',
				'B',
				'l',
				'a',
				'c',
				'k',
				'\0'
			});
			BuiltInFonts.Bodoni_MT_Condensed = new Ffn(79, 22, 400, 0, 0, new byte[10]
			{
				2,
				7,
				6,
				6,
				8,
				6,
				6,
				2,
				2,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'B',
				'o',
				'd',
				'o',
				'n',
				'i',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Bodoni_MT_Poster_Compressed = new Ffn(95, 22, 300, 0, 0, new byte[10]
			{
				2,
				7,
				7,
				6,
				8,
				6,
				1,
				5,
				2,
				4
			}, new byte[24]
			{
				7,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				17,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[28]
			{
				'B',
				'o',
				'd',
				'o',
				'n',
				'i',
				' ',
				'M',
				'T',
				' ',
				'P',
				'o',
				's',
				't',
				'e',
				'r',
				' ',
				'C',
				'o',
				'm',
				'p',
				'r',
				'e',
				's',
				's',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Book_Antiqua = new Ffn(65, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				6,
				2,
				5,
				3,
				5,
				3,
				3,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'B',
				'o',
				'o',
				'k',
				' ',
				'A',
				'n',
				't',
				'i',
				'q',
				'u',
				'a',
				'\0'
			});
			BuiltInFonts.Bookman_Old_Style = new Ffn(75, 22, 300, 0, 0, new byte[10]
			{
				2,
				5,
				6,
				4,
				5,
				5,
				5,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[18]
			{
				'B',
				'o',
				'o',
				'k',
				'm',
				'a',
				'n',
				' ',
				'O',
				'l',
				'd',
				' ',
				'S',
				't',
				'y',
				'l',
				'e',
				'\0'
			});
			BuiltInFonts.Bradley_Hand_ITC = new Ffn(73, 70, 400, 0, 0, new byte[10]
			{
				3,
				7,
				4,
				2,
				5,
				3,
				2,
				3,
				2,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'B',
				'r',
				'a',
				'd',
				'l',
				'e',
				'y',
				' ',
				'H',
				'a',
				'n',
				'd',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Britannic_Bold = new Ffn(69, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				9,
				3,
				6,
				7,
				3,
				2,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'B',
				'r',
				'i',
				't',
				'a',
				'n',
				'n',
				'i',
				'c',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			BuiltInFonts.Broadway = new Ffn(57, 86, 400, 0, 0, new byte[10]
			{
				4,
				4,
				9,
				5,
				8,
				11,
				2,
				2,
				5,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'B',
				'r',
				'o',
				'a',
				'd',
				'w',
				'a',
				'y',
				'\0'
			});
			BuiltInFonts.Brush_Script_MT = new Ffn(71, 70, 400, 0, 0, new byte[10]
			{
				3,
				6,
				8,
				2,
				4,
				4,
				6,
				7,
				3,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'B',
				'r',
				'u',
				's',
				'h',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Californian_FB = new Ffn(69, 22, 400, 0, 0, new byte[10]
			{
				2,
				7,
				4,
				3,
				6,
				8,
				11,
				3,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'C',
				'a',
				'l',
				'i',
				'f',
				'o',
				'r',
				'n',
				'i',
				'a',
				'n',
				' ',
				'F',
				'B',
				'\0'
			});
			BuiltInFonts.Calisto_MT = new Ffn(61, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				6,
				3,
				5,
				5,
				5,
				3,
				3,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[11]
			{
				'C',
				'a',
				'l',
				'i',
				's',
				't',
				'o',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Castellar = new Ffn(59, 22, 400, 0, 0, new byte[10]
			{
				2,
				10,
				4,
				2,
				6,
				4,
				6,
				1,
				3,
				1
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'C',
				'a',
				's',
				't',
				'e',
				'l',
				'l',
				'a',
				'r',
				'\0'
			});
			BuiltInFonts.Centaur = new Ffn(55, 22, 400, 0, 0, new byte[10]
			{
				2,
				3,
				5,
				4,
				5,
				2,
				5,
				2,
				3,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'C',
				'e',
				'n',
				't',
				'a',
				'u',
				'r',
				'\0'
			});
			BuiltInFonts.Century = new Ffn(55, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				6,
				4,
				5,
				5,
				5,
				2,
				3,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'C',
				'e',
				'n',
				't',
				'u',
				'r',
				'y',
				'\0'
			});
			BuiltInFonts.Century_Gothic = new Ffn(69, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				2,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'C',
				'e',
				'n',
				't',
				'u',
				'r',
				'y',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				'\0'
			});
			BuiltInFonts.Century_Schoolbook = new Ffn(77, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				6,
				4,
				5,
				5,
				5,
				2,
				3,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[19]
			{
				'C',
				'e',
				'n',
				't',
				'u',
				'r',
				'y',
				' ',
				'S',
				'c',
				'h',
				'o',
				'o',
				'l',
				'b',
				'o',
				'o',
				'k',
				'\0'
			});
			BuiltInFonts.Chiller = new Ffn(55, 86, 400, 0, 0, new byte[10]
			{
				4,
				2,
				4,
				4,
				3,
				16,
				7,
				2,
				6,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'C',
				'h',
				'i',
				'l',
				'l',
				'e',
				'r',
				'\0'
			});
			BuiltInFonts.Colonna_MT = new Ffn(61, 86, 400, 0, 0, new byte[10]
			{
				4,
				2,
				8,
				5,
				6,
				2,
				2,
				3,
				2,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[11]
			{
				'C',
				'o',
				'l',
				'o',
				'n',
				'n',
				'a',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Comic_Sans_MS = new Ffn(67, 70, 400, 0, 0, new byte[10]
			{
				3,
				15,
				7,
				2,
				3,
				3,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[14]
			{
				'C',
				'o',
				'm',
				'i',
				'c',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'S',
				'\0'
			});
			BuiltInFonts.Cooper_Black = new Ffn(65, 22, 400, 0, 0, new byte[10]
			{
				2,
				8,
				9,
				4,
				4,
				3,
				11,
				2,
				4,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'C',
				'o',
				'o',
				'p',
				'e',
				'r',
				' ',
				'B',
				'l',
				'a',
				'c',
				'k',
				'\0'
			});
			BuiltInFonts.Copperplate_Gothic_Bold = new Ffn(87, 38, 400, 0, 0, new byte[10]
			{
				2,
				14,
				7,
				5,
				2,
				2,
				6,
				2,
				4,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[24]
			{
				'C',
				'o',
				'p',
				'p',
				'e',
				'r',
				'p',
				'l',
				'a',
				't',
				'e',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			BuiltInFonts.Copperplate_Gothic_Light = new Ffn(89, 38, 400, 0, 0, new byte[10]
			{
				2,
				14,
				5,
				7,
				2,
				2,
				6,
				2,
				4,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[25]
			{
				'C',
				'o',
				'p',
				'p',
				'e',
				'r',
				'p',
				'l',
				'a',
				't',
				'e',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'L',
				'i',
				'g',
				'h',
				't',
				'\0'
			});
			BuiltInFonts.Courier_New = new Ffn(63, 53, 400, 0, 0, new byte[10]
			{
				2,
				7,
				3,
				9,
				2,
				2,
				5,
				2,
				4,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				32,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[12]
			{
				'C',
				'o',
				'u',
				'r',
				'i',
				'e',
				'r',
				' ',
				'N',
				'e',
				'w',
				'\0'
			});
			BuiltInFonts.Curlz_MT = new Ffn(57, 86, 400, 0, 0, new byte[10]
			{
				4,
				4,
				4,
				4,
				5,
				7,
				2,
				2,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'C',
				'u',
				'r',
				'l',
				'z',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Edwardian_Script_ITC = new Ffn(81, 70, 400, 0, 0, new byte[10]
			{
				3,
				3,
				3,
				2,
				4,
				7,
				7,
				13,
				8,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'E',
				'd',
				'w',
				'a',
				'r',
				'd',
				'i',
				'a',
				'n',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Elephant = new Ffn(57, 22, 400, 0, 0, new byte[10]
			{
				2,
				2,
				9,
				4,
				9,
				5,
				5,
				2,
				3,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'E',
				'l',
				'e',
				'p',
				'h',
				'a',
				'n',
				't',
				'\0'
			});
			BuiltInFonts.Engravers_MT = new Ffn(65, 22, 500, 0, 0, new byte[10]
			{
				2,
				9,
				7,
				7,
				8,
				5,
				5,
				2,
				3,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'E',
				'n',
				'g',
				'r',
				'a',
				'v',
				'e',
				'r',
				's',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Eras_Bold_ITC = new Ffn(67, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				9,
				7,
				3,
				5,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[14]
			{
				'E',
				'r',
				'a',
				's',
				' ',
				'B',
				'o',
				'l',
				'd',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Eras_Demi_ITC = new Ffn(67, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				8,
				5,
				3,
				5,
				4,
				2,
				8,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[14]
			{
				'E',
				'r',
				'a',
				's',
				' ',
				'D',
				'e',
				'm',
				'i',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Eras_Light_ITC = new Ffn(69, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				4,
				2,
				3,
				5,
				4,
				2,
				8,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'E',
				'r',
				'a',
				's',
				' ',
				'L',
				'i',
				'g',
				'h',
				't',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Eras_Medium_ITC = new Ffn(71, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				2,
				3,
				5,
				4,
				2,
				8,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'E',
				'r',
				'a',
				's',
				' ',
				'M',
				'e',
				'd',
				'i',
				'u',
				'm',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose = new byte[10];
			BuiltInFonts.Estrangelo_Edessa = new Ffn(75, 70, 400, 0, 0, panose, new byte[24]
			{
				67,
				96,
				0,
				128,
				0,
				0,
				0,
				0,
				128,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[18]
			{
				'E',
				's',
				't',
				'r',
				'a',
				'n',
				'g',
				'e',
				'l',
				'o',
				' ',
				'E',
				'd',
				'e',
				's',
				's',
				'a',
				'\0'
			});
			BuiltInFonts.Felix_Titling = new Ffn(67, 86, 400, 0, 0, new byte[10]
			{
				4,
				6,
				5,
				5,
				6,
				2,
				2,
				2,
				10,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[14]
			{
				'F',
				'e',
				'l',
				'i',
				'x',
				' ',
				'T',
				'i',
				't',
				'l',
				'i',
				'n',
				'g',
				'\0'
			});
			BuiltInFonts.Footlight_MT_Light = new Ffn(77, 22, 300, 0, 0, new byte[10]
			{
				2,
				4,
				6,
				2,
				6,
				3,
				10,
				2,
				3,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[19]
			{
				'F',
				'o',
				'o',
				't',
				'l',
				'i',
				'g',
				'h',
				't',
				' ',
				'M',
				'T',
				' ',
				'L',
				'i',
				'g',
				'h',
				't',
				'\0'
			});
			BuiltInFonts.Forte = new Ffn(51, 70, 400, 0, 0, new byte[10]
			{
				3,
				6,
				9,
				2,
				4,
				5,
				2,
				7,
				2,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'F',
				'o',
				'r',
				't',
				'e',
				'\0'
			});
			BuiltInFonts.Franklin_Gothic_Book = new Ffn(81, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				3,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'B',
				'o',
				'o',
				'k',
				'\0'
			});
			BuiltInFonts.Franklin_Gothic_Demi = new Ffn(81, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				7,
				3,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'D',
				'e',
				'm',
				'i',
				'\0'
			});
			BuiltInFonts.Franklin_Gothic_Demi_Cond = new Ffn(91, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				7,
				6,
				3,
				4,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[26]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'D',
				'e',
				'm',
				'i',
				' ',
				'C',
				'o',
				'n',
				'd',
				'\0'
			});
			BuiltInFonts.Franklin_Gothic_Heavy = new Ffn(83, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				9,
				3,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[22]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'H',
				'e',
				'a',
				'v',
				'y',
				'\0'
			});
			BuiltInFonts.Franklin_Gothic_Medium = new Ffn(85, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				3,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[23]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'M',
				'e',
				'd',
				'i',
				'u',
				'm',
				'\0'
			});
			BuiltInFonts.Franklin_Gothic_Medium_Cond = new Ffn(95, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				6,
				3,
				4,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[28]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'M',
				'e',
				'd',
				'i',
				'u',
				'm',
				' ',
				'C',
				'o',
				'n',
				'd',
				'\0'
			});
			BuiltInFonts.Freestyle_Script = new Ffn(73, 70, 400, 0, 0, new byte[10]
			{
				3,
				8,
				4,
				2,
				3,
				2,
				5,
				11,
				4,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'F',
				'r',
				'e',
				'e',
				's',
				't',
				'y',
				'l',
				'e',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				'\0'
			});
			BuiltInFonts.French_Script_MT = new Ffn(73, 70, 400, 0, 0, new byte[10]
			{
				3,
				2,
				4,
				2,
				4,
				6,
				7,
				4,
				6,
				5
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'F',
				'r',
				'e',
				'n',
				'c',
				'h',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Garamond = new Ffn(57, 22, 400, 0, 0, new byte[10]
			{
				2,
				2,
				4,
				4,
				3,
				3,
				1,
				1,
				8,
				3
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'G',
				'a',
				'r',
				'a',
				'm',
				'o',
				'n',
				'd',
				'\0'
			});
			BuiltInFonts.Gautami = new Ffn(55, 6, 400, 0, 0, new byte[10]
			{
				2,
				0,
				5,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				32,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'G',
				'a',
				'u',
				't',
				'a',
				'm',
				'i',
				'\0'
			});
			BuiltInFonts.Georgia = new Ffn(55, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				5,
				2,
				5,
				4,
				5,
				2,
				3,
				3
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'G',
				'e',
				'o',
				'r',
				'g',
				'i',
				'a',
				'\0'
			});
			BuiltInFonts.Gigi = new Ffn(49, 86, 400, 0, 0, new byte[10]
			{
				4,
				4,
				5,
				4,
				6,
				16,
				7,
				2,
				13,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[5]
			{
				'G',
				'i',
				'g',
				'i',
				'\0'
			});
			BuiltInFonts.Gill_Sans_MT = new Ffn(65, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				2,
				2,
				1,
				4,
				2,
				2,
				3
			}, new byte[24]
			{
				7,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Gill_Sans_MT_Condensed = new Ffn(85, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				6,
				2,
				1,
				4,
				2,
				2,
				3
			}, new byte[24]
			{
				7,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[23]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Gill_Sans_MT_Ext_Condensed_Bold = new Ffn(103, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				9,
				2,
				2,
				1,
				4,
				2,
				2,
				3
			}, new byte[24]
			{
				7,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[32]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'T',
				' ',
				'E',
				'x',
				't',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			BuiltInFonts.Gill_Sans_Ultra_Bold = new Ffn(81, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				10,
				2,
				2,
				1,
				4,
				2,
				2,
				3
			}, new byte[24]
			{
				7,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'U',
				'l',
				't',
				'r',
				'a',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			BuiltInFonts.Gill_Sans_Ultra_Bold_Condensed = new Ffn(101, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				10,
				6,
				2,
				1,
				4,
				2,
				2,
				3
			}, new byte[24]
			{
				7,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[31]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'U',
				'l',
				't',
				'r',
				'a',
				' ',
				'B',
				'o',
				'l',
				'd',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Gloucester_MT_Extra_Condensed = new Ffn(99, 22, 400, 0, 0, new byte[10]
			{
				2,
				3,
				8,
				8,
				2,
				6,
				1,
				1,
				1,
				1
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[30]
			{
				'G',
				'l',
				'o',
				'u',
				'c',
				'e',
				's',
				't',
				'e',
				'r',
				' ',
				'M',
				'T',
				' ',
				'E',
				'x',
				't',
				'r',
				'a',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Goudy_Stout = new Ffn(63, 22, 400, 0, 0, new byte[10]
			{
				2,
				2,
				9,
				4,
				7,
				3,
				11,
				2,
				4,
				1
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[12]
			{
				'G',
				'o',
				'u',
				'd',
				'y',
				' ',
				'S',
				't',
				'o',
				'u',
				't',
				'\0'
			});
			BuiltInFonts.Haettenschweiler = new Ffn(73, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				7,
				6,
				4,
				9,
				2,
				6,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'H',
				'a',
				'e',
				't',
				't',
				'e',
				'n',
				's',
				'c',
				'h',
				'w',
				'e',
				'i',
				'l',
				'e',
				'r',
				'\0'
			});
			BuiltInFonts.Harlow_Solid_Italic = new Ffn(79, 86, 400, 0, 0, new byte[10]
			{
				4,
				3,
				6,
				4,
				2,
				15,
				2,
				2,
				13,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'H',
				'a',
				'r',
				'l',
				'o',
				'w',
				' ',
				'S',
				'o',
				'l',
				'i',
				'd',
				' ',
				'I',
				't',
				'a',
				'l',
				'i',
				'c',
				'\0'
			});
			BuiltInFonts.Harrington = new Ffn(61, 86, 400, 0, 0, new byte[10]
			{
				4,
				4,
				5,
				5,
				5,
				10,
				2,
				2,
				7,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[11]
			{
				'H',
				'a',
				'r',
				'r',
				'i',
				'n',
				'g',
				't',
				'o',
				'n',
				'\0'
			});
			BuiltInFonts.High_Tower_Text = new Ffn(71, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				5,
				2,
				5,
				5,
				6,
				3,
				3,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'H',
				'i',
				'g',
				'h',
				' ',
				'T',
				'o',
				'w',
				'e',
				'r',
				' ',
				'T',
				'e',
				'x',
				't',
				'\0'
			});
			BuiltInFonts.Impact = new Ffn(53, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				8,
				6,
				3,
				9,
				2,
				5,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[7]
			{
				'I',
				'm',
				'p',
				'a',
				'c',
				't',
				'\0'
			});
			BuiltInFonts.Imprint_MT_Shadow = new Ffn(75, 86, 400, 0, 0, new byte[10]
			{
				4,
				2,
				6,
				5,
				6,
				3,
				3,
				3,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[18]
			{
				'I',
				'm',
				'p',
				'r',
				'i',
				'n',
				't',
				' ',
				'M',
				'T',
				' ',
				'S',
				'h',
				'a',
				'd',
				'o',
				'w',
				'\0'
			});
			BuiltInFonts.Informal_Roman = new Ffn(69, 70, 400, 0, 0, new byte[10]
			{
				3,
				6,
				4,
				2,
				3,
				4,
				6,
				11,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'I',
				'n',
				'f',
				'o',
				'r',
				'm',
				'a',
				'l',
				' ',
				'R',
				'o',
				'm',
				'a',
				'n',
				'\0'
			});
			BuiltInFonts.Jokerman = new Ffn(57, 86, 400, 0, 0, new byte[10]
			{
				4,
				9,
				6,
				5,
				6,
				13,
				6,
				2,
				7,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'J',
				'o',
				'k',
				'e',
				'r',
				'm',
				'a',
				'n',
				'\0'
			});
			BuiltInFonts.Juice_ITC = new Ffn(59, 86, 400, 0, 0, new byte[10]
			{
				4,
				4,
				4,
				3,
				4,
				10,
				2,
				2,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'J',
				'u',
				'i',
				'c',
				'e',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Kristen_ITC = new Ffn(63, 70, 400, 0, 0, new byte[10]
			{
				3,
				5,
				5,
				2,
				4,
				2,
				2,
				3,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[12]
			{
				'K',
				'r',
				'i',
				's',
				't',
				'e',
				'n',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Kunstler_Script = new Ffn(71, 70, 400, 0, 0, new byte[10]
			{
				3,
				3,
				4,
				2,
				2,
				6,
				7,
				13,
				13,
				6
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'K',
				'u',
				'n',
				's',
				't',
				'l',
				'e',
				'r',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				'\0'
			});
			BuiltInFonts.Latha = new Ffn(51, 6, 400, 0, 0, new byte[10]
			{
				2,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'L',
				'a',
				't',
				'h',
				'a',
				'\0'
			});
			BuiltInFonts.Lucida_Bright = new Ffn(67, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				6,
				2,
				5,
				5,
				5,
				2,
				3,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[14]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'B',
				'r',
				'i',
				'g',
				'h',
				't',
				'\0'
			});
			BuiltInFonts.Lucida_Calligraphy = new Ffn(77, 70, 400, 0, 0, new byte[10]
			{
				3,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[19]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'C',
				'a',
				'l',
				'l',
				'i',
				'g',
				'r',
				'a',
				'p',
				'h',
				'y',
				'\0'
			});
			BuiltInFonts.Lucida_Console = new Ffn(69, 53, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				9,
				4,
				5,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				143,
				2,
				0,
				128,
				0,
				24,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				31,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'C',
				'o',
				'n',
				's',
				'o',
				'l',
				'e',
				'\0'
			});
			BuiltInFonts.Lucida_Fax = new Ffn(61, 22, 400, 0, 0, new byte[10]
			{
				2,
				6,
				6,
				2,
				5,
				5,
				5,
				2,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[11]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'F',
				'a',
				'x',
				'\0'
			});
			BuiltInFonts.Onyx = new Ffn(49, 86, 400, 0, 0, new byte[10]
			{
				4,
				5,
				6,
				2,
				8,
				7,
				2,
				2,
				2,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[5]
			{
				'O',
				'n',
				'y',
				'x',
				'\0'
			});
			BuiltInFonts.Lucida_Handwriting = new Ffn(77, 70, 400, 0, 0, new byte[10]
			{
				3,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[19]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'H',
				'a',
				'n',
				'd',
				'w',
				'r',
				'i',
				't',
				'i',
				'n',
				'g',
				'\0'
			});
			BuiltInFonts.Lucida_Sans = new Ffn(63, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				2,
				3,
				5,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[12]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				'\0'
			});
			BuiltInFonts.Lucida_Sans_Typewriter = new Ffn(85, 53, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				9,
				3,
				5,
				4,
				3,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[23]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'T',
				'y',
				'p',
				'e',
				'w',
				'r',
				'i',
				't',
				'e',
				'r',
				'\0'
			});
			BuiltInFonts.Lucida_Sans_Unicode = new Ffn(79, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				2,
				3,
				5,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				255,
				26,
				0,
				128,
				107,
				57,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				63,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'U',
				'n',
				'i',
				'c',
				'o',
				'd',
				'e',
				'\0'
			});
			BuiltInFonts.Magneto = new Ffn(55, 86, 700, 0, 0, new byte[10]
			{
				4,
				3,
				8,
				5,
				5,
				8,
				2,
				2,
				13,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'M',
				'a',
				'g',
				'n',
				'e',
				't',
				'o',
				'\0'
			});
			BuiltInFonts.Maiandra_GD = new Ffn(63, 38, 400, 0, 0, new byte[10]
			{
				2,
				14,
				5,
				2,
				3,
				3,
				8,
				2,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[12]
			{
				'M',
				'a',
				'i',
				'a',
				'n',
				'd',
				'r',
				'a',
				' ',
				'G',
				'D',
				'\0'
			});
			BuiltInFonts.Mangal = new Ffn(53, 6, 400, 0, 0, new byte[10]
			{
				0,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				128,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[7]
			{
				'M',
				'a',
				'n',
				'g',
				'a',
				'l',
				'\0'
			});
			BuiltInFonts.Matura_MT_Script_Capitals = new Ffn(91, 70, 400, 0, 0, new byte[10]
			{
				3,
				2,
				8,
				2,
				6,
				6,
				2,
				7,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[26]
			{
				'M',
				'a',
				't',
				'u',
				'r',
				'a',
				' ',
				'M',
				'T',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'C',
				'a',
				'p',
				'i',
				't',
				'a',
				'l',
				's',
				'\0'
			});
			BuiltInFonts.Microsoft_Sans_Serif = new Ffn(81, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				33,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				1,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'M',
				'i',
				'c',
				'r',
				'o',
				's',
				'o',
				'f',
				't',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'S',
				'e',
				'r',
				'i',
				'f',
				'\0'
			});
			BuiltInFonts.Mistral = new Ffn(55, 70, 400, 0, 0, new byte[10]
			{
				3,
				9,
				7,
				2,
				3,
				4,
				7,
				2,
				4,
				3
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'M',
				'i',
				's',
				't',
				'r',
				'a',
				'l',
				'\0'
			});
			BuiltInFonts.Modern_No__20 = new Ffn(67, 22, 400, 0, 0, new byte[10]
			{
				2,
				7,
				7,
				4,
				7,
				5,
				5,
				2,
				3,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[14]
			{
				'M',
				'o',
				'd',
				'e',
				'r',
				'n',
				' ',
				'N',
				'o',
				'.',
				' ',
				'2',
				'0',
				'\0'
			});
			BuiltInFonts.Monotype_Corsiva = new Ffn(73, 70, 400, 0, 0, new byte[10]
			{
				3,
				1,
				1,
				1,
				1,
				2,
				1,
				1,
				1,
				1
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'M',
				'o',
				'n',
				'o',
				't',
				'y',
				'p',
				'e',
				' ',
				'C',
				'o',
				'r',
				's',
				'i',
				'v',
				'a',
				'\0'
			});
			BuiltInFonts.MS_Mincho = new Ffn(71, 53, 400, 128, 10, new byte[10]
			{
				2,
				2,
				6,
				9,
				4,
				2,
				5,
				8,
				3,
				4
			}, new byte[24]
			{
				191,
				2,
				0,
				160,
				251,
				252,
				199,
				104,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				2,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'M',
				'S',
				' ',
				'M',
				'i',
				'n',
				'c',
				'h',
				'o',
				'\0',
				'?',
				'?',
				' ',
				'?',
				'?',
				'\0'
			});
			BuiltInFonts.MS_Reference_Sans_Serif = new Ffn(87, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				3,
				5,
				4,
				4,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				32,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[24]
			{
				'M',
				'S',
				' ',
				'R',
				'e',
				'f',
				'e',
				'r',
				'e',
				'n',
				'c',
				'e',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'S',
				'e',
				'r',
				'i',
				'f',
				'\0'
			});
			byte[] panose2 = new byte[10];
			BuiltInFonts.MV_Boli = new Ffn(55, 6, 400, 0, 0, panose2, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'M',
				'V',
				' ',
				'B',
				'o',
				'l',
				'i',
				'\0'
			});
			BuiltInFonts.Niagara_Engraved = new Ffn(73, 86, 400, 0, 0, new byte[10]
			{
				4,
				2,
				5,
				2,
				7,
				7,
				3,
				3,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'N',
				'i',
				'a',
				'g',
				'a',
				'r',
				'a',
				' ',
				'E',
				'n',
				'g',
				'r',
				'a',
				'v',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Niagara_Solid = new Ffn(67, 86, 400, 0, 0, new byte[10]
			{
				4,
				2,
				5,
				2,
				7,
				7,
				2,
				2,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[14]
			{
				'N',
				'i',
				'a',
				'g',
				'a',
				'r',
				'a',
				' ',
				'S',
				'o',
				'l',
				'i',
				'd',
				'\0'
			});
			BuiltInFonts.OCR_A_Extended = new Ffn(69, 54, 400, 0, 0, new byte[10]
			{
				2,
				1,
				5,
				9,
				2,
				1,
				2,
				1,
				3,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'O',
				'C',
				'R',
				' ',
				'A',
				' ',
				'E',
				'x',
				't',
				'e',
				'n',
				'd',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Old_English_Text_MT = new Ffn(79, 70, 400, 0, 0, new byte[10]
			{
				3,
				4,
				9,
				2,
				4,
				5,
				8,
				3,
				8,
				6
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'O',
				'l',
				'd',
				' ',
				'E',
				'n',
				'g',
				'l',
				'i',
				's',
				'h',
				' ',
				'T',
				'e',
				'x',
				't',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Palace_Script_MT = new Ffn(73, 70, 400, 0, 0, new byte[10]
			{
				3,
				3,
				3,
				2,
				2,
				6,
				7,
				12,
				11,
				5
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'P',
				'a',
				'l',
				'a',
				'c',
				'e',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Palatino_Linotype = new Ffn(75, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				5,
				2,
				5,
				5,
				5,
				3,
				3,
				4
			}, new byte[24]
			{
				135,
				3,
				0,
				224,
				19,
				0,
				0,
				64,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[18]
			{
				'P',
				'a',
				'l',
				'a',
				't',
				'i',
				'n',
				'o',
				' ',
				'L',
				'i',
				'n',
				'o',
				't',
				'y',
				'p',
				'e',
				'\0'
			});
			BuiltInFonts.Papyrus = new Ffn(55, 70, 400, 0, 0, new byte[10]
			{
				3,
				7,
				5,
				2,
				6,
				5,
				2,
				3,
				2,
				5
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'P',
				'a',
				'p',
				'y',
				'r',
				'u',
				's',
				'\0'
			});
			BuiltInFonts.Parchment = new Ffn(59, 70, 400, 0, 0, new byte[10]
			{
				3,
				4,
				6,
				2,
				4,
				7,
				8,
				4,
				8,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'P',
				'a',
				'r',
				'c',
				'h',
				'm',
				'e',
				'n',
				't',
				'\0'
			});
			BuiltInFonts.Perpetua = new Ffn(57, 22, 400, 0, 0, new byte[10]
			{
				2,
				2,
				5,
				2,
				6,
				4,
				1,
				2,
				3,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'P',
				'e',
				'r',
				'p',
				'e',
				't',
				'u',
				'a',
				'\0'
			});
			BuiltInFonts.Perpetua_Titling_MT = new Ffn(79, 22, 300, 0, 0, new byte[10]
			{
				2,
				2,
				5,
				2,
				6,
				5,
				5,
				2,
				8,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'P',
				'e',
				'r',
				'p',
				'e',
				't',
				'u',
				'a',
				' ',
				'T',
				'i',
				't',
				'l',
				'i',
				'n',
				'g',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Playbill = new Ffn(57, 86, 400, 0, 0, new byte[10]
			{
				4,
				5,
				6,
				3,
				10,
				6,
				2,
				2,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'P',
				'l',
				'a',
				'y',
				'b',
				'i',
				'l',
				'l',
				'\0'
			});
			BuiltInFonts.Poor_Richard = new Ffn(65, 22, 400, 0, 0, new byte[10]
			{
				2,
				8,
				5,
				2,
				5,
				5,
				5,
				2,
				7,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'P',
				'o',
				'o',
				'r',
				' ',
				'R',
				'i',
				'c',
				'h',
				'a',
				'r',
				'd',
				'\0'
			});
			BuiltInFonts.Pristina = new Ffn(57, 70, 400, 0, 0, new byte[10]
			{
				3,
				6,
				4,
				2,
				4,
				4,
				6,
				8,
				2,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'P',
				'r',
				'i',
				's',
				't',
				'i',
				'n',
				'a',
				'\0'
			});
			BuiltInFonts.Raavi = new Ffn(51, 6, 400, 0, 0, new byte[10]
			{
				2,
				0,
				5,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'R',
				'a',
				'a',
				'v',
				'i',
				'\0'
			});
			BuiltInFonts.Rage_Italic = new Ffn(63, 70, 400, 0, 0, new byte[10]
			{
				3,
				7,
				5,
				2,
				4,
				5,
				7,
				7,
				3,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[12]
			{
				'R',
				'a',
				'g',
				'e',
				' ',
				'I',
				't',
				'a',
				'l',
				'i',
				'c',
				'\0'
			});
			BuiltInFonts.Ravie = new Ffn(51, 86, 400, 0, 0, new byte[10]
			{
				4,
				4,
				8,
				5,
				5,
				8,
				9,
				2,
				6,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'R',
				'a',
				'v',
				'i',
				'e',
				'\0'
			});
			BuiltInFonts.Rockwell = new Ffn(57, 22, 400, 0, 0, new byte[10]
			{
				2,
				6,
				6,
				3,
				2,
				2,
				5,
				2,
				4,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'R',
				'o',
				'c',
				'k',
				'w',
				'e',
				'l',
				'l',
				'\0'
			});
			BuiltInFonts.Rockwell_Condensed = new Ffn(77, 22, 400, 0, 0, new byte[10]
			{
				2,
				6,
				6,
				3,
				5,
				4,
				5,
				2,
				1,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[19]
			{
				'R',
				'o',
				'c',
				'k',
				'w',
				'e',
				'l',
				'l',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Rockwell_Extra_Bold = new Ffn(79, 22, 800, 0, 0, new byte[10]
			{
				2,
				6,
				9,
				3,
				4,
				5,
				5,
				2,
				4,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'R',
				'o',
				'c',
				'k',
				'w',
				'e',
				'l',
				'l',
				' ',
				'E',
				'x',
				't',
				'r',
				'a',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			BuiltInFonts.Script_MT_Bold = new Ffn(69, 70, 700, 0, 0, new byte[10]
			{
				3,
				4,
				6,
				2,
				4,
				6,
				7,
				8,
				9,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'M',
				'T',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			BuiltInFonts.Showcard_Gothic = new Ffn(71, 86, 400, 0, 0, new byte[10]
			{
				4,
				2,
				9,
				4,
				2,
				1,
				2,
				2,
				6,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'S',
				'h',
				'o',
				'w',
				'c',
				'a',
				'r',
				'd',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				'\0'
			});
			BuiltInFonts.Shruti = new Ffn(53, 6, 400, 0, 0, new byte[10]
			{
				2,
				0,
				5,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[7]
			{
				'S',
				'h',
				'r',
				'u',
				't',
				'i',
				'\0'
			});
			BuiltInFonts.SimSun = new Ffn(59, 6, 400, 134, 7, new byte[10]
			{
				2,
				1,
				6,
				0,
				3,
				1,
				1,
				1,
				1,
				1
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				14,
				8,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				4,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'S',
				'i',
				'm',
				'S',
				'u',
				'n',
				'\0',
				'?',
				'?',
				'\0'
			});
			BuiltInFonts.Snap_ITC = new Ffn(57, 86, 400, 0, 0, new byte[10]
			{
				4,
				4,
				10,
				7,
				6,
				10,
				2,
				2,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'S',
				'n',
				'a',
				'p',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Stencil = new Ffn(55, 86, 400, 0, 0, new byte[10]
			{
				4,
				4,
				9,
				5,
				13,
				8,
				2,
				2,
				4,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'S',
				't',
				'e',
				'n',
				'c',
				'i',
				'l',
				'\0'
			});
			BuiltInFonts.Sylfaen = new Ffn(55, 22, 400, 0, 0, new byte[10]
			{
				1,
				10,
				5,
				2,
				5,
				3,
				6,
				3,
				3,
				3
			}, new byte[24]
			{
				135,
				6,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'S',
				'y',
				'l',
				'f',
				'a',
				'e',
				'n',
				'\0'
			});
			BuiltInFonts.Tahoma = new Ffn(53, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				3,
				5,
				4,
				4,
				2,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				97,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				1,
				0,
				0,
				0,
				0,
				0
			}, new char[7]
			{
				'T',
				'a',
				'h',
				'o',
				'm',
				'a',
				'\0'
			});
			BuiltInFonts.Tempus_Sans_ITC = new Ffn(71, 86, 400, 0, 0, new byte[10]
			{
				4,
				2,
				4,
				4,
				3,
				13,
				7,
				2,
				2,
				2
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'T',
				'e',
				'm',
				'p',
				'u',
				's',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Trebuchet_MS = new Ffn(65, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				3,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'T',
				'r',
				'e',
				'b',
				'u',
				'c',
				'h',
				'e',
				't',
				' ',
				'M',
				'S',
				'\0'
			});
			BuiltInFonts.Tunga = new Ffn(51, 6, 400, 0, 0, new byte[10]
			{
				0,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				64,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'T',
				'u',
				'n',
				'g',
				'a',
				'\0'
			});
			BuiltInFonts.Tw_Cen_MT = new Ffn(59, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				2,
				2,
				1,
				4,
				2,
				6,
				3
			}, new byte[24]
			{
				7,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'T',
				'w',
				' ',
				'C',
				'e',
				'n',
				' ',
				'M',
				'T',
				'\0'
			});
			BuiltInFonts.Tw_Cen_MT_Condensed = new Ffn(79, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				6,
				2,
				1,
				4,
				2,
				2,
				3
			}, new byte[24]
			{
				7,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'T',
				'w',
				' ',
				'C',
				'e',
				'n',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			BuiltInFonts.Tw_Cen_MT_Condensed_Extra_Bold = new Ffn(101, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				8,
				3,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				7,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[31]
			{
				'T',
				'w',
				' ',
				'C',
				'e',
				'n',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				' ',
				'E',
				'x',
				't',
				'r',
				'a',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			BuiltInFonts.Verdana = new Ffn(55, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				3,
				5,
				4,
				4,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				32,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'V',
				'e',
				'r',
				'd',
				'a',
				'n',
				'a',
				'\0'
			});
			BuiltInFonts.Viner_Hand_ITC = new Ffn(69, 70, 400, 0, 0, new byte[10]
			{
				3,
				7,
				5,
				2,
				3,
				5,
				2,
				2,
				2,
				3
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'V',
				'i',
				'n',
				'e',
				'r',
				' ',
				'H',
				'a',
				'n',
				'd',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			BuiltInFonts.Vivaldi = new Ffn(55, 70, 400, 0, 0, new byte[10]
			{
				3,
				2,
				6,
				2,
				5,
				5,
				6,
				9,
				8,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'V',
				'i',
				'v',
				'a',
				'l',
				'd',
				'i',
				'\0'
			});
			BuiltInFonts.Vladimir_Script = new Ffn(71, 70, 400, 0, 0, new byte[10]
			{
				3,
				5,
				4,
				2,
				4,
				4,
				7,
				7,
				3,
				5
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'V',
				'l',
				'a',
				'd',
				'i',
				'm',
				'i',
				'r',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				'\0'
			});
			BuiltInFonts.Wide_Latin = new Ffn(61, 22, 400, 0, 0, new byte[10]
			{
				2,
				10,
				10,
				7,
				5,
				5,
				5,
				2,
				4,
				4
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[11]
			{
				'W',
				'i',
				'd',
				'e',
				' ',
				'L',
				'a',
				't',
				'i',
				'n',
				'\0'
			});
			BuiltInFonts.m_fontMap = new Hashtable();
			BuiltInFonts.BaseFontSize = 39;
			BuiltInFonts.m_fontMap["Times New Roman"] = BuiltInFonts.Times_New_Roman;
			BuiltInFonts.m_fontMap["Symbol"] = BuiltInFonts.Symbol;
			BuiltInFonts.m_fontMap["Arial"] = BuiltInFonts.Arial;
			BuiltInFonts.m_fontMap["Agency FB"] = BuiltInFonts.Agency_FB;
			BuiltInFonts.m_fontMap["Algerian"] = BuiltInFonts.Algerian;
			BuiltInFonts.m_fontMap["Arial Black"] = BuiltInFonts.Arial_Black;
			BuiltInFonts.m_fontMap["Arial Narrow"] = BuiltInFonts.Arial_Narrow;
			BuiltInFonts.m_fontMap["Arial Rounded MT Bold"] = BuiltInFonts.Arial_Rounded_MT_Bold;
			BuiltInFonts.m_fontMap["Arial Unicode MS"] = BuiltInFonts.Arial_Unicode_MS;
			BuiltInFonts.m_fontMap["Baskerville Old Face"] = BuiltInFonts.Baskerville_Old_Face;
			BuiltInFonts.m_fontMap["Batang"] = BuiltInFonts.Batang;
			BuiltInFonts.m_fontMap["Bauhaus 93"] = BuiltInFonts.Bauhaus_93;
			BuiltInFonts.m_fontMap["Bell MT"] = BuiltInFonts.Bell_MT;
			BuiltInFonts.m_fontMap["Berlin Sans FB"] = BuiltInFonts.Berlin_Sans_FB;
			BuiltInFonts.m_fontMap["Berlin Sans FB Demi"] = BuiltInFonts.Berlin_Sans_FB_Demi;
			BuiltInFonts.m_fontMap["Bernard MT Condensed"] = BuiltInFonts.Bernard_MT_Condensed;
			BuiltInFonts.m_fontMap["Bitstream Vera Sans"] = BuiltInFonts.Bitstream_Vera_Sans;
			BuiltInFonts.m_fontMap["Bitstream Vera Sans Mono"] = BuiltInFonts.Bitstream_Vera_Sans_Mono;
			BuiltInFonts.m_fontMap["Bitstream Vera Serif"] = BuiltInFonts.Bitstream_Vera_Serif;
			BuiltInFonts.m_fontMap["Blackadder ITC"] = BuiltInFonts.Blackadder_ITC;
			BuiltInFonts.m_fontMap["Bodoni MT"] = BuiltInFonts.Bodoni_MT;
			BuiltInFonts.m_fontMap["Bodoni MT Black"] = BuiltInFonts.Bodoni_MT_Black;
			BuiltInFonts.m_fontMap["Bodoni MT Condensed"] = BuiltInFonts.Bodoni_MT_Condensed;
			BuiltInFonts.m_fontMap["Bodoni MT Poster Compressed"] = BuiltInFonts.Bodoni_MT_Poster_Compressed;
			BuiltInFonts.m_fontMap["Book Antiqua"] = BuiltInFonts.Book_Antiqua;
			BuiltInFonts.m_fontMap["Bookman Old Style"] = BuiltInFonts.Bookman_Old_Style;
			BuiltInFonts.m_fontMap["Bradley Hand ITC"] = BuiltInFonts.Bradley_Hand_ITC;
			BuiltInFonts.m_fontMap["Britannic Bold"] = BuiltInFonts.Britannic_Bold;
			BuiltInFonts.m_fontMap["Broadway"] = BuiltInFonts.Broadway;
			BuiltInFonts.m_fontMap["Brush Script MT"] = BuiltInFonts.Brush_Script_MT;
			BuiltInFonts.m_fontMap["Californian FB"] = BuiltInFonts.Californian_FB;
			BuiltInFonts.m_fontMap["Calisto MT"] = BuiltInFonts.Calisto_MT;
			BuiltInFonts.m_fontMap["Castellar"] = BuiltInFonts.Castellar;
			BuiltInFonts.m_fontMap["Centaur"] = BuiltInFonts.Centaur;
			BuiltInFonts.m_fontMap["Century"] = BuiltInFonts.Century;
			BuiltInFonts.m_fontMap["Century Gothic"] = BuiltInFonts.Century_Gothic;
			BuiltInFonts.m_fontMap["Century Schoolbook"] = BuiltInFonts.Century_Schoolbook;
			BuiltInFonts.m_fontMap["Chiller"] = BuiltInFonts.Chiller;
			BuiltInFonts.m_fontMap["Colonna MT"] = BuiltInFonts.Colonna_MT;
			BuiltInFonts.m_fontMap["Comic Sans MS"] = BuiltInFonts.Comic_Sans_MS;
			BuiltInFonts.m_fontMap["Cooper Black"] = BuiltInFonts.Cooper_Black;
			BuiltInFonts.m_fontMap["Copperplate Gothic Bold"] = BuiltInFonts.Copperplate_Gothic_Bold;
			BuiltInFonts.m_fontMap["Copperplate Gothic Light"] = BuiltInFonts.Copperplate_Gothic_Light;
			BuiltInFonts.m_fontMap["Courier New"] = BuiltInFonts.Courier_New;
			BuiltInFonts.m_fontMap["Curlz MT"] = BuiltInFonts.Curlz_MT;
			BuiltInFonts.m_fontMap["Edwardian Script ITC"] = BuiltInFonts.Edwardian_Script_ITC;
			BuiltInFonts.m_fontMap["Elephant"] = BuiltInFonts.Elephant;
			BuiltInFonts.m_fontMap["Engravers MT"] = BuiltInFonts.Engravers_MT;
			BuiltInFonts.m_fontMap["Eras Bold ITC"] = BuiltInFonts.Eras_Bold_ITC;
			BuiltInFonts.m_fontMap["Eras Demi ITC"] = BuiltInFonts.Eras_Demi_ITC;
			BuiltInFonts.m_fontMap["Eras Light ITC"] = BuiltInFonts.Eras_Light_ITC;
			BuiltInFonts.m_fontMap["Eras Medium ITC"] = BuiltInFonts.Eras_Medium_ITC;
			BuiltInFonts.m_fontMap["Estrangelo Edessa"] = BuiltInFonts.Estrangelo_Edessa;
			BuiltInFonts.m_fontMap["Felix Titling"] = BuiltInFonts.Felix_Titling;
			BuiltInFonts.m_fontMap["Footlight MT Light"] = BuiltInFonts.Footlight_MT_Light;
			BuiltInFonts.m_fontMap["Forte"] = BuiltInFonts.Forte;
			BuiltInFonts.m_fontMap["Franklin Gothic Book"] = BuiltInFonts.Franklin_Gothic_Book;
			BuiltInFonts.m_fontMap["Franklin Gothic Demi"] = BuiltInFonts.Franklin_Gothic_Demi;
			BuiltInFonts.m_fontMap["Franklin Gothic Demi Cond"] = BuiltInFonts.Franklin_Gothic_Demi_Cond;
			BuiltInFonts.m_fontMap["Franklin Gothic Heavy"] = BuiltInFonts.Franklin_Gothic_Heavy;
			BuiltInFonts.m_fontMap["Franklin Gothic Medium"] = BuiltInFonts.Franklin_Gothic_Medium;
			BuiltInFonts.m_fontMap["Franklin Gothic Medium Cond"] = BuiltInFonts.Franklin_Gothic_Medium_Cond;
			BuiltInFonts.m_fontMap["Freestyle Script"] = BuiltInFonts.Freestyle_Script;
			BuiltInFonts.m_fontMap["French Script MT"] = BuiltInFonts.French_Script_MT;
			BuiltInFonts.m_fontMap["Garamond"] = BuiltInFonts.Garamond;
			BuiltInFonts.m_fontMap["Gautami"] = BuiltInFonts.Gautami;
			BuiltInFonts.m_fontMap["Georgia"] = BuiltInFonts.Georgia;
			BuiltInFonts.m_fontMap["Gigi"] = BuiltInFonts.Gigi;
			BuiltInFonts.m_fontMap["Gill Sans MT"] = BuiltInFonts.Gill_Sans_MT;
			BuiltInFonts.m_fontMap["Gill Sans MT Condensed"] = BuiltInFonts.Gill_Sans_MT_Condensed;
			BuiltInFonts.m_fontMap["Gill Sans MT Ext Condensed Bold"] = BuiltInFonts.Gill_Sans_MT_Ext_Condensed_Bold;
			BuiltInFonts.m_fontMap["Gill Sans Ultra Bold"] = BuiltInFonts.Gill_Sans_Ultra_Bold;
			BuiltInFonts.m_fontMap["Gill Sans Ultra Bold Condensed"] = BuiltInFonts.Gill_Sans_Ultra_Bold_Condensed;
			BuiltInFonts.m_fontMap["Gloucester MT Extra Condensed"] = BuiltInFonts.Gloucester_MT_Extra_Condensed;
			BuiltInFonts.m_fontMap["Goudy Stout"] = BuiltInFonts.Goudy_Stout;
			BuiltInFonts.m_fontMap["Haettenschweiler"] = BuiltInFonts.Haettenschweiler;
			BuiltInFonts.m_fontMap["Harlow Solid Italic"] = BuiltInFonts.Harlow_Solid_Italic;
			BuiltInFonts.m_fontMap["Harrington"] = BuiltInFonts.Harrington;
			BuiltInFonts.m_fontMap["High Tower Text"] = BuiltInFonts.High_Tower_Text;
			BuiltInFonts.m_fontMap["Impact"] = BuiltInFonts.Impact;
			BuiltInFonts.m_fontMap["Imprint MT Shadow"] = BuiltInFonts.Imprint_MT_Shadow;
			BuiltInFonts.m_fontMap["Informal Roman"] = BuiltInFonts.Informal_Roman;
			BuiltInFonts.m_fontMap["Jokerman"] = BuiltInFonts.Jokerman;
			BuiltInFonts.m_fontMap["Juice ITC"] = BuiltInFonts.Juice_ITC;
			BuiltInFonts.m_fontMap["Kristen ITC"] = BuiltInFonts.Kristen_ITC;
			BuiltInFonts.m_fontMap["Kunstler Script"] = BuiltInFonts.Kunstler_Script;
			BuiltInFonts.m_fontMap["Latha"] = BuiltInFonts.Latha;
			BuiltInFonts.m_fontMap["Lucida Bright"] = BuiltInFonts.Lucida_Bright;
			BuiltInFonts.m_fontMap["Lucida Calligraphy"] = BuiltInFonts.Lucida_Calligraphy;
			BuiltInFonts.m_fontMap["Lucida Console"] = BuiltInFonts.Lucida_Console;
			BuiltInFonts.m_fontMap["Lucida Fax"] = BuiltInFonts.Lucida_Fax;
			BuiltInFonts.m_fontMap["Onyx"] = BuiltInFonts.Onyx;
			BuiltInFonts.m_fontMap["Lucida Handwriting"] = BuiltInFonts.Lucida_Handwriting;
			BuiltInFonts.m_fontMap["Lucida Sans"] = BuiltInFonts.Lucida_Sans;
			BuiltInFonts.m_fontMap["Lucida Sans Typewriter"] = BuiltInFonts.Lucida_Sans_Typewriter;
			BuiltInFonts.m_fontMap["Lucida Sans Unicode"] = BuiltInFonts.Lucida_Sans_Unicode;
			BuiltInFonts.m_fontMap["Magneto"] = BuiltInFonts.Magneto;
			BuiltInFonts.m_fontMap["Maiandra GD"] = BuiltInFonts.Maiandra_GD;
			BuiltInFonts.m_fontMap["Mangal"] = BuiltInFonts.Mangal;
			BuiltInFonts.m_fontMap["Matura MT Script Capitals"] = BuiltInFonts.Matura_MT_Script_Capitals;
			BuiltInFonts.m_fontMap["Microsoft Sans Serif"] = BuiltInFonts.Microsoft_Sans_Serif;
			BuiltInFonts.m_fontMap["Mistral"] = BuiltInFonts.Mistral;
			BuiltInFonts.m_fontMap["Modern No. 20"] = BuiltInFonts.Modern_No__20;
			BuiltInFonts.m_fontMap["Monotype Corsiva"] = BuiltInFonts.Monotype_Corsiva;
			BuiltInFonts.m_fontMap["MS Mincho"] = BuiltInFonts.MS_Mincho;
			BuiltInFonts.m_fontMap["MS Reference Sans Serif"] = BuiltInFonts.MS_Reference_Sans_Serif;
			BuiltInFonts.m_fontMap["MV Boli"] = BuiltInFonts.MV_Boli;
			BuiltInFonts.m_fontMap["Niagara Engraved"] = BuiltInFonts.Niagara_Engraved;
			BuiltInFonts.m_fontMap["Niagara Solid"] = BuiltInFonts.Niagara_Solid;
			BuiltInFonts.m_fontMap["OCR A Extended"] = BuiltInFonts.OCR_A_Extended;
			BuiltInFonts.m_fontMap["Old English Text MT"] = BuiltInFonts.Old_English_Text_MT;
			BuiltInFonts.m_fontMap["Palace Script MT"] = BuiltInFonts.Palace_Script_MT;
			BuiltInFonts.m_fontMap["Palatino Linotype"] = BuiltInFonts.Palatino_Linotype;
			BuiltInFonts.m_fontMap["Papyrus"] = BuiltInFonts.Papyrus;
			BuiltInFonts.m_fontMap["Parchment"] = BuiltInFonts.Parchment;
			BuiltInFonts.m_fontMap["Perpetua"] = BuiltInFonts.Perpetua;
			BuiltInFonts.m_fontMap["Perpetua Titling MT"] = BuiltInFonts.Perpetua_Titling_MT;
			BuiltInFonts.m_fontMap["Playbill"] = BuiltInFonts.Playbill;
			BuiltInFonts.m_fontMap["Poor Richard"] = BuiltInFonts.Poor_Richard;
			BuiltInFonts.m_fontMap["Pristina"] = BuiltInFonts.Pristina;
			BuiltInFonts.m_fontMap["Raavi"] = BuiltInFonts.Raavi;
			BuiltInFonts.m_fontMap["Rage Italic"] = BuiltInFonts.Rage_Italic;
			BuiltInFonts.m_fontMap["Ravie"] = BuiltInFonts.Ravie;
			BuiltInFonts.m_fontMap["Rockwell"] = BuiltInFonts.Rockwell;
			BuiltInFonts.m_fontMap["Rockwell Condensed"] = BuiltInFonts.Rockwell_Condensed;
			BuiltInFonts.m_fontMap["Rockwell Extra Bold"] = BuiltInFonts.Rockwell_Extra_Bold;
			BuiltInFonts.m_fontMap["Script MT Bold"] = BuiltInFonts.Script_MT_Bold;
			BuiltInFonts.m_fontMap["Showcard Gothic"] = BuiltInFonts.Showcard_Gothic;
			BuiltInFonts.m_fontMap["Shruti"] = BuiltInFonts.Shruti;
			BuiltInFonts.m_fontMap["SimSun"] = BuiltInFonts.SimSun;
			BuiltInFonts.m_fontMap["Snap ITC"] = BuiltInFonts.Snap_ITC;
			BuiltInFonts.m_fontMap["Stencil"] = BuiltInFonts.Stencil;
			BuiltInFonts.m_fontMap["Sylfaen"] = BuiltInFonts.Sylfaen;
			BuiltInFonts.m_fontMap["Tahoma"] = BuiltInFonts.Tahoma;
			BuiltInFonts.m_fontMap["Tempus Sans ITC"] = BuiltInFonts.Tempus_Sans_ITC;
			BuiltInFonts.m_fontMap["Trebuchet MS"] = BuiltInFonts.Trebuchet_MS;
			BuiltInFonts.m_fontMap["Tunga"] = BuiltInFonts.Tunga;
			BuiltInFonts.m_fontMap["Tw Cen MT"] = BuiltInFonts.Tw_Cen_MT;
			BuiltInFonts.m_fontMap["Tw Cen MT Condensed"] = BuiltInFonts.Tw_Cen_MT_Condensed;
			BuiltInFonts.m_fontMap["Tw Cen MT Condensed Extra Bold"] = BuiltInFonts.Tw_Cen_MT_Condensed_Extra_Bold;
			BuiltInFonts.m_fontMap["Verdana"] = BuiltInFonts.Verdana;
			BuiltInFonts.m_fontMap["Viner Hand ITC"] = BuiltInFonts.Viner_Hand_ITC;
			BuiltInFonts.m_fontMap["Vivaldi"] = BuiltInFonts.Vivaldi;
			BuiltInFonts.m_fontMap["Vladimir Script"] = BuiltInFonts.Vladimir_Script;
			BuiltInFonts.m_fontMap["Wide Latin"] = BuiltInFonts.Wide_Latin;
		}
	}
}
