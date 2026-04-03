# MyInvoice - Hệ thống Quản lý và Phát hành Hóa đơn Điện tử

Dự án mô phỏng quy trình nghiệp vụ hóa đơn điện tử theo thông tư 123/2020/NĐ-CP, bao gồm từ bước đăng ký tờ khai sử dụng đến ký số và cấp mã từ Cơ quan Thuế.

## 🚀 Công nghệ sử dụng

### Backend

- **Framework:** .NET Core 8.0 (Web API)
- **Kiến trúc:** Phân lớp (Controller - Service - Repository) với Base Generic xử lý CRUD tự động.
- **Real-time:** SignalR (NotificationHub) để nhận phản hồi từ CQT.
- **ORM:** Entity Framework Core (tương tác MySQL).

### Frontend

- **Framework:** Vue.js 3 (Composition API - `<script setup>`)
- **Build Tool:** Vite
- **State Management:** Pinia
- **Giao tiếp API:** Axios (axiosClient)
- **Ký số:** Kết nối WebSocket tới ứng dụng MISA KYSO chạy dưới máy trạm.
- **UI Components:** Lucide Icons, Custom Components (MSTable, MSPagination, MSToast...).

### Cơ sở dữ liệu

- **Database:** MySQL 8.0

## 📁 Cấu trúc thư mục chính

- `my-invoice-fe/`: Mã nguồn Front-end (Vue.js)
- `MISA.WorkShift/`: Mã nguồn Back-end (.NET Core API)
- `Database/`: Chứa file script `.sql` khởi tạo cơ sở dữ liệu.

## 🛠 Cài đặt và Chạy ứng dụng

### 0. Cài đặt công cụ

1. Tải MISA_kyso và chứng thư số ở https://drive.google.com/drive/u/4/folders/1ePLEjA8v_EDU0yLzNvFHlwyLNqfFiOR6
2. Cài đặt MISA_kyso
3. Mở file chứng thư số -> import curent user -> Nhập mật khẩu: badssl.com

### 1. Cơ sở dữ liệu

1. Mở công cụ quản lý MySQL (MySQL Workbench, Navicat, hoặc phpMyAdmin).
2. Tạo một database mới tên là `my_invoice_development`.
3. Import file `my_invoice_development.sql` vào database vừa tạo.

### 2. Back-end

1. Di chuyển vào thư mục dự án Back-end: `MISA.WorkShift/MISA.WorkShift` Chọn mở `MISA.WorkShift.sln` để mở Solution của dự án bằng VS2022
2. Mở file `appsettings.json`, chỉnh sửa chuỗi kết nối `ConnectionStrings` phù hợp với tài khoản MySQL của bạn.
3. Chạy lệnh:
   ```bash
   dotnet restore
   dotnet run
   ```
   Hoặc ấn Nút |> https để chạy dự án
4. API sẽ chạy mặc định tại: `https://localhost:7248`

### 3. Front-end

1. Di chuyển vào thư mục Front-end: `my-invoice-fe`.
2. Cài đặt các thư viện cần thiết:
   ```bash
   npm install
   ```
3. Chạy ứng dụng ở chế độ phát triển:
   ```bash
   npm run dev
   ```
4. Truy cập ứng dụng tại: `http://localhost:5173`.

### 4. Công cụ ký số (MISA KYSO)

- Để sử dụng tính năng "Lưu và phát hành", bạn cần cài đặt và khởi động ứng dụng **MISA KYSO** trên máy tính.
- Đảm bảo Tool đang lắng nghe tại cổng WebSocket: `ws://localhost:11984/plugin`.

## 📝 Quy trình kiểm thử (Demo)

1.  **Đăng nhập:** Bắt buộc phải đăng ký bằng Postman
    (Vì tương lai phải mua tài nguyên thì mới được đăng ký tài khoản mới)
    `https://localhost:7248/api/v1/Auth/register`

         {
         "companyName": "Công ty Cổ phần NET Software",
         "taxCode": "6868686868",
         "fullName": "Khúc Xuân Triển",
         "email": "khucxuantrien2004@gmail.com",
         "password": "12345678"
         }

2.  **Lập tờ khai:** Vào mục "Đăng ký phát hành" -> "Tờ khai", nhấn "Thêm mới", nhập thông tin và nhấn "Lưu và gửi".
3.  **Khởi tạo mẫu:** Vào mục "Đăng ký phát hành" -> "Mẫu hoá đơn", nhấn "Thêm mới", nhập thông tin và nhấn "Lưu và gửi".
4.  **Lập hóa đơn:** Vào mục "Hóa đơn", chọn "Thêm", nhập hàng hóa.
5.  **Phát hành:** Nhấn "Lưu và phát hành", hệ thống sẽ gọi Tool ký số. Sau khi ký xong, hóa đơn sẽ ở trạng thái "Chờ CQT cấp mã".
6.  **Cấp mã:** Vào trang "Cổng Thuế" tại 'http://localhost:5173/tax-portal', mục "Hóa đơn chờ cấp mã", nhấn "Cấp mã CQT". Hóa đơn sẽ được cập nhật mã số chính thức và trạng thái "Đã phát hành".
7.  **Xem hóa đơn:** Di chuột vào 1 hoá đơn ở trang Danh sách hoá đơn, tiếp theo chọn "..." chọn xem hoá đơn được tạo ra từ XML và XSLT -> Từ đó có thể tải xuống Hoá đơn điện tử và In hoá đơn điện tử.
