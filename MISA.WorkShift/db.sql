-- =========================================================================
-- SCRIPT KHỞI TẠO DATABASE MYINVOICE (SAAS MODEL) - BẢN HOÀN CHỈNH
-- Chú ý: Các trường ID (GUID) sử dụng CHAR(36)
-- =========================================================================

-- 1. HỆ THỐNG QUẢN TRỊ & ĐA CHI NHÁNH (MULTI-TENANCY)
-- =========================================================================

CREATE TABLE Company (
    CompanyId CHAR(36) PRIMARY KEY,
    CompanyTaxCode VARCHAR(14) NOT NULL UNIQUE,
    CompanyName VARCHAR(255) NOT NULL,
    Address VARCHAR(500),
    Representative VARCHAR(100),
    Status INT DEFAULT 1, -- 1: Active, 2: Locked
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50)
);

CREATE TABLE User (
    UserId CHAR(36) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50)
);

CREATE TABLE CompanyUser (
    CompanyId CHAR(36) NOT NULL,
    UserId CHAR(36) NOT NULL,
    RoleId INT NOT NULL, -- 1: Admin, 2: Accountant, 3: Viewer
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    PRIMARY KEY (CompanyId, UserId),
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId),
    FOREIGN KEY (UserId) REFERENCES User(UserId)
);

-- 2. HỆ THỐNG DANH MỤC (MASTER DATA)
-- =========================================================================

CREATE TABLE Customer (
    CustomerId CHAR(36) PRIMARY KEY,
    CompanyId CHAR(36) NOT NULL,
    BuyerTaxCode VARCHAR(14),
    BuyerName VARCHAR(255),
    BuyerLegalName VARCHAR(255),
    BuyerAddress VARCHAR(500),
    BuyerEmail VARCHAR(100),
    BuyerPhone VARCHAR(20),
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50),
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId),
    INDEX idx_customer_company (CompanyId)
);

CREATE TABLE InvoiceTemplate (
    TemplateId CHAR(36) PRIMARY KEY,
    CompanyId CHAR(36) NOT NULL,
    TemplateCode VARCHAR(20) NOT NULL,
    TemplateName VARCHAR(100) NOT NULL,
    InvSeries VARCHAR(10) NOT NULL,
    CssCustom TEXT,
    IsDefault BOOLEAN DEFAULT FALSE,
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50),
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId),
    INDEX idx_template_company (CompanyId)
);

-- 3. HỆ THỐNG HÓA ĐƠN & CHỨNG TỪ (TRANSACTION DATA)
-- =========================================================================

CREATE TABLE Invoice (
    InvoiceId CHAR(36) PRIMARY KEY,
    CompanyId CHAR(36) NOT NULL, -- Core Tenant ID
    CustomerId CHAR(36),
    TemplateId CHAR(36),
    
    -- Phân loại & Số hiệu
    InvoiceType INT NOT NULL, -- 1: GTGT, 2: Bán hàng
    TemplateCode VARCHAR(20), 
    Series VARCHAR(10) NOT NULL,
    InvoiceNumber VARCHAR(8),
    InvoiceDate DATETIME NOT NULL,
    
    -- Thông tin người bán (Snapshot tại thời điểm xuất HĐ)
    SellerTaxCode VARCHAR(14),
    SellerName VARCHAR(255),
    SellerAddress VARCHAR(500),

    -- Thông tin người mua (Lưu snapshot để không bị ảnh hưởng nếu update danh bạ)
    BuyerTaxCode VARCHAR(14),
    BuyerName VARCHAR(255),
    BuyerLegalName VARCHAR(255),
    BuyerAddress VARCHAR(500),
    BuyerEmail VARCHAR(100),
    BuyerPhone VARCHAR(20),
    BuyerBankAccount VARCHAR(50),  -- Mới bổ sung
    BuyerBankName VARCHAR(255),    -- Mới bổ sung
    
    -- Tiền tệ & Thanh toán
    PaymentMethod VARCHAR(50), -- TM/CK, CK
    CurrencyCode VARCHAR(10) DEFAULT 'VND',
    ExchangeRate DECIMAL(18,4) DEFAULT 1,
    PaymentStatus INT DEFAULT 0, -- 0: Chưa thanh toán, 1: Đã thanh toán (Mới bổ sung)
    
    -- Tổng hợp Tài chính
    TotalBeforeTax DECIMAL(18,2) DEFAULT 0,
    TotalDiscount DECIMAL(18,2) DEFAULT 0,
    TotalTaxAmount DECIMAL(18,2) DEFAULT 0,
    TotalAmount DECIMAL(18,2) DEFAULT 0,
    TotalAmountInWords VARCHAR(500),
    
    -- Trạng thái & Cơ quan Thuế
    IssueStatus INT NOT NULL DEFAULT 1, -- 1: Mới, 2: Đã ký, 3: Đã phát hành, 4: Hủy
    CqtCode VARCHAR(50),
    CqtStatus INT, -- 1: Chưa gửi, 2: Đang chờ, 3: Chấp nhận, 4: Từ chối
    SellerSignedDate DATETIME,
    ReferenceInvoiceId CHAR(36), -- Trỏ đến hóa đơn bị thay thế/điều chỉnh (nếu có)
    
    -- Lưu trữ File gốc có giá trị pháp lý (Mới bổ sung)
    XmlFilePath VARCHAR(1000), 
    PdfFilePath VARCHAR(1000),

    -- Hệ thống & Audit
    Note VARCHAR(1000),
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50),
    
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId),
    FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
    FOREIGN KEY (TemplateId) REFERENCES InvoiceTemplate(TemplateId),
    INDEX idx_invoice_company (CompanyId)
);

