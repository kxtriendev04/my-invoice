using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MISA.WorkShift.Core.Enums;

namespace MISA.WorkShift.Core.Services
{
    public class InvoiceRegistrationService : BaseService<InvoiceRegistration>, IInvoiceRegistrationService
    {
        private readonly IInvoiceRegistrationRepository _registrationRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly INotificationService _notificationService;

        public InvoiceRegistrationService(IInvoiceRegistrationRepository registrationRepository, IInvoiceRepository invoiceRepository, IServiceScopeFactory scopeFactory, INotificationService notificationService) : base(registrationRepository)
        {
            _registrationRepository = registrationRepository;
            _invoiceRepository = invoiceRepository;
            _scopeFactory = scopeFactory;
            _notificationService = notificationService;
        }

        public IEnumerable<InvoiceRegistration> GetByCompanyId(Guid companyId)
        {
            return _registrationRepository.GetByCompanyId(companyId);
        }

        /// <summary>
        /// Xử lý ký số và lưu trữ XML hóa đơn/tờ khai
        /// </summary>
        public async Task<int> SignAndPublishInvoice(Guid invoiceId, string signedXmlContent)
        {
            // 1. Lấy thông tin Hóa đơn từ bảng Invoice (Dữ liệu thật)
            var invoice = _invoiceRepository.GetById(invoiceId);
            if (invoice == null) 
                throw new Exception("Không tìm thấy hóa đơn.");

            // 2. Xác định đường dẫn lưu trữ file XML
            var relativePath = Path.Combine("Storage", "Invoices", invoice.CompanyId.ToString());
            var companyDir = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            
            if (!Directory.Exists(companyDir))
            {
                Directory.CreateDirectory(companyDir);
            }

            var fileName = $"{invoiceId}.xml";
            var fullFilePath = Path.Combine(companyDir, fileName);

            // 3. Ghi nội dung XML (đã bao gồm chữ ký) vào file
            await File.WriteAllTextAsync(fullFilePath, signedXmlContent, new UTF8Encoding(false));

            // 4. Cập nhật trạng thái Hóa đơn sang "Đã ký / Chờ CQT"
            invoice.XmlFilePath = "/" + Path.Combine(relativePath, fileName).Replace("\\", "/");
            invoice.IssueStatus = (int)InvoiceIssueStatus.Signed; // 2: Đã ký
            invoice.CqtStatus = 2; // 2: Đang chờ CQT phê duyệt
            invoice.SellerSignedDate = DateTime.Now;
            invoice.ModifiedDate = DateTime.Now;
            invoice.ModifiedBy = "System (Digital Sign)";
            
            return _invoiceRepository.Update(invoiceId, invoice);
        }

        public async Task<string> SaveFullRegistration(RegistrationSaveDto dto)
        {
            try
            {
                // 1. Khởi tạo ID chính
                var regId = Guid.NewGuid();
                dto.Registration.RegistrationId = regId;
                dto.Registration.CreatedDate = DateTime.Now;

                // 2. Chuẩn bị dữ liệu Chữ ký số
                if (dto.Signatures != null)
                {
                    foreach (var sig in dto.Signatures)
                    {
                        sig.SignatureId = Guid.NewGuid();
                        sig.RegistrationId = regId;
                        sig.CreatedDate = DateTime.Now;
                    }
                }

                // 3. Gọi Repository để thực hiện lưu trong 1 Transaction
                await _registrationRepository.SaveFullRegistrationAsync(dto.Registration, dto.Signatures);

                // 5. GIẢ LẬP GỬI CƠ QUAN THUẾ (Background Process)
                _ = Task.Run(async () => 
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var repo = scope.ServiceProvider.GetRequiredService<IInvoiceRegistrationRepository>();
                        var notification = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        await SimulateTaxAuthorityProcessInScope(repo, notification, regId);
                    }
                });

                return "Tờ khai đã được ký và gửi lên Cơ quan Thuế thành công. Vui lòng đợi thông báo chấp nhận.";
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Xử lý CQT phê duyệt và cấp mã cho hóa đơn (Giả lập)
        /// </summary>
        public async Task<bool> ApproveInvoiceFromCqt(Guid invoiceId)
        {
            // 1. Lấy thông tin hóa đơn từ bảng Invoice (Dữ liệu thật)
            var invoice = _invoiceRepository.GetById(invoiceId); 
            if (invoice == null) return false;

            // 2. Cập nhật trạng thái theo đúng thiết kế db.sql
            invoice.IssueStatus = (int)InvoiceIssueStatus.Issued; // 3: Đã phát hành
            invoice.CqtStatus = 3; // 3: CQT Chấp nhận
            invoice.ModifiedDate = DateTime.Now;
            invoice.ModifiedBy = "TCT_SYSTEM_CAP_MA";
            
            // 3. Giả lập sinh mã CQT
            var cqtCode = "CQT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            invoice.CqtCode = cqtCode;

            var rowEffect = _invoiceRepository.Update(invoiceId, invoice);

            // 4. Gửi thông báo thời gian thực về cho Client qua SignalR
            await _notificationService.SendNotification("ReceiveInvoiceResult", new
            {
                InvoiceId = invoiceId,
                CqtCode = invoice.CqtCode,
                Status = 3, // Đã phát hành
                Message = $"Hóa đơn số {invoice.InvoiceNumber} đã được CQT cấp mã thành công: {cqtCode}"
            });

            return rowEffect > 0;
        }

        private async Task SimulateTaxAuthorityProcessInScope(IInvoiceRegistrationRepository repo, INotificationService notification, Guid registrationId)
        {
            // Giả lập thời gian CQT xử lý (khoảng 10 giây)
            await Task.Delay(10000);

            var reg = repo.GetById(registrationId);
            if (reg != null)
            {
                // Cập nhật trạng thái thành 4: Chấp nhận
                reg.Status = 4; 
                reg.ModifiedDate = DateTime.Now;
                reg.ModifiedBy = "TCT_SYSTEM";
                repo.Update(registrationId, reg);
                
                // Gửi thông báo thời gian thực xuống Frontend
                await notification.SendNotification("ReceiveRegistrationResult", new {
                    RegistrationNo = reg.RegistrationNo,
                    Status = 4,
                    Message = $"Tờ khai số {reg.RegistrationNo} đã được Cơ quan Thuế CHẤP NHẬN."
                });
                Console.WriteLine($"[SIMULATION] Tờ khai {registrationId} đã được Cơ quan Thuế CHẤP NHẬN.");
            }
        }

        /// <summary>
        /// Xử lý duyệt tờ khai từ phía CQT (Dữ liệu thật)
        /// </summary>
        public async Task<bool> ApproveRegistrationFromCqt(Guid id)
        {
            // 1. Lấy tờ khai từ bảng InvoiceRegistration
            var reg = _registrationRepository.GetById(id);
            if (reg == null) return false;

            // 2. Cập nhật trạng thái (4: Chấp nhận theo db.sql)
            reg.Status = 4; 
            reg.ModifiedDate = DateTime.Now;
            reg.ModifiedBy = "TCT_SYSTEM";
            var rowEffect = _registrationRepository.Update(id, reg);

            // 3. Gửi tín hiệu SignalR để Frontend nhận được thông báo ngay lập tức
            await _notificationService.SendNotification("ReceiveRegistrationResult", new {
                RegistrationNo = reg.RegistrationNo,
                Status = 4,
                Message = $"Tờ khai số {reg.RegistrationNo} đã được Cơ quan Thuế CHẤP NHẬN."
            });

            return rowEffect > 0;
        }
    }
}