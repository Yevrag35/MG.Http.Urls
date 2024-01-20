using MG.Http.Urls.Exceptions;
using MG.Http.Urls.Queries;

namespace MG.Http.Urls.Tests
{
    public class QueryParameterCollectionTests
    {
        [Fact]
        public void Constructor_InitializesEmptyCollection()
        {
            var collection = new QueryParameterCollection();

            Assert.Empty(collection);
            Assert.Equal(0, collection.MaxLength);
        }

        [Fact]
        public void Constructor_InitializesCollectionWithCapacity()
        {
            int capacity = 5;
            var collection = new QueryParameterCollection(capacity);
            Assert.True(capacity <= collection.EnsureCapacity(capacity));
            Assert.Equal(0, collection.MaxLength);
        }

        [Fact]
        public void AddParameter_IncreasesCount()
        {
            string key = "key1";
            string value = "value1";
            var collection = new QueryParameterCollection();
            collection.Add(key, value);

            Assert.Single(collection);
            Assert.Equal(key.Length + value.Length + 1, collection.MaxLength);
        }

        [Fact]
        public void AddOrUpdate_AddsNewParameter_WhenNotPresent()
        {
            string key = "key1";
            string value = "value1";
            var collection = new QueryParameterCollection();
            var parameter = QueryParameter.Create(key, value);

            collection.AddOrUpdate(parameter);

            Assert.Single(collection);
            Assert.Equal(key.Length + value.Length + 1, collection.MaxLength);
        }

        [Fact]
        public void AddOrUpdate_UpdatesParameter_WhenAlreadyPresent()
        {
            var collection = new QueryParameterCollection();
            var parameter1 = QueryParameter.Create("key", "value1");
            var parameter2 = QueryParameter.Create("key", "value2");

            collection.Add(parameter1);
            collection.AddOrUpdate(parameter2);

            Assert.Single(collection);
            Assert.Equal(parameter2.ToString(), collection["key"].ToString());
            Assert.Equal(parameter2.MaxLength, collection.MaxLength);
        }

        [Fact]
        public void RemoveParameter_DecreasesCount()
        {
            var collection = new QueryParameterCollection();
            collection.Add("key1", "value1");
            collection.Remove("key1");

            Assert.Empty(collection);
            Assert.Equal(0, collection.MaxLength);
        }

        [Fact]
        public void Clear_RemovesAllParameters()
        {
            var p1 = QueryParameter.Create("key1", "value1");
            var p2 = QueryParameter.Create("key2", "value2");

            var collection = new QueryParameterCollection
            {
                p1,
                p2,
            };

            Assert.Equal(p1.MaxLength + p2.MaxLength + 1, collection.MaxLength);

            collection.Clear();

            Assert.Empty(collection);
            Assert.Equal(0, collection.MaxLength);
        }

        [Fact]
        public void Indexer_ReturnsCorrectParameter()
        {
            string key = "key";

            var collection = new QueryParameterCollection();
            var parameter = QueryParameter.Create(key, "value");
            collection.Add(parameter);

            var retrievedParameter = collection[key];

            var retQp = Assert.IsType<QueryParameter>(retrievedParameter);
            Assert.Equal(parameter, retQp);
            Assert.Equal(parameter.MaxLength, retQp.MaxLength);
            Assert.Equal(parameter.IsFormattable, retQp.IsFormattable);
        }

        [Fact]
        public void Indexer_ReturnsEmptyWhenKeyNotFound()
        {
            var collection = new QueryParameterCollection()
            {
                { "key", "value" },
            };

            IQueryParameter empty = collection["notKey"];
            Assert.NotNull(empty);
            Assert.Equal(string.Empty, empty.Key);
            Assert.Equal(0, empty.MaxLength);
            Assert.False(empty.TryValueAsNumber<int>(out _));

            char[] chars = new char[4];
            Assert.True(empty.TryFormat(chars, out int written, default, null));
            Assert.Equal(0, written);

            Assert.All(chars, c => Assert.Equal(default, c));
        }

        [Fact]
        public void Indexer_ThrowsInvalidQueryKeyException()
        {
            var collection = new QueryParameterCollection()
            {
                { "key", "value" },
            };

            Assert.Throws<InvalidQueryKeyException>(() =>
            {
                var p = collection[null!];
            });
            Assert.Throws<InvalidQueryKeyException>(() =>
            {
                var p = collection[string.Empty];
            });
            Assert.Throws<InvalidQueryKeyException>(() =>
            {
                var p = collection["  "];
            });
        }
    }
}

