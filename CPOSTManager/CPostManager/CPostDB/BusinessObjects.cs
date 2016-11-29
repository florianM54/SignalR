using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CPostManager.BusinessObjects
{
    public class SupplierSync
    {
        public string UpdateType { get; set; }
        public string Lnum { get; set; }
        public string Snum { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public string FaxNo { get; set; }
        public string Contact { get; set; }
        public string Cmt1 { get; set; }
        public string Cmt2 { get; set; }
        public DateTime LpDate { get; set; }
        public string SupNum { get; set; }
        public string FastIn { get; set; }
        public string Dolpcn { get; set; }
        public string FilterC { get; set; }
        public string TaxNum { get; set; }
        public string AsNum { get; set; }
        public int LeadTime { get; set; }
        public decimal PayTerm { get; set; }
        public string TermType { get; set; }
        public string CatC { get; set; }
        public string OrdInst { get; set; }
        public decimal Dfpcnt { get; set; }
        public string FCur { get; set; }
        public int Ols { get; set; }
        public string Phone2 { get; set; }
        public string Cnt2 { get; set; }
        public decimal Retp { get; set; }
        public string Phone3 { get; set; }
        public string Phone4 { get; set; }
        public string FaxNo2 { get; set; }
        public string Country { get; set; }
        public string EMail { get; set; }
        public string CnsNum { get; set; }
        public string CnpCat { get; set; }
        public string HosNum { get; set; }
        public string HopCat { get; set; }
        public string SndOrd { get; set; }
        public string SndQuo { get; set; }
        public string SordFmt { get; set; }
        public string Des2 { get; set; }
        public string Des3 { get; set; }
        public int Ngrn { get; set; }
        public string CnCatc { get; set; }
        public DateTime LcDate { get; set; }
        public string LcBy { get; set; }
        public DateTime CDate { get; set; }
        public string CBy { get; set; }
        public string Status { get; set; }
        public bool Discont { get; set; }
        public bool Activate { get; set; }
        public string PayNote { get; set; }
        public string Web { get; set; }
        public string Bank1 { get; set; }
        public string Bank2 { get; set; }
    }

    public class Task
    {
        public string ConnId { get; set; }
        public string TaskCode { get; set; }
        public string DataConnStr { get; set; }
        public string LogConnStr { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }
        public string QString { get; set; }
        public string SpName { get; set; }
        public string ConType { get; set; }
        public string LocalDir { get; set; }
        public string FtpAddr { get; set; }
        public string FtpUsrId { get; set; }
        public string FtpUsrPassw { get; set; }
        public int Status { get; set; }
    }

    public class Arguments 
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }

    public class Message 
    {
        public string ConnId { get; set; }
        public string Msg { get; set; }
    }

    public class TaskLog 
    {
        public int LogId { get; set; }
        public DateTime Timeg { get; set; }
        public DateTime NotifTime { get; set; }
        public string Task { get; set; }
        public string QId { get; set; }
        public string FileCreated { get; set; }
        public string FileProcessed { get; set; }
        public int NoOfRecords { get; set; }
        public int NoOfRetries { get; set; }
        public string SystemError { get; set; }
        public string TaskError { get; set; }
        public string Acknowledged { get; set; }
        public DateTime? DateAcknowledged { get; set; }
        public int AcknowledgedFileRecs { get; set; }
        public string AlertError { get; set; }
        public bool Success { get; set; }
    }

    public enum TaskStatus
    {
        ForRunnning,
        Running,
        ToFile,
        ToFtp,
        Done,
        Failed
    }
}
