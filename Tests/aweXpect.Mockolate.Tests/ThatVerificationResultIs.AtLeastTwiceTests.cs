using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class AtLeastTwiceTests
	{
		[Fact]
		public async Task WhenInvokedTwice_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);
			mock.MyMethod(1, false);

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice();

			await That(Act).DoesNotThrow();
		}
		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);
			mock.MyMethod(1, false);
			mock.MyMethod(1, false);

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the ThatVerificationResultIs.IMyService mock
					invoked method MyMethod(1, false) at least twice,
					but never found it
					
					Interactions:
					[]
					""");
		}

		[Fact]
		public async Task WhenInvokedOnce_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);

			async Task Act()
				=> await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the ThatVerificationResultIs.IMyService mock
					invoked method MyMethod(1, false) at least twice,
					but found it only once
					
					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False)
					]
					""");
		}
	}
}
