using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Rol : ComponentePermiso
    {
        public List<ComponentePermiso> Componentes { get; set; } = new List<ComponentePermiso>();

        public override List<Permiso> ObtenerPermisos()
        {
            var resultado = new List<Permiso>();
            foreach (var componente in Componentes)
            {
                resultado.AddRange(componente.ObtenerPermisos());
            }
            return resultado;
        }

        public override bool TienePermiso(string nombrePermiso)
        {
            return Componentes.Any(c => c.TienePermiso(nombrePermiso));
        }
    }
}
