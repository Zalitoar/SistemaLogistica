using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public abstract class ComponentePermiso
    {
        public int Id_Permiso { get; set; }
        public string Nombre_Permiso { get; set; }

        public abstract List<Permiso> ObtenerPermisos();
        public abstract bool TienePermiso(string nombrePermiso);
    }
}
