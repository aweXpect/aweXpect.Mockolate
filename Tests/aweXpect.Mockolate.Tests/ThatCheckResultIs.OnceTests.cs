using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatCheckResultIs
{
	public sealed class OnceTests
	{
		[Fact]
		public async Task WhenInvokedOnce_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.Object.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).Once();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenInvokedMoreThanOnce_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.Object.MyMethod(1, false);
			mock.Object.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).Once();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that mock.Invoked.MyMethod(1, false)
					invoked method MyMethod(1, False) once,
					but found it 2 times

					Interactions:
					[0] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1, False), [1] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1, False)
					""");
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();


			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).Once();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that mock.Invoked.MyMethod(1, false)
					invoked method MyMethod(1, False) once,
					but found it 0 times

					Interactions:
					
					""");
		}
	}

	public interface IMyService
	{
		int MyProperty { get; set; }
		int MyMethod(int value, bool flag);
	}
}
