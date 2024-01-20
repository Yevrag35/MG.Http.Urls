using MG.Http.Urls.Internal;
using System.Numerics;

namespace MG.Http.Urls.Queries
{
    /// <summary>
    /// Represents a boolean query parameter, encapsulating a key and a boolean value, 
    /// with support for formatting as part of a query string.
    /// This struct extends the <see cref="QueryParameter"/> and is specialized to handle boolean values.
    /// </summary>
    /// <remarks>
    /// The <see cref="QueryBooleanParameter"/> struct is useful for representing parameters in a query 
    /// where the value is distinctly boolean (true or false). It provides an efficient way to handle, 
    /// format, and compare these boolean query parameters.
    /// </remarks>
    public readonly struct QueryBooleanParameter : IQueryParameter
    {
        readonly string? _key;
        readonly bool _value;
        readonly int _maxLength;
        readonly bool _isNotEmpty;

        /// <inheritdoc cref="QueryParameter.IsEmpty"/>
        public bool IsEmpty => !_isNotEmpty;
        public string Key => _key ?? string.Empty;
        public int MaxLength => _maxLength;
        /// <summary>
        /// The <see cref="bool"/> value of the query parameter.
        /// </summary>
        public bool Value => _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBooleanParameter"/> struct with the 
        /// specified key and <see cref="bool"/> value.
        /// </summary>
        /// <inheritdoc cref="Guard.InvalidKey(string?, string)" path="/exception"/>
        internal QueryBooleanParameter(string key, in bool value)
        {
            Guard.InvalidKey(key);
            _key = key;
            _value = value;
            _maxLength = key.Length + 1 + bool.FalseString.Length;
            _isNotEmpty = true;
        }

        public bool Equals(IQueryParameter? other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(_key, other?.Key);
        }
        public bool Equals(QueryParameter other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(_key, other.Key);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.Key));
        }
        /// <inheritdoc cref="QueryParameter.ToString"/>
        public override string ToString()
        {
            return this.ToString(null, null);
        }
        /// <inheritdoc cref="QueryParameter.ToString(string, IFormatProvider)"/>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            Span<char> span = stackalloc char[this.MaxLength];
            _ = this.TryFormat(span, out int written, default, null);
            return new string(span.Slice(0, written));
        }

        /// <summary>
        /// Creates a new <see cref="QueryBooleanParameter"/> instance with a specified key and a false 
        /// boolean value.
        /// </summary>
        /// <param name="key">The key of the query parameter.</param>
        /// <returns>A new <see cref="QueryBooleanParameter"/> instance with the given key and a boolean
        /// value of <see langword="false"/>.</returns>
        /// <inheritdoc cref="QueryBooleanParameter(string, in bool)" path="/exception"/>
        public static QueryBooleanParameter False(string key)
        {
            return new(key, false);
        }
        /// <summary>
        /// Creates a new <see cref="QueryBooleanParameter"/> instance with a specified key and a true 
        /// boolean value.
        /// </summary>
        /// <param name="key">The key of the query parameter.</param>
        /// <returns>A new <see cref="QueryBooleanParameter"/> instance with the given key and a boolean 
        /// value of <see langword="true"/>.</returns>
        /// <inheritdoc cref="QueryBooleanParameter(string, in bool)" path="/exception"/>
        public static QueryBooleanParameter True(string key)
        {
            return new(key, true);
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            ReadOnlySpan<char> key = this.Key;
            key.CopyTo(destination);
            ref char first = ref destination[0];
            first = char.ToLower(first);

            charsWritten = key.Length;

            destination[charsWritten++] = '=';

            if (!_value.TryFormat(destination.Slice(charsWritten), out int written))
            {
                charsWritten += written;
                return false;
            }

            ref char c = ref destination[charsWritten];
            c = char.ToLower(c);

            charsWritten += written;
            return true;
        }

#if NET7_0_OR_GREATER
        public bool TryValueAsNumber<T>(out T value) where T : struct, INumber<T>
        {
            if (!this.IsEmpty)
            {
                value = T.CreateChecked(_value ? 1 : 0);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
#endif
    }
}

