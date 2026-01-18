#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Mockolate.Parameters;

// ReSharper disable once CheckNamespace
namespace Mockolate.Web;

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
/// <summary>
///     Extensions for parameter matchers for HTTP-related types.
/// </summary>
public static class AweXpectItExtensions
{
	/// <inheritdoc cref="ItExtensions" />
	extension(It _)
	{
		/// <summary>
		///     Expects the <see cref="HttpContent" /> parameter to be a JSON content.
		/// </summary>
		public static IJsonContentParameter IsJsonContent()
			=> new JsonContentParameter();

		/// <summary>
		///     Expects the <see cref="HttpContent" /> parameter to be a JSON content
		///     with the given <paramref name="mediaType" />.
		/// </summary>
		public static IJsonContentParameter IsJsonContent(string mediaType)
			=> new JsonContentParameter().WithMediaType(mediaType);
	}

	/// <summary>
	///     Further expectations on the JSON <see cref="HttpContent" />.
	/// </summary>
	public interface IJsonContentParameter : ItExtensions.IHttpContentParameter<IJsonContentParameter>
	{
		/// <summary>
		///     Expects the <see cref="StringContent" /> to have a body equal to the given <paramref name="json" />.
		/// </summary>
		IJsonContentBodyParameter WithBody(string json, JsonDocumentOptions? options = null);

		/// <summary>
		///     Expects the <see cref="StringContent" /> to have a JSON body which matches the <paramref name="expected" /> value.
		/// </summary>
		IJsonContentBodyParameter WithBodyMatching(object? expected, JsonDocumentOptions? options = null);

		/// <summary>
		///     Expects the <see cref="StringContent" /> to have a JSON body which matches the <paramref name="expected" /> value.
		/// </summary>
		IJsonContentBodyParameter WithBodyMatching<T>(IEnumerable<T> expected, JsonDocumentOptions? options = null);
	}

	/// <summary>
	///     Further expectations on the matching of a JSON body of the <see cref="HttpContent" />.
	/// </summary>
	public interface IJsonContentBodyParameter : IJsonContentParameter
	{
		/// <summary>
		///     Ignores additional properties in JSON objects when comparing.
		/// </summary>
		IJsonContentBodyParameter IgnoringAdditionalProperties(bool ignoreAdditionalProperties = true);
	}

	private sealed class JsonContentParameter : HttpContentParameter<IJsonContentParameter>, IJsonContentBodyParameter
	{
		private string? _body;
		private bool _ignoringAdditionalProperties = true;
		private JsonDocumentOptions? _jsonDocumentOptions;

		/// <inheritdoc cref="HttpContentParameter{TParameter}.GetThis" />
		protected override IJsonContentParameter GetThis => this;

		/// <inheritdoc cref="IJsonContentParameter.WithBody(string, JsonDocumentOptions?)" />
		public IJsonContentBodyParameter WithBody(string json,
			JsonDocumentOptions? options = null)
		{
			_body = json;
			_jsonDocumentOptions = options;
			return this;
		}

		/// <inheritdoc cref="IJsonContentParameter.WithBodyMatching(object, JsonDocumentOptions?)" />
		public IJsonContentBodyParameter WithBodyMatching(object? expected,
			JsonDocumentOptions? options = null)
			=> WithBody(JsonSerializer.Serialize(expected, JsonSerializerOptions.Default), options);

		public IJsonContentBodyParameter WithBodyMatching<T>(IEnumerable<T> expected,
			JsonDocumentOptions? options = null)
			=> WithBody(JsonSerializer.Serialize<object>(expected, JsonSerializerOptions.Default), options);

		/// <inheritdoc cref="IJsonContentBodyParameter.IgnoringAdditionalProperties(bool)" />
		public IJsonContentBodyParameter IgnoringAdditionalProperties(bool ignoreAdditionalProperties = true)
		{
			_ignoringAdditionalProperties = ignoreAdditionalProperties;
			return this;
		}

		protected override bool Matches(HttpContent value)
		{
			if (!base.Matches(value))
			{
				return false;
			}

			if (_body is not null)
			{
				try
				{
					JsonDocumentOptions options = _jsonDocumentOptions ?? GetDefaultOptions();
					using JsonDocument actualDocument = JsonDocument.Parse(value.ReadAsStream(), options);
					using JsonDocument expectedDocument = JsonDocument.Parse(_body, options);

					if (!Compare(actualDocument.RootElement, expectedDocument.RootElement,
						    _ignoringAdditionalProperties))
					{
						return false;
					}
				}
				catch (JsonException)
				{
					return false;
				}
			}

			return true;
		}

