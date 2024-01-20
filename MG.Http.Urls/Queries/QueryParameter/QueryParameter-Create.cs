using MG.Http.Urls.Internal;
using System;

namespace MG.Http.Urls.Queries
{
    public readonly partial struct QueryParameter
    {
        /// <summary>
        /// Creates a new <see cref="QueryParameter"/> instance from the specified key and formattable value.
        /// </summary>
        /// <param name="key">The key for the query parameter.</param>
        /// <param name="formattable">The formattable value of the query parameter.</param>
        /// <param name="maxLength">The maximum length of the formatted value.</param>
        /// <param name="format">The format to use for the value, if applicable.</param>
        /// <returns>A new <see cref="QueryParameter"/> instance.</returns>
        /// <inheritdoc cref="QueryParameter(string, OneOf{string, ISpanFormattable}, int, string)"
        ///     path="/exception"/>
        public static QueryParameter Create(string key, ISpanFormattable formattable, int maxLength, string? format = null)
        {
            return new QueryParameter(key, OneOf<string?, ISpanFormattable>.FromT1(formattable), maxLength, format);
        }
        /// <summary>
        /// Creates a new <see cref="IQueryParameter"/> instance with a specified key and a boolean value.
        /// </summary>
        /// <param name="key">The key of the query parameter.</param>
        /// <param name="value">The boolean value of the query parameter.</param>
        /// <returns>A new instance of an object implementing <see cref="IQueryParameter"/> with the 
        /// given key and boolean value.</returns>
        /// <inheritdoc cref="QueryBooleanParameter(string, in bool)" path="/exception"/>
        public static IQueryParameter Create(string key, bool value)
        {
            return new QueryBooleanParameter(key, in value);
        }
        /// <summary>
        /// Creates a new <see cref="QueryParameter"/> instance with a specified key and a string value.
        /// </summary>
        /// <param name="key">The key of the query parameter.</param>
        /// <param name="value">The string value of the query parameter. Can be <see langword="null"/>.
        /// </param>
        /// <param name="format">The optional format to be used for the value.</param>
        /// <returns>A new <see cref="QueryParameter"/> instance with the given key and string value.
        /// </returns>
        /// <inheritdoc cref="QueryParameter(string, OneOf{string, ISpanFormattable}, int, string)"
        ///     path="/exception"/>
        public static QueryParameter Create(string key, string? value, string? format = null)
        {
            return new(key, OneOf<string?, ISpanFormattable>.FromT0(value), value?.Length ?? 0, format);
        }

        /// <summary>
        /// Returns an <see cref="IQueryParameter"/> instance that only holds the parameter's key.
        /// </summary>
        /// <remarks>
        ///     This is used for comparison uses where the value of the parameter is not relevant.
        /// </remarks>
        /// <inheritdoc cref="KeyOnlyQueryParameter.Create(string)" path="/exception"/>
        public static IQueryParameter FromKeyOnly(string key)
        {
            return KeyOnlyQueryParameter.Create(key);
        }
    }
}

