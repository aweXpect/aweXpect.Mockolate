using Mockolate;
using Mockolate.Verify;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatMockVerifyIs
{
	public sealed class AllSetupsAreUsedTests
	{
		[Fact]
		public async Task WhenAllInvocationsWereVerified_ShouldNotThrow()
		{
			IMyService mock = Mock.Create<IMyService>();
			mock.SetupMock.Method.DoWork(It.IsAny<int>());

			mock.DoWork(1);
			mock.DoWork(2);

			async Task Act()
			{
				await That(mock.VerifyMock).AllSetupsAreUsed();
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenOneSetupIsNotUsed_ShouldThrow()
		{
			IMyService mock = Mock.Create<IMyService>();
			mock.SetupMock.Method.DoWork(It.Is(1));
			mock.SetupMock.Method.DoWork(It.Is(2));

			mock.DoWork(1);

			async Task Act()
			{
				await That(mock.VerifyMock).AllSetupsAreUsed();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatMockVerifyIs.IMyService mock
				             has used all setups,
				             but the following setup was not used:
				              - void aweXpect.Mockolate.Tests.ThatMockVerifyIs.IMyService.DoWork(2)
				             """);
		}

		[Fact]
		public async Task WhenMultipleSetupsAreNotUsed_ShouldThrow()
		{
			IMyService mock = Mock.Create<IMyService>();
			mock.SetupMock.Method.DoWork(It.Is(1));
			mock.SetupMock.Method.DoWork(It.Is(2));
			mock.SetupMock.Method.DoWork(It.Is(3));

			mock.DoWork(2);

			async Task Act()
			{
				await That(mock.VerifyMock).AllSetupsAreUsed();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatMockVerifyIs.IMyService mock
				             has used all setups,
				             but the following 2 setups were not used:
				              - void aweXpect.Mockolate.Tests.ThatMockVerifyIs.IMyService.DoWork(3)
				              - void aweXpect.Mockolate.Tests.ThatMockVerifyIs.IMyService.DoWork(1)
				             """);
		}

		[Fact]
		public async Task Negated_WhenAllSetupsAreUsed_ShouldThrow()
		{
			IMyService mock = Mock.Create<IMyService>();
			mock.SetupMock.Method.DoWork(It.IsAny<int>());

			mock.DoWork(1);
			mock.DoWork(2);

			async Task Act()
			{
				await That(mock.VerifyMock).DoesNotComplyWith(it => it.AllSetupsAreUsed());
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatMockVerifyIs.IMyService mock
				             has not used all setups,
				             but all were
				             """);
		}
	}
}
