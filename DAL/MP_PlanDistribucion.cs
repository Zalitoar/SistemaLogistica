using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using BE;

namespace DAL
{
    public class MP_PlanDistribucion
    {
        public List<RequerimientoDistribucion> ListarPendientes()
        {
            return MP_TD.Leer("TD_LISTAR_REQUERIMIENTOS").AsEnumerable().Select(r => new RequerimientoDistribucion
            {
                IdRequerimiento = MP_TD.Id(r, "Id_Requerimiento"), Numero = (string)r["Numero"],
                Fecha = (DateTime)r["Fecha"], Origen = (string)r["Origen"], Estado = (string)r["Estado"]
            }).ToList();
        }
        public List<Mercaderia> ListarMercaderia()
        {
            return MP_TD.Leer("TD_LISTAR_MERCADERIA").AsEnumerable().Select(r => new Mercaderia
            {
                IdMercaderia = MP_TD.Id(r, "Id_Mercaderia"), Codigo = (string)r["Codigo"], Descripcion = (string)r["Descripcion"]
            }).ToList();
        }
        public int Recibir(string numero, string origen)
        {
            return MP_TD.Id(MP_TD.Leer("TD_RECIBIR_REQUERIMIENTO", MP_TD.Parametro("@Numero", numero), MP_TD.Parametro("@Origen", origen)).Rows[0], "Id");
        }
        public int CrearMercaderia(string codigo, string descripcion)
        {
            return MP_TD.Id(MP_TD.Leer("TD_CREAR_MERCADERIA", MP_TD.Parametro("@Codigo", codigo), MP_TD.Parametro("@Descripcion", descripcion)).Rows[0], "Id");
        }
        public int Confirmar(PlanDistribucion plan)
        {
            var xml = new XElement("viajes", plan.Viajes.Select(v => new XElement("viaje",
                new XAttribute("numero", v.Numero.Trim()), new XAttribute("fecha", v.FechaPrevista),
                new XElement("entregas", v.Entregas.Select(e => new XElement("entrega",
                    new XAttribute("destino", e.Destino.Trim()), new XAttribute("fecha", e.FechaPrevista)))),
                new XElement("carga", v.CargaPlanificada.Select(c => new XElement("item",
                    new XAttribute("mercaderia", c.IdMercaderia), new XAttribute("cantidad", c.CantidadPrevista)))))));
            return MP_TD.Id(MP_TD.Leer("TD_CONFIRMAR_PLAN", MP_TD.Parametro("@IdRequerimiento", plan.IdRequerimiento),
                MP_TD.Parametro("@Numero", plan.Numero.Trim()), MP_TD.Parametro("@Viajes", xml.ToString())).Rows[0], "Id");
        }
    }
}
