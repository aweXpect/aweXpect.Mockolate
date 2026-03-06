using System.Threading;
using aweXpect.Chronology;
using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class TwiceTests
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
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Twice();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) exactly twice,
				             but never found it

				             Interactions:
				             []
				             """);
			cts.Cancel();
			await backgroundTask;
		}

		[Theory]
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

			await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Twice().Within(30.Seconds());

			await backgroundTask;
		}

		[Theory]
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

			await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Twice().WithTimeout(30.Seconds());

			await backgroundTask;
		}

		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldFail()
		{
			IMyService mock = Mock.Create<IMyService>();

			mock.MyMethod(1, false);
			mock.MyMethod(1, false);
			mock.MyMethod(1, false);

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Twice();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) exactly twice,
				             but found it 3 times

				             Interactions:
				             [
				               [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False),
				               [1] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False),
				               [2] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False)
				             ]
				             """);
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldFail()
		{
			IMyService mock = Mock.Create<IMyService>();


			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Twice();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) exactly twice,
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
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Twice();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) exactly twice,
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
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).Twice();
			}

			await That(Act).DoesNotThrow();
		}
	}
}
