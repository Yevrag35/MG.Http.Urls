using MG.Http.Urls.Internal;
using MG.Http.Urls.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MG.Http.Urls.Queries
{
    /// <summary>
    /// Represents a query parameter, encapsulating key and value information, with support for 
    /// custom formatting and numeric conversion.
    /// This struct implements <see cref="IComparable{T}"/>, <see cref="IEquatable{T}"/>, 
    /// <see cref="ISpanFormattable"/>, and <see cref="IQueryParameter"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="QueryParameter"/> struct provides a robust way to handle query parameters, 
    /// allowing for different types of values (including formattable types) and supporting operations 
    /// like equality comparison and custom formatting.
    /// </remarks>
    public readonly partial struct QueryParameter : IComparable<QueryParameter>, IQueryParameter
    {
        const char EQUALS_SIGN = '=';

        readonly string? _key;
        readonly string? _value;
        readonly ISpanFormattable? _formattable;
        readonly int _maxLength;
        readonly bool _isNotEmpty;
        readonly bool _isFormattable;
        readonly string? _format;

        /// <summary>
        /// Indicates whether the query parameter is empty.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the query parameter is empty; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsEmpty => !_isNotEmpty;
        /// <summary>
        /// Indicates whether the query parameter is formattable.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the query parameter is formattable; otherwise, <see langword="false"/>.
        /// </value>
        [MemberNotNullWhen(true, nameof(_formattable))]
        public bool IsFormattable => _isFormattable;
        /// <inheritdoc cref="IQueryParameter.Key"/>
        public string Key => _key ?? string.Empty;
        /// <inheritdoc cref="IQueryParameter.MaxLength"/>
        public int MaxLength => _maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter"/> struct with the specified key.
        /// </summary>
        /// <param name="key">The key for the query parameter.</param>
        /// <inheritdoc cref="Guard.InvalidKey(string, string)" path="/exception"/>
        private QueryParameter(string key)
        {
            Guard.InvalidKey(key);
            _key = key;

            _value = null;
            _formattable = null;
            _isFormattable = false;
            _maxLength = 0;
            _isNotEmpty = true;
            _format = null;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter"/> struct with the specified key,
        /// value, length, and format.
        /// </summary>
        /// <param name="key">The key for the query parameter.</param>
        /// <param name="oneOf">The value of the query parameter, either as a string or an 
        /// <see cref="ISpanFormattable"/>.</param>
        /// <param name="oneOfLength">The length of the value.</param>
        /// <param name="format">The format to use for the value, if applicable.</param>
        /// <inheritdoc cref="Guard.InvalidKey(string?, string)" path="/exception"/>
        public QueryParameter(string key, OneOf<string?, ISpanFormattable> oneOf, int oneOfLength, string? format = null)
        {
            Guard.InvalidKey(key);
            _key = key;
            bool isStr = oneOf.TryPickT0(out string? val, out ISpanFormattable? remainder);
            _value = val;
            _formattable = remainder;
            _isFormattable = !isStr;

            _maxLength = key.Length + 1 + oneOfLength;
            _isNotEmpty = true;
            _format = format;
        }

        public int CompareTo(QueryParameter other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(_key, other._key);
        }
        public bool Equals(QueryParameter other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(_key, other._key);
        }
        public bool Equals(IQueryParameter? other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(_key, other?.Key);
        }
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is QueryParameter other && this.Equals(other);
        }
        public override int GetHashCode()
        {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.Key);
        }

        /// <summary>
        /// Converts the current instance to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of this instance.
        /// The output which looks like: <c>key=value</c>
        /// </returns>
        public override string ToString()
        {
            return this.ToString(null, null);
        }
        /// <summary>
        /// Converts the current instance to its equivalent string representation using the specified 
        /// format and culture-specific format information.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A string representation of the value of this instance, formatted as specified.
        /// The output which looks like: <c>key=value</c></returns>
        public string ToString(string? format, IFormatProvider? provider)
        {
            Span<char> span = stackalloc char[_maxLength];
            _ = this.TryFormat(span, out int written, format, provider);
            return new string(span.Slice(0, written));
        }
        /// <summary>
        /// Tries to format the current instance into the provided span of characters.
        /// </summary>
        /// <param name="span">The destination for the formatted characters.</param>
        /// <param name="written">When this method returns, contains the number of characters that 
        /// were written in <paramref name="span"/>.</param>
        /// <param name="format">The format to use.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// <see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryFormat(Span<char> span, out int written, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            if (this.IsEmpty)
            {
                written = 0;
                return true;
            }

            ReadOnlySpan<char> key = this.Key;
            written = key.Length;
            key.CopyTo(span);
            ref char first = ref span[0];
            first = char.ToLower(first);

            if (format.IsEmpty)
            {
                format = _format;
            }

            span[written++] = EQUALS_SIGN;
            if (this.IsFormattable && _formattable.TryFormat(span.Slice(written), out int formatWritten, format, provider))
            {
                written += formatWritten;
                return true;
            }
            else if (!string.IsNullOrEmpty(_value) && _value.TryCopyTo(span.Slice(written)))
            {
                written += _value.Length;
                return true;
            }
            else
            {
                return false;
            }
        }

#if NET7_0_OR_GREATER
        public bool TryValueAsNumber<T>(out T value) where T : struct, INumber<T>
        {
            if (this.IsEmpty)
            {
                value = default;
                return false;
            }

            if (this.IsFormattable && _formattable is T number)
            {
                value = number;
                return true;
            }
            else if (T.TryParse(_value.AsSpan(), null, out T result))
            {
                value = result;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
#endif
        /// <inheritdoc cref="QueryParameter(string, OneOf{string?, ISpanFormattable}, int, string?)"
        ///     path="/exception"/>
        public static implicit operator QueryParameter(KeyValuePair<string, string?> kvp)
        {
            return Create(kvp.Key, kvp.Value);
        }
        /// <inheritdoc cref = "QueryParameter(string)"/>
        public static explicit operator QueryParameter(string key)
        {
            return new(key);
        }

        public static bool operator ==(QueryParameter x, QueryParameter y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(QueryParameter x, QueryParameter y)
        {
            return !(x == y);
        }
    }
}

