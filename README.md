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
mock.Object.MyMethod();

await That(mock.Invoked.MyMethod()).Once();             // Exactly once
await That(mock.Invoked.MyMethod()).Twice();            // Exactly twice
await That(mock.Invoked.MyMethod()).Never();            // Never called
await That(mock.Invoked.MyMethod()).AtLeastOnce();      // At least once
await That(mock.Invoked.MyMethod()).AtLeastTwice();     // At least twice
await That(mock.Invoked.MyMethod()).AtLeast(3.Times()); // At least 3 times
await That(mock.Invoked.MyMethod()).AtMostOnce();       // At most once
await That(mock.Invoked.MyMethod()).AtMostTwice();      // At most twice
await That(mock.Invoked.MyMethod()).AtMost(4.Times());  // At most 4 times
await That(mock.Invoked.MyMethod()).Exactly(2.Times()); // Exactly 2 times
```

### Interaction order
Verify that methods were called in a specific sequence:

```csharp
var mock = Mock.Create<IMyService>();
mock.Object.MyMethod(1);
mock.Object.MyMethod(2);
mock.Object.MyMethod(3);
mock.Object.MyMethod(4);

// Verifies MyMethod(1), then MyMethod(2), then MyMethod(4) were called in order
await That(mock.Invoked.MyMethod(1)).Then(
    m => m.Invoked.MyMethod(2),
    m => m.Invoked.MyMethod(4)
);
```

