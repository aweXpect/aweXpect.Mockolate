using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Checks;

namespace aweXpect;

public static partial class ThatCheckResult
{
	/// <summary>
	///     Verifies that the checked interaction happened never.
	/// </summary>
	public static AndOrResult<CheckResult<TMock>, IThat<CheckResult<TMock>>> Never<TMock>(
		this IThat<CheckResult<TMock>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasExactlyConstraint<TMock>(expectationBuilder, it, grammars, 0)),
			subject);
}
