using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;

namespace DAL
{
    public class MP_MF
    {
        private static SqlParameter P(string nombre, object valor)
        {
            return new SqlParameter(nombre, valor ?? DBNull.Value);
        }

        private static DataTable Leer(string procedimiento, params SqlParameter[] parametros)
        {
            var acceso = new ACCESO();
            bool abierto = false;
            try
            {
                acceso.Abrir();
                abierto = true;
                return acceso.Leer(procedimiento, parametros.ToList());
            }
            catch (SqlException ex)
            {
                if (ex.Number >= 50101 && ex.Number <= 50107)
                    throw new ReglaMFException(ex.Message);
                if (ex.Number == 2601 || ex.Number == 2627)
                    throw new ReglaMFException("MF.Duplicado");
                if (ex.Number == 547)
                    throw new ReglaMFException("MF.DatosInvalidos");
                throw;
            }
            finally
            {
                if (abierto)
                    acceso.Cerrar();
            }
        }

        public List<UnidadFlota> Unidades()
        {
            return Leer("MF_LISTAR_UNIDADES").AsEnumerable().Select(r => new UnidadFlota { IdUnidadFlota = r.Field<int>("IdUnidadFlota"), Dominio = r.Field<string>("Dominio"), Tipo = r.Field<string>("Tipo"), Marca = r.Field<string>("Marca"), Modelo = r.Field<string>("Modelo"), Anio = r.Field<int>("Anio"), Capacidad = r.Field<decimal>("Capacidad"), KilometrajeActual = r.Field<decimal>("KilometrajeActual"), EstadoOperativo = r.Field<string>("EstadoOperativo"), Activa = r.Field<bool>("Activa") }).ToList();
        }

        public List<ParteEstadoUnidad> Partes(int? idUnidad = null)
        {
            return Leer("MF_LISTAR_PARTES", P("@IdUnidadFlota", idUnidad)).AsEnumerable().Select(r => new ParteEstadoUnidad { IdParteEstadoUnidad = r.Field<int>("IdParteEstadoUnidad"), IdUnidadFlota = r.Field<int>("IdUnidadFlota"), Dominio = r.Field<string>("Dominio"), FechaHora = r.Field<DateTime>("FechaHora"), Kilometraje = r.Field<decimal>("Kilometraje"), TipoNovedad = r.Field<string>("TipoNovedad"), Descripcion = r.Field<string>("Descripcion"), RequiereMantenimiento = r.Field<bool>("RequiereMantenimiento"), EstadoParte = r.Field<string>("EstadoParte") }).ToList();
        }

        public List<OrdenMantenimiento> Ordenes(int? idUnidad = null)
        {
            return Leer("MF_LISTAR_ORDENES", P("@IdUnidadFlota", idUnidad)).AsEnumerable().Select(r => new OrdenMantenimiento { IdOrdenMantenimiento = r.Field<int>("IdOrdenMantenimiento"), IdUnidadFlota = r.Field<int>("IdUnidadFlota"), IdParteEstadoUnidad = r.Field<int>("IdParteEstadoUnidad"), Dominio = r.Field<string>("Dominio"), FechaEmision = r.Field<DateTime>("FechaEmision"), FechaProgramada = r.Field<DateTime>("FechaProgramada"), TipoMantenimiento = r.Field<string>("TipoMantenimiento"), Prioridad = r.Field<string>("Prioridad"), DescripcionTrabajo = r.Field<string>("DescripcionTrabajo"), EstadoOrden = r.Field<string>("EstadoOrden") }).ToList();
        }

