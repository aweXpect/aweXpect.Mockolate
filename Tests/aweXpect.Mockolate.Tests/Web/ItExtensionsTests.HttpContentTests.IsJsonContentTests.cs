#if NET8_0_OR_GREATER
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Mockolate;
using Mockolate.Web;

namespace aweXpect.Mockolate.Tests.Web;

public sealed partial class ItExtensionsTests
{
	public sealed class HttpContentTests
	{
		public sealed class IsJsonContentTests
		{
			[Theory]
			[InlineData("true", true, true)]
			[InlineData("true", false, false)]
			[InlineData("false", true, false)]
			[InlineData("false", false, true)]
			public async Task BooleanValue_ShouldSucceedWhenMatching(string body, bool expected, bool expectSuccess)
			{
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(expected))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent(body),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(expectSuccess ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
			}

			[Theory]
			[InlineData("42.1", 42.1, true)]
			[InlineData("1.2", 2.1, false)]
			public async Task DoubleValue_ShouldSucceedWhenMatching(string body, double expected, bool expectSuccess)
			{
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(expected))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent(body),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(expectSuccess ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
			}

			[Theory]
			[InlineData("42", 42, true)]
			[InlineData("1", 2, false)]
			public async Task IntegerValue_ShouldSucceedWhenMatching(string body, int expected, bool expectSuccess)
			{
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(expected))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent(body),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(expectSuccess ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
			}

			[Theory]
			[InlineData("null", true)]
			[InlineData("{}", false)]
			public async Task NullValue_ShouldSucceedWhenMatching(string body, bool expectSuccess)
			{
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(null))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent(body),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(expectSuccess ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
			}

			[Fact]
			public async Task ShouldSupportNestedObjects()
			{
				string body = "[{\"foo\": 2}, {\"foo\": 3}, {\"foo\": 4}]";
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching([
						new
						{
							foo = 2,
						},
						new
						{
							foo = 3,
						},
						new
						{
							foo = 4,
						},
					]))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent(body),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(HttpStatusCode.OK);
			}

			[Theory]
			[InlineData("\"foo\"", "foo", true)]
			[InlineData("\"foo\"", "bar", false)]
			public async Task StringValue_ShouldSucceedWhenMatching(string body, string expected, bool expectSuccess)
			{
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(expected))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent(body),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(expectSuccess ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
			}

			[Theory]
			[InlineData("{\n  \"foo\": 1,\n  \"bar\": 2,\n}", "{\"bar\":2,\"foo\": 1}", true)]
			[InlineData("\"foo\"", "\"foo\"", true)]
			[InlineData("foo", "bar", false)]
			public async Task WithBody_ShouldCompareAsJson(string body,
				string expected, bool expectSuccess)
			{
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJson(expected))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent(body),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(expectSuccess ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
			}

			[Fact]
			public async Task WithBodyMatching_ShouldCompareAsJson()
			{
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(new
					{
						foo = 1,
						bar = 2,
					}))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent("""
					                  {
					                    "bar": 2,
					                    "foo": 1
					                  }
					                  """),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(HttpStatusCode.OK);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WithBodyMatching_WithAdditionalProperties_ShouldMatchWhenIgnoringAdditionalProperties(
				bool ignoreAdditionalProperties)
			{
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(new
					{
						foo = 1,
						bar = 2,
					}).IgnoringAdditionalProperties(ignoreAdditionalProperties))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent("""
					                  {
					                    "bar": 2,
					                    "foo": 1,
					                    "baz": null,
					                  }
					                  """),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(ignoreAdditionalProperties ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
			}

			[Theory]
			[InlineData("[1, 2,]", "[1, 2,]", true)]
			[InlineData("[1, 2,]", "[1, 2,]", false)]
			[InlineData("[1, 2]", "[1, 2,]", false)]
			[InlineData("[1, 2,]", "[1, 2]", false)]
			public async Task WithOptions_ShouldApplyOptions(string body, string expected, bool allowTrailingCommas)
			{
				HttpClient httpClient = Mock.Create<HttpClient>();
				httpClient.SetupMock.Method
					.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJson(expected, new JsonDocumentOptions
					{
						AllowTrailingCommas = allowTrailingCommas,
					}))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

				HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
					new StringContent(body),
					CancellationToken.None);

				await That(result.StatusCode)
					.IsEqualTo(allowTrailingCommas ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
			}

			public sealed class ArrayTests
			{
				[Theory]
				[MemberData(nameof(MatchingArrayValues))]
				public async Task MatchingValues_ShouldSucceed(string[] expected, string body)
				{
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(expected))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsEqualTo(HttpStatusCode.OK);
				}

