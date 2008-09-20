using System;

using Machine.Specifications.Model;

namespace NAntExtensions.Machine.Specifications.RunListeners
{
	internal abstract class RunListener
	{
		protected Context _currentContext;

		public virtual void OnContextStart(Context context)
		{
			_currentContext = context;
		}

		public virtual void OnContextEnd(Context context)
		{
			_currentContext = null;
		}

		internal static string GetContextSpecName(Context context, Specification specification)
		{
			return String.Format("{0} {1}", context.FullName, specification.Name);
		}

		internal static string GetContextSpecNameWithFormat(Context context, Specification specification)
		{
			return String.Format("{0}{1}    \x00bb {2}", context.FullName, Environment.NewLine, specification.Name);
		}
	}
}