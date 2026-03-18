# Student Management Software - Hệ thống Quản lý Sinh viên

Ứng dụng desktop hỗ trợ quản lý thông tin sinh viên, được xây dựng bằng **C#**, **WPF** và áp dụng mô hình kiến trúc **MVVM**.  
Phần mềm cung cấp giao diện trực quan, dễ sử dụng, hỗ trợ các thao tác quản lý dữ liệu như thêm, sửa, xóa, tìm kiếm, lọc, sắp xếp, lưu tệp và xuất dữ liệu.

## Giới thiệu

**Hệ thống Quản lý Sinh viên** là một dự án desktop nhẹ, phục vụ cho việc quản lý danh sách sinh viên một cách hiệu quả.  
Dự án phù hợp cho mục đích học tập, thực hành mô hình **MVVM**, cũng như làm mẫu tham khảo cho các ứng dụng quản lý dữ liệu bằng **WPF**.

## Tính năng chính

- Thêm sinh viên mới
- Chỉnh sửa thông tin sinh viên
- Xóa sinh viên khỏi danh sách
- Tìm kiếm sinh viên theo từ khóa
- Lọc sinh viên theo giới tính và thành phố
- Sắp xếp danh sách theo tiêu chí lựa chọn
- Hoàn tác / làm lại thao tác
- Lưu dữ liệu vào tệp **JSON**
- Đọc dữ liệu từ tệp **JSON**
- Xuất danh sách sinh viên ra tệp **CSV**
- Hiển thị thống kê tổng số sinh viên, số lượng nam và nữ

## Công nghệ sử dụng

- **Ngôn ngữ:** C#
- **Nền tảng:** .NET 8
- **Giao diện:** WPF
- **Kiến trúc:** MVVM
- **Định dạng dữ liệu:** JSON, CSV

## Cấu trúc dự án

```bash
Student-Management-Software/
├── App/
├── Models/
│   └── StudentModels.cs
├── ViewModels/
│   ├── BaseViewModel.cs
│   ├── MainViewModel.cs
│   ├── RelayCommand.cs
│   └── InverseBooleanConverter.cs
├── Views/
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
├── App.xaml
├── App.xaml.cs
├── Student Management System.csproj
└── Student Management System.sln
```

## Mô hình dữ liệu

Mỗi sinh viên bao gồm các thông tin cơ bản sau:

- Họ và tên
- Tuổi
- Giới tính
- Thành phố

## Yêu cầu hệ thống

Trước khi chạy dự án, hãy đảm bảo máy tính của bạn có:

- Hệ điều hành Windows
- .NET 8 SDK trở lên
- Visual Studio 2022 với workload phát triển ứng dụng desktop bằng WPF

## Cài đặt và chạy dự án

### 1. Clone repository

```bash
git clone https://github.com/gibor06/Student-Management-Software.git
```

### 2. Mở project bằng Visual Studio

Mở file:

```bash
Student Management System.sln
```

### 3. Build và chạy ứng dụng

- Khôi phục các package cần thiết

- Build solution

- Nhấn Run để khởi động chương trình

---

## Cách sử dụng

Sau khi chạy ứng dụng, bạn có thể:

- Nhập thông tin sinh viên vào biểu mẫu

- Thêm sinh viên vào danh sách

- Chọn sinh viên để chỉnh sửa hoặc xóa

- Sử dụng bộ lọc và sắp xếp để tìm dữ liệu nhanh hơn

- Lưu danh sách sinh viên thành tệp JSON

- Mở lại dữ liệu từ tệp JSON đã lưu

- Xuất dữ liệu hiện tại sang định dạng CSV

- Hoàn tác hoặc làm lại các thao tác gần nhất

---

# 👨‍💻 Tác giả

**Trần Gia Bảo** — **gibor06**

Sinh viên ngành **Công Nghệ Thông Tin** - **Trường Đại học Công Thương TP.HCM (HUIT)**

📍 TP.HCM, Việt Nam

---

# 📬 Liên hệ

📧 Email [gibor06.dev@gmail.com](mailto:gibor06.dev@gmail.com)

🌐 Facebook https://www.facebook.com/gibor06

---

# 📜 License

Repository được phát triển phục vụ **mục đích học tập và ứng dụng**.
