using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Verify;

namespace aweXpect;

public static partial class ThatVerificationResult
{
	/// <summary>
	///     Verifies that the checked interaction happened at least once.
	/// </summary>
	public static AndOrResult<VerificationResult<TMock>, IThat<VerificationResult<TMock>>> AtLeastOnce<TMock>(
		this IThat<VerificationResult<TMock>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasAtLeastConstraint<TMock>(expectationBuilder, it, grammars, 1)),
			subject);
}
