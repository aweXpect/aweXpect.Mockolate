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
using Mockolate.Checks;
using Mockolate.Interactions;

namespace aweXpect;

public static partial class ThatCheckResult
{
	/// <summary>
	///     Verifies that the <paramref name="interactions" /> happen after the current interaction in the given order.
	/// </summary>
	public static AndOrResult<CheckResult<TMock>, IThat<CheckResult<TMock>>> Then<TMock>(
		this IThat<CheckResult<TMock>> subject, params Func<TMock, CheckResult<TMock>>[] interactions)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new ThenConstraint<TMock>(expectationBuilder, it, grammars, interactions)),
			subject);

	private sealed class ThenConstraint<TMock>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TMock, CheckResult<TMock>>[] interactions)
		: ConstraintResult.WithValue<CheckResult<TMock>>(grammars),
			IValueConstraint<CheckResult<TMock>>
	{
		private readonly List<string> _expectations = [];
		private string? _error;

		public ConstraintResult IsMetBy(CheckResult<TMock> actual)
		{
			Actual = actual;
			_expectations.Clear();
			bool result = true;
			CheckResult<TMock> checkResult = actual;
			int after = -1;
			foreach (Func<TMock, CheckResult<TMock>>? check in interactions)
			{
				_expectations.Add(checkResult.Expectation);
				if (!checkResult.Verify(VerifyInteractions))
				{
					result = false;
				}
				checkResult = check(checkResult.Mock);
			}

			_expectations.Add(checkResult.Expectation);
			result = checkResult.Verify(VerifyInteractions) && result;
			Outcome = result ? Outcome.Success : Outcome.Failure;
			if (!result)
			{
				string context = Formatter.Format(((IMock)actual.Mock!).Interactions.Interactions, FormattingOptions.MultipleLines);
				expectationBuilder.UpdateContexts(contexts => contexts.Add(
					new ResultContext("Interactions", () => context)));
			}
			return this;
			bool VerifyInteractions(IInteraction[] interactions)
			{
				bool hasInteractionAfter = interactions.Any(x => x.Index > after);
				after = hasInteractionAfter
					? interactions.Where(x => x.Index > after).Min(x => x.Index)
					: int.MaxValue;
				if (!hasInteractionAfter && _error is null)
				{
					_error = interactions.Length > 0 ? $"{checkResult.Expectation} too early" : $"{checkResult.Expectation} not at all";
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
				new MyDescribableSubject<TMock>() is TValue describableSubject)
			{
				value = describableSubject;
				return true;
			}

			return base.TryGetValue(out value);
		}
	}
}
