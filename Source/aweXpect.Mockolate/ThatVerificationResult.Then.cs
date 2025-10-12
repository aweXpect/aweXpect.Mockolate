using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate;
using Mockolate.Verify;
using Mockolate.Interactions;

namespace aweXpect;

public static partial class ThatVerificationResult
{
	/// <summary>
	///     Verifies that the <paramref name="interactions" /> happen after the current interaction in the given order.
	/// </summary>
	public static AndOrResult<VerificationResult<TVerify>, IThat<VerificationResult<TVerify>>> Then<TVerify>(
		this IThat<VerificationResult<TVerify>> subject, params Func<TVerify, VerificationResult<TVerify>>[] interactions)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new ThenConstraint<TVerify>(expectationBuilder, it, grammars, interactions)),
			subject);

	private sealed class ThenConstraint<TVerify>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TVerify, VerificationResult<TVerify>>[] interactions)
		: ConstraintResult.WithValue<VerificationResult<TVerify>>(grammars),
			IValueConstraint<VerificationResult<TVerify>>
	{
		private readonly List<string> _expectations = [];
		private string? _error;

		public ConstraintResult IsMetBy(VerificationResult<TVerify> actual)
		{
			Actual = actual;
			_expectations.Clear();
			bool result = true;
			IVerificationResult<TVerify> verificationResult = actual;
			int after = -1;
			foreach (Func<TVerify, VerificationResult<TVerify>>? check in interactions)
			{
				_expectations.Add(verificationResult.Expectation);
				if (!verificationResult.Verify(VerifyInteractions))
				{
					result = false;
				}
				verificationResult = check(verificationResult.Object);
			}

			_expectations.Add(verificationResult.Expectation);
			result = verificationResult.Verify(VerifyInteractions) && result;
			Outcome = result ? Outcome.Success : Outcome.Failure;
			/* TODO: Disable and check if it can be re-enabled with `IMockVerify`
			if (!result)
			{
				string context = Formatter.Format(((IVerificationResult<TVerify>)actual).Interactions.Interactions, FormattingOptions.MultipleLines);
				expectationBuilder.UpdateContexts(contexts => contexts.Add(
					new ResultContext("Interactions", () => context)));
			}
			*/
			_ = expectationBuilder;
			return this;
			bool VerifyInteractions(IInteraction[] interactions)
			{
				bool hasInteractionAfter = interactions.Any(x => x.Index > after);
				after = hasInteractionAfter
					? interactions.Where(x => x.Index > after).Min(x => x.Index)
					: int.MaxValue;
				if (!hasInteractionAfter && _error is null)
				{
					_error = interactions.Length > 0 ? $"{verificationResult.Expectation} too early" : $"{verificationResult.Expectation} not at all";
				}
				return hasInteractionAfter;
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			var separator = $", then{Environment.NewLine}{indentation}";
			stringBuilder.Append(string.Join(separator, _expectations)).Append(" in order");
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(' ').Append(_error);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			var separator = $", then{Environment.NewLine}{indentation}";
			stringBuilder.Append(string.Join(separator, _expectations)).Append(" not in order");
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did");

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