				[Theory]
				[MemberData(nameof(NotMatchingArrayValues))]
				public async Task NotMatchingValues_ShouldFail(string[] expected, string body)
				{
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(expected))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsNotEqualTo(HttpStatusCode.OK);
				}

				[Fact]
				public async Task WhenElementsAreInDifferentOrder_ShouldFail()
				{
					string body = "[1, 2]";
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching([2, 1,]))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsNotEqualTo(HttpStatusCode.OK);
				}

				[Fact]
				public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
				{
					string body = "[1, 2]";
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching([1, 2, 3,]))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsNotEqualTo(HttpStatusCode.OK);
				}

				[Fact]
				public async Task WhenSubjectContainsAdditionalElements_ShouldSucceed()
				{
					string body = "[1, 2, 3]";
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching([1, 2,]))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsEqualTo(HttpStatusCode.OK);
				}

				[Fact]
				public async Task WhenSubjectContainsAdditionalElements_WhenNotIgnoringAdditionalProperties_ShouldFail()
				{
					string body = "[1, 2, 3]";
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(),
							It.IsHttpContent().WithJsonMatching([1, 2,]).IgnoringAdditionalProperties(false))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsNotEqualTo(HttpStatusCode.OK);
				}

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WithOptions_ShouldApplyOptions(bool allowTrailingCommas)
				{
					string body = "[1, 2,]";
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching([1, 2,], new JsonDocumentOptions
						{
							AllowTrailingCommas = allowTrailingCommas,
						}))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsEqualTo(allowTrailingCommas ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
				}

				public static TheoryData<string[], string> MatchingArrayValues
					=> new()
					{
						{
							[], "[]"
						},
						{
							[], "[\"foo\"]"
						},
						{
							[
								"foo", "bar",
							],
							"[\"foo\", \"bar\"]"
						},
					};

				public static TheoryData<string[], string> NotMatchingArrayValues
					=> new()
					{
						{
							[
								"foo",
							],
							"[]"
						},
						{
							[
								"bar", "foo",
							],
							"[\"foo\", \"bar\"]"
						},
					};
			}

			public sealed class ObjectTests
			{
				[Theory]
				[InlineData("{}", false)]
				[InlineData("{\"foo\": 2}", true)]
				public async Task ShouldFailIfPropertyIsMissing(string body, bool expectSuccess)
				{
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(new
						{
							foo = 2,
						}))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsEqualTo(expectSuccess ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
				}

				[Theory]
				[InlineData("{}")]
				[InlineData("{\"foo\": 1}")]
				public async Task WhenExpectedIsEmpty_ShouldSucceed(string body)
				{
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(new object()))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsEqualTo(HttpStatusCode.OK);
				}

				[Fact]
				public async Task WhenPropertyHasDifferentValue_ShouldFail()
				{
					string body = "{\"bar\": 2}";
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(new
						{
							bar = 3,
						}))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsNotEqualTo(HttpStatusCode.OK);
				}

				[Fact]
				public async Task WhenSubjectHasAdditionalProperties_ShouldSucceed()
				{
					string body = "{\"foo\": null, \"bar\": 2}";
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(new
						{
							bar = 2,
						}))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsEqualTo(HttpStatusCode.OK);
				}

				[Fact]
				public async Task WhenSubjectHasAdditionalProperties_WhenNotIgnoringAdditionalProperties_ShouldFail()
				{
					string body = "{\"foo\": null, \"bar\": 2}";
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(new
						{
							bar = 2,
						}).IgnoringAdditionalProperties(false))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsNotEqualTo(HttpStatusCode.OK);
				}

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WithOptions_ShouldApplyOptions(bool allowTrailingCommas)
				{
					string body = "{\"foo\": 1,}";
					HttpClient httpClient = Mock.Create<HttpClient>();
					httpClient.SetupMock.Method
						.PostAsync(It.IsAny<Uri>(), It.IsHttpContent().WithJsonMatching(new
							{
								foo = 1,
							},
							new JsonDocumentOptions
							{
								AllowTrailingCommas = allowTrailingCommas,
							}))
						.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

					HttpResponseMessage result = await httpClient.PostAsync("https://www.aweXpect.com",
						new StringContent(body),
						CancellationToken.None);

					await That(result.StatusCode)
						.IsEqualTo(allowTrailingCommas ? HttpStatusCode.OK : HttpStatusCode.NotImplemented);
				}
			}
		}
	}
}
#endif
