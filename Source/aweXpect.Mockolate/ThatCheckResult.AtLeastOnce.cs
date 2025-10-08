using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Checks;

namespace aweXpect;

public static partial class ThatCheckResult
{
	/// <summary>
	///     Verifies that the checked interaction happened at least once.
	/// </summary>
	public static AndOrResult<CheckResult<TMock>, IThat<CheckResult<TMock>>> AtLeastOnce<TMock>(
		this IThat<CheckResult<TMock>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasAtLeastConstraint<TMock>(expectationBuilder, it, grammars, 1)),
			subject);
}