CREATE TABLE InvoiceDetail (
    DetailId CHAR(36) PRIMARY KEY,
    InvoiceId CHAR(36) NOT NULL,
    ItemName VARCHAR(500) NOT NULL,
    UnitName VARCHAR(50),
    Quantity DECIMAL(18,4) DEFAULT 0,
    UnitPrice DECIMAL(18,4) DEFAULT 0,
    DiscountRate DECIMAL(18,4) DEFAULT 0,  -- Mới bổ sung
    DiscountAmount DECIMAL(18,2) DEFAULT 0, -- Mới bổ sung
    TaxRate INT, -- 0, 5, 8, 10, -1 (Không chịu thuế)
    TaxAmount DECIMAL(18,2) DEFAULT 0,
    Amount DECIMAL(18,2) DEFAULT 0,
    
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50),
    
    FOREIGN KEY (InvoiceId) REFERENCES Invoice(InvoiceId) ON DELETE CASCADE
);

-- 4. HỆ THỐNG TỜ KHAI ĐĂNG KÝ (REGISTRATION SYSTEM - MẪU 01/ĐKTĐ-HĐĐT)
-- =========================================================================

CREATE TABLE InvoiceRegistration (
    RegistrationId CHAR(36) PRIMARY KEY,
    CompanyId CHAR(36) NOT NULL,
    RegistrationNo VARCHAR(25),
    TransType INT NOT NULL, -- 1: Đăng ký mới, 2: Thay đổi thông tin
    
    -- Thông tin người nộp thuế
    TaxCode VARCHAR(14) NOT NULL,
    TaxpayerName VARCHAR(255) NOT NULL,
    Address VARCHAR(500),
    TaxAuthorityCode VARCHAR(50),
    
    -- Liên hệ & Đại diện (Đã bổ sung các trường từ UI)
    ContactPerson VARCHAR(100),
    ContactPhone VARCHAR(20),
    ContactEmail VARCHAR(100) NOT NULL,
    RepresentativeName VARCHAR(100) NOT NULL,
    IdentityType INT, -- 1: CCCD, 2: Hộ chiếu
    IdentityNo VARCHAR(20),
    Gender INT,                 -- 1: Nam, 2: Nữ (Mới bổ sung)
    BirthDate DATE,             -- Mới bổ sung
    Nationality VARCHAR(100),   -- Mới bổ sung
    ContactAddress VARCHAR(500),-- Mới bổ sung (Địa chỉ liên hệ của NNT)
    
    -- Hình thức đăng ký
    InvoiceAppType INT NOT NULL, -- 1: Có mã, 2: Không mã, 3: Máy tính tiền
    SubjectCategory INT,         -- Dành cho Radio button đối tượng gửi dữ liệu (Mới bổ sung)
    DataTransferMode INT NOT NULL, -- 1: Trực tiếp, 2: Qua TVAN
    MethodOfTransfer INT, -- 1: Từng hóa đơn, 2: Bảng tổng hợp
    
    -- Thông tin ký tờ khai (Mới bổ sung)
    IssuePlace VARCHAR(100), -- Nơi lập (VD: Hà Nội)
    
    -- Trạng thái tờ khai
    Status INT NOT NULL DEFAULT 1, -- 1: Tạm lưu, 2: Đã ký, 3: Chờ CQT, 4: Chấp nhận, 5: Từ chối
    
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50),
    
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId),
    INDEX idx_registration_company (CompanyId)
);

