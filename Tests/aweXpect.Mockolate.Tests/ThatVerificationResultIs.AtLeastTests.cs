using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class AtLeastTests
	{
		[Theory]
		[InlineData(0, false)]
		[InlineData(3, true)]
		[InlineData(8, true)]
		public async Task WhenInvokedNever_ShouldFailUnlessZero(int times, bool shouldThrow)
		{
			var mock = Mock.Create<IMyService>();

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).AtLeast(times);

			await That(Act).Throws<XunitException>().OnlyIf(shouldThrow)
				.WithMessage($"""
					Expected that the ThatVerificationResultIs.IMyService mock
					invoked method MyMethod(1, false) at least {times} times,
					but never found it
					
					Interactions:
					[]
					""");
		}

		[Theory]
		[InlineData(4, 3)]
		[InlineData(8, 6)]
		public async Task WhenInvokedFewerTimes_ShouldFail(int times, int invocationTimes)
		{
			var mock = Mock.Create<IMyService>();

			for (var i = 0; i < invocationTimes; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).AtLeast(times);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
					Expected that the ThatVerificationResultIs.IMyService mock
					invoked method MyMethod(1, false) at least {times} times,
					but found it only {invocationTimes} times
					
					Interactions:
					[
					*
					]
					""").AsWildcard();
		}

		[Theory]
		[InlineData(3, 4)]
		[InlineData(6, 8)]
		public async Task WhenInvokedMoreOften_ShouldSucceed(int times, int invocationTimes)
		{
			var mock = Mock.Create<IMyService>();

			for (var i = 0; i < invocationTimes; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).AtLeast(times);

			await That(Act).DoesNotThrow();
		}

		[Theory]
		[InlineData(3)]
		[InlineData(6)]
		[InlineData(18)]
		public async Task WhenInvokedAtLeastTheSameTimes_ShouldSucceed(int times)
		{
			var mock = Mock.Create<IMyService>();

			for (var i = 0; i < times; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).AtLeast(times);

			await That(Act).DoesNotThrow();
		}
	}
}
