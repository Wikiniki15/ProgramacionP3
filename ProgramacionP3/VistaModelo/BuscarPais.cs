using SQLite;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ProgramacionP3.Modelo;

namespace ProgramacionP3.VistaModelo
{
    public class BuscarPais : INotifyPropertyChanged
    {
        private string _nombrePais;
        private string _resultadoBusqueda;
        private SQLiteConnection _db;

        public string NombrePais
        {
            get => _nombrePais;
            set
            {
                _nombrePais = value;
                OnPropertyChanged(nameof(NombrePais));
            }
        }

        public string ResultadoBusqueda
        {
            get => _resultadoBusqueda;
            set
            {
                _resultadoBusqueda = value;
                OnPropertyChanged(nameof(ResultadoBusqueda));
            }
        }

        public ObservableCollection<PaisDB> PaisesConsultados { get; set; }
        public ICommand BuscarCommand { get; }
        public ICommand LimpiarCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public BuscarPais()
        {
            _db = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "scordova_db.sqlite"));
            _db.CreateTable<PaisDB>();
            PaisesConsultados = new ObservableCollection<PaisDB>(_db.Table<PaisDB>().ToList());
            BuscarCommand = new Command(OnBuscarClicked);
            LimpiarCommand = new Command(OnLimpiarClicked);
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void OnBuscarClicked()
        {
            if (string.IsNullOrEmpty(NombrePais))
            {
                ResultadoBusqueda = "Por favor ingrese el nombre de un país.";
                return;
            }
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync($"https://restcountries.com/v3.1/name/{NombrePais}?fields=name,region,maps");
                var pais = JsonConvert.DeserializeObject<List<Pais>>(response).FirstOrDefault();
                if (pais != null)
                {
                    ResultadoBusqueda = $"Nombre: {pais.Nombre}, Región: {pais.Region}, Link: {pais.LinkGoogleMaps}";

                    var nuevoPais = new PaisDB
                    {
                        Nombre = pais.Nombre,
                        Region = pais.Region,
                        LinkGoogleMaps = pais.LinkGoogleMaps,
                        NombreBD = "SCordova"
                    };
                    _db.Insert(nuevoPais);
                    PaisesConsultados.Add(nuevoPais);
                }
                else
                {
                    ResultadoBusqueda = "País no encontrado.";
                }
            }
            catch (Exception ex)
            {
                ResultadoBusqueda = $"Error: {ex.Message}";
            }
        }

        private void OnLimpiarClicked()
        {
            NombrePais = string.Empty;
            ResultadoBusqueda = string.Empty;
        }
    }

}
