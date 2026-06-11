using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Permiso : ComponentePermiso
    {
        public override List<Permiso> ObtenerPermisos()
        {
            return new List<Permiso> { this };
        }

        public override bool TienePermiso(string nombrePermiso)
        {
            return Nombre_Permiso == nombrePermiso;
        }
    }
}
