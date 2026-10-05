using System;

namespace BE
{
    public class ReglaMFException : Exception
    {
        public ReglaMFException(string clave) : base(clave)
        {
        }
    }
}
