﻿using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public sealed class TwiceTests
	{
		[Fact]
		public async Task WhenInvokedTwice_ShouldSucceed()
		{
			var mock = Mock.Create<IMyService>();

			mock.Subject.MyMethod(1, false);
			mock.Subject.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).Twice();

			await That(Act).DoesNotThrow();
		}
		[Fact]
		public async Task WhenInvokedMoreThanTwice_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.Subject.MyMethod(1, false);
			mock.Subject.MyMethod(1, false);
			mock.Subject.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).Twice();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					invoked method MyMethod(1, False) exactly twice,
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
			var mock = Mock.Create<IMyService>();


			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).Twice();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					invoked method MyMethod(1, False) exactly twice,
					but never found it
					
					Interactions:
					[]
					""");
		}

		[Fact]
		public async Task WhenInvokedOnce_ShouldFail()
		{
			var mock = Mock.Create<IMyService>();

			mock.Subject.MyMethod(1, false);

			async Task Act()
				=> await That(mock.Verify.Invoked.MyMethod(1, false)).Twice();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
					Expected that the Mock<ThatVerificationResultIs.IMyService>
					invoked method MyMethod(1, False) exactly twice,
					but found it only once
					
					Interactions:
					[
					  [0] invoke method aweXpect.Mockolate.Tests.ThatVerificationResultIs.IMyService.MyMethod(1, False)
					]
					""");
		}
	}
}
