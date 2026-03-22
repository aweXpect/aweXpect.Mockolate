using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class NeverTests
	{
		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldFail()
		{
			IMyService sut = IMyService.CreateMock();

			sut.MyMethod(1, false);
			sut.MyMethod(1, false);
			sut.MyMethod(1, false);

			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Never();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService mock
				             never invoked method MyMethod(1, false),
				             but found it 3 times

				             Interactions:
				             [
				               [0] invoke method global::aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False),
				               [1] invoke method global::aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False),
				               [2] invoke method global::aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False)
				             ]
				             """);
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldSucceed()
		{
			IMyService sut = IMyService.CreateMock();

			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Never();
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenInvokedOnce_ShouldFail()
		{
			IMyService sut = IMyService.CreateMock();

			sut.MyMethod(1, false);

			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Never();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService mock
				             never invoked method MyMethod(1, false),
				             but found it once

				             Interactions:
				             [
				               [0] invoke method global::aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False)
				             ]
				             """);
		}

		[Fact]
		public async Task WhenInvokedTwice_ShouldFail()
		{
			IMyService sut = IMyService.CreateMock();

			sut.MyMethod(1, false);
			sut.MyMethod(1, false);

			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Never();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService mock
				             never invoked method MyMethod(1, false),
				             but found it twice

				             Interactions:
				             [
				               [0] invoke method global::aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False),
				               [1] invoke method global::aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False)
				             ]
				             """);
		}
	}
}
