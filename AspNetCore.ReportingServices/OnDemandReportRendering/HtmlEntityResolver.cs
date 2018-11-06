using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class HtmlEntityResolver
	{
		private static Dictionary<string, char> m_entityLookupTable;

		static HtmlEntityResolver()
		{
			HtmlEntityResolver.m_entityLookupTable = new Dictionary<string, char>(StringEqualityComparer.Instance);
			HtmlEntityResolver.m_entityLookupTable.Add("&quot;", '"');
			HtmlEntityResolver.m_entityLookupTable.Add("&QUOT;", '"');
			HtmlEntityResolver.m_entityLookupTable.Add("&#34;", '"');
			HtmlEntityResolver.m_entityLookupTable.Add("&apos;", '\'');
			HtmlEntityResolver.m_entityLookupTable.Add("&APOS;", '\'');
			HtmlEntityResolver.m_entityLookupTable.Add("&#39;", '\'');
			HtmlEntityResolver.m_entityLookupTable.Add("&amp;", '&');
			HtmlEntityResolver.m_entityLookupTable.Add("&AMP;", '&');
			HtmlEntityResolver.m_entityLookupTable.Add("&#38;", '&');
			HtmlEntityResolver.m_entityLookupTable.Add("&lt;", '<');
			HtmlEntityResolver.m_entityLookupTable.Add("&LT;", '<');
			HtmlEntityResolver.m_entityLookupTable.Add("&#60;", '<');
			HtmlEntityResolver.m_entityLookupTable.Add("&gt;", '>');
			HtmlEntityResolver.m_entityLookupTable.Add("&GT;", '>');
			HtmlEntityResolver.m_entityLookupTable.Add("&#62;", '>');
			HtmlEntityResolver.m_entityLookupTable.Add("&nbsp;", '\u00a0');
			HtmlEntityResolver.m_entityLookupTable.Add("&#160;", '\u00a0');
			HtmlEntityResolver.m_entityLookupTable.Add("&iexcl;", '¡');
			HtmlEntityResolver.m_entityLookupTable.Add("&#161;", '¡');
			HtmlEntityResolver.m_entityLookupTable.Add("&cent;", '¢');
			HtmlEntityResolver.m_entityLookupTable.Add("&#162;", '¢');
			HtmlEntityResolver.m_entityLookupTable.Add("&pound;", '£');
			HtmlEntityResolver.m_entityLookupTable.Add("&#163;", '£');
			HtmlEntityResolver.m_entityLookupTable.Add("&curren;", '¤');
			HtmlEntityResolver.m_entityLookupTable.Add("&#164;", '¤');
			HtmlEntityResolver.m_entityLookupTable.Add("&yen;", '¥');
			HtmlEntityResolver.m_entityLookupTable.Add("&#165;", '¥');
			HtmlEntityResolver.m_entityLookupTable.Add("&brvbar;", '¦');
			HtmlEntityResolver.m_entityLookupTable.Add("&#166;", '¦');
			HtmlEntityResolver.m_entityLookupTable.Add("&sect;", '§');
			HtmlEntityResolver.m_entityLookupTable.Add("&#167;", '§');
			HtmlEntityResolver.m_entityLookupTable.Add("&uml;", '\u00a8');
			HtmlEntityResolver.m_entityLookupTable.Add("&#168;", '\u00a8');
			HtmlEntityResolver.m_entityLookupTable.Add("&copy;", '©');
			HtmlEntityResolver.m_entityLookupTable.Add("&COPY;", '©');
			HtmlEntityResolver.m_entityLookupTable.Add("&#169;", '©');
			HtmlEntityResolver.m_entityLookupTable.Add("&ordf;", 'ª');
			HtmlEntityResolver.m_entityLookupTable.Add("&#170;", 'ª');
			HtmlEntityResolver.m_entityLookupTable.Add("&laquo;", '«');
			HtmlEntityResolver.m_entityLookupTable.Add("&#171;", '«');
			HtmlEntityResolver.m_entityLookupTable.Add("&not;", '¬');
			HtmlEntityResolver.m_entityLookupTable.Add("&#172;", '¬');
			HtmlEntityResolver.m_entityLookupTable.Add("&shy;", '­');
			HtmlEntityResolver.m_entityLookupTable.Add("&#173;", '­');
			HtmlEntityResolver.m_entityLookupTable.Add("&reg;", '®');
			HtmlEntityResolver.m_entityLookupTable.Add("&REG;", '®');
			HtmlEntityResolver.m_entityLookupTable.Add("&#174;", '®');
			HtmlEntityResolver.m_entityLookupTable.Add("&macr;", '\u00af');
			HtmlEntityResolver.m_entityLookupTable.Add("&#175;", '\u00af');
			HtmlEntityResolver.m_entityLookupTable.Add("&deg;", '°');
			HtmlEntityResolver.m_entityLookupTable.Add("&#176;", '°');
			HtmlEntityResolver.m_entityLookupTable.Add("&plusmn;", '±');
			HtmlEntityResolver.m_entityLookupTable.Add("&#177;", '±');
			HtmlEntityResolver.m_entityLookupTable.Add("&sup2;", '²');
			HtmlEntityResolver.m_entityLookupTable.Add("&#178;", '²');
			HtmlEntityResolver.m_entityLookupTable.Add("&sup3;", '³');
			HtmlEntityResolver.m_entityLookupTable.Add("&#179;", '³');
			HtmlEntityResolver.m_entityLookupTable.Add("&acute;", '\u00b4');
			HtmlEntityResolver.m_entityLookupTable.Add("&#180;", '\u00b4');
			HtmlEntityResolver.m_entityLookupTable.Add("&micro;", 'µ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#181;", 'µ');
			HtmlEntityResolver.m_entityLookupTable.Add("&para;", '¶');
			HtmlEntityResolver.m_entityLookupTable.Add("&#182;", '¶');
			HtmlEntityResolver.m_entityLookupTable.Add("&middot;", '·');
			HtmlEntityResolver.m_entityLookupTable.Add("&#183;", '·');
			HtmlEntityResolver.m_entityLookupTable.Add("&cedil;", '\u00b8');
			HtmlEntityResolver.m_entityLookupTable.Add("&#184;", '\u00b8');
			HtmlEntityResolver.m_entityLookupTable.Add("&sup1;", '¹');
			HtmlEntityResolver.m_entityLookupTable.Add("&#185;", '¹');
			HtmlEntityResolver.m_entityLookupTable.Add("&ordm;", 'º');
			HtmlEntityResolver.m_entityLookupTable.Add("&#186;", 'º');
			HtmlEntityResolver.m_entityLookupTable.Add("&raquo;", '»');
			HtmlEntityResolver.m_entityLookupTable.Add("&#187;", '»');
			HtmlEntityResolver.m_entityLookupTable.Add("&frac14;", '¼');
			HtmlEntityResolver.m_entityLookupTable.Add("&#188;", '¼');
			HtmlEntityResolver.m_entityLookupTable.Add("&frac12;", '½');
			HtmlEntityResolver.m_entityLookupTable.Add("&#189;", '½');
			HtmlEntityResolver.m_entityLookupTable.Add("&frac34;", '¾');
			HtmlEntityResolver.m_entityLookupTable.Add("&#190;", '¾');
			HtmlEntityResolver.m_entityLookupTable.Add("&iquest;", '¿');
			HtmlEntityResolver.m_entityLookupTable.Add("&#191;", '¿');
			HtmlEntityResolver.m_entityLookupTable.Add("&Agrave;", 'À');
			HtmlEntityResolver.m_entityLookupTable.Add("&#192;", 'À');
			HtmlEntityResolver.m_entityLookupTable.Add("&Aacute;", 'Á');
			HtmlEntityResolver.m_entityLookupTable.Add("&#193;", 'Á');
			HtmlEntityResolver.m_entityLookupTable.Add("&Acirc;", 'Â');
			HtmlEntityResolver.m_entityLookupTable.Add("&#194;", 'Â');
			HtmlEntityResolver.m_entityLookupTable.Add("&Atilde;", 'Ã');
			HtmlEntityResolver.m_entityLookupTable.Add("&#195;", 'Ã');
			HtmlEntityResolver.m_entityLookupTable.Add("&Auml;", 'Ä');
			HtmlEntityResolver.m_entityLookupTable.Add("&#196;", 'Ä');
			HtmlEntityResolver.m_entityLookupTable.Add("&Aring;", 'Å');
			HtmlEntityResolver.m_entityLookupTable.Add("&#197;", 'Å');
			HtmlEntityResolver.m_entityLookupTable.Add("&AElig;", 'Æ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#198;", 'Æ');
			HtmlEntityResolver.m_entityLookupTable.Add("&Ccedil;", 'Ç');
			HtmlEntityResolver.m_entityLookupTable.Add("&#199;", 'Ç');
			HtmlEntityResolver.m_entityLookupTable.Add("&Egrave;", 'È');
			HtmlEntityResolver.m_entityLookupTable.Add("&#200;", 'È');
			HtmlEntityResolver.m_entityLookupTable.Add("&Eacute;", 'É');
			HtmlEntityResolver.m_entityLookupTable.Add("&#201;", 'É');
			HtmlEntityResolver.m_entityLookupTable.Add("&Ecirc;", 'Ê');
			HtmlEntityResolver.m_entityLookupTable.Add("&#202;", 'Ê');
			HtmlEntityResolver.m_entityLookupTable.Add("&Euml;", 'Ë');
			HtmlEntityResolver.m_entityLookupTable.Add("&#203;", 'Ë');
			HtmlEntityResolver.m_entityLookupTable.Add("&Igrave;", 'Ì');
			HtmlEntityResolver.m_entityLookupTable.Add("&#204;", 'Ì');
			HtmlEntityResolver.m_entityLookupTable.Add("&Iacute;", 'Í');
			HtmlEntityResolver.m_entityLookupTable.Add("&#205;", 'Í');
			HtmlEntityResolver.m_entityLookupTable.Add("&Icirc;", 'Î');
			HtmlEntityResolver.m_entityLookupTable.Add("&#206;", 'Î');
			HtmlEntityResolver.m_entityLookupTable.Add("&Iuml;", 'Ï');
			HtmlEntityResolver.m_entityLookupTable.Add("&#207;", 'Ï');
			HtmlEntityResolver.m_entityLookupTable.Add("&ETH;", 'Ð');
			HtmlEntityResolver.m_entityLookupTable.Add("&#208;", 'Ð');
			HtmlEntityResolver.m_entityLookupTable.Add("&Ntilde;", 'Ñ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#209;", 'Ñ');
			HtmlEntityResolver.m_entityLookupTable.Add("&Ograve;", 'Ò');
			HtmlEntityResolver.m_entityLookupTable.Add("&#210;", 'Ò');
			HtmlEntityResolver.m_entityLookupTable.Add("&Oacute;", 'Ó');
			HtmlEntityResolver.m_entityLookupTable.Add("&#211;", 'Ó');
			HtmlEntityResolver.m_entityLookupTable.Add("&Ocirc;", 'Ô');
			HtmlEntityResolver.m_entityLookupTable.Add("&#212;", 'Ô');
			HtmlEntityResolver.m_entityLookupTable.Add("&Otilde;", 'Õ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#213;", 'Õ');
			HtmlEntityResolver.m_entityLookupTable.Add("&Ouml;", 'Ö');
			HtmlEntityResolver.m_entityLookupTable.Add("&#214;", 'Ö');
			HtmlEntityResolver.m_entityLookupTable.Add("&times;", '×');
			HtmlEntityResolver.m_entityLookupTable.Add("&#215;", '×');
			HtmlEntityResolver.m_entityLookupTable.Add("&Oslash;", 'Ø');
			HtmlEntityResolver.m_entityLookupTable.Add("&#216;", 'Ø');
			HtmlEntityResolver.m_entityLookupTable.Add("&Ugrave;", 'Ù');
			HtmlEntityResolver.m_entityLookupTable.Add("&#217;", 'Ù');
			HtmlEntityResolver.m_entityLookupTable.Add("&Uacute;", 'Ú');
			HtmlEntityResolver.m_entityLookupTable.Add("&#218;", 'Ú');
			HtmlEntityResolver.m_entityLookupTable.Add("&Ucirc;", 'Û');
			HtmlEntityResolver.m_entityLookupTable.Add("&#219;", 'Û');
			HtmlEntityResolver.m_entityLookupTable.Add("&Uuml;", 'Ü');
			HtmlEntityResolver.m_entityLookupTable.Add("&#220;", 'Ü');
			HtmlEntityResolver.m_entityLookupTable.Add("&Yacute;", 'Ý');
			HtmlEntityResolver.m_entityLookupTable.Add("&#221;", 'Ý');
			HtmlEntityResolver.m_entityLookupTable.Add("&THORN;", 'Þ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#222;", 'Þ');
			HtmlEntityResolver.m_entityLookupTable.Add("&szlig;", 'ß');
			HtmlEntityResolver.m_entityLookupTable.Add("&#223;", 'ß');
			HtmlEntityResolver.m_entityLookupTable.Add("&agrave;", 'à');
			HtmlEntityResolver.m_entityLookupTable.Add("&#224;", 'à');
			HtmlEntityResolver.m_entityLookupTable.Add("&aacute;", 'á');
			HtmlEntityResolver.m_entityLookupTable.Add("&#225;", 'á');
			HtmlEntityResolver.m_entityLookupTable.Add("&acirc;", 'â');
			HtmlEntityResolver.m_entityLookupTable.Add("&#226;", 'â');
			HtmlEntityResolver.m_entityLookupTable.Add("&atilde;", 'ã');
			HtmlEntityResolver.m_entityLookupTable.Add("&#227;", 'ã');
			HtmlEntityResolver.m_entityLookupTable.Add("&auml;", 'ä');
			HtmlEntityResolver.m_entityLookupTable.Add("&#228;", 'ä');
			HtmlEntityResolver.m_entityLookupTable.Add("&aring;", 'å');
			HtmlEntityResolver.m_entityLookupTable.Add("&#229;", 'å');
			HtmlEntityResolver.m_entityLookupTable.Add("&aelig;", 'æ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#230;", 'æ');
			HtmlEntityResolver.m_entityLookupTable.Add("&ccedil;", 'ç');
			HtmlEntityResolver.m_entityLookupTable.Add("&#231;", 'ç');
			HtmlEntityResolver.m_entityLookupTable.Add("&egrave;", 'è');
			HtmlEntityResolver.m_entityLookupTable.Add("&#232;", 'è');
			HtmlEntityResolver.m_entityLookupTable.Add("&eacute;", 'é');
			HtmlEntityResolver.m_entityLookupTable.Add("&#233;", 'é');
			HtmlEntityResolver.m_entityLookupTable.Add("&ecirc;", 'ê');
			HtmlEntityResolver.m_entityLookupTable.Add("&#234;", 'ê');
			HtmlEntityResolver.m_entityLookupTable.Add("&euml;", 'ë');
			HtmlEntityResolver.m_entityLookupTable.Add("&#235;", 'ë');
			HtmlEntityResolver.m_entityLookupTable.Add("&igrave;", 'ì');
			HtmlEntityResolver.m_entityLookupTable.Add("&#236;", 'ì');
			HtmlEntityResolver.m_entityLookupTable.Add("&iacute;", 'í');
			HtmlEntityResolver.m_entityLookupTable.Add("&#237;", 'í');
			HtmlEntityResolver.m_entityLookupTable.Add("&icirc;", 'î');
			HtmlEntityResolver.m_entityLookupTable.Add("&#238;", 'î');
			HtmlEntityResolver.m_entityLookupTable.Add("&iuml;", 'ï');
			HtmlEntityResolver.m_entityLookupTable.Add("&#239;", 'ï');
			HtmlEntityResolver.m_entityLookupTable.Add("&eth;", 'ð');
			HtmlEntityResolver.m_entityLookupTable.Add("&#240;", 'ð');
			HtmlEntityResolver.m_entityLookupTable.Add("&ntilde;", 'ñ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#241;", 'ñ');
			HtmlEntityResolver.m_entityLookupTable.Add("&ograve;", 'ò');
			HtmlEntityResolver.m_entityLookupTable.Add("&#242;", 'ò');
			HtmlEntityResolver.m_entityLookupTable.Add("&oacute;", 'ó');
			HtmlEntityResolver.m_entityLookupTable.Add("&#243;", 'ó');
			HtmlEntityResolver.m_entityLookupTable.Add("&ocirc;", 'ô');
			HtmlEntityResolver.m_entityLookupTable.Add("&#244;", 'ô');
			HtmlEntityResolver.m_entityLookupTable.Add("&otilde;", 'õ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#245;", 'õ');
			HtmlEntityResolver.m_entityLookupTable.Add("&ouml;", 'ö');
			HtmlEntityResolver.m_entityLookupTable.Add("&#246;", 'ö');
			HtmlEntityResolver.m_entityLookupTable.Add("&divide;", '÷');
			HtmlEntityResolver.m_entityLookupTable.Add("&#247;", '÷');
			HtmlEntityResolver.m_entityLookupTable.Add("&oslash;", 'ø');
			HtmlEntityResolver.m_entityLookupTable.Add("&#248;", 'ø');
			HtmlEntityResolver.m_entityLookupTable.Add("&ugrave;", 'ù');
			HtmlEntityResolver.m_entityLookupTable.Add("&#249;", 'ù');
			HtmlEntityResolver.m_entityLookupTable.Add("&uacute;", 'ú');
			HtmlEntityResolver.m_entityLookupTable.Add("&#250;", 'ú');
			HtmlEntityResolver.m_entityLookupTable.Add("&ucirc;", 'û');
			HtmlEntityResolver.m_entityLookupTable.Add("&#251;", 'û');
			HtmlEntityResolver.m_entityLookupTable.Add("&uuml;", 'ü');
			HtmlEntityResolver.m_entityLookupTable.Add("&#252;", 'ü');
			HtmlEntityResolver.m_entityLookupTable.Add("&yacute;", 'ý');
			HtmlEntityResolver.m_entityLookupTable.Add("&#253;", 'ý');
			HtmlEntityResolver.m_entityLookupTable.Add("&thorn;", 'þ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#254;", 'þ');
			HtmlEntityResolver.m_entityLookupTable.Add("&yuml;", 'ÿ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#255;", 'ÿ');
			HtmlEntityResolver.m_entityLookupTable.Add("&OElig;", 'Œ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#338;", 'Œ');
			HtmlEntityResolver.m_entityLookupTable.Add("&oelig;", 'œ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#339;", 'œ');
			HtmlEntityResolver.m_entityLookupTable.Add("&Scaron;", 'Š');
			HtmlEntityResolver.m_entityLookupTable.Add("&#352;", 'Š');
			HtmlEntityResolver.m_entityLookupTable.Add("&scaron;", 'š');
			HtmlEntityResolver.m_entityLookupTable.Add("&#353;", 'š');
			HtmlEntityResolver.m_entityLookupTable.Add("&hellip;", '…');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8230;", '…');
			HtmlEntityResolver.m_entityLookupTable.Add("&permil;", '‰');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8240;", '‰');
			HtmlEntityResolver.m_entityLookupTable.Add("&lsaquo;", '‹');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8249;", '‹');
			HtmlEntityResolver.m_entityLookupTable.Add("&rsaquo;", '›');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8250;", '›');
			HtmlEntityResolver.m_entityLookupTable.Add("&euro;", '€');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8364;", '€');
			HtmlEntityResolver.m_entityLookupTable.Add("&Yuml;", 'Ÿ');
			HtmlEntityResolver.m_entityLookupTable.Add("&#376;", 'Ÿ');
			HtmlEntityResolver.m_entityLookupTable.Add("&circ;", '\u02c6');
			HtmlEntityResolver.m_entityLookupTable.Add("&#710;", '\u02c6');
			HtmlEntityResolver.m_entityLookupTable.Add("&tilde;", '\u02dc');
			HtmlEntityResolver.m_entityLookupTable.Add("&#732;", '\u02dc');
			HtmlEntityResolver.m_entityLookupTable.Add("&ensp;", '\u2002');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8194;", '\u2002');
			HtmlEntityResolver.m_entityLookupTable.Add("&emsp;", '\u2003');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8195;", '\u2003');
			HtmlEntityResolver.m_entityLookupTable.Add("&thinsp;", '\u2009');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8201;", '\u2009');
			HtmlEntityResolver.m_entityLookupTable.Add("&zwnj;", '\u200c');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8204;", '\u200c');
			HtmlEntityResolver.m_entityLookupTable.Add("&zwj;", '\u200d');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8205;", '\u200d');
			HtmlEntityResolver.m_entityLookupTable.Add("&lrm;", '\u200e');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8206;", '\u200e');
			HtmlEntityResolver.m_entityLookupTable.Add("&rlm;", '\u200f');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8207;", '\u200f');
			HtmlEntityResolver.m_entityLookupTable.Add("&ndash;", '–');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8211;", '–');
			HtmlEntityResolver.m_entityLookupTable.Add("&mdash;", '—');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8212;", '—');
			HtmlEntityResolver.m_entityLookupTable.Add("&lsquo;", '‘');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8216;", '‘');
			HtmlEntityResolver.m_entityLookupTable.Add("&rsquo;", '’');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8217;", '’');
			HtmlEntityResolver.m_entityLookupTable.Add("&sbquo;", '‚');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8218;", '‚');
			HtmlEntityResolver.m_entityLookupTable.Add("&ldquo;", '“');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8220;", '“');
			HtmlEntityResolver.m_entityLookupTable.Add("&rdquo;", '”');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8221;", '”');
			HtmlEntityResolver.m_entityLookupTable.Add("&bdquo;", '„');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8222;", '„');
			HtmlEntityResolver.m_entityLookupTable.Add("&dagger;", '†');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8224;", '†');
			HtmlEntityResolver.m_entityLookupTable.Add("&Dagger;", '‡');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8225;", '‡');
			HtmlEntityResolver.m_entityLookupTable.Add("&trade;", '™');
			HtmlEntityResolver.m_entityLookupTable.Add("&TRADE;", '™');
			HtmlEntityResolver.m_entityLookupTable.Add("&#8482;", '™');
		}

		internal static string ResolveEntities(string html)
		{
			StringBuilder stringBuilder = new StringBuilder(html);
			HtmlEntityResolver.ResolveEntities(stringBuilder);
			return stringBuilder.ToString();
		}

		internal static void ResolveEntities(StringBuilder sb)
		{
			for (int i = 0; i < sb.Length; i++)
			{
				string key = default(string);
				char c = default(char);
				string text = default(string);
				if (sb[i] == '&' && HtmlEntityResolver.GetEntity(sb, i, out text, out key) && HtmlEntityResolver.m_entityLookupTable.TryGetValue(key, out c))
				{
					sb.Replace(text, c.ToString(CultureInfo.InvariantCulture), i, text.Length);
				}
			}
		}

		private static bool GetEntity(StringBuilder sb, int index, out string entity, out string entityName)
		{
			entity = null;
			entityName = null;
			for (int i = index + 1; i < sb.Length; i++)
			{
				char c = sb[i];
				switch (c)
				{
				case ' ':
				case '&':
					i--;
					goto case ';';
				case ';':
				{
					int num = i - index + 1;
					if (num >= 2)
					{
						entity = sb.ToString(index, num);
						if (c != ';')
						{
							entityName = entityName + entity + ";";
						}
						else
						{
							entityName = entity;
						}
						return true;
					}
					return false;
				}
				}
			}
			return false;
		}

		internal static string ResolveEntity(string entity)
		{
			char c = default(char);
			if (HtmlEntityResolver.m_entityLookupTable.TryGetValue(entity, out c))
			{
				return new string(c, 1);
			}
			int num = default(int);
			if (entity.Length > 3 && entity[1] == '#' && int.TryParse(entity.Substring(2, entity.Length - 3), out num))
			{
				return new string((char)(ushort)num, 1);
			}
			return entity;
		}
	}
}