		private JsonDocumentOptions GetDefaultOptions() => new()
		{
			AllowTrailingCommas = true,
		};

		private static bool Compare(
			JsonElement actualElement,
			JsonElement expectedElement,
			bool ignoreAdditionalProperties)
		{
			if (actualElement.ValueKind != expectedElement.ValueKind)
			{
				return false;
			}

			return actualElement.ValueKind switch
			{
				JsonValueKind.Array => CompareJsonArray(actualElement, expectedElement, ignoreAdditionalProperties),
				JsonValueKind.Number => CompareJsonNumber(actualElement, expectedElement),
				JsonValueKind.String => CompareJsonString(actualElement, expectedElement),
				JsonValueKind.Object => CompareJsonObject(actualElement, expectedElement, ignoreAdditionalProperties),
				_ => true,
			};
		}

		private static bool CompareJsonObject(JsonElement actualElement, JsonElement expectedElement,
			bool ignoreAdditionalProperties)
		{
			foreach (JsonProperty item in expectedElement.EnumerateObject())
			{
				if (!actualElement.TryGetProperty(item.Name, out JsonElement property))
				{
					return false;
				}

				if (!Compare(property, item.Value, ignoreAdditionalProperties))
				{
					return false;
				}
			}

			if (!ignoreAdditionalProperties)
			{
				foreach (JsonProperty property in actualElement.EnumerateObject())
				{
					if (!expectedElement.TryGetProperty(property.Name, out _))
					{
						return false;
					}
				}
			}

			return true;
		}

		private static bool CompareJsonArray(JsonElement actualElement, JsonElement expectedElement,
			bool ignoreAdditionalProperties)
		{
			for (int index = 0; index < expectedElement.GetArrayLength(); index++)
			{
				JsonElement expectedArrayElement = expectedElement[index];
				if (actualElement.GetArrayLength() <= index)
				{
					return false;
				}

				JsonElement actualArrayElement = actualElement[index];
				if (!Compare(actualArrayElement, expectedArrayElement, ignoreAdditionalProperties))
				{
					return false;
				}
			}

			return ignoreAdditionalProperties || actualElement.GetArrayLength() <= expectedElement.GetArrayLength();
		}

		private static bool CompareJsonString(JsonElement actualElement, JsonElement expectedElement)
		{
			string? value1 = actualElement.GetString();
			string? value2 = expectedElement.GetString();
			return value1 == value2;
		}

		private static bool CompareJsonNumber(JsonElement actualElement, JsonElement expectedElement)
		{
			if (actualElement.TryGetInt32(out int v1) && expectedElement.TryGetInt32(out int v2))
			{
				return v1 == v2;
			}

			if (actualElement.TryGetDouble(out double n1) && expectedElement.TryGetDouble(out double n2))
			{
				return n1.Equals(n2);
			}

			return false;
		}
	}

	private abstract class HttpContentParameter<TParameter>
		: ItExtensions.IHttpContentParameter<TParameter>, IParameter
	{
		private List<Action<HttpContent?>>? _callbacks;
		private string? _mediaType;

		/// <summary>
		///     Returns <c>this</c> typed as <typeparamref name="TParameter" /> for fluent API.
		/// </summary>
		protected abstract TParameter GetThis { get; }

		/// <inheritdoc cref="ItExtensions.IHttpContentParameter{TParameter}.WithMediaType(string?)" />
		public TParameter WithMediaType(string? mediaType)
		{
			_mediaType = mediaType;
			return GetThis;
		}

		/// <inheritdoc cref="IParameter{T}.Do(Action{T})" />
		public IParameter<HttpContent?> Do(Action<HttpContent?> callback)
		{
			_callbacks ??= [];
			_callbacks.Add(callback);
			return this;
		}

		/// <inheritdoc cref="IParameter.Matches(object?)" />
		public bool Matches(object? value)
			=> value is HttpContent typedValue && Matches(typedValue);

		/// <inheritdoc cref="IParameter.InvokeCallbacks(object?)" />
		public void InvokeCallbacks(object? value)
		{
			if (value is HttpContent httpContent)
			{
				_callbacks?.ForEach(a => a.Invoke(httpContent));
			}
		}

		/// <summary>
		///     Checks whether the given <see cref="HttpContent" /> <paramref name="value" /> matches the expectations.
		/// </summary>
		protected virtual bool Matches(HttpContent value)
		{
			if (_mediaType is not null &&
			    value.Headers.ContentType?.MediaType?.Equals(_mediaType, StringComparison.OrdinalIgnoreCase) != true)
			{
				return false;
			}

			return true;
		}
	}
}
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
#endif
