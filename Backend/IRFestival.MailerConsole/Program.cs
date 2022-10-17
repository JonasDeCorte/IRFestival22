// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;
using System.Text.Json;

Console.WriteLine("Hello, I'm a Mailer Console Application!");
var connectionString = "Endpoint=sb://irfestivalservicebusnamespacejdc.servicebus.windows.net/;SharedAccessKeyName=listener;SharedAccessKey=8ru8hJqf68BxJaj4V+bCZjmGuaOFUPV8HD4dKM59ES4=;EntityPath=mails";
var queueName = "mails";

await using (var client = new ServiceBusClient(connectionString))
{
    var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();

    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    Console.WriteLine("\n stopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
static async Task MessageHandler(ProcessMessageEventArgs args)
{
    var body = args.Message.Body;

    Console.WriteLine($"mail to send: {body}");

    MessageModel deserialized = JsonSerializer.Deserialize<MessageModel>(body);

    Console.WriteLine($"mail to send: {deserialized.Message} " +
        $"email: {deserialized.Email}");

    await args.CompleteMessageAsync(args.Message);
}
static Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

internal class MessageModel
{
    public string Message { get; set; }
    public string Email { get; set; }
}