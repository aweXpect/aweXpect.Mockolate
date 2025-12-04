using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using Mockolate;
using Mockolate.Setup;
using Mockolate.Verify;

namespace aweXpect;

public static partial class ThatMockVerify
{
	/// <summary>
	///     Verifies that all setups on the mock have been used.
	/// </summary>
	public static AndOrResult<IMockVerify<TVerify>, IThat<IMockVerify<TVerify>>>
		AllSetupsAreUsed<TVerify>(
			this IThat<IMockVerify<TVerify>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((_, _, grammars)
				=> new AllSetupsAreUsedConstraint<TVerify>(grammars)),
			subject);

	private sealed class AllSetupsAreUsedConstraint<TVerify>(
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IMockVerify<TVerify>>(grammars),
			IValueConstraint<IMockVerify<TVerify>>
	{
		public ConstraintResult IsMetBy(IMockVerify<TVerify> actual)
		{
			Actual = actual;
			Outcome = actual.ThatAllSetupsAreUsed()
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has used all setups");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is IHasMockRegistration mockRegistration)
			{
				IReadOnlyCollection<ISetup> unusedSetups =
					mockRegistration.Registrations.GetUnusedSetups(mockRegistration.Registrations.Interactions);
				stringBuilder.Append("the following ");
				if (unusedSetups.Count == 1)
				{
					stringBuilder.Append("setup was not used:");
				}
				else
				{
					stringBuilder.Append(unusedSetups.Count)
						.Append(" setups were not used:");
				}

				stringBuilder.AppendLine().Append(" - ");
				stringBuilder.Append(string.Join($"{Environment.NewLine} - ", unusedSetups));
			}
			else
			{
				stringBuilder.Append("not all were");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has not used all setups");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were");

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
