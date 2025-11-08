using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Models;
using Inspection.Infrastructure.Mail.Helpers;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Infrastructure.Mail;

public class EmailSender : IEmailSender
{
    private EmailSettings _emailSettings { get; }
    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    public async Task<bool> SendEmail(Email email)
    {
        var client = new SendGridClient(_emailSettings.ApiKey);
        var to = new EmailAddress(email.To);
        var from = new EmailAddress
        {
            Email = _emailSettings.FromAddress,
            Name = _emailSettings.FromName
        };

        var message = MailHelper.CreateSingleEmail(from,to,email.Subject,email.Body,email.Body);
        var response = await client.SendEmailAsync(message);

        return response.StatusCode== System.Net.HttpStatusCode.OK || response.StatusCode== System.Net.HttpStatusCode.Accepted;
    }


    public async Task<bool> SendEmail(string To, string CC, string Subject, string Body)
    {
        //MailAddress MAF = new MailAddress(ServerAttrs.AgentMailAddress);

        //MailAddress MAT = new MailAddress(To);
        //MailAddressCollection MACT = new MailAddressCollection();

        MailMessage message = new MailMessage();//MAF, MAT);

        string[] delimiter = { ";" };

        string[] toEmails = To.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        foreach (string email in toEmails)
        {
            message.To.Add(new MailAddress(email));
        }

        string[] ccEmails = CC.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        foreach (string email in ccEmails)
        {
            if (!email.Trim().Equals(""))
                message.CC.Add(new MailAddress(email));
        }

        message.From = new MailAddress(EmailAttributes.AgentMailAddress);
        message.Subject = Subject;
        message.Priority = MailPriority.High;
        message.IsBodyHtml = true;


        message.Body = Body;
        System.Net.Mail.SmtpClient smc = new SmtpClient();
        smc.DeliveryMethod = SmtpDeliveryMethod.Network;
        smc.Host = EmailAttributes.MailServerURL;

        System.Net.NetworkCredential nc = new System.Net.NetworkCredential(EmailAttributes.MailServerUserName, EmailAttributes.MailServerPassword);
        smc.Credentials = nc;
        try
        {
            smc.Send(message);
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
           

        }
        return false;
    }

    public static bool SendEmailWithAttechment(string To, string CC, string Subject, string Body, List<System.Net.Mail.Attachment> attachments)
    {

        MailMessage message = new MailMessage();//MAF, MAT);

        string[] delimiter = { ";" };

        string[] toEmails = To.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        foreach (string email in toEmails)
        {
            message.To.Add(new MailAddress(email));
        }

        string[] ccEmails = CC.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        foreach (string email in ccEmails)
        {
            if (!email.Trim().Equals(""))
                message.CC.Add(new MailAddress(email));
        }

        message.From = new MailAddress(EmailAttributes.AgentMailAddress);
        message.Subject = Subject;
        message.Priority = MailPriority.High;
        message.IsBodyHtml = true;


        message.Body = Body;
        if (attachments.Count > 0)
        {
            foreach (System.Net.Mail.Attachment att in attachments)
                message.Attachments.Add(att);
        }
        System.Net.Mail.SmtpClient smc = new SmtpClient();
        smc.DeliveryMethod = SmtpDeliveryMethod.Network;
        smc.Host = EmailAttributes.MailServerURL;

        System.Net.NetworkCredential nc = new System.Net.NetworkCredential(EmailAttributes.MailServerUserName, EmailAttributes.MailServerPassword);
        smc.Credentials = nc;
        try
        {
            smc.Send(message);
            return true;
        }
        catch (Exception ex)
        {
            throw ex;

        }
    }
}
