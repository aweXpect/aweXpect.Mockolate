using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Checks;

namespace aweXpect;

public static partial class ThatCheckResult
{
	/// <summary>
	///     Verifies that the checked interaction happened exactly twice.
	/// </summary>
	public static AndOrResult<CheckResult<TMock>, IThat<CheckResult<TMock>>> Twice<TMock>(
		this IThat<CheckResult<TMock>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasExactlyConstraint<TMock>(expectationBuilder, it, grammars, 2)),
			subject);
}
