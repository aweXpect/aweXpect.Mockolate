using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class NeverTests
	{
		[Fact]
		public async Task WhenInvokedTwice_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.Subject.MyMethod(1, false);
			mock.Subject.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).Never();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					never invoked method MyMethod(1, False),
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

			mock.Subject.MyMethod(1, false);
			mock.Subject.MyMethod(1, false);
			mock.Subject.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).Never();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					never invoked method MyMethod(1, False),
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
		public async Task WhenInvokedOnce_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.Subject.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).Never();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					never invoked method MyMethod(1, False),
					but found it once
					
					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False)
					]
					""");
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).Never();

			await That(Act).DoesNotThrow();
		}
	}
}
