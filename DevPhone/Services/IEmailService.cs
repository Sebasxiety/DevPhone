using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IEmailService
    {
        Task<bool> SendInvoiceEmailAsync(string toEmail, string customerName, MOrdenServicio ordenServicio, List<MDetalleRepuesto> detalles);
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task<bool> SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachment, string attachmentName, bool isHtml = true);
    }
}