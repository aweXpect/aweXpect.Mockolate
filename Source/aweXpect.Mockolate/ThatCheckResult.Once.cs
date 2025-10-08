using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate.Checks;

namespace aweXpect;

/// <summary>
///     See https://awexpect.com/docs/extensions/write-extensions
/// </summary>
public static class ThatCheckResult
{
	/// <summary>
	///     Verifies that the <paramref name="subject" /> is an absolute path.
	/// </summary>
	public static AndOrResult<CheckResult<TMock>, IThat<CheckResult<TMock>>> Once<TMock>(
		this IThat<CheckResult<TMock>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new CheckResultConstraint<TMock>(expectationBuilder, it, grammars)),
			subject);

	private sealed class CheckResultConstraint<TMock>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<CheckResult<TMock>>(grammars),
			IValueConstraint<CheckResult<TMock>>
	{
		private int _count = 1;
		
		public ConstraintResult IsMetBy(CheckResult<TMock> actual)
		{
			Actual = actual;
			Outcome = actual.Verify(interactions =>
			{
				var context = string.Join(", ", interactions.Select(x => x.ToString()));
				expectationBuilder.UpdateContexts(contexts => contexts.Add(
					new ResultContext("Interactions", () => context)));
				_count = interactions.Length;
				return interactions.Length == 1;
			})
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Actual?.Expectation).Append(" once");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("found ").Append(it).Append(" ").Append(_count).Append(" times");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Actual?.Expectation).Append(" not once");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" was");
	}
}
