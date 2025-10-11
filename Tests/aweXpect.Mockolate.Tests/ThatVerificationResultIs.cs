using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatVerificationResultIs
{
	public interface IMyService
	{
		int MyProperty { get; set; }
		int MyMethod(int value);
		int MyMethod(int value, bool flag);
	}
}
