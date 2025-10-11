using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Verify;

namespace aweXpect;

public static partial class ThatVerificationResult
{
	/// <summary>
	///     Verifies that the checked interaction happened at most the number of <paramref name="times"/>.
	/// </summary>
	public static AndOrResult<VerificationResult<TMock>, IThat<VerificationResult<TMock>>> AtMost<TMock>(
		this IThat<VerificationResult<TMock>> subject, Times times)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasAtMostConstraint<TMock>(expectationBuilder, it, grammars, times.Value)),
			subject);
}
