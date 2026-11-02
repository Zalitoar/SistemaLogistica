using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using BE;
namespace DAL
{
    public class MP_DocumentoDespacho
    {
        public List<CargaDespacho> ObtenerCarga(int idViaje)
        {
            return MP_TD.Leer("TD_OBTENER_CARGA", MP_TD.Parametro("@IdViaje",idViaje)).AsEnumerable().Select(r => new CargaDespacho
            {
                IdMercaderia=MP_TD.Id(r,"Id_Mercaderia"), Codigo=(string)r["Codigo"], Descripcion=(string)r["Descripcion"],
                CantidadPrevista=(decimal)r["CantidadPrevista"], CantidadPreparada=(decimal)r["CantidadPreparada"]
            }).ToList();
        }
        public DocumentoDespacho Obtener(int idViaje)
        {
            var tabla=MP_TD.Leer("TD_OBTENER_DESPACHO", MP_TD.Parametro("@IdViaje",idViaje));
            if (tabla.Rows.Count==0) return null;
            var r=tabla.Rows[0];
            return new DocumentoDespacho { IdDespacho=MP_TD.Id(r,"Id_Despacho"), IdViaje=idViaje,
                Numero=(string)r["Numero"], Fecha=(DateTime)r["Fecha"], Estado=(string)r["Estado"], Observaciones=r["Observaciones"].ToString() };
        }
        public DocumentoDespacho Guardar(int idViaje, string numero, string observaciones, List<CargaDespacho> carga)
        {
            var xml=new XElement("carga",carga.Select(c => new XElement("item",
                new XAttribute("mercaderia",c.IdMercaderia),new XAttribute("cantidad",c.CantidadPreparada))));
            var r=MP_TD.Leer("TD_GUARDAR_DESPACHO",MP_TD.Parametro("@IdViaje",idViaje),MP_TD.Parametro("@Numero",numero),
                MP_TD.Parametro("@Observaciones",observaciones),MP_TD.Parametro("@Carga",xml.ToString())).Rows[0];
            return new DocumentoDespacho { IdDespacho=MP_TD.Id(r,"Id"), IdViaje=idViaje, Numero=numero, Estado=(string)r["Estado"] };
        }
    }
}
