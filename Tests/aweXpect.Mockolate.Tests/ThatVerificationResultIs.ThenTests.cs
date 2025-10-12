using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class ThenTests
	{
		[Fact]
		public async Task Then_ShouldVerifyInOrder()
		{
			Mock<IMyService> sut = Mock.Create<IMyService>();

			sut.Subject.MyMethod(1);
			sut.Subject.MyMethod(2);
			sut.Subject.MyMethod(3);
			sut.Subject.MyMethod(4);

			await That(sut.Verify.Invoked.MyMethod(3)).Then(m => m.Invoked.MyMethod(4));
			await That(async Task () => await That(sut.Verify.Invoked.MyMethod(2)).Then(m => m.Invoked.MyMethod(1)))
				.Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					invoked method MyMethod(2), then
					invoked method MyMethod(1) in order,
					but it invoked method MyMethod(1) too early

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(2),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(3),
					  [3] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(4)
					]
					""");
			await That(sut.Verify.Invoked.MyMethod(1)).Then(m => m.Invoked.MyMethod(2), m => m.Invoked.MyMethod(3));
		}

		[Fact]
		public async Task Then_WhenNoMatch_ShouldReturnFalse()
		{
			Mock<IMyService> sut = Mock.Create<IMyService>();

			sut.Subject.MyMethod(1);
			sut.Subject.MyMethod(2);
			sut.Subject.MyMethod(3);
			sut.Subject.MyMethod(4);

			await That(async Task () => await That(sut.Verify.Invoked.MyMethod(6)).Then(m => m.Invoked.MyMethod(4)))
				.Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					invoked method MyMethod(6), then
					invoked method MyMethod(4) in order,
					but it invoked method MyMethod(6) not at all

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(2),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(3),
					  [3] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(4)
					]
					""");

			await That(async Task () => await That(sut.Verify.Invoked.MyMethod(1)).Then(m => m.Invoked.MyMethod(6), m => m.Invoked.MyMethod(3)))
				.Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					invoked method MyMethod(1), then
					invoked method MyMethod(6), then
					invoked method MyMethod(3) in order,
					but it invoked method MyMethod(6) not at all

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(2),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(3),
					  [3] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(4)
					]
					""");

			await That(async Task () => await That(sut.Verify.Invoked.MyMethod(1)).Then(m => m.Invoked.MyMethod(2), m => m.Invoked.MyMethod(6)))
				.Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					invoked method MyMethod(1), then
					invoked method MyMethod(2), then
					invoked method MyMethod(6) in order,
					but it invoked method MyMethod(6) not at all

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(2),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(3),
					  [3] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(4)
					]
					""");
		}
	}
}
