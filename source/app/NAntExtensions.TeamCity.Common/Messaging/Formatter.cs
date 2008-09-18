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
			StringBuilder sb = new StringBuilder(value);
			FormatValue(sb);

			return sb.ToString();
		}
		
		internal static IEnumerable<string> FormatValues(object[] value)
		{
			StringBuilder sb = new StringBuilder();

			foreach (object v in value)
			{
				sb.Length = 0;
				sb.Append(v);
				FormatValue(sb);

				yield return sb.ToString();	
			}
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