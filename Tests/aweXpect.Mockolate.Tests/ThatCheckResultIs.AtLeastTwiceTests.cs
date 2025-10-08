﻿using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatCheckResultIs
{
	public sealed class AtLeastTwiceTests
	{
		[Fact]
		public async Task WhenInvokedTwice_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.Object.MyMethod(1, false);
			mock.Object.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).AtLeastTwice();

			await That(Act).DoesNotThrow();
		}
		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.Object.MyMethod(1, false);
			mock.Object.MyMethod(1, false);
			mock.Object.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).AtLeastTwice();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenInvokedNever_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();


			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).AtLeastTwice();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatCheckResultIs.IMyService>
					invoked method MyMethod(1, False) at least twice,
					but never found it
					
					Interactions:
					[]
					""");
		}

		[Fact]
		public async Task WhenInvokedOnce_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.Object.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Invoked.MyMethod(1, false)).AtLeastTwice();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatCheckResultIs.IMyService>
					invoked method MyMethod(1, False) at least twice,
					but found it only once
					
					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatCheckResultIs.IMyService.MyMethod(1, False)
					]
					""");
		}
	}
}
