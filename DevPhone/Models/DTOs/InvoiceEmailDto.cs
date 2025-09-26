namespace DevPhone.Models.DTOs
{
    public class InvoiceEmailDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string DeviceInfo { get; set; } = string.Empty;
        public string ServiceDescription { get; set; } = string.Empty;
        public decimal ServicePrice { get; set; }
        public decimal PartsPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public List<InvoicePartDto> Parts { get; set; } = new List<InvoicePartDto>();
    }

    public class InvoicePartDto
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}