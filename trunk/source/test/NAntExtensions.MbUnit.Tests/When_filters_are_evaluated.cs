using MbUnit.Core.Filters;
using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core.Types;

using NAntExtensions.ForTesting;
using NAntExtensions.MbUnit.Tasks;
using NAntExtensions.TeamCity.Common.BuildEnvironment;

namespace NAntExtensions.MbUnit.Tests
{
	public class When_filters_are_evaluated : Spec
	{
		const string ExcludedCategory = "exclude-category";
		const string IncludedAuthor = "include-author";
		const string IncludedCategory = "include-category";

		MbUnitTask _sut;

		protected override void Before_each_spec()
		{
			_sut = new MbUnitTask(Mocks.StrictMock<IBuildEnvironment>());
		}

		[Test]
		public void Should_create_empty_filter_if_no_filters_are_specified()
		{
			IFixtureFilter filter = (IFixtureFilter) Reflector.InvokeMethod(_sut, "CreateFilter");
			Assert.IsInstanceOfType(FixtureFilters.Any.GetType(),
			                        filter,
			                        "When no filters are set up, an instance of FixtureFilters.Any should be returned");
		}

		[Test]
		public void Should_create_author_include_filter()
		{
			Pattern pattern = new Pattern { PatternName = IncludedAuthor };
			_sut.FilterAuthors.Include.Add(pattern);

			IFixtureFilter filter = (IFixtureFilter) Reflector.InvokeMethod(_sut, "CreateFilter");

			Assert.IsFalse(filter.Filter(typeof(NoAttributes)), "The NoAttributes type should be excluded");
			Assert.IsTrue(filter.Filter(typeof(IncludeAuthor)), "The IncludeAuthor type should be included");
		}

		[Test]
		public void Should_create_category_include_filter()
		{
			Pattern pattern = new Pattern { PatternName = IncludedCategory };
			_sut.FilterCategories.Include.Add(pattern);

			IFixtureFilter filter = (IFixtureFilter) Reflector.InvokeMethod(_sut, "CreateFilter");

			Assert.IsFalse(filter.Filter(typeof(NoAttributes)), "The NoAttributes type should be excluded");
			Assert.IsTrue(filter.Filter(typeof(IncludeCategory)), "The IncludeCategory type should be included");
		}

		[Test]
		public void Should_create_category_exclude_filter()
		{
			Pattern pattern = new Pattern { PatternName = ExcludedCategory };
			_sut.FilterCategories.Exclude.Add(pattern);

			IFixtureFilter filter = (IFixtureFilter) Reflector.InvokeMethod(_sut, "CreateFilter");

			Assert.IsTrue(filter.Filter(typeof(NoAttributes)), "The NoAttributes type should be included");
			Assert.IsFalse(filter.Filter(typeof(ExcludeCategory)), "The ExcludeCategory type should be excluded");
		}

		[Test]
		public void Should_create_category_include_and_exclude_filters()
		{
			Pattern includePattern = new Pattern { PatternName = IncludedCategory };
			_sut.FilterCategories.Include.Add(includePattern);

			Pattern excludePattern = new Pattern { PatternName = ExcludedCategory };
			_sut.FilterCategories.Exclude.Add(excludePattern);

			IFixtureFilter filter = (IFixtureFilter) Reflector.InvokeMethod(_sut, "CreateFilter");

			Assert.IsFalse(filter.Filter(typeof(NoAttributes)), "The NoAttributes type should be excluded");
			Assert.IsTrue(filter.Filter(typeof(IncludeCategory)), "The IncludeCategory type should be included");
			Assert.IsFalse(filter.Filter(typeof(ExcludeCategory)), "The ExcludeCategory type should be excluded");
		}

		[Test]
		public void Should_create_namespace_include_filter()
		{
			Pattern pattern = new Pattern { PatternName = GetType().Namespace };
			_sut.FilterNamespaces.Include.Add(pattern);

			IFixtureFilter filter = (IFixtureFilter) Reflector.InvokeMethod(_sut, "CreateFilter");

			Assert.IsTrue(filter.Filter(typeof(NoAttributes)), "The NoAttributes type should be included");
			Assert.IsFalse(filter.Filter(typeof(object)), "The object type should be excluded");
		}

		[Test]
		public void Should_create_type_include_filter()
		{
			Pattern pattern = new Pattern { PatternName = typeof(NoAttributes).FullName };
			_sut.FilterTypes.Include.Add(pattern);

			IFixtureFilter filter = (IFixtureFilter) Reflector.InvokeMethod(_sut, "CreateFilter");

			Assert.IsTrue(filter.Filter(typeof(NoAttributes)), "The NoAttributes type should be included");
			Assert.IsFalse(filter.Filter(typeof(object)), "The object type should be excluded");
		}

		#region Nested type: ExcludeCategory
		[FixtureCategory(ExcludedCategory)]
		class ExcludeCategory
		{
		}
		#endregion

		#region Nested type: IncludeAuthor
		[Author(IncludedAuthor)]
		class IncludeAuthor
		{
		}
		#endregion

		#region Nested type: IncludeCategory
		[FixtureCategory(IncludedCategory)]
		class IncludeCategory
		{
		}
		#endregion

		#region Nested type: NoAttributes
		class NoAttributes
		{
		}
		#endregion
	}
}