using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace BLL
{
    public class GestorFlota
    {
        public const string Permiso = "MF_REGISTRAR_ESTADO";
        private readonly MP_MF datos = new MP_MF();
        public List<UnidadFlota> Unidades()
        {
            ReglasMF.Permiso(Permiso);
            return datos.Unidades();
        }

        public List<ParteEstadoUnidad> Partes(int unidad)
        {
            ReglasMF.Permiso(Permiso);
            return datos.Partes(unidad);
        }

        public int Crear(UnidadFlota u)
        {
            ReglasMF.Permiso(Permiso);
            ReglasMF.Exigir(u != null && ReglasMF.Texto(u.Dominio, 10) && ReglasMF.Texto(u.Tipo, 50) && ReglasMF.Texto(u.Marca, 50) && ReglasMF.Texto(u.Modelo, 50) && u.Anio >= 1900 && u.Anio <= 2100 && ReglasMF.Km(u.KilometrajeActual) && ReglasMF.Km(u.Capacidad) && u.Capacidad > 0);
            u.Dominio = u.Dominio.Trim().ToUpperInvariant();
            int id = datos.CrearUnidad(u);
            ReglasMF.Auditar("alta unidad", id);
            return id;
        }

        public int Registrar(ParteEstadoUnidad p)
        {
            ReglasMF.Permiso(Permiso);
            ReglasMF.Exigir(p != null && p.IdUnidadFlota > 0 && ReglasMF.Km(p.Kilometraje) && ReglasMF.Texto(p.TipoNovedad, 30) && ReglasMF.Texto(p.Descripcion, 500));
            int id = datos.RegistrarEstado(p);
            ReglasMF.Auditar("parte estado", id);
            return id;
        }
    }
}
