using MG.Http.Urls.Internal;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MG.Http.Urls.Queries
{
    /// <summary>
    /// A struct implementing <see cref="IQueryParameter"/> that represents a query parameter with no value.
    /// </summary>
    /// <remarks>
    /// Used for comparison/equality purposes.
    /// </remarks>
    internal readonly struct KeyOnlyQueryParameter : IQueryParameter
    {
        readonly bool _isNotEmpty;
        readonly string? _key;
        readonly int _maxLength;

        /// <inheritdoc cref="QueryParameter.IsEmpty"/>
        [MemberNotNullWhen(false, nameof(_key) ,nameof(Key))]
        internal bool IsEmpty => !_isNotEmpty;
        public string Key => _key ?? string.Empty;
        public int MaxLength => _maxLength;

        private KeyOnlyQueryParameter(string? key, bool isNotEmpty, int length)
        {
            _maxLength = length + 1;
            _key = key;
            _isNotEmpty = isNotEmpty;
        }

        public bool Equals(IQueryParameter? otherParam)
        {
            if (otherParam is QueryParameter qp)
            {
                return this.Equals(qp);
            }
            else if (otherParam is not null)
            {
                return StringComparer.InvariantCultureIgnoreCase.Equals(this.Key, otherParam.Key);
            }
            else
            {
                return false;
            }
        }
        public bool Equals(QueryParameter other)
        {
            return (this.IsEmpty && other.IsEmpty) || other.Equals(this);
        }
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is IQueryParameter qp && this.Equals(qp);
        }
        public override int GetHashCode()
        {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.Key);
        }

        /// <inheritdoc cref="QueryParameter.ToString"/>
        public override string ToString()
        {
            return this.ToString(null, null);
        }
        /// <inheritdoc cref="QueryParameter.ToString(string?, IFormatProvider?)"/>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return string.Create(this.MaxLength, this, (chars, state) =>
            {
                ReadOnlySpan<char> key = state._key;
                key.CopyTo(chars);

                chars[key.Length] = '=';
            });
        }

        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            if (!this.IsEmpty && _key.TryCopyTo(destination))
            {
                destination[_key.Length] = '=';
                charsWritten = _key.Length + 1;
                return true;
            }
            else
            {
                charsWritten = 0;
                return false;
            }
        }

        /// <inheritdoc cref="Guard.InvalidKey(string?, string)" path="/exception"/>
        /// <summary>
        /// Creates a new <see cref="KeyOnlyQueryParameter"/> instance from the specified key.
        /// </summary>
        internal static KeyOnlyQueryParameter Create(string key)
        {
            Guard.InvalidKey(key);
            return new(key, true, key.Length);
        }

#if NET7_0_OR_GREATER
        bool IQueryParameter.TryValueAsNumber<T>(out T value)
        {
            value = default;
            return false;
        }
#endif
    }
}

