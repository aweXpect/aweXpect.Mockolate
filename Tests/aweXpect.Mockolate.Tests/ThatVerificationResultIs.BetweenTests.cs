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
			Mock<IMyService> mock = Mock.Create<IMyService>();

			for (int i = 0; i < invocationTimes; i++)
			{
				mock.Subject.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.Verify.Invoked.MyMethod(1, false)).Between(minimum).And(maximum);
			}

			await That(Act).DoesNotThrow();
		}

		[Theory]
		[InlineData(0, 1, false)]
		[InlineData(3, 5, true)]
		[InlineData(1, 2, true)]
		public async Task WhenInvokedNever_ShouldFailUnlessZero(int minimum, int maximum, bool shouldThrow)
		{
			Mock<IMyService> mock = Mock.Create<IMyService>();

			async Task Act()
			{
				await That(mock.Verify.Invoked.MyMethod(1, false)).Between(minimum).And(maximum);
			}

			await That(Act).Throws<XunitException>().OnlyIf(shouldThrow)
				.WithMessage($"""
				              Expected that the Mock<ThatVerificationResultIs.IMyService>
				              invoked method MyMethod(1, False) between {minimum} and {maximum} times,
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
			Mock<IMyService> mock = Mock.Create<IMyService>();

			for (int i = 0; i < invocationTimes; i++)
			{
				mock.Subject.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.Verify.Invoked.MyMethod(1, false)).Between(minimum).And(maximum);
			}

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that the Mock<ThatVerificationResultIs.IMyService>
				              invoked method MyMethod(1, False) between {minimum} and {maximum} times,
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
			Mock<IMyService> mock = Mock.Create<IMyService>();

			for (int i = 0; i < invocationTimes; i++)
			{
				mock.Subject.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.Verify.Invoked.MyMethod(1, false)).Between(minimum).And(maximum);
			}

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that the Mock<ThatVerificationResultIs.IMyService>
				              invoked method MyMethod(1, False) between {minimum} and {maximum} times,
				              but found it {invocationTimes} times

				              Interactions:
				              [
				              *
				              ]
				              """).AsWildcard();
		}
	}
}
