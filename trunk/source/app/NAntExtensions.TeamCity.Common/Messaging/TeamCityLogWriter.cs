using System;
using System.Globalization;
using System.IO;
using System.Text;

using NAnt.Core;

using NAntExtensions.TeamCity.Common.Container;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	public class TeamCityLogWriter : TextWriter
	{
		static UnicodeEncoding UnicodeEncoding;
		readonly StringBuilder _builder = new StringBuilder();
		readonly Task _task;
		readonly bool _useTaskLogger;
		bool _isOpen;

		public TeamCityLogWriter(Task task) : base(CultureInfo.InvariantCulture)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			
			_isOpen = true;
			_task = task;
			_useTaskLogger = IoC.Resolve<IBuildEnvironment>().IsRunningWithTeamCityNAntRunner(task);
		}

		public override Encoding Encoding
		{
			get
			{
				if (UnicodeEncoding == null)
				{
					UnicodeEncoding = new UnicodeEncoding(false, false);
				}
				return UnicodeEncoding;
			}
		}

		public override void Close()
		{
			if (_isOpen)
			{
				Flush();
			}
			Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			_isOpen = false;
			base.Dispose(disposing);
		}

		public override void WriteLine()
		{
			Flush();
		}

		public override void WriteLine(string value)
		{
			Write(value);
			Flush();
		}

		public override void Write(char value)
		{
			if (!_isOpen)
			{
				throw new ObjectDisposedException(null, "Writer is closed.");
			}
			_builder.Append(value);
		}

		public override void Write(string value)
		{
			if (!_isOpen)
			{
				throw new ObjectDisposedException(null, "Writer is closed.");
			}
			if (value != null)
			{
				_builder.Append(value);
			}
		}

		public override void Write(char[] buffer, int index, int count)
		{
			if (!_isOpen)
			{
				throw new ObjectDisposedException(null, "Writer is closed.");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((buffer.Length - index) < count)
			{
				throw new ArgumentException("Invalid combination of offset and length.");
			}
			_builder.Append(buffer, index, count);
		}

		public override void Flush()
		{
			if (!_isOpen)
			{
				throw new ObjectDisposedException(null, "Writer is closed.");
			}

			if (_builder.Length > 0)
			{
				if (_useTaskLogger)
				{
					_task.Log(Level.Info, _builder.ToString());
				}
				else
				{
					Console.WriteLine(_builder.ToString());
				}

				_builder.Length = 0;
			}
		}
	}
}