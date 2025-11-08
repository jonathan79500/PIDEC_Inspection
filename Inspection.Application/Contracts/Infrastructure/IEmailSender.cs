

using Inspection.Application.Models;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Infrastructure;

public interface IEmailSender
{
    Task<bool> SendEmail(Email email);
    Task<bool> SendEmail(string To, string CC, string Subject, string Body);
}
