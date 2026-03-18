using System.ComponentModel;

namespace Student_Management_System.Models
{
    public class StudentModel : INotifyPropertyChanged
    {
        private string _hoTen = string.Empty;
        private int _tuoi;
        private bool _gioiTinh = true; // true = Nam, false = Nữ
        private string _thanhPho = string.Empty;

        public string HoTen
        {
            get => _hoTen;
            set
            {
                _hoTen = value;
                OnPropertyChanged(nameof(HoTen));
            }
        }

        public int Tuoi
        {
            get => _tuoi;
            set
            {
                _tuoi = value;
                OnPropertyChanged(nameof(Tuoi));
            }
        }

        public bool GioiTinh
        {
            get => _gioiTinh;
            set
            {
                _gioiTinh = value;
                OnPropertyChanged(nameof(GioiTinh));
                OnPropertyChanged(nameof(GioiTinhText));
            }
        }

        public string ThanhPho
        {
            get => _thanhPho;
            set
            {
                _thanhPho = value;
                OnPropertyChanged(nameof(ThanhPho));
            }
        }

        public string GioiTinhText => GioiTinh ? "Nam" : "Nữ";

        public StudentModel Clone()
        {
            return new StudentModel
            {
                HoTen = HoTen,
                Tuoi = Tuoi,
                GioiTinh = GioiTinh,
                ThanhPho = ThanhPho
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
