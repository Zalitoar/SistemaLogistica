using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DVVUsuario
    {

        //public List<BE.DVVUsuario> Listar()
        //{
        //    List<BE.DVVUsuario> listadvv = new List<BE.DVVUsuario>();
        //    listadvv = new DAL.MP_DVVUsuario().Listar();
        //    return listadvv;
        //}

        //public string Calcular()
        //{
        //    List<BE.Usuario> usuarios = new DAL.MP_Usuario().Listar();
            
        //    var ordenados = usuarios.OrderBy(u => u.Id_Usuario);

        //    StringBuilder sb = new StringBuilder();
        //    foreach (var u in ordenados)
        //        sb.Append(u.Id_Usuario).Append(u.Nombre).Append(u.Clave).Append(u.Id_Perfil);

        //    return Servicios.CryptoManager.Hash(sb.ToString());
        //}
        //public void Actualizar()
        //{
        //    var dvv = new BE.DVVUsuario
        //    {
        //        Tabla_DVV = "Usuario",
        //        Valor_DVV = Calcular()
        //    };
        //    new DAL.MP_DVVUsuario().Editar(dvv);
        //}
        //public bool Verificar()
        //{
        //    var registros = new DAL.MP_DVVUsuario().Listar();
        //    if (registros.Count == 0) return false;

        //    return registros[0].Valor_DVV == Calcular();
        //}


    }
}
