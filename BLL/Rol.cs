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

        public int Editar(int idRol, string nuevoNombre)
        {
            BE.Rol rol = new BE.Rol { Id_Permiso = idRol, Nombre_Permiso = nuevoNombre };
            return new DAL.MP_Rol().Editar(rol);
        }

        public string Borrar(int idRol)
        {
            DAL.MP_RolComponente mp_rolComponente = new DAL.MP_RolComponente();

            if (mp_rolComponente.ListarRolesQueContienen(idRol).Count > 0)
                return "No se puede eliminar: este rol está incluido como componente de otro rol.";

            if (new DAL.MP_Usuario().Listar().Any(u => u.Id_Rol == idRol))
                return "No se puede eliminar: hay usuarios asignados a este rol.";

            foreach (int idComponente in mp_rolComponente.ListarComponentes(idRol))
                mp_rolComponente.Quitar(idRol, idComponente);

            BE.Rol rol = new BE.Rol { Id_Permiso = idRol };
            return new DAL.MP_Rol().Borrar(rol) > 0 ? null : "No se pudo eliminar el rol.";
        }
    }
}
