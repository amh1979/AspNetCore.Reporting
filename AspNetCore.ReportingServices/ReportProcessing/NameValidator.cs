using System.Collections;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal abstract class NameValidator
	{
		private const string m_identifierStart = "\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}";

		private const string m_identifierExtend = "\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}";

		private static Regex m_clsIdentifierRegex = new Regex("^[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		private Hashtable m_dictionary = new Hashtable();

		protected static bool IsCLSCompliant(string name)
		{
			Match match = NameValidator.m_clsIdentifierRegex.Match(name);
			return match.Success;
		}

		protected bool IsUnique(string name)
		{
			if (this.m_dictionary.ContainsKey(name))
			{
				return false;
			}
			this.m_dictionary.Add(name, null);
			return true;
		}
	}
}
