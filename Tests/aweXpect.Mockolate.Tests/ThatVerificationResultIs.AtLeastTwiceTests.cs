using System.Threading;
using aweXpect.Chronology;
using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class AtLeastTwiceTests
	{
		[Fact]
		public async Task WhenInvokedInBackground_ShouldFail()
		{
			IMyService mock = Mock.Create<IMyService>();
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;

			Task backgroundTask = Task.Run(async () =>
			{
				try
				{
					await Task.Delay(5000, token);
					while (!token.IsCancellationRequested)
					{
						await Task.Delay(50, token).ConfigureAwait(false);
						mock.MyMethod(1, false);
					}
				}
				catch (OperationCanceledException)
				{
					// Ignore cancellation
				}
			}, CancellationToken.None);

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) at least twice,
				             but never found it

				             Interactions:
				             []
				             """);
			cts.Cancel();
			await backgroundTask;
		}

		[Theory]
		[InlineData(7)]
		[InlineData(2)]
		public async Task WhenInvokedInBackground_Within_ShouldSucceed(int invocationTimes)
		{
			IMyService mock = Mock.Create<IMyService>();

			Task backgroundTask = Task.Delay(50).ContinueWith(_ =>
			{
				for (int i = 0; i < invocationTimes; i++)
				{
					mock.MyMethod(1, false);
				}
			});

			await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice().Within(30.Seconds());

			await backgroundTask;
		}

		[Theory]
		[InlineData(7)]
		[InlineData(2)]
		public async Task WhenInvokedInBackground_WithTimeout_ShouldSucceed(int invocationTimes)
		{
			IMyService mock = Mock.Create<IMyService>();

			Task backgroundTask = Task.Delay(50).ContinueWith(_ =>
			{
				for (int i = 0; i < invocationTimes; i++)
				{
					mock.MyMethod(1, false);
				}
			});

			await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice()
				.WithTimeout(30.Seconds());

			await backgroundTask;
		}

		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldSucceed()
		{
			IMyService mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);
			mock.MyMethod(1, false);
			mock.MyMethod(1, false);

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice();
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldFail()
		{
			IMyService mock = Mock.Create<IMyService>();

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice();
			}

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
			IMyService mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice();
			}

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

		[Fact]
		public async Task WhenInvokedTwice_ShouldSucceed()
		{
			IMyService mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);
			mock.MyMethod(1, false);

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeastTwice();
			}

			await That(Act).DoesNotThrow();
		}
	}
}
