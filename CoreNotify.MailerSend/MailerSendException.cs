namespace CoreNotify.MailerSend;

public class MailerSendException() : Exception("Email sent successfully, but X-Message-Id header not found in response")
{
}
