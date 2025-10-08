using Mockolate;
using Xunit.Sdk;

namespace aweXpect.Mockolate.Tests;

public sealed partial class ThatCheckResultIs
{
	public interface IMyService
	{
		int MyProperty { get; set; }
		int MyMethod(int value, bool flag);
	}
}
