using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class BetweenTests
	{
		[Theory]
		[InlineData(3, 5, 3)]
		[InlineData(3, 5, 5)]
		[InlineData(3, 5, 4)]
		[InlineData(3, 8, 6)]
		public async Task WhenInvokedInRange_ShouldSucceed(int minimum, int maximum, int invocationTimes)
		{
			IMyService mock = Mock.Create<IMyService>();

			for (int i = 0; i < invocationTimes; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Between(minimum).And(maximum);
			}

			await That(Act).DoesNotThrow();
		}

		[Theory]
		[InlineData(0, 1, false)]
		[InlineData(3, 5, true)]
		[InlineData(1, 2, true)]
		public async Task WhenInvokedNever_ShouldFailUnlessZero(int minimum, int maximum, bool shouldThrow)
		{
			IMyService mock = Mock.Create<IMyService>();

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Between(minimum).And(maximum);
			}

			await That(Act).Throws<XunitException>().OnlyIf(shouldThrow)
				.WithMessage($"""
				              Expected that the ThatVerificationResultIs.IMyService mock
				              invoked method MyMethod(1, false) between {minimum} and {maximum} times,
				              but never found it

				              Interactions:
				              []
				              """);
		}

		[Theory]
		[InlineData(4, 5, 3)]
		[InlineData(8, 12, 6)]
		public async Task WhenInvokedTooFewTimes_ShouldFail(int minimum, int maximum, int invocationTimes)
		{
			IMyService mock = Mock.Create<IMyService>();

			for (int i = 0; i < invocationTimes; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Between(minimum).And(maximum);
			}

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that the ThatVerificationResultIs.IMyService mock
				              invoked method MyMethod(1, false) between {minimum} and {maximum} times,
				              but found it only {invocationTimes} times

				              Interactions:
				              [
				              *
				              ]
				              """).AsWildcard();
		}

		[Theory]
		[InlineData(4, 5, 6)]
		[InlineData(8, 12, 14)]
		public async Task WhenInvokedTooOften_ShouldFail(int minimum, int maximum, int invocationTimes)
		{
			IMyService mock = Mock.Create<IMyService>();

			for (int i = 0; i < invocationTimes; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Between(minimum).And(maximum);
			}

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that the ThatVerificationResultIs.IMyService mock
				              invoked method MyMethod(1, false) between {minimum} and {maximum} times,
				              but found it {invocationTimes} times

				              Interactions:
				              [
				              *
				              ]
				              """).AsWildcard();
		}
	}
}
