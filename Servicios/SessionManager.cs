using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class SessionManager
    {
        private static SessionManager session;

        private BE.Usuario Usuario { set; get; }

        public DateTime FechaInicio { set; get; } 

        public static SessionManager GetInstance()
        {
            return session;            
        }

        public BE.Usuario GetUsuario()
        {
            return Usuario;
        }

        public static void Login(BE.Usuario usuario)
        {
            if (session == null)
            {
                session = new SessionManager();
                session.Usuario = usuario;
                session.FechaInicio = DateTime.Now;
            }
        }

        public static void Logout()
        {
            if (session != null)
            {
                session = null;                
            }            
        }

        private SessionManager() 
        {            
        
        }



    }
}
