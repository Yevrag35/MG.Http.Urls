using System;
using System.Numerics;

namespace MG.Http.Urls.Queries
{
    /// <summary>
    /// An interface for a query parameter, providing key and length information, and methods for numeric 
    /// conversion and formatting.
    /// </summary>
    public interface IQueryParameter : IEquatable<IQueryParameter>, IEquatable<QueryParameter>, ISpanFormattable
    {
        /// <summary>
        /// Gets the key of the query parameter.
        /// </summary>
        /// <value>
        /// The key associated with the query parameter.
        /// </value>
        string Key { get; }
        /// <summary>
        /// Gets the maximum length of the string representation of the query parameter.
        /// </summary>
        /// <value>
        /// The maximum length of the string representation of the query parameter.
        /// </value>
        int MaxLength { get; }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Tries to convert the value of the query parameter to a specified numeric type.
        /// </summary>
        /// <typeparam name="T">The numeric type to which the value is to be converted. Must be an 
        /// unmanaged type implementing <see cref="INumber{T}"/>.</typeparam>
        /// <param name="value">
        /// When this method returns, contains the numeric value equivalent to the query parameter's value,
        /// if the conversion succeeded, or the default value of the type if the conversion failed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the query parameter's value was successfully converted; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        bool TryValueAsNumber<T>(out T value) where T : struct, INumber<T>;
#endif
    }
}

