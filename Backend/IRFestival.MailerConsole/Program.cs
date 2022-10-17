// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
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
    var email = new MimeMessage();
    email.From.Add(MailboxAddress.Parse("salvador.little5@ethereal.email"));
    email.To.Add(MailboxAddress.Parse("jdecorte6@gmail.com"));
    email.Subject = "Test Email Subject";
    email.Body = new TextPart(TextFormat.Html) { Text = $"<h1>Example HTML Message Body {body}</h1>" };
    MessageModel deserialized = JsonSerializer.Deserialize<MessageModel>(body);

    Console.WriteLine($"mail to send: {deserialized.Message} " +
        $"email: {deserialized.Email}");

    using var smtp = new SmtpClient();
    smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
    smtp.Authenticate("salvador.little5@ethereal.email", "cuEuRjjfSTXdyFnw91");
    smtp.Send(email);
    smtp.Disconnect(true);
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