using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Usuario
    {
        public void Grabar(BE.Usuario u)
        {
            DAL.MP_Usuario mp_usuario = new DAL.MP_Usuario();
            if (u.Id_Usuario == 0)
            {
                mp_usuario.Insertar(u);
            }
            else
            {
                mp_usuario.Editar(u);
            }
        }

        public void Borrar(BE.Usuario u)
        {
            DAL.MP_Usuario mp_usuario = new DAL.MP_Usuario();
            mp_usuario.Borrar(u);
        }

        public List<BE.Usuario> Listar()
        {
            DAL.MP_Usuario mp_usuario = new DAL.MP_Usuario();

            return mp_usuario.Listar();
        }

    }
}
