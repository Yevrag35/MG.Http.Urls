using MG.Http.Urls.Queries;

namespace MG.Http.Urls.Tests
{
    public class QueryNumericParameterTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            string key = "testKey";
            int value = 42;
            var queryNumericParameter = new QueryNumericParameter<int>(key, value);

            Assert.Equal(key, queryNumericParameter.Key);
            Assert.Equal(value, queryNumericParameter.Value);
            Assert.False(queryNumericParameter.IsEmpty);
        }

        [Fact]
        public void Equals_ReturnsTrueForSameKeyAndValue()
        {
            var queryParam1 = new QueryNumericParameter<int>("key", 42);
            var queryParam2 = new QueryNumericParameter<int>("key", 42);

            Assert.True(queryParam1.Equals(queryParam2));
        }

        [Fact]
        public void Equals_ReturnsTrueForSameKeyAndDifferentValues()
        {
            var queryParam1 = new QueryNumericParameter<int>("key", 42);
            var queryParam2 = new QueryNumericParameter<int>("key", 40);

            Assert.True(queryParam1.Equals(queryParam2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentKeys()
        {
            var queryParam1 = new QueryNumericParameter<int>("key1", 42);
            var queryParam2 = new QueryNumericParameter<int>("key2", 42);

            Assert.False(queryParam1.Equals(queryParam2));
        }

        [Fact]
        public void ToString_FormatsCorrectly()
        {
            string key = "TestKey";
            string formattedKey = "testKey";
            int value = 42;
            var queryNumericParameter = new QueryNumericParameter<int>(key, value);
            string expectedFormat = $"{formattedKey}={value}";

            Assert.Equal(expectedFormat, queryNumericParameter.ToString());
        }

        [Fact]
        public void TryFormat_FormatsCorrectly()
        {
            string key = "TestKey";
            string formattedKey = "testKey";
            int value = 42;
            var queryNumericParameter = new QueryNumericParameter<int>(key, value);
            Span<char> span = new char[key.Length + 15];
            string expectedFormat = $"{formattedKey}={value}";

            bool result = queryNumericParameter.TryFormat(span, out int written, default, null);

            Assert.True(result);
            Assert.Equal(expectedFormat.Length, written);
            Assert.Equal(expectedFormat, new string(span.Slice(0, written)));
        }

        // Additional tests can be written for other methods and behaviors...
    }
}

