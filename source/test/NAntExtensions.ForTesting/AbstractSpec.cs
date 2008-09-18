using System;

using Rhino.Mocks;

namespace NAntExtensions.ForTesting
{
	public abstract class AbstractSpec
	{
		protected MockRepository _mocks = new MockRepository();

		protected MockRepository Mocks
		{
			get
			{
				if (StaticMocks != null)
				{
					return StaticMocks;
				}
				return _mocks;
			}
		}

		/// <summary>
		/// Gets the static mocks.
		/// </summary>
		/// <value>The static mocks.</value>
		/// <remarks>Override this member if you want to use CombinatorialTests with factory
		/// methods that created instances of systems under test with mocked dependencies.</remarks>
		protected virtual MockRepository StaticMocks
		{
			get { return null; }
		}

		protected IDisposable Record
		{
			get { return Mocks.Unordered(); }
		}

		protected IDisposable RecordWithOrder
		{
			get { return Mocks.Ordered(); }
		}

		public virtual void SetUp()
		{
			_mocks = new MockRepository();
		}

		public virtual void TearDown()
		{
			try
			{
				Mocks.ReplayAll();
				Mocks.VerifyAll();
			}
			finally
			{
				if (StaticMocks != null)
				{
					Mocks.BackToRecordAll();
				}
			}
		}
	}
}