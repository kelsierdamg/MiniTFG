using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MiniTFG
{
    public class Skin : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public string Id { get; set; }
        public string Image { get; set; }
        public int Precio { get; set; }
        private bool _comprado;
        public bool Comprado
        {
            get => _comprado;
            set { if (_comprado != value) { _comprado = value; OnPropertyChanged(); } }
        }
    }
}
