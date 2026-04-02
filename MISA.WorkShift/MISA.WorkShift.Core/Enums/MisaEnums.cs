namespace MISA.WorkShift.Core.Enums
{
    public enum ShiftStatus
    {
        Inactive = 0,
        Active = 1
    }

    public enum InvoiceType
    {
        GTGT = 1,
        BanHang = 2
    }

    public enum IssueStatus
    {
        New = 1,
        Signed = 2,
        Published = 3,
        Canceled = 4
    }

    public enum CompanyStatus
    {
        Active = 1,
        Locked = 2
    }

    public enum UserRole
    {
        Admin = 1,
        Accountant = 2,
        Viewer = 3
    }
    
    public enum TransType
    {
        NewRegistration = 1,
        ChangeInformation = 2
    }
}