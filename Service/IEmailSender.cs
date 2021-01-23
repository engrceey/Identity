using System.Threading.Tasks;

namespace identity.Service
{
    public interface IEmailSender
    {
         Task SendEmailAsync(string fromAddress, string toAddress, string subject, string message);
         
    }
}