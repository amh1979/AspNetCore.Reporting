/* ===============================================
* 功能描述：AspNetCore.Reporting.Check
* 创 建 者：WeiGe
* 创建日期：8/20/2018 3:43:46 PM
* ===============================================*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;


namespace AspNetCore.Reporting
{
    internal class Logger
    {
        static readonly string LogPath;
        static Logger()
        {
            LogPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
        }
        public static void Info(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"######### {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} ###########");
            sb.AppendLine(message);
            sb.AppendLine();
            File.AppendAllText(Path.Combine(LogPath, $"{DateTime.Now.ToString("yyyyMMdd")}.txt"), sb.ToString());
        }
    }
    /// <summary>
    /// Check
    /// </summary>
    internal sealed class Check
    {
        /// <summary>
        /// NotNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static T NotNull<T>(T value, string parameterName)
        {
            if (value == null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// NotNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T NotNull<T>(
             T value,
            string parameterName,
             string propertyName)
        {
            if (value == null)
            {
                NotEmpty(parameterName, nameof(parameterName));
                NotEmpty(propertyName, nameof(propertyName));

                throw new ArgumentException($"The property '{propertyName}' of the argument '{parameterName}' cannot be null.");
            }

            return value;
        }

        /// <summary>
        /// NotEmpty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static IReadOnlyList<T> NotEmpty<T>(IReadOnlyList<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException($"The collection argument '{parameterName}' must contain at least one element.");
            }

            return value;
        }

        /// <summary>
        /// NotEmpty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string NotEmpty(string value, string parameterName)
        {
            Exception e = null;
            if (value == null)
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException($"The string argument '{parameterName}' cannot be empty.");
            }

            if (e != null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw e;
            }

            return value;
        }
        /// <summary>
        /// NullButNotEmpty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string NullButNotEmpty(string value, string parameterName)
        {
            if (value != null
                && value.Length == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException($"The string argument '{parameterName}' cannot be empty.");
            }

            return value;
        }
        /// <summary>
        /// HasNoNulls
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static IList<T> HasNoNulls<T>(IList<T> value, string parameterName)
            where T : class
        {
            NotNull(value, parameterName);

            if (value.Any(e => e == null))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(parameterName);
            }

            return value;
        }

        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
        public static void Assert(bool condition, string format, params object[] values)
        {
            if (!condition)
            {
                throw new InvalidOperationException(string.Format(format, values));
            }
        }
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new InvalidOperationException();
            }
        }
    }
}