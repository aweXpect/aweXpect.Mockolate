using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class OnceTests
	{
		[Fact]
		public async Task WhenInvokedTwice_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);
			mock.MyMethod(1, false);

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).Once();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the ThatVerificationResultIs.IMyService mock
					invoked method MyMethod(1, false) exactly once,
					but found it twice

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False)
					]
					""");
		}
		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);
			mock.MyMethod(1, false);
			mock.MyMethod(1, false);

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).Once();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the ThatVerificationResultIs.IMyService mock
					invoked method MyMethod(1, false) exactly once,
					but found it 3 times

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False)
					]
					""");
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();


			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).Once();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the ThatVerificationResultIs.IMyService mock
					invoked method MyMethod(1, false) exactly once,
					but never found it
					
					Interactions:
					[]
					""");
		}

		[Fact]
		public async Task WhenInvokedOnce_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(Match.With(1), Match.With(false))).Once();

			await That(Act).DoesNotThrow();
		}
	}
}
