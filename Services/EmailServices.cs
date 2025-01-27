using MailKit.Net.Smtp;
using MimeKit;
namespace PetProjectOne.Services;
public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _senderEmail = "adegunwaanu@gmail.com";
    private readonly string _senderName = "Task App";
    private readonly string _senderPassword = "kdor-innz-yimj-njbu";

    public async Task SendTaskAssignedEmail(string recipientEmail, string taskTitle)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_senderName, _senderEmail));
        emailMessage.To.Add(new MailboxAddress("Tasker", recipientEmail));
        emailMessage.Subject = "You have a new task assigned!";
        emailMessage.Body = new TextPart("plain")
        {
            Text = $"Hello Tasker,\n\nYou have been assigned a new task: '{taskTitle}'. Please check your task list for more details."
        };

        using (var smtpClient = new SmtpClient())
        {
            await smtpClient.ConnectAsync(_smtpServer, _smtpPort, false);
            await smtpClient.AuthenticateAsync("adegunwaanu@gmail.com", "kdor-innz-yimj-njbu"); // Use your email credentials here
            await smtpClient.SendAsync(emailMessage);
            await smtpClient.DisconnectAsync(true);
        }
    }

   
}
