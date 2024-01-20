using MG.Http.Urls.Queries;

namespace MG.Http.Urls.Tests
{
    public class KeyOnlyQueryParameterTests
    {
        [Fact]
        public void Equals_ReturnsTrueForSameKey()
        {
            string key = "testKey";
            var param1 = KeyOnlyQueryParameter.Create(key);
            var param2 = KeyOnlyQueryParameter.Create(key);

            Assert.True(param1.Equals(param2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentKeys()
        {
            var param1 = KeyOnlyQueryParameter.Create("key1");
            var param2 = KeyOnlyQueryParameter.Create("key2");

            Assert.False(param1.Equals(param2));
        }

        [Fact]
        public void Equals_ReturnsFalseWhenComparedWithNull()
        {
            var param = KeyOnlyQueryParameter.Create("key");
            IQueryParameter? nullParam = null;

            Assert.False(param.Equals(nullParam));
        }

        [Fact]
        public void Equals_ReturnsTrueWhenComparedWithQueryParameterWithSameKey()
        {
            string key = "testKey";
            var keyOnlyParam = KeyOnlyQueryParameter.Create(key);
            var queryParam = QueryParameter.Create(key, "value");

            Assert.True(keyOnlyParam.Equals(queryParam));
        }

        [Fact]
        public void Equals_ReturnsFalseWhenComparedWithQueryParameterWithDifferentKey()
        {
            var keyOnlyParam = KeyOnlyQueryParameter.Create("key1");
            var queryParam = QueryParameter.Create("key2", "value");

            Assert.False(keyOnlyParam.Equals(queryParam));
        }

        [Fact]
        public void GetHashCode_ReturnsSameValueForSameKey()
        {
            string key = "testKey";
            var param1 = KeyOnlyQueryParameter.Create(key);
            var param2 = KeyOnlyQueryParameter.Create(key);

            Assert.Equal(param1.GetHashCode(), param2.GetHashCode());
        }

        // Additional tests can be written for other methods and behaviors...
    }
}

