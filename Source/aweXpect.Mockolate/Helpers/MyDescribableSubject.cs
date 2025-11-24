using System;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal sealed class MyDescribableSubject<TVerify> : IDescribableSubject
{
	public string GetDescription()
	{
		string mockVerify = Formatter.Format(typeof(TVerify));
		if (mockVerify.StartsWith("MockVerify", StringComparison.Ordinal))
		{
			mockVerify = $"Mock{mockVerify.Substring("MockVerify".Length)}";
			int genericSeparator = mockVerify.IndexOf(", ", StringComparison.Ordinal);
			if (genericSeparator > 0)
			{
				mockVerify = $"{mockVerify.Substring(0, genericSeparator)}>";
			}
		}

		return $"the {mockVerify} mock";
	}
}
