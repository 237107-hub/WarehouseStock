namespace WarehouseStock.Tests.Legacy;

// Локальна імітація з назвою з методички: SMTP-з’єднання не створюється.
public sealed class SmtpMailer
{
    private readonly TextWriter _output = TextWriter.Null;

    public void Send(string to, string subject, string body)
    {
        _output.WriteLine($"{to}: {subject}: {body}");
    }
}
