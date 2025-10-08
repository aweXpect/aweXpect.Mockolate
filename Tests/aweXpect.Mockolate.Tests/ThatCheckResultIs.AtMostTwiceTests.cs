using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatCheckResultIs
{
	public sealed class AtMostTwiceTests
	{
		[Fact]
		public async Task WhenInvokedTwice_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.Object.MyMethod(1, false);
			mock.Object.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).AtMostTwice();

			await That(Act).DoesNotThrow();
		}
		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.Object.MyMethod(1, false);
			mock.Object.MyMethod(1, false);
			mock.Object.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).AtMostTwice();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatCheckResultIs.IMyService>
					invoked method MyMethod(1, False) at most twice,
					but found it 3 times

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1, False),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1, False),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1, False)
					]
					""");
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).AtMostTwice();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenInvokedOnce_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.Object.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).AtMostTwice();

			await That(Act).DoesNotThrow();
		}
	}
}
