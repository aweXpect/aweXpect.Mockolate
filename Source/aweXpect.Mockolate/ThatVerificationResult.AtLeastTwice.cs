using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Verify;

namespace aweXpect;

public static partial class ThatVerificationResult
{
	/// <summary>
	///     Verifies that the checked interaction happened at least twice.
	/// </summary>
	public static AndOrResult<VerificationResult<TVerify>, IThat<VerificationResult<TVerify>>> AtLeastTwice<TVerify>(
		this IThat<VerificationResult<TVerify>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasAtLeastConstraint<TVerify>(expectationBuilder, it, grammars, 2)),
			subject);
}
