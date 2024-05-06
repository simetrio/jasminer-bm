using System.Text.Json;
using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Text;
using System.Globalization;

namespace BestMiningTgChannel.Publisher;

// BTC
// BCH

// ETC
// ETHW
// OctaSpace
// Larissa
// Canxium
// Bitnet Money
// PowBlocks
// MetaChain
// Etica
// Dogether
// QuarkChain
// Calisto

// Litecoin
// Dogecoin

// Dash

// Kaspa

public class Handler : HandlerBase
{
    protected override string HandleRequest(string body, RequestData? requestData)
    {
        ChartSender.Send();
        return "Ok";
    }
}

public static class ChartSender
{
    public static string[] _imgs =
    {
        "https://jasminer-bm.ru/img/tg-channel/tg-channel-chart-1.jpg",
        "https://jasminer-bm.ru/img/tg-channel/tg-channel-chart-2.jpg",
        "https://jasminer-bm.ru/img/tg-channel/tg-channel-chart-3.jpg",
    };

    public static void Send()
    {
        if (DateTime.UtcNow.Hour != Settings.ChartHour)
        {
            return;
        }

        var charts = LoadCharts().ToArray();
        var minings = LoadMinings().ToArray();

        var message = MessageTemplate.Format(BuildMessage(charts, minings));
        var img = _imgs[DateTime.Now.Ticks % _imgs.Length];

        Telegram.SendMessage(message, img);
    }

    private static string BuildMessage(Chart[] charts, Mining[] minings)
    {
        var sb = new StringBuilder();

        sb.AppendLine("⚡️ Изменение курсов криптовалют за 24 часа ⚡️");

        foreach (var chart in charts)
        {
            var upDown = chart.DiffPercent >= 0 ? "↗️" : "↘️";
            var plusMinus = chart.DiffPercent >= 0 ? "+" : "";
            sb.AppendLine();
            sb.AppendLine($"{upDown} {chart.Currency} - {chart.Value.ToString("C", CultureInfo.CreateSpecificCulture("en-US"))}  ({plusMinus}{chart.DiffPercent:P2})");
        }

        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine("⚡️ Доходность майнинга за 24 часа без учета ээ ⚡️");

        foreach (var mining in minings)
        {
            sb.AppendLine();
            sb.AppendLine($"🪙 {mining.Currency} - {mining.Value.ToString("C", CultureInfo.CreateSpecificCulture("en-US"))} на {mining.HashRate}");
        }

        return sb.ToString();
    }

    private static IEnumerable<Chart> LoadCharts()
    {
        var html = new HttpClient()
            .GetStringAsync("https://www.coingecko.com")
            .GetAwaiter()
            .GetResult()
            .Replace("\r", "")
            .Replace("\n", "")
            ;

        var currencies = new[] { "Bitcoin", "Ethereum" };

        foreach (var currency in currencies)
        {
            var block = html.Substring(html.IndexOf($">{currency}<"));

            var name = block.Substring(block.IndexOf(">") + 1);
            name = name.Substring(0, name.IndexOf("<"));
            name = name.Trim();

            var value = block.Substring(block.IndexOf("$") + 1);
            value = value.Substring(0, value.IndexOf("<"));
            value = value.Trim().Replace(",", "");

            var percent = block.Substring(block.IndexOf("data-24h=\"true\""));
            percent = percent.Substring(percent.IndexOf(":") + 1);
            percent = percent.Substring(0, percent.IndexOf("}"));
            percent = percent.Trim();

            yield return new Chart
            {
                Currency = name,
                Value = decimal.Parse(value, CultureInfo.InvariantCulture),
                DiffPercent = decimal.Parse(percent, CultureInfo.InvariantCulture) / 100m,
            };
        }
    }

    private static IEnumerable<Mining> LoadMinings()
    {
        var currencies = new[]
        {
            "https://whattomine.com/coins/1-btc-sha-256?hr=100.0&p=3500.0&fee=0.0&cost=0.1&cost_currency=USD&hcost=0.0&span_br=1h&span_d=&commit=Calculate",
            "https://whattomine.com/coins/162-etc-etchash?hr=1000&p=390.0&fee=0.0&cost=0.1&cost_currency=USD&hcost=0.0&span_br=&span_d=24&commit=Calculate",
            "https://whattomine.com/coins/353-ethw-ethash?hr=1000&p=390.0&fee=0.0&cost=0.1&cost_currency=USD&hcost=0.0&span_br=1h&span_d=24&commit=Calculate",
        };

        foreach (var currency in currencies)
        {
            var html = new HttpClient()
                        .GetStringAsync(currency)
                        .GetAwaiter()
                        .GetResult()
                        .Replace("\r", "")
                        .Replace("\n", "")
                        ;

            var block = html.Substring(html.IndexOf($"<h1>"));

            var name = block.Substring(block.IndexOf(">") + 1);
            name = name.Substring(0, name.IndexOf("<"));
            name = name.Trim();

            var value = block.Substring(block.IndexOf("Please note that calculations"));
            value = value.Substring(value.IndexOf("Day"));
            value = value.Substring(value.IndexOf("$") + 1);
            value = value.Substring(0, value.IndexOf("<"));
            value = value.Trim().Replace(",", "");

            var hashRate = block.Substring(block.IndexOf("id=\"hr\""));
            hashRate = hashRate.Substring(hashRate.IndexOf("value"));
            hashRate = hashRate.Substring(hashRate.IndexOf("\"") + 1);
           
            var hashRateValue = hashRate.Substring(0, hashRate.IndexOf("\""));
            hashRateValue = hashRateValue.Trim();

            hashRate = hashRate.Substring(hashRate.IndexOf("span"));
            hashRate = hashRate.Substring(hashRate.IndexOf(">") + 1);
            hashRate = hashRate.Substring(0, hashRate.IndexOf("<"));
            hashRate = hashRate.Trim();

            yield return new Mining
            {
                Currency = name,
                Value = decimal.Parse(value, CultureInfo.InvariantCulture),
                HashRate = $"{hashRateValue} {hashRate}",
            };
        }
    }

