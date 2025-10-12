using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Verify;

namespace aweXpect;

public static partial class ThatVerificationResult
{
	/// <summary>
	///     Verifies that the checked interaction happened exactly the number of <paramref name="times"/>.
	/// </summary>
	public static AndOrResult<VerificationResult<TVerify>, IThat<VerificationResult<TVerify>>> Exactly<TVerify>(
		this IThat<VerificationResult<TVerify>> subject, Times times)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasExactlyConstraint<TVerify>(expectationBuilder, it, grammars, times.Value)),
			subject);
}
