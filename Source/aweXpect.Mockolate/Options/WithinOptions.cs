using System;
using System.Threading;

namespace aweXpect.Options;

/// <summary>
///     The options for a verification result which allows specifying a timeout and/or cancellation token for the
///     verification.
/// </summary>
public class WithinOptions
{
	/// <summary>
	///     The timeout that is applied to the verification.
	/// </summary>
	public TimeSpan? Timeout { get; set; }

	/// <summary>
	///     The cancellation token that is used to cancel the verification.
	/// </summary>
	public CancellationToken? CancellationToken { get; set; }
}
