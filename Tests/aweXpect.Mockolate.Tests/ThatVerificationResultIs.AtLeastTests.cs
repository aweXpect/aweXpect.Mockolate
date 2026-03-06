using System.Threading;
using aweXpect.Chronology;
using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class AtLeastTests
	{
		[Theory]
		[InlineData(3)]
		[InlineData(6)]
		[InlineData(18)]
		public async Task WhenInvokedAtLeastTheSameTimes_ShouldSucceed(int times)
		{
			IMyService mock = Mock.Create<IMyService>();

			for (int i = 0; i < times; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeast(times);
			}

			await That(Act).DoesNotThrow();
		}

		[Theory]
		[InlineData(4, 3)]
		[InlineData(8, 6)]
		public async Task WhenInvokedFewerTimes_ShouldFail(int times, int invocationTimes)
		{
			IMyService mock = Mock.Create<IMyService>();

			for (int i = 0; i < invocationTimes; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeast(times);
			}

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that the ThatVerificationResultIs.IMyService mock
				              invoked method MyMethod(1, false) at least {times} times,
				              but found it only {invocationTimes} times

				              Interactions:
				              [
				              *
				              ]
				              """).AsWildcard();
		}

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
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeast(3);
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that the ThatVerificationResultIs.IMyService mock
				             invoked method MyMethod(1, false) at least 3 times,
				             but never found it

				             Interactions:
				             []
				             """);
			cts.Cancel();
			await backgroundTask;
		}

		[Theory]
		[InlineData(3, 7)]
		[InlineData(5, 5)]
		public async Task WhenInvokedInBackground_Within_ShouldSucceed(int times, int invocationTimes)
		{
			IMyService mock = Mock.Create<IMyService>();

			Task backgroundTask = Task.Delay(50).ContinueWith(_ =>
			{
				for (int i = 0; i < invocationTimes; i++)
				{
					mock.MyMethod(1, false);
				}
			});

			await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeast(times).Within(30.Seconds());

			await backgroundTask;
		}

		[Theory]
		[InlineData(3, 7)]
		[InlineData(5, 5)]
		public async Task WhenInvokedInBackground_WithTimeout_ShouldSucceed(int times, int invocationTimes)
		{
			IMyService mock = Mock.Create<IMyService>();

			Task backgroundTask = Task.Delay(50).ContinueWith(_ =>
			{
				for (int i = 0; i < invocationTimes; i++)
				{
					mock.MyMethod(1, false);
				}
			});

			await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeast(times)
				.WithTimeout(30.Seconds());

			await backgroundTask;
		}

		[Theory]
		[InlineData(3, 4)]
		[InlineData(6, 8)]
		public async Task WhenInvokedMoreOften_ShouldSucceed(int times, int invocationTimes)
		{
			IMyService mock = Mock.Create<IMyService>();

			for (int i = 0; i < invocationTimes; i++)
			{
				mock.MyMethod(1, false);
			}

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeast(times);
			}

			await That(Act).DoesNotThrow();
		}

		[Theory]
		[InlineData(0, false)]
		[InlineData(3, true)]
		[InlineData(8, true)]
		public async Task WhenInvokedNever_ShouldFailUnlessZero(int times, bool shouldThrow)
		{
			IMyService mock = Mock.Create<IMyService>();

			async Task Act()
			{
				await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1), It.Is(false))).AtLeast(times);
			}

			await That(Act).Throws<XunitException>().OnlyIf(shouldThrow)
				.WithMessage($"""
				              Expected that the ThatVerificationResultIs.IMyService mock
				              invoked method MyMethod(1, false) at least {times} times,
				              but never found it

				              Interactions:
				              []
				              """);
		}
	}
}
