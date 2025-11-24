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
using Mockolate.Exceptions;
using Mockolate.Verify;
using Mockolate.Interactions;

namespace aweXpect;

public static partial class ThatVerificationResult
{
	/// <summary>
	///     Verifies that the <paramref name="interactions" /> happen after the current interaction in the given order.
	/// </summary>
	public static AndOrResult<VerificationResult<T>, IThat<VerificationResult<T>>> Then<T>(
		this IThat<VerificationResult<T>> subject, params Func<IMockVerify<T>, VerificationResult<T>>[] interactions)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new ThenConstraint<T>(expectationBuilder, it, grammars, interactions)),
			subject);

	private sealed class ThenConstraint<T>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<IMockVerify<T>, VerificationResult<T>>[] interactions)
		: ConstraintResult.WithValue<VerificationResult<T>>(grammars),
			IValueConstraint<VerificationResult<T>>
	{
		private readonly List<string> _expectations = [];
		private string? _error;

		public ConstraintResult IsMetBy(VerificationResult<T> actual)
		{
			Actual = actual;
			_expectations.Clear();
			bool result = true;
			IMockVerify<T> verify = GetMockVerify(((IVerificationResult<T>)actual).Object);
			IVerificationResult verificationResult = actual;
			int after = -1;
			foreach (Func<IMockVerify<T>, VerificationResult<T>> check in interactions)
			{
				IVerificationResult currentVerificationResult = verificationResult;
				_expectations.Add(currentVerificationResult.Expectation);
				if (!verificationResult.Verify(i => VerifyInteractions(i, currentVerificationResult)))
				{
					result = false;
				}
				verificationResult = check(verify);
			}

			_expectations.Add(verificationResult.Expectation);
			result = verificationResult.Verify(i => VerifyInteractions(i, verificationResult)) && result;
			Outcome = result ? Outcome.Success : Outcome.Failure;
			if (!result)
			{
				string context = Formatter.Format(((IVerificationResult)actual).MockInteractions.Interactions, FormattingOptions.MultipleLines);
				expectationBuilder.UpdateContexts(contexts => contexts.Add(
					new ResultContext.SyncCallback("Interactions", () => context)));
			}
			return this;
			bool VerifyInteractions(IInteraction[] filteredInteractions, IVerificationResult currentVerificationResult)
			{
				bool hasInteractionAfter = filteredInteractions.Any(x => x.Index > after);
				after = hasInteractionAfter
					? filteredInteractions.Where(x => x.Index > after).Min(x => x.Index)
					: int.MaxValue;
				if (!hasInteractionAfter && _error is null)
				{
					_error = filteredInteractions.Length > 0 ? $"{currentVerificationResult.Expectation} too early" : $"{currentVerificationResult.Expectation} not at all";
				}
				return hasInteractionAfter;
			}
			static IMockVerify<T> GetMockVerify(T subject)
			{
				if (subject is IMockSubject<T> mockSubject)
				{
					return mockSubject.Mock;
				}

				if (subject is IHasMockRegistration hasMockRegistration)
				{
					return new Mock<T>(subject, hasMockRegistration.Registrations);
				}

				throw new MockException("The subject is no mock subject.");
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
				new MyDescribableSubject<T>() is TValue describableSubject)
			{
				value = describableSubject;
				return true;
			}

			return base.TryGetValue(out value);
		}
	}
}
