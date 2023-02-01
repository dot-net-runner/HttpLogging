# DotNetRunner.HttpLogging
## Об этом проекте
Middleware для логирования http запросов. При запросе к приложению, middleware логирует запрос, ответ и время выполнения.
### Пример использования
#### Стандартное использование
```
app.UseHttpRequestsLogging(config => config
    .IgnoreSwagger()
    .CreateDefaultSerializer()
    .IgnoreAllHeaders());
```
#### Игнорирование некоторых хедеров
```
app.UseHttpRequestsLogging(config => config.IgnoreSwagger()
        .CreateDefaultSerializer()
        .IgnoredHeaderPatterns.Add("User-Agent.*"));
```
#### Игнорирование некоторых запросов
```
app.UseHttpRequestsLogging(config => config.IgnoreAllHeaders()
    .CreateDefaultSerializer()
    .IgnoredHeaderPatterns.Add("/About.*"));
```
#### Настройка вывода через сериалайзер
```
var serializerOptions = new JsonSerializerOptions()
{
//Настройки
};

app.UseHttpRequestsLogging(config => config.IgnoreAllHeaders()
.CreateDefaultSerializer()
.SerializerOptions = serializerOptions);
```
#### Игнорировать некторые свойства в теле JSON
```
app.UseHttpRequestsLogging(options =>
{
    options.AddBodyFormatter(new IgnorePattern("POST", ".*"),
        new JsonPropertyIgnoreFormatter("Content.data", "Content.Error", "NoMatter", "Content.Hello"));
});
```

### Выводимая информация
Информация выводится по шаблону `{request} {response} {elapsed}`
####Пример
```
{
  "method": "GET",
  "uri": "https://localhost:7161/",
  "body": "",
  "headers": {
    "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8",
    "Host": "localhost:7161",
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; W\r\nin64; x64; rv:98.0) Gecko/20100101 Firefox/98.0",
    ":method": "GET",
    "Accept-Encoding": "gzip, deflate, br",
    "Accept-Language": "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3",
    "TE": "trailers",
    "Upgrade-Insecure-Requests": "1",
    "sec-fetch-dest": "document",
    "sec-f\r\netch-mode": "navigate",
    "sec-fetch-site": "none",
    "sec-fetch-user": "?1"
  }
}

{
  "code": 200,
  "body": "{\"payload\":\"payload\"}"
}

 00:00:00.0198780
```
###Настройка
Настройка осуществляется при помощи класса `HttpLoggingOptions`

####Свойства
- `IgnoredPathPatterns` - список регулярных выражений, определяющий запросы, которые не должны лгироваться
- `IgnoredHeaderPatterns` - список регулярных выражений, определяющий хедеры, которые не должны лгироваться
- `RequestBodyLimit` - длину вывода тела запроса
- `JsonSerializerOptions` - настройки сериализатора
####Методы расширения
- `IgnoreSwagger` - отключает логирование запросов к свагеру
- `IgnoreAllHeaders` - отключает логирование хедеров
- `CreateDefaultSerializer` - задает дефолтные значения свойств сериалайзера
- `AddBodyFormatter` - добавить форматиривания тела запроса и ответа 

Свойства сериалайзера при использовании `CreateDefaultSerializer ` 
```
 PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
 Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
 DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
```
### Форматирование тела запроса/ответа
Для форматирования тела следует реализовать интерфейс `IBodyFormatter` и добавить его в настройках при помощи метода `AddBodyFormatter`.

#### Существующие форматеры тела
1) `JsonPropertyIgnoreFormatter` - игнорирует свойства в теле Json 