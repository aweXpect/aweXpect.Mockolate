using aweXpect.Core;
using Mockolate;

namespace aweXpect.Helpers;

internal sealed class MyDescribableSubject<T>(IMock? mock) : IDescribableSubject
{
	public string GetDescription()
		=> mock is not null
			? $"the {mock.ToString()}"
			: $"the {Formatter.Format(typeof(T))} mock";
}
