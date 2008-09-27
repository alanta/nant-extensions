using System;
using System.Text;

using NAnt.Core;

using NAntExtensions.TeamCity.Common.BuildEnvironment;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	internal class DefaultTeamCityLogWriter : TeamCityLogWriter
	{
		static UnicodeEncoding UnicodeEncoding;
		readonly StringBuilder _builder = new StringBuilder();
		IBuildEnvironment _buildEnvironment;
		bool _isOpen;
		Task _task;

		public DefaultTeamCityLogWriter(IBuildEnvironment buildEnvironment)
		{
			BuildEnvironment = buildEnvironment;

			_isOpen = true;
		}

		public override Task Task
		{
			get { return _task; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_task = value;
			}
		}

		bool UseTaskLogger
		{
			get { return BuildEnvironment.IsRunningWithTeamCityNAntRunner; }
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

		IBuildEnvironment BuildEnvironment
		{
			get { return _buildEnvironment; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_buildEnvironment = value;
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
				if (UseTaskLogger)
				{
					Task.Log(Level.Info, _builder.ToString());
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