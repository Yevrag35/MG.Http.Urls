using MG.Http.Urls.Internal;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MG.Http.Urls.Queries
{
    /// <summary>
    /// A <see langword="static"/> class for constructing <see cref="IQueryParameter"/> instances with 
    /// numeric values.
    /// </summary>
    public static class QueryNumericParameter
    {
        /// <summary>
        /// Attempts to create a new <see cref="IQueryParameter"/> instance from the specified key and
        /// formattable, numeric value.
        /// </summary>
        /// <param name="key">The key of the parameter.</param>
        /// <param name="formattable">The formattable, numeric value to use.</param>
        /// <param name="result">When this method returns, this variable will be the resulting
        /// <see cref="IQueryParameter"/> instance that was constructed, or <see langword="null"/> if the
        /// type of <paramref name="formattable"/> is not supported.</param>
        /// <returns>
        ///     <see langword="true"/> if the <paramref name="formattable"/> is supported and was created; 
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryCreateFromSupported(string key, ISpanFormattable formattable, [NotNullWhen(true)] out IQueryParameter? result)
        {
            result = null;
            if (formattable is null)
            {
                return false;
            }

            return formattable switch
            {
                sbyte => TryGetParameter<sbyte>(key, formattable, LengthConstants.INT_MAX, out result),
                byte => TryGetParameter<byte>(key, formattable, LengthConstants.UINT_MAX, out result),
                short => TryGetParameter<short>(key, formattable, LengthConstants.INT_MAX, out result),
                ushort => TryGetParameter<ushort>(key, formattable, LengthConstants.UINT_MAX, out result),
                int => TryGetParameter<int>(key, formattable, LengthConstants.INT_MAX, out result),
                uint => TryGetParameter<uint>(key, formattable, LengthConstants.UINT_MAX, out result),
                long => TryGetParameter<long>(key, formattable, LengthConstants.LONG_MAX, out result),
                ulong => TryGetParameter<ulong>(key, formattable, LengthConstants.ULONG_MAX, out result),
                float => TryGetParameter<float>(key, formattable, LengthConstants.DOUBLE_MAX, out result),
                double => TryGetParameter<double>(key, formattable, LengthConstants.DOUBLE_MAX, out result),
                decimal => TryGetParameter<decimal>(key, formattable, LengthConstants.DECIMAL_MAX, out result),
                BigInteger => TryGetParameter<BigInteger>(key, formattable, LengthConstants.INT128_MAX, out result),
                _ => false,
            };
        }

        /// <exception cref="NotSupportedException">
        ///     Thrown when <typeparamref name="T"/> is not supported.
        /// </exception>
        internal static int GetMaxLength<T>(in T value) where T : struct, INumber<T>
        {
            return value switch
            {
                sbyte => LengthConstants.INT_MAX,
                byte => LengthConstants.UINT_MAX,
                short => LengthConstants.INT_MAX,
                ushort => LengthConstants.UINT_MAX,
                int => LengthConstants.INT_MAX,
                uint => LengthConstants.UINT_MAX,
                long => LengthConstants.LONG_MAX,
                ulong => LengthConstants.ULONG_MAX,
                float => LengthConstants.DOUBLE_MAX,
                double => LengthConstants.DOUBLE_MAX,
                decimal => LengthConstants.DECIMAL_MAX,
                BigInteger => LengthConstants.INT128_MAX,
                _ => ThrowNotSupportedException<T, int>(in value),
            };
        }

        private static bool TryGetParameter<T>(string key, ISpanFormattable formattable, int maxLength, [NotNullWhen(true)] out IQueryParameter? parameter) where T : struct, INumber<T>
        {
            if (TryCast(formattable, out T value))
            {
                parameter = new QueryNumericParameter<T>(key, value, maxLength, null);
                return true;
            }
            else
            {
                parameter = null;
                return false;
            }
        }

        private static bool TryCast<T>(ISpanFormattable formattable, out T result) where T : struct, INumber<T>
        {
            try
            {
                result = (T)formattable;
                return true;
            }
            catch (InvalidCastException e)
            {
                Debug.Fail(e.Message);
                result = default;
                return false;
            }
        }

        const string TYPE_NOT_SUPPORTED_FORMAT = "Cannot find the default maximum character length for type \"{0}\". It is not natively supported.";
        [DoesNotReturn]
        private static TOutput ThrowNotSupportedException<T, TOutput>(in T value)
        {
            throw new NotSupportedException(string.Format(TYPE_NOT_SUPPORTED_FORMAT, typeof(T)));
        }
    }
    /// <summary>
    /// A struct implementing <see cref="IQueryParameter"/> that represents a query parameter with a number
    /// value.
    /// </summary>
    /// <typeparam name="T">The type of number the parameter's value is.</typeparam>
    public readonly struct QueryNumericParameter<T> : IEquatable<QueryNumericParameter<T>>, IQueryParameter
        where T : struct, INumber<T>
    {
        readonly string? _key;
        readonly string? _format;
        readonly bool _isNotEmpty;
        readonly int _maxLength;
        readonly T _value;

        /// <inheritdoc cref="QueryParameter.IsEmpty"/>
        public bool IsEmpty => !_isNotEmpty;
        /// <inheritdoc cref="QueryParameter.Key"/>
        public string Key => _key ?? string.Empty;
        /// <inheritdoc cref="QueryParameter.MaxLength"/>"
        public int MaxLength => _maxLength;
        /// <summary>
        /// The numeric value of the query parameter.
        /// </summary>
        public T Value => _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryNumericParameter{T}"/> struct with the 
        /// specified key and numeric value. The maximum length of the default formatted value is determined 
        /// by the type of <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        ///     If the type of <typeparamref name="T"/> is not natively supported, an 
        ///     <see cref="ArgumentException"/> is thrown. To use an unsupported type, use the 
        ///     <see cref="QueryNumericParameter{T}(string, T, int, string)"/> constructor instead.
        ///     <para>Natively supported types are: <see cref="sbyte"/>, <see cref="byte"/>, 
        ///     <see cref="short"/>, <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, 
        ///     <see cref="long"/>, <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/>, 
        ///     <see cref="decimal"/>, and <see cref="BigInteger"/>.
        ///     </para>
        /// </remarks>
        /// <param name="key">The key for the query parameter.</param>
        /// <param name="value">The numeric value of the query parameter.</param>
        /// <inheritdoc cref="QueryNumericParameter{T}(string, T, int, string?)" path="/exception"/>
        /// <inheritdoc cref="QueryNumericParameter.GetMaxLength{T}(in T)" path="/exception"/>
        public QueryNumericParameter(string key, T value)
            : this(key, value, QueryNumericParameter.GetMaxLength(in value), null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryNumericParameter{T}"/> struct with the specified key, numeric value, maximum length, and format.
        /// </summary>
        /// <param name="key">The key for the query parameter.</param>
        /// <param name="value">The numeric value of the query parameter.</param>
        /// <param name="maxLength">The maximum length of the formatted value.</param>
        /// <param name="format">The optional format to be used for the value.</param>
        /// <inheritdoc cref="Guard.InvalidKey(string?, string)" path="/exception"/>
        public QueryNumericParameter(string key, T value, int maxLength, string? format)
        {
            Guard.InvalidKey(key);
            _format = !string.IsNullOrWhiteSpace(format)
                ? format : null;

            _key = key;
            _value = value;
            _maxLength = maxLength;
            _isNotEmpty = true;
        }

        /// <inheritdoc cref="QueryParameter.Equals(IQueryParameter?)"/>
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
        /// <inheritdoc cref="QueryParameter.Equals(QueryParameter)"/>
        public bool Equals(QueryParameter other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(this.Key, other.Key);
        }
        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(QueryNumericParameter<T> other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(this.Key, other.Key);
        }
        /// <inheritdoc cref="object.Equals(object?)"/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is QueryParameter qp)
            {
                return this.Equals(qp);
            }
            else if (obj is QueryNumericParameter<T> qnp)
            {
                return this.Equals(qnp);
            }
            else if (obj is IQueryParameter iqp)
            {
                return this.Equals(iqp);
            }
            else
            {
                return false;
            }
        }
        /// <inheritdoc cref="QueryParameter.GetHashCode"/>"
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

        bool IQueryParameter.TryValueAsNumber<TNum>(out TNum value)
        {
            if (typeof(TNum) == typeof(T))
            {
                value = Unsafe.As<T, TNum>(ref Unsafe.AsRef(in _value));
                return true;
            }
            else
            {
                try
                {
                    value = TNum.CreateChecked(_value);
                    return true;
                }
                catch (SystemException e)
                {
                    Debug.Fail(e.Message);
                    value = default!;
                    return false;
                }
            }
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

            span[written++] = '=';

            if (!_value.TryFormat(span.Slice(written), out int valueWritten, format, provider))
            {
                return false;
            }

            written += valueWritten;
            return true;
        }

        /// <inheritdoc cref="QueryParameter.operator ==(QueryParameter, QueryParameter)"/>
        public static bool operator ==(QueryNumericParameter<T> left, QueryNumericParameter<T> right)
        {
            return left.Equals(right);
        }
        /// <inheritdoc cref="QueryParameter.operator !=(QueryParameter, QueryParameter)"/>
        public static bool operator !=(QueryNumericParameter<T> left, QueryNumericParameter<T> right)
        {
            return !(left == right);
        }
    }
}

