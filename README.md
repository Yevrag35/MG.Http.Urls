# URL Parameter Building

## Overview
MG.Http.Urls (URL Parameter Building) is an open-source .NET 8 library designed to provide high-performance URL query parameter building capabilities. It's main use is for my own projects, but feel free to use it as well.
It simplifies the process of constructing query parameters for HTTP requests, ensuring efficient and error-free parameter encoding.

## Features
- **Efficient Parameter Handling**: Streamlines the process of adding and manipulating query parameters.
- **Type Safety**: Offers robust type-checking to prevent common errors associated with query parameter handling.
- **Localization Support**: Designed with localization in mind, it can adapt to different language requirements.
- **.NET 8 Compatibility**: Optimized for use with .NET 8, leveraging the latest features for maximum performance.

## Getting Started
To get started with MG.Http.Urls, include it in your .NET 8 project via NuGet or by cloning the repository.

### Basic Usage
```csharp
// Using the QueryParameterCollection
public class CustomUrlBuilder
{
    readonly QueryParameterCollection _parameters;
    public string BaseUrl { get; }

    public CustomUrlBuilder(string baseUrl)
    {
        _parameters = new QueryParameterCollection();
        this.BaseUrl = baseUrl;
    }

    public void AddId(int id)
    {
        // Calling AddId(int) multiple times only adds the very first ID provided, the rest are discarded.
        _parameters.Add(nameof(id), id);
    }
    public void AddGlobalId(Guid globalId)
    {
        // Calling AddGlobalId(Guid) multiple times simply overwrites the existing "globalId" parameter.
        // 32 is the max length of a GUID when formatted with the option "N".
        IQueryParameter p = QueryParameter.Create(nameof(globalId), globalId, maxLength: 32, format: "N");
        _parameters.AddOrUpdate(p);
    }

    // SIMPLE - Example of using string formatting to build the full URL.
    public string Build()
    {
        if (_parameters.Count == 0)
        {
            return this.BaseUrl;
        }

        return string.Format("{0}?{1}", this.BaseUrl, _parameters);
    }

    // ADVANCED - Example of writing the full URL to the provided Span<char>. This can be done because
    //            QueryParameterCollection implements System.ISpanFormattable
    public bool TryFormat(Span<char> span, out int charsWritten)
    {
        charsWritten = 0;
        if (!this.BaseUrl.AsSpan().TryCopyTo(span))
        {
            return false;
        }

        charsWritten += this.BaseUrl.Length;
        
        if (_parameters.Count == 0)
        {
            return true;
        }

        try
        {
            span[charsWritten++] = '?';
        }
        catch (IndexOutOfBoundsException)
        {
            return false;
        }

        bool formatted = _parameters.TryFormat(span.Slice(charsWritten), out int paramsWritten, default, null);
        charsWritten += paramsWritten;

        return formatted;
    }
}

```

## Contributing
Contributions are welcome! If you would like to contribute to the project, feel free.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
