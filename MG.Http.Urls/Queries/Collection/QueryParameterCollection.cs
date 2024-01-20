using MG.Http.Urls.Internal;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace MG.Http.Urls.Queries
{
    /// <summary>
    /// Represents a collection of query parameters, providing methods for managing them, and implements
    /// <see cref="IReadOnlyCollection{T}"/> for <see cref="IQueryParameter"/> and 
    /// <see cref="ISpanFormattable"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This collection works most efficiently
    /// when used repeatedly, so it is recommended to incorporate it into object pooling.
    /// </para>
    /// <para>Utilizing the <see cref="MaxLength"/> property of this collection, you can determine the size needed
    /// for allocating a buffer for the formatted query parameters.  An example of how to
    /// use 
    /// <code>
    ///     Span&lt;char&gt; chars = stackalloc char[queryParameters.MaxLength];
    ///     collection.TryFormat(chars, out int written, format, provider);
    ///     
    ///     string urlQuery = new string(chars.Slice(0, written));
    /// </code>
    /// </para>
    /// </remarks>
    public sealed class QueryParameterCollection : 
        ICollection<IQueryParameter>, 
        IReadOnlyDictionary<string, IQueryParameter>,
        ISpanFormattable
    {
        readonly HashSet<IQueryParameter> _params;
        int _maxLength;

        /// <summary>
        /// Gets the query parameter associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the query parameter to get.</param>
        /// <returns>
        /// The query parameter associated with the specified key. If not found, returns an empty 
        /// <see cref="IQueryParameter"/>.
        /// </returns>
        /// <inheritdoc cref="QueryParameter.FromKeyOnly(string)" path="/exception"/>
        public IQueryParameter this[string key]
        {
            get
            {
                return _params.TryGetValue(QueryParameter.FromKeyOnly(key), out IQueryParameter? actual)
                    ? actual
                    : (QueryParameter)default;
            }
        }

        /// <summary>
        /// Gets the count of query parameters in the collection.
        /// </summary>
        /// <value>
        /// The number of query parameters contained in the collection.
        /// </value>
        public int Count => _params.Count;

        bool ICollection<IQueryParameter>.IsReadOnly => false;
        IEnumerable<string> IReadOnlyDictionary<string, IQueryParameter>.Keys => _params.Select(p => p.Key);
        IEnumerable<IQueryParameter> IReadOnlyDictionary<string, IQueryParameter>.Values => _params.AsEnumerable();

        /// <summary>
        /// Gets the maximum length of the query parameters combined.
        /// </summary>
        /// <value>
        /// The maximum length of the combined query parameters. Adjusts based on the number of parameters 
        /// in the collection.
        /// </value>
        public int MaxLength
        {
            get
            {
                return this.Count <= 1
                    ? _maxLength
                    : _maxLength + (this.Count - 1);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class with a 
        /// specified capacity.
        /// </summary>
        /// <remarks>
        /// The default capacity is 1. This constructor initializes the collection with the given capacity.
        /// </remarks>
        /// <param name="capacity">The initial number of elements that the collection can contain.</param>
        public QueryParameterCollection(int capacity = 1)
        {
            _params = new(capacity);
        }

        /// <summary>
        /// Adds a new query parameter with a string value to the collection.
        /// </summary>
        /// <param name="key">The key of the query parameter to add.</param>
        /// <param name="value">The string value of the query parameter.</param>
        /// <returns>
        /// <see langword="true"/> if the parameter was successfully added; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        /// <inheritdoc cref="QueryParameter.Create(string, string?, string)" path="/exception"/>
        public bool Add(string key, string? value)
        {
            var p = QueryParameter.Create(key, value);
            return this.Add(p);
        }
        /// <summary>
        /// Adds a new query parameter with a boolean value to the collection.
        /// </summary>
        /// <param name="key"><inheritdoc cref="Add(string, string)" path="/param[1]"/></param>
        /// <param name="value">The boolean value of the query parameter.</param>
        /// <returns>
        ///     <inheritdoc cref="Add(string, string)"/>
        /// </returns>
        /// <inheritdoc cref="QueryParameter.Create(string, bool)" path="/exception"/>
        public bool Add(string key, bool value)
        {
            IQueryParameter adding = QueryParameter.Create(key, value);
            return this.Add(adding);
        }
        /// <summary>
        /// Adds a new query parameter with an integer value to the collection.
        /// </summary>
        /// <param name="key"><inheritdoc cref="Add(string, string)" path="/param[1]"/></param>
        /// <param name="value">The integer value of the query parameter.</param>
        /// <param name="format">The optional format to be used for the value.</param>
        /// <returns>
        ///     <inheritdoc cref="Add(string, string?)"/>
        /// </returns>
        /// <inheritdoc cref="QueryParameter.Create(string, ISpanFormattable, int, string?)"
        ///     path="/exception"/>
        public bool Add(string key, int value, string? format = null)
        {
            return this.Add(key, value, LengthConstants.INT_MAX, format);
        }
        /// <summary>
        /// Adds a new query parameter to the collection.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to be added, constrained to types implementing 
        ///     <see cref="ISpanFormattable"/>.
        /// </typeparam>
        /// <param name="key"><inheritdoc cref="Add(string, string?)" path="/param[1]"/></param>
        /// <param name="value">The value of the query parameter.</param>
        /// <param name="maxLength">The maximum length of the formatted value.</param>
        /// <param name="format">The optional format to be used for the value.</param>
        /// <returns>
        ///     <inheritdoc cref="Add(string, string)"/>
        /// </returns>
        /// <inheritdoc cref="QueryParameter.Create(string, ISpanFormattable, int, string)"
        ///     path="/exception"/>
        public bool Add<T>(string key, T value, in int maxLength, string? format = null) where T : notnull, ISpanFormattable
        {
            QueryParameter p = QueryParameter.Create(key, value, maxLength, format);
            return this.Add(p);
        }
        /// <inheritdoc cref="Add(string, string?)" path="/*[not(self::summary) and not(self::exception)]"/>
        /// <summary>
        /// Adds a query parameter object to the collection.
        /// </summary>
        /// <param name="parameter">The <see cref="IQueryParameter"/> to add to the collection.</param>
        public bool Add(IQueryParameter parameter)
        {
            if (_params.Add(parameter))
            {
                _maxLength += parameter.MaxLength;
                return true;
            }
            else
            {
                return false;
            }
        }
        void ICollection<IQueryParameter>.Add(IQueryParameter item)
        {
            _ = this.Add(item);
        }

        /// <inheritdoc cref="HashSet{T}.UnionWith(IEnumerable{T})" path="/*[not(self::summary)]"/>
        /// <summary>
        /// Adds the specified collection's parameters to this <see cref="QueryParameterCollection"/>
        /// instance omitting parameters whose keys already exist in this collection.
        /// </summary>
        public void AddMany(IEnumerable<IQueryParameter> other)
        {
            foreach (IQueryParameter p in other)
            {
                _ = this.Add(p);
            }
        }

        /// <summary>
        /// Adds a query parameter to the collection, or updates it if it already exists.
        /// </summary>
        /// <remarks>
        /// This method checks if a query parameter with the same key already exists in the collection. 
        /// If it does, the existing parameter is removed, and the new one is added. 
        /// If it does not exist, the new parameter is simply added to the collection.
        /// </remarks>
        /// <param name="parameter">The <see cref="IQueryParameter"/> to add or update in the collection.
        /// </param>
        public void AddOrUpdate(IQueryParameter parameter)
        {
            if (_params.TryGetValue(parameter, out IQueryParameter? actual))
            {
                _ = this.Remove(actual);
                _ = this.Add(parameter);
            }
            else
            {
                _ = this.Add(parameter);
            }
        }

        /// <summary>
        /// Clears all the query parameters from the collection and resets the <see cref="MaxLength"/> to 0.
        /// </summary>
        public void Clear()
        {
            _params.Clear();
            _maxLength = 0;
        }
        /// <summary>
        /// Determines whether the collection contains the specified query parameter.
        /// </summary>
        /// <param name="item">The parameter to locate in the collection.</param>
        /// <returns>
        ///     <see langword="true"/> if the parameter is found in the collection; otherwise,
        ///     <see langword="false"/>.
        /// </returns>
        public bool Contains(IQueryParameter item)
        {
            return _params.Contains(item);
        }
        /// <summary>
        /// Determines whether the collection contains a query parameter with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the collection.</param>
        /// <returns>
        /// <see langword="true"/> if the collection contains a query parameter with the specified key;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && _params.Contains(KeyOnlyQueryParameter.Create(key));
        }

        /// <inheritdoc cref="ICollection{T}.CopyTo(T[], int)"/>
        public void CopyTo(IQueryParameter[] array, int arrayIndex)
        {
            _params.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc cref="HashSet{T}.EnsureCapacity(int)" path="/*[not(self::summary)]"/>
        /// <summary>
        /// Ensures that this collection can hold the specified number of parameter elements without growing.
        /// </summary>
        public int EnsureCapacity(int capacity)
        {
            return _params.EnsureCapacity(capacity);
        }

        /// <summary>
        /// Removes the query parameter associated with the specified key from the collection.
        /// </summary>
        /// <param name="key">The key of the query parameter to remove.</param>
        /// <returns>
        /// <see langword="true"/> if the parameter was successfully removed; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool Remove(string key)
        {
            return !string.IsNullOrWhiteSpace(key)
                   &&
                   this.Remove(KeyOnlyQueryParameter.Create(key));
        }
        /// <summary>
        /// Removes the specified query parameter from the collection.
        /// </summary>
        /// <param name="parameter">The parameter to remove.</param>
        /// <returns>
        ///     <inheritdoc cref="Remove(string)"/>
        /// </returns>
        public bool Remove(IQueryParameter parameter)
        {
            if (_params.TryGetValue(parameter, out IQueryParameter? actual) && _params.Remove(actual))
            {
                _maxLength -= actual.MaxLength;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation using the 
        /// specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// A string representation of the value of this instance, formatted as specified by 
        /// <paramref name="format"/> and <paramref name="provider"/>.
        /// <para>
        ///     An example of the output: <c>key1=value1&amp;key2=value2</c>
        /// </para>
        /// </returns>
        public string ToString(string? format, IFormatProvider? provider)
        {
            Span<char> chars = stackalloc char[this.MaxLength];
            _ = this.TryFormat(chars, out int written, format, provider);
            return new string(chars.Slice(0, written));
        }

        /// <summary>
        /// Sets the capacity of the <see cref="QueryParameterCollection"/> to the actual number of 
        /// parameters it contains, rounded up to a nearby, implementation-specific value.
        /// </summary>
        public void TrimExcess()
        {
            _params.TrimExcess();
        }

        /// <summary>
        /// Tries to format the current instance into the provided span of characters.
        /// </summary>
        /// <remarks>
        ///      An example of the written output: <c>key1=value1&amp;key2=value2</c>
        /// </remarks>
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
            written = 0;
            int count = 0;
            foreach (IQueryParameter p in _params)
            {
                try
                {
                    if (!p.TryFormat(span.Slice(written), out int wri, format, provider))
                    {
                        written += wri;
                        return false;
                    }
                    else
                    {
                        written += wri;
                    }
                }
                catch
                {
                    return false;
                }

                if (count < _params.Count - 1)
                {
                    span[written++] = '&';
                }

                count++;
            }

            return true;
        }

        /// <summary>
        /// Searches the collection for a query parameter with the specified key and returns the equal value
        /// it finds, if any.
        /// </summary>
        /// <param name="key">The key of the parameter to locate in the collection.</param>
        /// <param name="value">
        ///     The parameter from the collection that the search found, or <see langword="null"/> if the 
        ///     search yielded no match.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the search was successful; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetValue(string key, [NotNullWhen(true)] out IQueryParameter? value)
        {
            value = null;
            return !string.IsNullOrWhiteSpace(key)
                   &&
                   _params.TryGetValue(QueryParameter.FromKeyOnly(key), out value);
        }

        #region ENUMERATORS
        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<IQueryParameter> GetEnumerator()
        {
            return _params.GetEnumerator();
        }
        IEnumerator<KeyValuePair<string, IQueryParameter>> IEnumerable<KeyValuePair<string, IQueryParameter>>.GetEnumerator()
        {
            foreach (IQueryParameter p in _params)
            {
                yield return new KeyValuePair<string, IQueryParameter>(p.Key, p);
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
