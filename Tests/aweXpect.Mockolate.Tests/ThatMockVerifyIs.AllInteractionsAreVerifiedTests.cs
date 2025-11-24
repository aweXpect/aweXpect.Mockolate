using Mockolate;
using Mockolate.Verify;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatMockVerifyIs
{
	public sealed class AllInteractionsAreVerifiedTests
	{
		[Fact]
		public async Task WhenAllInvocationsWereVerified_ShouldNotThrow()
		{
			IMyService mock = Mock.Create<IMyService>();

			mock.DoWork(1);
			mock.DoWork(2);

			mock.VerifyMock.Invoked.DoWork(Match.Any<int>()).AtLeastOnce();

			async Task Act()
			{
				await That(mock.VerifyMock).AllInteractionsAreVerified();
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenOneInvocationIsNotVerified_ShouldThrow()
		{
			IMyService mock = Mock.Create<IMyService>();

			mock.DoWork(1);
			mock.DoWork(2);

			mock.VerifyMock.Invoked.DoWork(Match.With(1)).AtLeastOnce();

			async Task Act()
			{
				await That(mock.VerifyMock).AllInteractionsAreVerified();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatMockVerifyIs.IMyService mock
				             has all interactions verified,
				             but the following interaction was not verified:
				              - [1] invoke method aweXpect.Mockolate.Tests.ThatMockVerifyIs.IMyService.DoWork(2)
				             """);
		}

		[Fact]
		public async Task WhenMultipleInvocationIsNotVerified_ShouldThrow()
		{
			IMyService mock = Mock.Create<IMyService>();

			mock.DoWork(1);
			mock.DoWork(2);
			mock.DoWork(3);

			mock.VerifyMock.Invoked.DoWork(Match.With(2)).AtLeastOnce();

			async Task Act()
			{
				await That(mock.VerifyMock).AllInteractionsAreVerified();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatMockVerifyIs.IMyService mock
				             has all interactions verified,
				             but the following 2 interactions were not verified:
				              - [0] invoke method aweXpect.Mockolate.Tests.ThatMockVerifyIs.IMyService.DoWork(1)
				              - [2] invoke method aweXpect.Mockolate.Tests.ThatMockVerifyIs.IMyService.DoWork(3)
				             """);
		}

		[Fact]
		public async Task Negated_WhenAllAreVerified_ShouldThrow()
		{
			IMyService mock = Mock.Create<IMyService>();

			mock.DoWork(1);
			mock.DoWork(2);

			mock.VerifyMock.Invoked.DoWork(Match.Any<int>()).AtLeastOnce();

			async Task Act()
			{
				await That(mock.VerifyMock).DoesNotComplyWith(it => it.AllInteractionsAreVerified());
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatMockVerifyIs.IMyService mock
				             has not all interactions verified,
				             but all were
				             """);
		}
	}
}
