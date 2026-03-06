using System;
using System.Threading;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of a verification result which allows specifying a timeout and/or cancellation token for the
///     verification.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TCollection, TThat}" />
/// </remarks>
public class AndOrWithinResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	WithinOptions options)
	: AndOrWithinResult<TType, TThat, AndOrWithinResult<TType, TThat>>(
		expectationBuilder,
		returnValue,
		options);

/// <inheritdoc cref="AndOrWithinResult{TType, TThat}" />
public class AndOrWithinResult<TType, TThat, TSelf> : AndOrResult<TType, TThat>
	where TSelf : AndOrWithinResult<TType, TThat, TSelf>
{
	private readonly WithinOptions _options;

	/// <inheritdoc cref="AndOrWithinResult{TType, TThat, TSelf}" />
	protected AndOrWithinResult(ExpectationBuilder expectationBuilder, TThat returnValue, WithinOptions options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

	/// <summary>
	///     …within the given <paramref name="timeout" />.
	/// </summary>
	public TSelf Within(TimeSpan timeout)
	{
		_options.Timeout = timeout;
		return (TSelf)this;
	}

	/// <summary>
	///     …with the given <paramref name="cancellationToken" />.
	/// </summary>
	public new TSelf WithCancellation(CancellationToken cancellationToken)
	{
		_options.CancellationToken = cancellationToken;
		base.WithCancellation(cancellationToken);
		return (TSelf)this;
	}
}
