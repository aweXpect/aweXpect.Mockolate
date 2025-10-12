using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Verify;

namespace aweXpect;

public static partial class ThatVerificationResult
{
	/// <summary>
	///     Verifies that the checked interaction happened at least the number of <paramref name="times"/>.
	/// </summary>
	public static AndOrResult<VerificationResult<TVerify>, IThat<VerificationResult<TVerify>>> AtLeast<TVerify>(
		this IThat<VerificationResult<TVerify>> subject, Times times)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasAtLeastConstraint<TVerify>(expectationBuilder, it, grammars, times.Value)),
			subject);
}
