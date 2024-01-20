using MG.Http.Urls.Queries;

namespace MG.Http.Urls.Tests
{
    public class QueryParameterTests
    {
        [Fact]
        public void Constructor_SetsKeyAndValueCorrectly()
        {
            string key = "testKey";
            string value = "testValue";
            var queryParameter = QueryParameter.Create(key, value);

            Assert.Equal(key, queryParameter.Key);
            Assert.Equal(key.Length + value.Length + 1, queryParameter.MaxLength);
        }

        [Fact]
        public void Equals_ReturnsTrueForSameKey()
        {
            string key = "testKey";
            var queryParam1 = QueryParameter.Create(key, "value1");
            var queryParam2 = QueryParameter.Create(key, "value2");

            Assert.True(queryParam1.Equals(queryParam2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentKeys()
        {
            var queryParam1 = QueryParameter.Create("key1", "value");
            var queryParam2 = QueryParameter.Create("key2", "value");

            Assert.False(queryParam1.Equals(queryParam2));
        }

        [Fact]
        public void ToString_FormatsCorrectly()
        {
            string key = "TestKey";
            string formattedKey = "testKey";
            string value = "TestValue";
            var queryParameter = QueryParameter.Create(key, value);
            string expectedFormat = $"{formattedKey}={value}";

            Assert.Equal(expectedFormat, queryParameter.ToString());
        }

        [Fact]
        public void TryFormat_FormatsCorrectly()
        {
            string key = "TestKey";
            string formattedKey = "testKey";
            string value = "testValue";
            var queryParameter = QueryParameter.Create(key, value);
            Span<char> span = new char[key.Length + value.Length + 11];
            string expectedFormat = $"{formattedKey}={value}";

            bool result = queryParameter.TryFormat(span, out int written, default, null);

            Assert.True(result);
            Assert.Equal(expectedFormat.Length, written);
            Assert.Equal(expectedFormat, new string(span.Slice(0, written)));
        }

        // Additional tests can be written for other methods and properties...
    }
}