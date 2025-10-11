using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class AtLeastOnceTests
	{
		[Fact]
		public async Task WhenInvokedTwice_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.Subject.MyMethod(1, false);
			mock.Subject.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).AtLeastOnce();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.Subject.MyMethod(1, false);
			mock.Subject.MyMethod(1, false);
			mock.Subject.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).AtLeastOnce();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).AtLeastOnce();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					invoked method MyMethod(1, False) at least once,
					but never found it
					
					Interactions:
					[]
					""");
		}

		[Fact]
		public async Task WhenInvokedOnce_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.Subject.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).AtLeastOnce();

			await That(Act).DoesNotThrow();
		}
	}
}
