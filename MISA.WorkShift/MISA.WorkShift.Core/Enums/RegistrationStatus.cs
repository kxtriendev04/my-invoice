namespace MISA.WorkShift.Core.Enums
{
    /// <summary>
    /// Tr?ng thái c?a t? khai ??ng ký s? d?ng hóa ??n
    /// ??ng b? hi?n th? gi?a Server và Client
    /// </summary>
    public enum RegistrationStatus
    {
        /// <summary>Nháp / Ch?a g?i</summary>
        Draft = 1,

        /// <summary>?ã g?i lên C? quan Thu?</summary>
        Sent = 2,

        /// <summary>?ang x? lý t?i CQT</summary>
        Processing = 3,

        /// <summary>CQT ch?p nh?n</summary>
        Accepted = 4,

        /// <summary>CQT t? ch?i</summary>
        Rejected = 5
    }
}
