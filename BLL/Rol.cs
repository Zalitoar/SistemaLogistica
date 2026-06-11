using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Rol
    {
        public List<BE.Rol> Listar()
        {
            return new DAL.MP_Rol().Listar();
        }

        public ComponentePermiso ObtenerArbol(int idRol)
        {
            return new DAL.MP_Permiso().ObtenerComponente(idRol);
        }
        public int Insertar(string nombreRol)
        {
            BE.Rol nuevoRol = new BE.Rol { Nombre_Permiso = nombreRol };
            return new DAL.MP_Rol().Insertar(nuevoRol);
        }
        public string AsignarComponente(int idRol, int idComponente)
        {
            if (idRol == idComponente)
                return "Un rol no puede contenerse a sí mismo.";

            DAL.MP_RolComponente mp_rolComponente = new DAL.MP_RolComponente();
            if (mp_rolComponente.ListarComponentes(idRol).Contains(idComponente))
                return "Este componente ya está asignado al rol.";

            ComponentePermiso componente = new DAL.MP_Permiso().ObtenerComponente(idComponente);

            if (componente is BE.Rol subRol && Contiene(subRol, idRol))
                return "No se puede asignar: generaría un ciclo.";

            mp_rolComponente.Asignar(idRol, idComponente);
            return null;
        }

        private bool Contiene(ComponentePermiso nodo, int idBuscado)
        {
            if (nodo.Id_Permiso == idBuscado)
                return true;

            if (nodo is BE.Rol rol)
            {
                foreach (ComponentePermiso hijo in rol.Componentes)
                {
                    if (Contiene(hijo, idBuscado))
                        return true;
                }
            }

            return false;
        }

        public void QuitarComponente(int idRol, int idComponente)
        {
            new DAL.MP_RolComponente().Quitar(idRol, idComponente);
        }
    }
}
