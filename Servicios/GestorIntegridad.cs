using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public static class GestorIntegridad
    {
        //Validar DVH
        public static List<BE.Usuario> ValidarIntegridadDVH()
        {
            List<BE.Usuario> usuarios = new List<Usuario>();
            DAL.MP_Usuario mP_Usuario = new DAL.MP_Usuario();
            usuarios = mP_Usuario.Listar();
            List<BE.Usuario> regError = new List<Usuario>();

            foreach (BE.Usuario u in usuarios)
            {
                string dvhCalculado = CryptoManager.Hash($"{u.Nombre}|{u.Clave}|{u.Id_Rol}|{u.Borrado}");
                if (u.DVH != dvhCalculado)
                {
                    regError.Add(u);
                }
            }
            return regError;
        }


        //Validar DVV
        public static bool ValidarIntegridadDVV()
        {
            try
            {
                string nuevoCalculoDvv = Calcular();
                List<BE.DVVUsuario> listaDVV = Listar();

                if (listaDVV == null || listaDVV.Count == 0)
                    return false;

                BE.DVVUsuario dvvUsuario = listaDVV
                    .FirstOrDefault(d => string.Equals(d.Tabla_DVV, "Usuario", StringComparison.OrdinalIgnoreCase));

                if (dvvUsuario == null)
                    return false;

                return nuevoCalculoDvv == dvvUsuario.Valor_DVV;
            }
            catch
            {
                return false;
            }
        }

        public static List<BE.DVVUsuario> Listar()
        {
            List<BE.DVVUsuario> listadvv = new List<BE.DVVUsuario>();
            listadvv = new DAL.MP_DVVUsuario().Listar();
            return listadvv;
        }

        public static string Calcular()
        {
            List<BE.Usuario> usuarios = new DAL.MP_Usuario().Listar();

            var ordenados = usuarios.OrderBy(u => u.Id_Usuario);

            StringBuilder sb = new StringBuilder();
            foreach (var u in ordenados)
                sb.Append(u.Id_Usuario).Append(u.Nombre).Append(u.Clave).Append(u.Id_Rol);

            return Servicios.CryptoManager.Hash(sb.ToString());
        }
        public static void Actualizar()
        {
            var dvv = new BE.DVVUsuario
            {
                Tabla_DVV = "Usuario",
                Valor_DVV = Calcular()
            };
            new DAL.MP_DVVUsuario().Editar(dvv);
        }
        public static bool Verificar()
        {
            try
            {
                var registros = new DAL.MP_DVVUsuario().Listar();

                if (registros == null || registros.Count == 0)
                    return false;

                var dvvUsuario = registros
                    .FirstOrDefault(d => string.Equals(d.Tabla_DVV, "Usuario", StringComparison.OrdinalIgnoreCase));

                if (dvvUsuario == null)
                    return false;

                return dvvUsuario.Valor_DVV == Calcular();
            }
            catch
            {
                return false;
            }
        }

        public static int Restore()
        {
            int ok = new DAL.MP_GestorIdentidad().Restore();
             if (ok == 1)
            {                
                BitacoraManager.Registrar("Se restauró la base de datos a partir del backup.");
                return 1;
            }
            return 0;
        }
    }
}
