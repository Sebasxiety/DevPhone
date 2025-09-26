using DevPhone.Models;
using DevPhone.Models.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace DevPhone.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfigurationDto _emailConfig;
        private readonly ILogger<EmailService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EmailService(IOptions<EmailConfigurationDto> emailConfig, ILogger<EmailService> logger, IServiceProvider serviceProvider)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> SendInvoiceEmailAsync(string toEmail, string customerName, MOrdenServicio ordenServicio, List<MDetalleRepuesto> detalles)
        {
            try
            {
                var invoiceData = new InvoiceEmailDto
                {
                    ClientName = customerName,
                    ClientEmail = toEmail,
                    OrderNumber = ordenServicio.IdOrden.ToString(),
                    OrderDate = ordenServicio.Fecha,
                    OrderStatus = "Finalizada",
                    DeviceInfo = $"{ordenServicio.Dispositivo?.Marca} {ordenServicio.Dispositivo?.Modelo}",
                    ServiceDescription = ordenServicio.DescripcionFalla,
                    ServicePrice = ordenServicio.PrecioServicio,
                    PartsPrice = ordenServicio.SubtotalRepuestos,
                    TotalPrice = ordenServicio.TotalConIva,
                    Parts = detalles.Select(d => new InvoicePartDto
                    {
                        Name = d.Repuesto?.Nombre ?? "Repuesto",
                        Quantity = d.Cantidad,
                        UnitPrice = d.Repuesto?.PrecioUnitario ?? 0,
                        TotalPrice = d.Cantidad * (d.Repuesto?.PrecioUnitario ?? 0)
                    }).ToList()
                };

                var subject = $"Factura de Reparación - Orden #{invoiceData.OrderNumber}";
                var body = await RenderViewToStringAsync("InvoiceEmailTemplate", invoiceData);

                return await SendEmailAsync(toEmail, subject, body, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar factura por email a {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.FromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                {
                    bodyBuilder.HtmlBody = body;
                }
                else
                {
                    bodyBuilder.TextBody = body;
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email enviado exitosamente a {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email a {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachment, string attachmentName, bool isHtml = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.FromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                {
                    bodyBuilder.HtmlBody = body;
                }
                else
                {
                    bodyBuilder.TextBody = body;
                }

                // Agregar adjunto
                bodyBuilder.Attachments.Add(attachmentName, attachment);

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email con adjunto enviado exitosamente a {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email con adjunto a {Email}", toEmail);
                return false;
            }
        }

        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            using var scope = _serviceProvider.CreateScope();
            var httpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var viewEngine = scope.ServiceProvider.GetRequiredService<ICompositeViewEngine>();
            var tempDataProvider = scope.ServiceProvider.GetRequiredService<ITempDataProvider>();

            var viewResult = viewEngine.FindView(actionContext, viewName, false);
            if (!viewResult.Success)
            {
                throw new ArgumentNullException($"Unable to find view '{viewName}'");
            }

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(actionContext.HttpContext, tempDataProvider);

            using var stringWriter = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewData,
                tempData,
                stringWriter,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return stringWriter.ToString();
        }
    }
}