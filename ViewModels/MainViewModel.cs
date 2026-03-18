using Microsoft.Win32;
using Student_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Student_Management_System.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private const string TatCa = "Tất cả";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        private readonly Stack<List<StudentModel>> _undoStack = new();
        private readonly Stack<List<StudentModel>> _redoStack = new();

        private StudentModel? _selectedStudent;
        private string _inputHoTen = string.Empty;
        private string _inputTuoi = string.Empty;
        private bool _inputGioiTinh = true;
        private string? _inputThanhPho;

        private string _filterText = string.Empty;
        private string _filterThanhPho = TatCa;
        private string _filterGioiTinh = TatCa;
        private string _sortOption = "Mặc định";

        private string _thongBaoTrangThai = "Sẵn sàng thao tác.";

        public ObservableCollection<StudentModel> Students { get; }
        public ICollectionView StudentsView { get; }

        public ObservableCollection<string> ThanhPhos { get; }
        public ObservableCollection<string> ThanhPhoBoLoc { get; }
        public ObservableCollection<string> GioiTinhBoLoc { get; }
        public ObservableCollection<string> SapXepTuyChon { get; }

        public StudentModel? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));

                if (_selectedStudent != null)
                {
                    InputHoTen = _selectedStudent.HoTen;
                    InputTuoi = _selectedStudent.Tuoi.ToString();
                    InputGioiTinh = _selectedStudent.GioiTinh;
                    InputThanhPho = _selectedStudent.ThanhPho;
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string InputHoTen
        {
            get => _inputHoTen;
            set
            {
                _inputHoTen = value;
                OnPropertyChanged(nameof(InputHoTen));
            }
        }

        public string InputTuoi
        {
            get => _inputTuoi;
            set
            {
                _inputTuoi = value;
                OnPropertyChanged(nameof(InputTuoi));
            }
        }

        public bool InputGioiTinh
        {
            get => _inputGioiTinh;
            set
            {
                _inputGioiTinh = value;
                OnPropertyChanged(nameof(InputGioiTinh));
            }
        }

        public string? InputThanhPho
        {
            get => _inputThanhPho;
            set
            {
                _inputThanhPho = value;
                OnPropertyChanged(nameof(InputThanhPho));
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged(nameof(FilterText));
                StudentsView.Refresh();
            }
        }

        public string FilterThanhPho
        {
            get => _filterThanhPho;
            set
            {
                _filterThanhPho = value;
                OnPropertyChanged(nameof(FilterThanhPho));
                StudentsView.Refresh();
            }
        }

        public string FilterGioiTinh
        {
            get => _filterGioiTinh;
            set
            {
                _filterGioiTinh = value;
                OnPropertyChanged(nameof(FilterGioiTinh));
                StudentsView.Refresh();
            }
        }

        public string SortOption
        {
            get => _sortOption;
            set
            {
                _sortOption = value;
                OnPropertyChanged(nameof(SortOption));
                ApplySorting();
            }
        }

        public string ThongBaoTrangThai
        {
            get => _thongBaoTrangThai;
            set
            {
                _thongBaoTrangThai = value;
                OnPropertyChanged(nameof(ThongBaoTrangThai));
            }
        }

        public int TongSoSinhVien => Students.Count;
        public int SoNam => Students.Count(x => x.GioiTinh);
        public int SoNu => Students.Count(x => !x.GioiTinh);

        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand UndoCommand { get; }
        public RelayCommand RedoCommand { get; }
        public RelayCommand SaveJsonCommand { get; }
        public RelayCommand LoadJsonCommand { get; }
        public RelayCommand ExportCsvCommand { get; }
        public RelayCommand ClearInputCommand { get; }

        public MainViewModel()
        {
            Students = new ObservableCollection<StudentModel>
            {
                new StudentModel { HoTen = "Nguyễn Minh An", Tuoi = 20, GioiTinh = true, ThanhPho = "Hà Nội" },
                new StudentModel { HoTen = "Trần Thị Bình", Tuoi = 19, GioiTinh = false, ThanhPho = "Đà Nẵng" },
                new StudentModel { HoTen = "Lê Quốc Dũng", Tuoi = 21, GioiTinh = true, ThanhPho = "TP. Hồ Chí Minh" }
            };

            ThanhPhos = new ObservableCollection<string>
            {
                "Hà Nội", "TP. Hồ Chí Minh", "Đà Nẵng", "Cần Thơ", "Hải Phòng", "Huế"
            };

            ThanhPhoBoLoc = new ObservableCollection<string>();
            GioiTinhBoLoc = new ObservableCollection<string> { TatCa, "Nam", "Nữ" };
            SapXepTuyChon = new ObservableCollection<string>
            {
                "Mặc định", "Tên A-Z", "Tên Z-A", "Tuổi tăng dần", "Tuổi giảm dần"
            };

            _inputThanhPho = ThanhPhos.FirstOrDefault();

            StudentsView = CollectionViewSource.GetDefaultView(Students);
            StudentsView.Filter = FilterStudents;

            BuildThanhPhoBoLoc();
            ApplySorting();

            AddCommand = new RelayCommand(_ => AddStudent());
            EditCommand = new RelayCommand(_ => EditStudent(), _ => SelectedStudent != null);
            DeleteCommand = new RelayCommand(_ => DeleteStudent(), _ => SelectedStudent != null);
            UndoCommand = new RelayCommand(_ => Undo(), _ => _undoStack.Count > 0);
            RedoCommand = new RelayCommand(_ => Redo(), _ => _redoStack.Count > 0);
            SaveJsonCommand = new RelayCommand(_ => SaveJson());
            LoadJsonCommand = new RelayCommand(_ => LoadJson());
            ExportCsvCommand = new RelayCommand(_ => ExportCsv(), _ => Students.Count > 0);
            ClearInputCommand = new RelayCommand(_ => ClearInput());

            UpdateThongKe();
            SetStatus("Hệ thống đã sẵn sàng.");
        }

        private bool FilterStudents(object obj)
        {
            if (obj is not StudentModel sv)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(FilterThanhPho) &&
                !FilterThanhPho.Equals(TatCa, StringComparison.OrdinalIgnoreCase) &&
                !sv.ThanhPho.Equals(FilterThanhPho, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (FilterGioiTinh == "Nam" && !sv.GioiTinh)
            {
                return false;
            }

            if (FilterGioiTinh == "Nữ" && sv.GioiTinh)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(FilterText))
            {
                return true;
            }

            string searchKeyword = NormalizeSearchText(FilterText);
            string hoTen = NormalizeSearchText(sv.HoTen);

            return hoTen.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase);
        }

        private void ApplySorting()
        {
            using (StudentsView.DeferRefresh())
            {
                StudentsView.SortDescriptions.Clear();

                switch (SortOption)
                {
                    case "Tên A-Z":
                        StudentsView.SortDescriptions.Add(new SortDescription(nameof(StudentModel.HoTen), ListSortDirection.Ascending));
                        break;

                    case "Tên Z-A":
                        StudentsView.SortDescriptions.Add(new SortDescription(nameof(StudentModel.HoTen), ListSortDirection.Descending));
                        break;

                    case "Tuổi tăng dần":
                        StudentsView.SortDescriptions.Add(new SortDescription(nameof(StudentModel.Tuoi), ListSortDirection.Ascending));
                        break;

                    case "Tuổi giảm dần":
                        StudentsView.SortDescriptions.Add(new SortDescription(nameof(StudentModel.Tuoi), ListSortDirection.Descending));
                        break;
                }
            }
        }

        private static string NormalizeSearchText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            string normalized = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new(normalized.Length);

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'đ')
                    {
                        sb.Append('d');
                    }
                    else if (c == 'Đ')
                    {
                        sb.Append('D');
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).Trim();
        }

        private bool ValidateInput(out int tuoi, out string hoTen)
        {
            tuoi = 0;
            hoTen = InputHoTen.Trim();

            if (string.IsNullOrWhiteSpace(hoTen) ||
                string.IsNullOrWhiteSpace(InputTuoi) ||
                string.IsNullOrWhiteSpace(InputThanhPho))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ họ tên, tuổi và thành phố.",
                    "Thiếu thông tin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(InputTuoi, out tuoi))
            {
                MessageBox.Show(
                    "Tuổi phải là số nguyên hợp lệ.",
                    "Sai định dạng",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (tuoi < 16 || tuoi > 120)
            {
                MessageBox.Show(
                    "Tuổi cần nằm trong khoảng 16 đến 120.",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void SaveStateForUndo()
        {
            _undoStack.Push(Students.Select(x => x.Clone()).ToList());
            _redoStack.Clear();
            CommandManager.InvalidateRequerySuggested();
        }

        private void RestoreState(List<StudentModel> state)
        {
            Students.Clear();
            foreach (StudentModel item in state)
            {
                Students.Add(item.Clone());
            }

            SelectedStudent = null;
            BuildThanhPhoBoLoc();
            StudentsView.Refresh();
            UpdateThongKe();
        }

        private void BuildThanhPhoBoLoc()
        {
            foreach (string thanhPho in Students.Select(x => x.ThanhPho).Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                if (!ThanhPhos.Contains(thanhPho))
                {
                    ThanhPhos.Add(thanhPho);
                }
            }

            string selectedBefore = FilterThanhPho;
            ThanhPhoBoLoc.Clear();
            ThanhPhoBoLoc.Add(TatCa);

            foreach (string item in ThanhPhos
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .OrderBy(NormalizeSearchText))
            {
                ThanhPhoBoLoc.Add(item);
            }

            if (ThanhPhoBoLoc.Contains(selectedBefore))
            {
                FilterThanhPho = selectedBefore;
            }
            else
            {
                FilterThanhPho = TatCa;
            }

            if (string.IsNullOrWhiteSpace(InputThanhPho) || !ThanhPhos.Contains(InputThanhPho))
            {
                InputThanhPho = ThanhPhos.FirstOrDefault();
            }
        }

        private static string EscapeCsvValue(string value)
        {
            string safeValue = value.Replace("\"", "\"\"");
            return $"\"{safeValue}\"";
        }

        private void SetStatus(string message)
        {
            ThongBaoTrangThai = $"[{DateTime.Now:HH:mm:ss}] {message}";
        }

        public void AddStudent()
        {
            if (!ValidateInput(out int tuoi, out string hoTen))
            {
                return;
            }

            SaveStateForUndo();

            Students.Add(new StudentModel
            {
                HoTen = hoTen,
                Tuoi = tuoi,
                GioiTinh = InputGioiTinh,
                ThanhPho = InputThanhPho ?? string.Empty
            });

            BuildThanhPhoBoLoc();
            ClearInput();
            UpdateThongKe();
            SetStatus($"Đã thêm sinh viên: {hoTen}.");
        }

        public void EditStudent()
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn sinh viên cần sửa.",
                    "Chưa chọn dữ liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            if (!ValidateInput(out int tuoi, out string hoTen))
            {
                return;
            }

            SaveStateForUndo();

            SelectedStudent.HoTen = hoTen;
            SelectedStudent.Tuoi = tuoi;
            SelectedStudent.GioiTinh = InputGioiTinh;
            SelectedStudent.ThanhPho = InputThanhPho ?? string.Empty;

            BuildThanhPhoBoLoc();
            StudentsView.Refresh();
            UpdateThongKe();
            SetStatus($"Đã cập nhật sinh viên: {hoTen}.");
        }

        public void DeleteStudent()
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn sinh viên cần xóa.",
                    "Chưa chọn dữ liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa sinh viên đang chọn không?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            string tenSinhVien = SelectedStudent.HoTen;

            SaveStateForUndo();
            Students.Remove(SelectedStudent);

            BuildThanhPhoBoLoc();
            ClearInput();
            UpdateThongKe();
            SetStatus($"Đã xóa sinh viên: {tenSinhVien}.");
        }

        public void Undo()
        {
            if (_undoStack.Count == 0)
            {
                return;
            }

            _redoStack.Push(Students.Select(x => x.Clone()).ToList());
            List<StudentModel> previousState = _undoStack.Pop();
            RestoreState(previousState);
            SetStatus("Đã hoàn tác thao tác gần nhất.");
            CommandManager.InvalidateRequerySuggested();
        }

        public void Redo()
        {
            if (_redoStack.Count == 0)
            {
                return;
            }

            _undoStack.Push(Students.Select(x => x.Clone()).ToList());
            List<StudentModel> nextState = _redoStack.Pop();
            RestoreState(nextState);
            SetStatus("Đã thực hiện lại thao tác gần nhất.");
            CommandManager.InvalidateRequerySuggested();
        }

        public void SaveJson()
        {
            try
            {
                SaveFileDialog dlg = new()
                {
                    Filter = "JSON file (*.json)|*.json",
                    FileName = "students.json"
                };

                if (dlg.ShowDialog() != true)
                {
                    return;
                }

                string json = JsonSerializer.Serialize(Students, JsonOptions);
                File.WriteAllText(dlg.FileName, json, Encoding.UTF8);

                SetStatus($"Đã lưu JSON: {Path.GetFileName(dlg.FileName)}.");
                MessageBox.Show(
                    "Lưu file JSON thành công.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                SetStatus("Lưu JSON thất bại.");
                MessageBox.Show(
                    "Lỗi khi lưu file: " + ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void LoadJson()
        {
            try
            {
                OpenFileDialog dlg = new()
                {
                    Filter = "JSON file (*.json)|*.json"
                };

                if (dlg.ShowDialog() != true)
                {
                    return;
                }

                string json = File.ReadAllText(dlg.FileName, Encoding.UTF8);
                List<StudentModel>? list = JsonSerializer.Deserialize<List<StudentModel>>(json, JsonOptions);

                if (list == null)
                {
                    MessageBox.Show(
                        "File không có dữ liệu hợp lệ.",
                        "Lỗi dữ liệu",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                SaveStateForUndo();
                Students.Clear();

                foreach (StudentModel item in list)
                {
                    Students.Add(new StudentModel
                    {
                        HoTen = item.HoTen?.Trim() ?? string.Empty,
                        Tuoi = item.Tuoi,
                        GioiTinh = item.GioiTinh,
                        ThanhPho = item.ThanhPho ?? string.Empty
                    });
                }

                BuildThanhPhoBoLoc();
                ClearInput();
                StudentsView.Refresh();
                UpdateThongKe();

                SetStatus($"Đã tải JSON: {Path.GetFileName(dlg.FileName)}.");
                MessageBox.Show(
                    "Tải file JSON thành công.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                SetStatus("Tải JSON thất bại.");
                MessageBox.Show(
                    "Lỗi khi tải file: " + ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void ExportCsv()
        {
            try
            {
                SaveFileDialog dlg = new()
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    FileName = "students.csv"
                };

                if (dlg.ShowDialog() != true)
                {
                    return;
                }

                StringBuilder builder = new();
                builder.AppendLine("Họ và tên,Tuổi,Giới tính,Thành phố");

                foreach (StudentModel item in Students)
                {
                    builder.AppendLine(
                        string.Join(",",
                            EscapeCsvValue(item.HoTen),
                            item.Tuoi,
                            EscapeCsvValue(item.GioiTinhText),
                            EscapeCsvValue(item.ThanhPho)));
                }

                File.WriteAllText(dlg.FileName, builder.ToString(), Encoding.UTF8);

                SetStatus($"Đã xuất CSV: {Path.GetFileName(dlg.FileName)}.");
                MessageBox.Show(
                    "Xuất CSV thành công.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                SetStatus("Xuất CSV thất bại.");
                MessageBox.Show(
                    "Lỗi khi xuất CSV: " + ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void ClearInput()
        {
            SelectedStudent = null;
            InputHoTen = string.Empty;
            InputTuoi = string.Empty;
            InputGioiTinh = true;
            InputThanhPho = ThanhPhos.FirstOrDefault();
            SetStatus("Đã làm mới vùng nhập liệu.");
        }

        private void UpdateThongKe()
        {
            OnPropertyChanged(nameof(TongSoSinhVien));
            OnPropertyChanged(nameof(SoNam));
            OnPropertyChanged(nameof(SoNu));
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
