using BestMiningTgChannel.Publisher;

namespace BestMiningTgChannel.Tests;

public class HandlerTests
{
    [Fact]
    public void Send()
    {
        Environment.SetEnvironmentVariable("TgBotId", File.ReadAllText("/home/roman/TgBotId.txt").Trim());
        Environment.SetEnvironmentVariable("TgBotToken", File.ReadAllText("/home/roman/TgBotToken.txt").Trim());
        Environment.SetEnvironmentVariable("TgChannelId", File.ReadAllText("/home/roman/TgChannelId.txt").Trim());
        Environment.SetEnvironmentVariable("TgChatId", File.ReadAllText("/home/roman/TgChatId.txt").Trim());
        Environment.SetEnvironmentVariable("TgUserName", File.ReadAllText("/home/roman/TgUserName.txt").Trim());
        Environment.SetEnvironmentVariable("ChartHour", DateTime.UtcNow.Hour.ToString());
        Environment.SetEnvironmentVariable("YaIamToken", File.ReadAllText("/home/roman/YaIamToken.txt").Trim());
        Environment.SetEnvironmentVariable("YaFolderId", File.ReadAllText("/home/roman/YaFolderId.txt").Trim());

        new Handler().FunctionHandler("{\"Action\": \"Send\"}");
    }
}