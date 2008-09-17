using System;
using System.Globalization;
using System.IO;
using System.Text;

using NAnt.Core;

namespace Alanta.MbUnit.Tasks.TeamCity
{
	public class LogWriter : TextWriter
	{
		static UnicodeEncoding _encoding;
		readonly StringBuilder _sb = new StringBuilder();
		readonly Task _task;
		readonly bool _useTaskLogger;
		bool _isOpen;

		public LogWriter(Task task, bool useTaskLogger) : base(CultureInfo.InvariantCulture)
		{
			if (null == task)
			{
				throw new ArgumentNullException("task");
			}

			_isOpen = true;
			_useTaskLogger = useTaskLogger;
			_task = task;
		}

		public override Encoding Encoding
		{
			get
			{
				if (_encoding == null)
				{
					_encoding = new UnicodeEncoding(false, false);
				}
				return _encoding;
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
			_sb.Append(value);
		}

		public override void Write(string value)
		{
			if (!_isOpen)
			{
				throw new ObjectDisposedException(null, "Writer is closed.");
			}
			if (value != null)
			{
				_sb.Append(value);
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
			_sb.Append(buffer, index, count);
		}

		public override void Flush()
		{
			if (!_isOpen)
			{
				throw new ObjectDisposedException(null, "Writer is closed.");
			}

			if (_sb.Length > 0)
			{
				if (_useTaskLogger)
				{
					_task.Log(Level.Info, _sb.ToString());
				}
				else
				{
					Console.WriteLine(_sb.ToString());
				}

				_sb.Length = 0;
			}
		}
	}
}