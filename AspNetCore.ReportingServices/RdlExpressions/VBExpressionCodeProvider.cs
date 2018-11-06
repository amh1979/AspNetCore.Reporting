using Microsoft.VisualBasic;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AspNetCore.ReportingServices.RdlExpressions
{


    internal sealed class VBExpressionCodeProvider : VBCodeProvider
    {
        private const string LineContinuation = " _";

        private const string LineTooLongError = "BC30494";

        private const int MaxLineLength = 65535;

        public override CompilerResults CompileAssemblyFromDom(CompilerParameters options, params CodeCompileUnit[] compilationUnits)
        {
            if (compilationUnits.Length == 1)
            {
                return this.CompileAssemblyFromDomWithRetry(options, compilationUnits[0]);
            }
            return base.CompileAssemblyFromDom(options, compilationUnits);
        }
        public override ICodeCompiler CreateCompiler() {

            return new Microsoft.VisualBasic1.VBCodeGenerator();
            return base.CreateCompiler();
        }
        private CompilerResults CompileAssemblyFromDomWithRetry(CompilerParameters options, CodeCompileUnit compilationUnit)
        {
            options.TempFiles.KeepFiles = true;
            //options.UserToken = System.Security.Principal.WindowsIdentity.GetCurrent().Token;
            //options.GenerateInMemory = true;
            CompilerResults compilerResults;
            try
            {

                if (!System.IO.Directory.Exists(options.TempFiles.TempDir))
                {
                    System.IO.Directory.CreateDirectory(options.TempFiles.TempDir);
                }
                compilerResults = base.CompileAssemblyFromDom(options, compilationUnit);
                if (compilerResults.Errors.HasErrors)
                {
                    options.TempFiles = new TempFileCollection(compilerResults.TempFiles.TempDir);
                    return this.RetryCompile(options, compilerResults.Errors) ?? compilerResults;
                }
                return compilerResults;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private CompilerResults RetryCompile(CompilerParameters options, CompilerErrorCollection compilerErrorCollection)
        {
            CompilerResults result = null;
            string text = null;
            List<int> list = new List<int>();
            foreach (CompilerError item in compilerErrorCollection)
            {
                if (string.Equals(item.ErrorNumber, "BC30494", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(item.Line);
                    if (text == null)
                    {
                        text = item.FileName;
                    }
                }
            }
            if (text != null)
            {
                List<string> list2 = File.ReadAllLines(text).ToList();
                VBExpressionCodeProvider.SplitLongLines(list2, list);
                File.WriteAllLines(text, list2.ToArray());
                options.TempFiles = new TempFileCollection();
                result = base.CompileAssemblyFromFile(options, text);
            }
            return result;
        }

        private static void SplitLongLines(List<string> lines, List<int> longLineNumbers)
        {
            if (longLineNumbers.Count > 0)
            {
                longLineNumbers.Sort();
            }
            for (int num = longLineNumbers.Count - 1; num >= 0; num--)
            {
                int num2 = longLineNumbers[num] - 1;
                string text = lines[num2];
                lines.RemoveAt(num2);
                while (text.Length > 65535)
                {
                    int num3 = VBExpressionCodeProvider.FindSafeSplitPosition(text);
                    if (num3 <= 0)
                    {
                        lines.Insert(num2, text);
                        return;
                    }
                    string item = text.Substring(0, num3) + " _";
                    lines.Insert(num2, item);
                    num2++;
                    text = text.Substring(num3 + 1);
                }
                lines.Insert(num2, text);
            }
        }

        private static int FindSafeSplitPosition(string line)
        {
            int num = line.LastIndexOf(' ', 65535 - " _".Length);
            bool flag = true;
            while (flag && num > 80)
            {
                int num2 = line.LastIndexOf('"', num, 80);
                flag = (num2 != -1);
                if (flag)
                {
                    num = line.LastIndexOf(' ', num2);
                }
            }
            if (flag)
            {
                return -1;
            }
            return num;
        }

    }
}
