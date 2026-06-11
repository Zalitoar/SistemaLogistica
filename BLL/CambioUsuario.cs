using DAL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CambioUsuario
    {
        public void Grabar(BE.CambioUsuario u)
        {
            DAL.MP_CambioUsuario mp_usuario = new DAL.MP_CambioUsuario();
           
            mp_usuario.Insertar(u);     
        }
        public List<BE.CambioUsuario> Listar()
        {
            DAL.MP_CambioUsuario mp_usuario = new DAL.MP_CambioUsuario();

            return mp_usuario.Listar();
        }
    }
}
