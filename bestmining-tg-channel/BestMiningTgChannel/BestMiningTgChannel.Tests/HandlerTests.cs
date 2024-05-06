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

        new Handler().FunctionHandler("{\"Action\": \"Send\"}");
    }
}