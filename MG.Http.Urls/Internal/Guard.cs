using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MG.Http.Urls.Internal
{
    internal static class Guard
    {
        const string EMPTY_MSG = "The string cannot be empty.";
        const string EMPTY_WHITESPC = "The string cannot consist of only whitespace characters.";

        /// <summary>
        /// Throws an exception if <paramref name="key"/> is null, empty, or whitespace.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramName"></param>
        /// <exception cref="InvalidQueryKeyException"><paramref name="key"/> is invalid.</exception>
        internal static void InvalidKey([NotNull] string? key, [CallerArgumentExpression(nameof(key))] string paramName = "")
        {
            try
            {
                NotNullOrWhitespace(key, paramName);
            }
            catch (ArgumentException e)
            {
                throw new InvalidQueryKeyException(e);
            }
        }

        /// <summary>
        /// Throws an exception if <paramref name="argument"/> is null or empty.</summary>
        /// <param name="argument">The string argument to validate as non-null and non-empty.</param>
        /// <param name="paramName">
        ///     The name of the parameter with which <paramref name="argument"/> corresponds.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="argument"/> is empty.</exception>
        internal static void NotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
#if NET8_0_OR_GREATER
            ArgumentException.ThrowIfNullOrEmpty(value, paramName);
        }

        /// <inheritdoc cref="ArgumentException.ThrowIfNullOrWhiteSpace(string, string)"/>
        internal static void NotNullOrWhitespace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
        }
#else
            ArgumentNullException.ThrowIfNull(value, paramName);
            if (string.Empty.Equals(value))
            {
                throw new ArgumentException(EMPTY_MSG, paramName);
            }
        }

        /// <summary>
        /// Throws an exception if <paramref name="argument"/> is null, empty, or consists only of
        /// white-space characters.
        /// </summary>
        /// <param name="value">The string argument to validate.</param>
        /// <param name="paramName">
        ///     The name of the parameter with which <paramref name="value"/> corresponds.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument"/> is empty or consists only of white-space characters.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
        internal static void NotNullOrWhitespace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            NotNullOrEmpty(value, paramName);
            if (value.AsSpan().IsWhiteSpace())
            {
                throw new ArgumentException(EMPTY_WHITESPC, paramName);
            }
        }
#endif
    }
}