    private class Chart
    {
        public string Currency { get; set; }
        public decimal Value { get; set; }
        public decimal DiffPercent { get; set; }
    }

    private class Mining
    {
        public string Currency { get; set; }
        public decimal Value { get; set; }
        public string HashRate { get; set; }
    }
}

public static class MessageTemplate
{
    public static string Format(string message)
    {
        return @$"{Escape(message)}

👉 [K1Pool](https://k1pool.com/invite/dd03779e65) \| [ByBit](https://www.bybit.com/invite?ref=ENN1VM8) \| [Прайс](https://t.me/BestMiningRu/8) \| [Заказ](https://t.me/BestMiningManager)";
    }

    private static string Escape(string message)
    {
        return message
        .Replace("=", "\\=")
        .Replace("-", "\\-")
        .Replace("+", "\\+")
        .Replace(".", "\\.")
        .Replace("|", "\\|")
        .Replace("(", "\\(")
        .Replace(")", "\\)")
        ;
    }
}

#region Telegram

public static class Telegram
{
    private static Lazy<TelegramBotClient> _botClient;

    static Telegram()
    {
        _botClient = new Lazy<TelegramBotClient>(() => new TelegramBotClient(Settings.TgBotToken));
    }

    public static void SendMessage(string message, string img)
    {
        _botClient.Value.SendPhotoAsync(
            chatId: Settings.TgChannelId,
            photo: InputFile.FromUri(img),
            caption: message,
            parseMode: ParseMode.MarkdownV2
        )
        .GetAwaiter()
        .GetResult();
    }
}

#endregion

#region Base

public static class Settings
{
    public static string TgBodId => Get("TgBotId");
    public static string TgBotToken => Get("TgBotToken");
    public static string TgChannelId => Get("TgChannelId");
    public static int ChartHour => GetInt("ChartHour");

    private static string Get(string name) => Environment.GetEnvironmentVariable(name)!;
    private static int GetInt(string name) => int.Parse(Get(name)!);
}

public abstract class HandlerBase
{
    public Response FunctionHandler(string body)
    {
        Logger.Debug(body);

        var request = GetRequest(body);
        var response = HandleRequest(request);

        return new Response(200, response);
    }

    private string HandleRequest(Request request)
    {
        var requestData = Parse(request.body);
        return HandleRequest(request.body, requestData);
    }

    protected abstract string HandleRequest(string request, RequestData? requestData);

    private Request GetRequest(string body)
    {
        return body.Contains("httpMethod")
                ? JsonSerializer.Deserialize<Request>(body)!
                : new Request { body = body, };
    }

    private RequestData? Parse(string body)
    {
        try
        {
            var requestDataDto = JsonSerializer.Deserialize<RequestDataDto>(body)!;

            return new RequestData
            {
                Action = Enum.Parse<Action>(requestDataDto.Action),
                Command = requestDataDto.Command,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    protected string SafeExecute(Func<string> func, Func<string>? onError = null)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            Logger.Log(e.Message);
            Logger.Log(e.StackTrace!);

            return onError?.Invoke() ?? "Sorry! I have an error(";
        }
    }
}

public enum Action
{
    Send,
}

public class Request
{
    public string httpMethod { get; set; } = null!;
    public string body { get; set; } = null!;
}

public class RequestData
{
    public Action Action { get; set; }

    public string Command { get; set; } = null!;
}

public class RequestDataDto
{
    public string Action { get; set; } = null!;

    public string Command { get; set; } = null!;
}

public class Response
{
    public Response(int statusCode, string body)
    {
        StatusCode = statusCode;
        Body = body;
    }

    public int StatusCode { get; set; }
    public string Body { get; set; }
}

public static class Logger
{
    public static bool IsDebug { get; set; }

    public static void Debug(string message)
    {
        if (IsDebug)
        {
            Console(message);
        }
    }

    public static void Console(string message)
    {
        message = $"{DateTime.Now} - {message}";

        System.Console.WriteLine(message);
    }

    public static void Log(string message)
    {
        Console(message);
    }
}

#endregion
