namespace WebApplication25.Models
{
    public class EntityGetResponseForInvoicing
    {
        public int ID { get; set; }
        public string? internalID { get; set; }
        public string? Success { get; set; }
        public string? ReciverName { get; set; }
        public byte[]? Binary_File { get; set; } = null;

        public string? salesOrderReference { get; set; }
        public string? Registration_Number { get; set; }
        public Int64? Site_ID { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
