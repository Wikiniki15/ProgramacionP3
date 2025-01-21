using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramacionP3.Modelo
{
    public class PaisDB
    { 
        [PrimaryKey, AutoIncrement] 
        public int Id { get; set; } 
        public string Nombre { get; set; } 
        public string Region { get; set; } 
        public string LinkGoogleMaps { get; set; } 
        public string NombreBD { get; set; } 
        public string Display => $"Nombre del País: {Nombre}, Región: {Region}, Link: {LinkGoogleMaps}, NombreBD: {NombreBD}"; 
    }
}
