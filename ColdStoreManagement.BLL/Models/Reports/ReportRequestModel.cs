using System;

namespace ColdStoreManagement.BLL.Models.Reports
{
    public class ReportRequestModel
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PartyId { get; set; }
        public int GrowerId { get; set; }
        public string? VehicleNo { get; set; }
        public string? ProductName { get; set; }
        public string? Uom { get; set; }
        public string? Status { get; set; }
        public string? CommonSearch { get; set; }
        public string? ChamberName { get; set; }
        public string? ReportType { get; set; }
    }
}
