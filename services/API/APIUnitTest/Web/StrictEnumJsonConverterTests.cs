using System.Text.Json;
using AqLife.Shared.Options;
using AqLife.Web.Json;

namespace AqLife.APIUnitTest.Web;

public class StrictEnumJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = CreateOptions();

    [Fact]
    public void Deserialize_ShouldAcceptDefinedEnumValue()
    {
        var value = JsonSerializer.Deserialize<FileStatus>("2", Options);

        Assert.Equal(FileStatus.Published, value);
    }

    [Fact]
    public void Deserialize_ShouldRejectUndefinedEnumValue()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FileStatus>("99", Options));
    }

    [Fact]
    public void Deserialize_ShouldRejectOutOfRangeEnumValue()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FileStatus>("-1", Options));
    }

    [Fact]
    public void Serialize_ShouldWriteUnderlyingNumericValue()
    {
        var json = JsonSerializer.Serialize(FileStatus.Scheduled, Options);

        Assert.Equal("1", json);
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new StrictEnumJsonConverterFactory());
        return options;
    }
}
