using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Checks;

namespace aweXpect;

public static partial class ThatCheckResult
{
	/// <summary>
	///     Verifies that the checked interaction happened at most the number of <paramref name="times"/>.
	/// </summary>
	public static AndOrResult<CheckResult<TMock>, IThat<CheckResult<TMock>>> AtMost<TMock>(
		this IThat<CheckResult<TMock>> subject, Times times)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasAtMostConstraint<TMock>(expectationBuilder, it, grammars, times.Value)),
			subject);
}
