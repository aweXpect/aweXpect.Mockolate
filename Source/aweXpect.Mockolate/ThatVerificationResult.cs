using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using Mockolate.Verify;

namespace aweXpect;

/// <summary>
///     Expectations on the <see cref="VerificationResult{TMock}" /> returned from a mockolate Mock.
/// </summary>
public static partial class ThatVerificationResult
{
	private sealed class HasExactlyConstraint<TMock>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		int expected)
		: ConstraintResult.WithValue<VerificationResult<TMock>>(grammars),
			IValueConstraint<VerificationResult<TMock>>
	{
		private int _count = -1;

		public ConstraintResult IsMetBy(VerificationResult<TMock> actual)
		{
			Actual = actual;
			Outcome = actual.Verify(interactions =>
			{
				string context = Formatter.Format(interactions, FormattingOptions.MultipleLines);
				expectationBuilder.UpdateContexts(contexts => contexts.Add(
					new ResultContext("Interactions", () => context)));
				_count = interactions.Length;
				return interactions.Length == expected;
			})
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected == 0)
			{
				stringBuilder.Append("never ").Append(Actual?.Expectation);
			}
			else
			{
				stringBuilder.Append(Actual?.Expectation).Append(" exactly ").Append(expected.ToAmountString());
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_count == 0)
			{
				stringBuilder.Append("never found ").Append(it);
			}
			else
			{
				stringBuilder.Append("found ").Append(it).Append(_count < expected ? " only " : " ").Append(_count.ToAmountString());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected == 0)
			{
				stringBuilder.Append(Actual?.Expectation).Append(" at least once");
			}
			else
			{
				stringBuilder.Append(Actual?.Expectation).Append(" not exactly ").Append(expected.ToAmountString());
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" was");

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

	private sealed class HasAtMostConstraint<TMock>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		int expected)
		: ConstraintResult.WithValue<VerificationResult<TMock>>(grammars),
			IValueConstraint<VerificationResult<TMock>>
	{
		private int _count = -1;

		public ConstraintResult IsMetBy(VerificationResult<TMock> actual)
		{
			Actual = actual;
			Outcome = actual.Verify(interactions =>
			{
				string context = Formatter.Format(interactions, FormattingOptions.MultipleLines);
				expectationBuilder.UpdateContexts(contexts => contexts.Add(
					new ResultContext("Interactions", () => context)));
				_count = interactions.Length;
				return interactions.Length <= expected;
			})
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Actual?.Expectation).Append(" at most ").Append(expected.ToAmountString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("found ").Append(it).Append(' ').Append(_count.ToAmountString());
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Actual?.Expectation).Append(" more than ").Append(expected.ToAmountString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_count == 0)
			{
				stringBuilder.Append("never found ").Append(it);
			}
			else
			{
				stringBuilder.Append("found ").Append(it).Append(" only ").Append(_count.ToAmountString());
			}
		}

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

	private sealed class HasAtLeastConstraint<TMock>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		int expected)
		: ConstraintResult.WithValue<VerificationResult<TMock>>(grammars),
			IValueConstraint<VerificationResult<TMock>>
	{
		private int _count = -1;

		public ConstraintResult IsMetBy(VerificationResult<TMock> actual)
		{
			Actual = actual;
			Outcome = actual.Verify(interactions =>
			{
				string context = Formatter.Format(interactions, FormattingOptions.MultipleLines);
				expectationBuilder.UpdateContexts(contexts => contexts.Add(
					new ResultContext("Interactions", () => context)));
				_count = interactions.Length;
				return interactions.Length >= expected;
			})
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Actual?.Expectation).Append(" at least ").Append(expected.ToAmountString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_count == 0)
			{
				stringBuilder.Append("never found ").Append(it);
			}
			else
			{
				stringBuilder.Append("found ").Append(it).Append(" only ").Append(_count.ToAmountString());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Actual?.Expectation).Append(" less than ").Append(expected.ToAmountString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("found ").Append(it).Append(' ').Append(_count.ToAmountString());
		}

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

	private sealed class MyDescribableSubject<TMock> : IDescribableSubject
	{
		public string GetDescription() => $"the {Formatter.Format(typeof(TMock))}";
	}

	private static string ToAmountString(this int number)
		=> number switch
		{
			0 => "never",
			1 => "once",
			2 => "twice",
			_ => $"{number} times"
		};
}
