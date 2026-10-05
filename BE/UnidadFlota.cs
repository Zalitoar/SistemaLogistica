using System;

namespace BE
{
    public class UnidadFlota
    {
        public int IdUnidadFlota { get; set; }
        public string Dominio { get; set; }
        public string Tipo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Anio { get; set; }
        public decimal Capacidad { get; set; }
        public decimal KilometrajeActual { get; set; }
        public string EstadoOperativo { get; set; }
        public bool Activa { get; set; }
    }
}