        public List<IntervencionMantenimiento> Intervenciones(int idOrden)
        {
            return Leer("MF_LISTAR_INTERVENCIONES", P("@IdOrdenMantenimiento", idOrden)).AsEnumerable().Select(r => new IntervencionMantenimiento { IdIntervencionMantenimiento = r.Field<int>("IdIntervencionMantenimiento"), IdOrdenMantenimiento = r.Field<int>("IdOrdenMantenimiento"), FechaInicio = r.Field<DateTime>("FechaInicio"), FechaFin = r.Field<DateTime?>("FechaFin"), Kilometraje = r.Field<decimal>("Kilometraje"), TrabajoRealizado = r.Field<string>("TrabajoRealizado"), Resultado = r.Field<string>("Resultado"), Observaciones = r.Field<string>("Observaciones") }).ToList();
        }

        public List<InformeDisponibilidad> Informes(int? idUnidad = null)
        {
            return Leer("MF_LISTAR_INFORMES", P("@IdUnidadFlota", idUnidad)).AsEnumerable().Select(r => new InformeDisponibilidad { IdInformeDisponibilidad = r.Field<int>("IdInformeDisponibilidad"), IdUnidadFlota = r.Field<int>("IdUnidadFlota"), IdIntervencionMantenimiento = r.Field<int?>("IdIntervencionMantenimiento"), FechaHora = r.Field<DateTime>("FechaHora"), Disponible = r.Field<bool>("Disponible"), EstadoUnidad = r.Field<string>("EstadoUnidad"), Observaciones = r.Field<string>("Observaciones"), Vigente = r.Field<bool>("Vigente") }).ToList();
        }

        public int CrearUnidad(UnidadFlota u)
        {
            return Convert.ToInt32(Leer("MF_CREAR_UNIDAD", P("@Dominio", u.Dominio), P("@Tipo", u.Tipo), P("@Marca", u.Marca), P("@Modelo", u.Modelo), P("@Anio", u.Anio), P("@Capacidad", u.Capacidad), P("@KilometrajeActual", u.KilometrajeActual)).Rows[0]["Id"]);
        }

        public int RegistrarEstado(ParteEstadoUnidad p)
        {
            return Convert.ToInt32(Leer("MF_REGISTRAR_ESTADO", P("@IdUnidadFlota", p.IdUnidadFlota), P("@Kilometraje", p.Kilometraje), P("@TipoNovedad", p.TipoNovedad), P("@Descripcion", p.Descripcion), P("@RequiereMantenimiento", p.RequiereMantenimiento)).Rows[0]["Id"]);
        }

        public int Programar(OrdenMantenimiento o)
        {
            return Convert.ToInt32(Leer("MF_PROGRAMAR", P("@IdParteEstadoUnidad", o.IdParteEstadoUnidad), P("@FechaProgramada", o.FechaProgramada), P("@TipoMantenimiento", o.TipoMantenimiento), P("@Prioridad", o.Prioridad), P("@DescripcionTrabajo", o.DescripcionTrabajo)).Rows[0]["Id"]);
        }

        public int Iniciar(int orden)
        {
            return Convert.ToInt32(Leer("MF_INICIAR_INTERVENCION", P("@IdOrdenMantenimiento", orden)).Rows[0]["Id"]);
        }

        public int Finalizar(IntervencionMantenimiento i)
        {
            return Convert.ToInt32(Leer("MF_FINALIZAR_INTERVENCION", P("@IdIntervencionMantenimiento", i.IdIntervencionMantenimiento), P("@Kilometraje", i.Kilometraje), P("@TrabajoRealizado", i.TrabajoRealizado), P("@Resultado", i.Resultado), P("@Observaciones", i.Observaciones)).Rows[0]["Id"]);
        }

        public int Habilitar(int unidad, string observaciones)
        {
            return Convert.ToInt32(Leer("MF_HABILITAR", P("@IdUnidadFlota", unidad), P("@Observaciones", observaciones)).Rows[0]["Id"]);
        }

        public int Asignar(int unidad, int viaje)
        {
            return Convert.ToInt32(Leer("MF_ASIGNAR", P("@IdUnidadFlota", unidad), P("@IdViaje", viaje)).Rows[0]["Id"]);
        }
    }
}