-- Bảng lưu trữ Danh sách chứng thư số
CREATE TABLE RegDigitalSignature (
    SignatureId CHAR(36) PRIMARY KEY,
    RegistrationId CHAR(36) NOT NULL,
    OrganizationName VARCHAR(255) NOT NULL,
    SerialNumber VARCHAR(100) NOT NULL,
    FromDate DATETIME NOT NULL,
    ToDate DATETIME NOT NULL,
    ActionType INT NOT NULL, -- 1: Thêm mới, 2: Gia hạn, 3: Ngừng
    
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50),
    
    FOREIGN KEY (RegistrationId) REFERENCES InvoiceRegistration(RegistrationId) ON DELETE CASCADE
);

-- Bảng lưu trữ Loại hóa đơn sử dụng
CREATE TABLE RegInvoiceType (
    RegInvoiceTypeId CHAR(36) PRIMARY KEY,
    RegistrationId CHAR(36) NOT NULL,
    InvoiceTypeCode VARCHAR(10) NOT NULL, -- 1: GTGT, 2: Bán hàng, ...
    IsSelected BOOLEAN DEFAULT FALSE,
    
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50),
    
    FOREIGN KEY (RegistrationId) REFERENCES InvoiceRegistration(RegistrationId) ON DELETE CASCADE
);

-- Bảng lưu trữ Tổ chức cung cấp dịch vụ HĐĐT (TVAN)
CREATE TABLE RegTVAN (
    RegTVANId CHAR(36) PRIMARY KEY,
    RegistrationId CHAR(36) NOT NULL,
    TVANName VARCHAR(255) NOT NULL,
    TVANTaxCode VARCHAR(14) NOT NULL,
    FromDate DATETIME NOT NULL,
    ToDate DATETIME,
    
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50),
    
    FOREIGN KEY (RegistrationId) REFERENCES InvoiceRegistration(RegistrationId) ON DELETE CASCADE
);

-- Bảng: Thông tin đơn vị truyền nhận hóa đơn điện tử (Mới bổ sung theo UI)
CREATE TABLE RegTransmissionUnit (
    TransmissionUnitId CHAR(36) PRIMARY KEY,
    RegistrationId CHAR(36) NOT NULL,
    UnitName VARCHAR(255) NOT NULL,
    TaxCode VARCHAR(14) NOT NULL,
    FromDate DATETIME,
    ToDate DATETIME,
    Note VARCHAR(500),
    FOREIGN KEY (RegistrationId) REFERENCES InvoiceRegistration(RegistrationId) ON DELETE CASCADE
);

-- Bảng: Thông tin đơn vị hạch toán phụ thuộc cần cấp quyền tra cứu (Mới bổ sung theo UI)
CREATE TABLE RegDependentUnit (
    DependentUnitId CHAR(36) PRIMARY KEY,
    RegistrationId CHAR(36) NOT NULL,
    UnitName VARCHAR(255) NOT NULL,
    TaxCode VARCHAR(14) NOT NULL,
    FromDate DATETIME,
    ToDate DATETIME,
    FOREIGN KEY (RegistrationId) REFERENCES InvoiceRegistration(RegistrationId) ON DELETE CASCADE
);

-- Bảng Đề nghị tạm ngừng sử dụng hóa đơn điện tử
CREATE TABLE RegSuspension (
    SuspensionId CHAR(36) PRIMARY KEY,
    RegistrationId CHAR(36) NOT NULL,
    FromDate DATETIME NOT NULL,
    ToDate DATETIME NOT NULL,
    Reason VARCHAR(500),
    
    CreatedDate DATETIME NOT NULL,
    CreatedBy VARCHAR(50),
    ModifiedDate DATETIME,
    ModifiedBy VARCHAR(50),
    
    FOREIGN KEY (RegistrationId) REFERENCES InvoiceRegistration(RegistrationId) ON DELETE CASCADE
);