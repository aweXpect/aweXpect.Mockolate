# aweXpect.Mockolate

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Mockolate)](https://www.nuget.org/packages/aweXpect.Mockolate)
[![Build](https://github.com/aweXpect/aweXpect.Mockolate/actions/workflows/build.yml/badge.svg)](https://github.com/aweXpect/aweXpect.Mockolate/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Mockolate&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.Mockolate)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Mockolate&metric=coverage)](https://sonarcloud.io/summary/overall?id=aweXpect_aweXpect.Mockolate)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FaweXpect%2FaweXpect.Mockolate%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/aweXpect/aweXpect.Mockolate/main)

Expectations to verify interactions with mocks from [Mockolate](https://github.com/aweXpect/Mockolate).

## Features

### Interaction count
Verify that a method was called a specific number of times:

```csharp
var mock = Mock.Create<IMyService>();
mock.MyMethod();

await That(mock.VerifyMock.Invoked.MyMethod()).Once();             // Exactly once
await That(mock.VerifyMock.Invoked.MyMethod()).Twice();            // Exactly twice
await That(mock.VerifyMock.Invoked.MyMethod()).Never();            // Never called
await That(mock.VerifyMock.Invoked.MyMethod()).AtLeastOnce();      // At least once
await That(mock.VerifyMock.Invoked.MyMethod()).AtLeastTwice();     // At least twice
await That(mock.VerifyMock.Invoked.MyMethod()).AtLeast(3.Times()); // At least 3 times
await That(mock.VerifyMock.Invoked.MyMethod()).AtMostOnce();       // At most once
await That(mock.VerifyMock.Invoked.MyMethod()).AtMostTwice();      // At most twice
await That(mock.VerifyMock.Invoked.MyMethod()).AtMost(4.Times());  // At most 4 times
await That(mock.VerifyMock.Invoked.MyMethod()).Exactly(2.Times()); // Exactly 2 times
```

### Interaction order
Verify that methods were called in a specific sequence:

```csharp
var mock = Mock.Create<IMyService>();
mock.MyMethod(1);
mock.MyMethod(2);
mock.MyMethod(3);
mock.MyMethod(4);

// Verifies MyMethod(1), then MyMethod(2), then MyMethod(4) were called in order
await That(mock.VerifyMock.Invoked.MyMethod(It.Is(1))).Then(
    m => m.Invoked.MyMethod(It.Is(2)),
    m => m.Invoked.MyMethod(It.Is(4))
);
```

### Additional Verifications

#### All interactions are verified
With `AllInteractionsAreVerified` you can check whether all interactions with the mock have actually been verified. This helps to detect unintended or forgotten calls.

```csharp
var mock = Mock.Create<IMyService>();
mock.MyMethod(1);
mock.MyMethod(2);

await That(mock.VerifyMock.Invoked.MyMethod(It.IsAny<int>())).AtLeastOnce();
 // Succeeds, because the verification applies to both method calls.
await That(mock.VerifyMock).AllInteractionsAreVerified();
```

#### All setups are used
With `AllSetupsAreUsed` you can check whether all defined setups on the mock have actually been used. This ensures that no setup configurations remain unused.

```csharp
var mock = Mock.Create<IMyService>();
mock.SetupMock.Method.MyMethod(It.Is(1)).Returns(10);
mock.SetupMock.Method.MyMethod(It.Is(2)).Returns(20);

mock.DoWork(1);

// Fails, because the setup for MyMethod(2) was never used.
await That(mock.VerifyMock).AllSetupsAreUsed();
```
