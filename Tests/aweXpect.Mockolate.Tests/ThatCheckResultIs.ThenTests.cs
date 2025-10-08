using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatCheckResultIs
{
	public sealed class ThenTests
	{
		[Fact]
		public async Task Then_ShouldVerifyInOrder()
		{
			Mock<IMyService> sut = Mock.Create<IMyService>();

			sut.Object.MyMethod(1);
			sut.Object.MyMethod(2);
			sut.Object.MyMethod(3);
			sut.Object.MyMethod(4);

			await That(sut.Invoked.MyMethod(3)).Then(m => m.Invoked.MyMethod(4));
			await That(async Task () => await That(sut.Invoked.MyMethod(2)).Then(m => m.Invoked.MyMethod(1)))
				.Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatCheckResultIs.IMyService>
					invoked method MyMethod(2), then
					invoked method MyMethod(1) in order,
					but it invoked method MyMethod(1) too early

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(2),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(3),
					  [3] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(4)
					]
					""");
			await That(sut.Invoked.MyMethod(1)).Then(m => m.Invoked.MyMethod(2), m => m.Invoked.MyMethod(3));
		}

		[Fact]
		public async Task Then_WhenNoMatch_ShouldReturnFalse()
		{
			Mock<IMyService> sut = Mock.Create<IMyService>();

			sut.Object.MyMethod(1);
			sut.Object.MyMethod(2);
			sut.Object.MyMethod(3);
			sut.Object.MyMethod(4);

			await That(async Task () => await That(sut.Invoked.MyMethod(6)).Then(m => m.Invoked.MyMethod(4)))
				.Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatCheckResultIs.IMyService>
					invoked method MyMethod(6), then
					invoked method MyMethod(4) in order,
					but it invoked method MyMethod(6) not at all

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(2),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(3),
					  [3] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(4)
					]
					""");

			await That(async Task () => await That(sut.Invoked.MyMethod(1)).Then(m => m.Invoked.MyMethod(6), m => m.Invoked.MyMethod(3)))
				.Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatCheckResultIs.IMyService>
					invoked method MyMethod(1), then
					invoked method MyMethod(6), then
					invoked method MyMethod(3) in order,
					but it invoked method MyMethod(6) not at all

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(2),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(3),
					  [3] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(4)
					]
					""");

			await That(async Task () => await That(sut.Invoked.MyMethod(1)).Then(m => m.Invoked.MyMethod(2), m => m.Invoked.MyMethod(6)))
				.Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatCheckResultIs.IMyService>
					invoked method MyMethod(1), then
					invoked method MyMethod(2), then
					invoked method MyMethod(6) in order,
					but it invoked method MyMethod(6) not at all

					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1),
					  [1] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(2),
					  [2] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(3),
					  [3] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(4)
					]
					""");
		}
	}
}
