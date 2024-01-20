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
        }

        [Fact]
        public void Constructor_InitializesCollectionWithCapacity()
        {
            int capacity = 5;
            var collection = new QueryParameterCollection(capacity);
            Assert.True(capacity <= collection.EnsureCapacity(capacity));
        }

        [Fact]
        public void AddParameter_IncreasesCount()
        {
            var collection = new QueryParameterCollection();
            collection.Add("key1", "value1");

            Assert.Single(collection);
        }

        [Fact]
        public void AddOrUpdate_AddsNewParameter_WhenNotPresent()
        {
            var collection = new QueryParameterCollection();
            var parameter = QueryParameter.Create("key", "value");

            collection.AddOrUpdate(parameter);

            Assert.Single(collection);
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
        }

        [Fact]
        public void RemoveParameter_DecreasesCount()
        {
            var collection = new QueryParameterCollection();
            collection.Add("key1", "value1");
            collection.Remove("key1");

            Assert.Empty(collection);
        }

        [Fact]
        public void Clear_RemovesAllParameters()
        {
            var collection = new QueryParameterCollection
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            collection.Clear();

            Assert.Empty(collection);
        }

        [Fact]
        public void Indexer_ReturnsCorrectParameter()
        {
            var collection = new QueryParameterCollection();
            var parameter = QueryParameter.Create("key", "value");
            collection.Add(parameter);

            var retrievedParameter = collection["key"];

            Assert.Equal(parameter, retrievedParameter);
        }

        // Additional tests can be written for other methods and behaviors...
    }
}

