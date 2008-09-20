using System;
using System.Collections.Generic;
using System.Text;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	internal class Formatter
	{
		internal static void FormatException(Exception exception, StringBuilder builder)
		{
			if (exception == null)
			{
				return;
			}

			if (builder.Length > 0)
			{
				builder.AppendLine("--------------------------------");
			}

			builder.AppendFormat("{0}: {1}", exception.GetType(), exception.Message);
			builder.AppendLine();

			if (!String.IsNullOrEmpty(exception.Source))
			{
				builder.AppendFormat(" Source: {0}", exception.Source);
			}
			builder.AppendLine();

			builder.AppendFormat(" Stack: {0}", exception.StackTrace);
			builder.AppendLine();

			if (exception.InnerException != null)
			{
				FormatException(exception.InnerException, builder);
			}
		}

		internal static string FormatValue(string value)
		{
			StringBuilder builder = new StringBuilder(value);
			FormatValue(builder);

			return builder.ToString();
		}

		internal static object[] FormatValues(object[] values)
		{
			List<object> result = new List<object>();

			StringBuilder builder = new StringBuilder();

			foreach (object value in values)
			{
				builder.Length = 0;
				builder.Append(value);
				FormatValue(builder);

				result.Add(builder.ToString());
			}

			return result.ToArray();
		}

		internal static void FormatValue(StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}

			builder.Replace("|", "||");
			builder.Replace("'", "|'");
			builder.Replace("\n", "|n");
			builder.Replace("\r", "|r");
			builder.Replace("]", "|]");
		}
	}
}