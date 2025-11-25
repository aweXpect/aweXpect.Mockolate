using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
	///     Verifies that the checked interaction happened according to the <paramref name="predicate" />.
	/// </summary>
	public static AndOrResult<VerificationResult<TVerify>, IThat<VerificationResult<TVerify>>> Times<TVerify>(
		this IThat<VerificationResult<TVerify>> subject, Func<int, bool> predicate,
		[CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "")
		=> new(subject.Get()
				.ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new TimesConstraint<TVerify>(expectationBuilder, it, grammars, predicate,
						doNotPopulateThisValue)),
			subject);

	private sealed class TimesConstraint<TVerify>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<int, bool> predicate,
		string predicateExpression)
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
				return predicate(_count);
			})
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_expectation).Append(" according to the predicate ").Append(predicateExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_count == 0)
			{
				stringBuilder.Append("never found ").Append(it);
			}
			else
			{
				stringBuilder.Append("found ").Append(it).Append(' ').Append(_count.ToAmountString());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_expectation).Append(" not according to the predicate ")
				.Append(predicateExpression);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

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
