using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramacionP3.Modelo
{
    public class Pais
    {
        public string Nombre { get; set; }
        public string Region { get; set; }
        public string LinkGoogleMaps { get; set; }
    }

    public class PaisDB : Pais
    {
        public string NombreBD { get; set; }
    }

}
