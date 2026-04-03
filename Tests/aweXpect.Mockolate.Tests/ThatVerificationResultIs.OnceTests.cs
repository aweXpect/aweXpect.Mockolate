using System.Threading;
using aweXpect.Chronology;
using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class OnceTests
	{
		[Fact]
		public async Task WhenInvokedInBackground_ShouldFail()
		{
			IMyService sut = IMyService.CreateMock();
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
						sut.MyMethod(1, false);
					}
				}
				catch (OperationCanceledException)
				{
					// Ignore cancellation
				}
			}, CancellationToken.None);

			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Once();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) exactly once,
				             but never found it

				             Interactions:
				             []
				             """);
			cts.Cancel();
			await backgroundTask;
		}

		[Theory]
		[InlineData(1)]
		public async Task WhenInvokedInBackground_WithCancellation_ShouldSucceed(int invocationTimes)
		{
			IMyService sut = IMyService.CreateMock();
			using CancellationTokenSource cts = new(30.Seconds());
			CancellationToken token = cts.Token;

			Task backgroundTask = Task.Delay(50, token).ContinueWith(_ =>
			{
				for (int i = 0; i < invocationTimes; i++)
				{
					sut.MyMethod(1, false);
				}
			}, token);

			await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Once().WithCancellation(token);

			await backgroundTask;
		}

		[Theory]
		[InlineData(1)]
		public async Task WhenInvokedInBackground_Within_ShouldSucceed(int invocationTimes)
		{
			IMyService sut = IMyService.CreateMock();

			Task backgroundTask = Task.Delay(50).ContinueWith(_ =>
			{
				for (int i = 0; i < invocationTimes; i++)
				{
					sut.MyMethod(1, false);
				}
			});

			await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Once().Within(30.Seconds());

			await backgroundTask;
		}

		[Theory]
		[InlineData(1)]
		public async Task WhenInvokedInBackground_WithTimeout_ShouldSucceed(int invocationTimes)
		{
			IMyService sut = IMyService.CreateMock();

			Task backgroundTask = Task.Delay(50).ContinueWith(_ =>
			{
				for (int i = 0; i < invocationTimes; i++)
				{
					sut.MyMethod(1, false);
				}
			});

			await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Once().WithTimeout(30.Seconds());

			await backgroundTask;
		}

		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldFail()
		{
			IMyService sut = IMyService.CreateMock();

			sut.MyMethod(1, false);
			sut.MyMethod(1, false);
			sut.MyMethod(1, false);

			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Once();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) exactly once,
				             but found it 3 times

				             Interactions:
				             [
				               [0] invoke method MyMethod(1, False),
				               [1] invoke method MyMethod(1, False),
				               [2] invoke method MyMethod(1, False)
				             ]
				             """);
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldFail()
		{
			IMyService sut = IMyService.CreateMock();


			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Once();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) exactly once,
				             but never found it

				             Interactions:
				             []
				             """);
		}

		[Fact]
		public async Task WhenInvokedOnce_ShouldSucceed()
		{
			IMyService sut = IMyService.CreateMock();

			sut.MyMethod(1, false);

			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Once();
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenInvokedTwice_ShouldFail()
		{
			IMyService sut = IMyService.CreateMock();

			sut.MyMethod(1, false);
			sut.MyMethod(1, false);

			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.Is(1), It.Is(false))).Once();
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) exactly once,
				             but found it twice

				             Interactions:
				             [
				               [0] invoke method MyMethod(1, False),
				               [1] invoke method MyMethod(1, False)
				             ]
				             """);
		}

		[Fact]
		public async Task WhenNotInvoked_Within_ShouldFailWithDescriptiveMessage()
		{
			IMyService sut = IMyService.CreateMock();

			async Task Act()
			{
				await That(sut.Mock.Verify.MyMethod(It.IsAny<int>(), It.Is(true))).Once().Within(50.Milliseconds());
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(It.IsAny<int>(), true) exactly once,
				             but never found it

				             Interactions:
				             []
				             """);
		}
	}
}
