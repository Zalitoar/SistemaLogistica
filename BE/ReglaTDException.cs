using System;
namespace BE
{
    // La UI traduce la clave; las capas inferiores no dependen del idioma activo.
    public class ReglaTDException : Exception
    {
        public ReglaTDException(string clave) : base(clave) { }
    }
}
