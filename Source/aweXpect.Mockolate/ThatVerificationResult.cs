using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using Mockolate.Verify;

namespace aweXpect;

/// <summary>
///     Expectations on the <see cref="VerificationResult{TVerify}" /> returned from a mockolate Mock.
/// </summary>
public static partial class ThatVerificationResult
{
	private sealed class HasExactlyConstraint<TVerify>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		int expected)
		: ConstraintResult.WithValue<VerificationResult<TVerify>>(grammars),
			IValueConstraint<VerificationResult<TVerify>>
	{
		private int _count = -1;
		private string _expectation = "";

		public ConstraintResult IsMetBy(VerificationResult<TVerify> actual)
		{
			IVerificationResult<TVerify> result = actual;
			_expectation = result.Expectation;
			Actual = actual;
			Outcome = result.Verify(interactions =>
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
				stringBuilder.Append("never ").Append(_expectation);
			}
			else
			{
				stringBuilder.Append(_expectation).Append(" exactly ").Append(expected.ToAmountString());
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
				stringBuilder.Append(_expectation).Append(" at least once");
			}
			else
			{
				stringBuilder.Append(_expectation).Append(" not exactly ").Append(expected.ToAmountString());
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" was");

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

	private sealed class HasAtMostConstraint<TVerify>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		int expected)
		: ConstraintResult.WithValue<VerificationResult<TVerify>>(grammars),
			IValueConstraint<VerificationResult<TVerify>>
	{
		private int _count = -1;
		private string _expectation = "";

		public ConstraintResult IsMetBy(VerificationResult<TVerify> actual)
		{
			IVerificationResult<TVerify> result = actual;
			_expectation = result.Expectation;
			Actual = actual;
			Outcome = result.Verify(interactions =>
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
			stringBuilder.Append(_expectation).Append(" at most ").Append(expected.ToAmountString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("found ").Append(it).Append(' ').Append(_count.ToAmountString());
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(_expectation).Append(" more than ").Append(expected.ToAmountString());
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
				new MyDescribableSubject<TVerify>() is TValue describableSubject)
			{
				value = describableSubject;
				return true;
			}

			return base.TryGetValue(out value);
		}
	}

	private sealed class HasAtLeastConstraint<TVerify>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		int expected)
		: ConstraintResult.WithValue<VerificationResult<TVerify>>(grammars),
			IValueConstraint<VerificationResult<TVerify>>
	{
		private int _count = -1;
		private string _expectation = "";

		public ConstraintResult IsMetBy(VerificationResult<TVerify> actual)
		{
			IVerificationResult<TVerify> result = actual;
			_expectation = result.Expectation;
			Actual = actual;
			Outcome = result.Verify(interactions =>
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
			stringBuilder.Append(_expectation).Append(" at least ").Append(expected.ToAmountString());
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
			stringBuilder.Append(_expectation).Append(" less than ").Append(expected.ToAmountString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("found ").Append(it).Append(' ').Append(_count.ToAmountString());
		}

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

	private sealed class MyDescribableSubject<TVerify> : IDescribableSubject
	{
		public string GetDescription()
		{
			var mockVerify = Formatter.Format(typeof(TVerify));
			if (mockVerify.StartsWith("MockVerify", StringComparison.Ordinal))
			{
				mockVerify = $"Mock{mockVerify.Substring("MockVerify".Length)}";
				var genericSeparator = mockVerify.IndexOf(", ");
				if (genericSeparator > 0)
				{
					mockVerify = $"{mockVerify.Substring(0, genericSeparator)}>";
				}
			}
			return $"the {mockVerify}";
		}
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
