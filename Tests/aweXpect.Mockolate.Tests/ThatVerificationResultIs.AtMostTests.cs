using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class AtMost
	{
		[Theory]
		[InlineData(2, 0)]
		[InlineData(4, 3)]
		[InlineData(8, 6)]
		public async Task WhenInvokedFewerTimes_ShouldSucceed(int times, int invocationTimes)
		{
			var mock = Mock.Create<IMyService>();

			for (var i = 0; i < invocationTimes; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).AtMost(times);

			await That(Act).DoesNotThrow();
		}

		[Theory]
		[InlineData(3, 4)]
		[InlineData(6, 8)]
		public async Task WhenInvokedMoreOften_ShouldFail(int times, int invocationTimes)
		{
			var mock = Mock.Create<IMyService>();

			for (var i = 0; i < invocationTimes; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).AtMost(times);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
					Expected that the ThatVerificationResultIs.IMyService mock
					invoked method MyMethod(1, false) at most {times} times,
					but found it {invocationTimes} times
					
					Interactions:
					[
					*
					]
					""").AsWildcard();
		}

		[Theory]
		[InlineData(3)]
		[InlineData(6)]
		[InlineData(18)]
		public async Task WhenInvokedAtMostTheSameTimes_ShouldSucceed(int times)
		{
			var mock = Mock.Create<IMyService>();

			for (var i = 0; i < times; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).AtMost(times);

			await That(Act).DoesNotThrow();
		}
	}
}
