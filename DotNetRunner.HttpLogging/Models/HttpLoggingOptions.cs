using System.Text.Json;
using DotNetRunner.HttpLogging.BodyFormat;

namespace DotNetRunner.HttpLogging.Models;

public class HttpLoggingOptions
{
    /// <summary>
    /// Не логирующиеся запросы
    /// </summary>
    public IList<string> IgnoredPathPatterns { get; } = new List<string>();

    /// <summary>
    /// Не логирующиеся хедеры
    /// </summary>
    public IList<string> IgnoredHeaderPatterns { get; } = new List<string>();

    /// <summary>
    /// Преобразователи тела запроса
    /// </summary>
    public IList<PatternBodyFormatter> BodyFormatters { get; } = new List<PatternBodyFormatter>();

    /// <summary>
    /// Лимит на длинну тела запроса
    /// </summary>
    public int? RequestBodyLimit { get; set; }

    /// <summary>
    /// Конфигурация сериалайзера
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; set; }
}