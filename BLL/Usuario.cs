using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Usuario
    {
        public BE.Usuario ValidarIngreso(string nombre, string clave)
        {
            DAL.MP_Usuario mp_usuario = new DAL.MP_Usuario();
            List<BE.Usuario> usuarios = mp_usuario.Listar();
            BE.Usuario uEncontrado = usuarios.FirstOrDefault(u => u.Nombre == nombre);
            if (uEncontrado == null || uEncontrado.Clave != Servicios.CryptoManager.Hash(clave))
            {
                return null;
            }
            return uEncontrado;
        }
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
            new DVVUsuario().Actualizar();
        }

        public void Borrar(BE.Usuario u)
        {
            DAL.MP_Usuario mp_usuario = new DAL.MP_Usuario();
            mp_usuario.Borrar(u);
            new DVVUsuario().Actualizar();
        }

        public List<BE.Usuario> Listar()
        {
            DAL.MP_Usuario mp_usuario = new DAL.MP_Usuario();

            return mp_usuario.Listar();
        }

    }
}
