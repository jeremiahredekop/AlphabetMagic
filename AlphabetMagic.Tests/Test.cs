using NUnit.Framework;
using System;

namespace AlphabetMagic.Tests
{

	public class When_the_magic_happens : SpecificationBase
	{

		protected override void Given ()
		{

		}

		[Test ()]
		public void TestCase ()
		{
		}
	}

	public class SpecificationBase
	{
		[SetUp]
		public void Setup()
		{
			Given();
			When();
		}
		protected virtual void Given()
		{ }
		protected virtual void When()
		{ }
	}

	public class ThenAttribute : TestAttribute
	{ }
}

