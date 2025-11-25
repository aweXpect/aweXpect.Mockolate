using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class TimesTests
	{
		[Theory]
		[InlineData(0, true)]
		[InlineData(1, false)]
		[InlineData(2, true)]
		[InlineData(3, false)]
		[InlineData(4, true)]
		[InlineData(5, false)]
		public async Task Times_WithEvenPredicate_ShouldReturnExpectedResult(int count, bool expectSuccess)
		{
			IMyService mock = Mock.Create<IMyService>();
			for (int i = 0; i < count; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).Times(n => n % 2 == 0);
			}

			string expectedFoundTimes = count switch
			{
				0 => "never found it",
				1 => "found it once",
				2 => "found it twice",
				_ => $"found it {count} times",
			};

			await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
				.WithMessage($"""
				              Expected that the ThatVerificationResultIs.IMyService mock
				              invoked method MyMethod(1, false) according to the predicate n => n % 2 == 0,
				              but {expectedFoundTimes}

				              Interactions:
				              [*]
				              """).AsWildcard();
		}

		[Theory]
		[InlineData(0, false)]
		[InlineData(1, true)]
		[InlineData(2, false)]
		[InlineData(3, true)]
		[InlineData(4, false)]
		[InlineData(5, true)]
		public async Task Times_WithOddPredicate_ShouldReturnExpectedResult(int count, bool expectSuccess)
		{
			IMyService mock = Mock.Create<IMyService>();
			for (int i = 0; i < count; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).Times(n => n % 2 == 1);
			}

			string expectedFoundTimes = count switch
			{
				0 => "never found it",
				1 => "found it once",
				2 => "found it twice",
				_ => $"found it {count} times",
			};

			await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
				.WithMessage($"""
				              Expected that the ThatVerificationResultIs.IMyService mock
				              invoked method MyMethod(1, false) according to the predicate n => n % 2 == 1,
				              but {expectedFoundTimes}

				              Interactions:
				              [*]
				              """).AsWildcard();
		}
	}
}
