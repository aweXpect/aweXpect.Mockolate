using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Verify;

namespace aweXpect;

public static partial class ThatVerificationResult
{
	/// <summary>
	///     Verifies that the checked interaction happened between <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<AndOrResult<VerificationResult<TVerify>, IThat<VerificationResult<TVerify>>>, Times>
		Between<TVerify>(
			this IThat<VerificationResult<TVerify>> subject, int minimum)
		=> new(maximum => new AndOrResult<VerificationResult<TVerify>, IThat<VerificationResult<TVerify>>>(subject.Get()
				.ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new HasBetweenConstraint<TVerify>(expectationBuilder, it, grammars, minimum, maximum.Value)),
			subject));
	
	private sealed class HasBetweenConstraint<TVerify>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		int minimum,
		int maximum)
		: ConstraintResult.WithValue<VerificationResult<TVerify>>(grammars),
			IValueConstraint<VerificationResult<TVerify>>
	{
		private int _count = -1;
		private string _expectation = "";

		public ConstraintResult IsMetBy(VerificationResult<TVerify> actual)
		{
			IVerificationResult result = actual;
			_expectation = result.Expectation;
			Actual = actual;
			Outcome = result.Verify(interactions =>
			{
				string context = Formatter.Format(interactions, FormattingOptions.MultipleLines);
				expectationBuilder.UpdateContexts(contexts => contexts.Add(
					new ResultContext.SyncCallback("Interactions", () => context)));
				_count = interactions.Length;
				return interactions.Length >= minimum && interactions.Length <= maximum;
			})
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_expectation).Append(" between ").Append(minimum).Append(" and ").Append(maximum)
				.Append(" times");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_count == 0)
			{
				stringBuilder.Append("never found ").Append(it);
			}
			else
			{
				stringBuilder.Append("found ").Append(it).Append(_count < minimum ? " only " : " ")
					.Append(_count.ToAmountString());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_expectation).Append(" not between ").Append(minimum).Append(" and ")
				.Append(maximum).Append(" times");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("found ").Append(it).Append(' ').Append(_count.ToAmountString());

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (typeof(TValue) == typeof(IDescribableSubject) &&
			    new MyDescribableSubject<TVerify>() is TValue describableSubject)
			{
				value = describableSubject;
				return true;
			}

			return base.TryGetValue(out value);
		}
	}
}
