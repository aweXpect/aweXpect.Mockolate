using BenchmarkDotNet.Attributes;

namespace aweXpect.Mockolate.Benchmarks;

/// <summary>
///     This is a dummy benchmark in the Mockolate template.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public TimeSpan Dummy_aweXpect()
		=> TimeSpan.FromSeconds(10);
}
