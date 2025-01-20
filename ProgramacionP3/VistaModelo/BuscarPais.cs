using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;
using ProgramacionP3.Modelo;
using Newtonsoft.Json;
using SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            _db = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "jolalla_db.sqlite"));
            _db.CreateTable<PaisDB>();
            PaisesConsultados = new ObservableCollection<PaisDB>(_db.Table<PaisDB>().ToList());
            BuscarCommand = new Command(async () => await OnBuscarClicked());
            LimpiarCommand = new Command(OnLimpiarClicked);
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async Task OnBuscarClicked()
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
                    ResultadoBusqueda = $"Nombre: {pais.Name.Common}, Región: {pais.Region}, Link: {pais.Maps.GoogleMaps}";

                    var nuevoPais = new PaisDB
                    {
                        Nombre = pais.Name.Common,
                        Region = pais.Region,
                        LinkGoogleMaps = pais.Maps.GoogleMaps,
                        NombreBD = "JOlalla"
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
