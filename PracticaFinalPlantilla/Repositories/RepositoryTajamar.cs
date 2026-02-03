using Microsoft.Data.SqlClient;
using PracticaFinalPlantilla.Models;
using System.Data;

namespace PracticaFinalPlantilla.Repositories
{
    public class RepositoryTajamar
    {
        private DataTable tablaResumen;

        public RepositoryTajamar()
        {
            string connectionString = @"Data Source=LOCALHOST\DEVELOPER;Initial Catalog=DEPORTESTAJAMAR;Persist Security Info=True;User ID=SA;Trust Server Certificate=True";

            // La consulta con todos los campos necesarios para ambos métodos
            string sql = @"SELECT 
                    u.IDUSUARIO, u.NOMBRE, u.APELLIDOS, u.EMAIL, u.IMAGEN, u.IDCURSO,
                    c.NOMBRE as NombreCurso,
                    a.nombre as NombreActividad,
                    i.quiere_ser_capitan,
                    i.fecha_inscripcion
                  FROM USUARIOSTAJAMAR u
                  INNER JOIN CURSOSTAJAMAR c ON u.IDCURSO = c.IDCURSO
                  INNER JOIN INSCRIPCIONES i ON u.IDUSUARIO = i.id_usuario
                  INNER JOIN EVENTO_ACTIVIDADES ea ON i.IdEventoActividad = ea.IdEventoActividad
                  INNER JOIN ACTIVIDADES a ON ea.IdActividad = a.id_actividad
                  INNER JOIN EVENTOS e ON ea.IdEvento = e.id_evento";

            SqlDataAdapter ad = new SqlDataAdapter(sql, connectionString);
            this.tablaResumen = new DataTable();
            ad.Fill(this.tablaResumen);
        }

        // 1. OBTENER TODOS LOS USUARIOS (Lista simple)
        public List<UsuarioTajamar> GetUsuarios()
        {
            var consulta = from datos in this.tablaResumen.AsEnumerable()
                           select datos;


            //// Usamos GroupBy para no repetir usuarios si están en varias actividades
            //// en la tabla con JOINS
            //var consultaUnicos = consulta.GroupBy(x => x.Field<int>("IDUSUARIO"))
            //                             .Select(g => g.First());

            List<UsuarioTajamar> usuarios = new List<UsuarioTajamar>();
            foreach (var row in consulta)
            {
                UsuarioTajamar u = new UsuarioTajamar();
                u.IdUsuario = row.Field<int>("IDUSUARIO");
                u.Nombre = row.Field<string>("NOMBRE");
                u.Apellidos = row.Field<string>("APELLIDOS");
                u.Email = row.Field<string>("EMAIL");
                u.Imagen = row.Field<string>("IMAGEN");
                //u.Idcurso = row.Field<int>("IDCURSO");

                usuarios.Add(u);
            }
            return usuarios;
        }

        // 2. ENCONTRAMOS AL USUARIO CON SUS DETALLES (Resumen)
        public ResumenTajamar DetailsUsuarioTajamar(int idusuario)
        {
            var consulta = from datos in this.tablaResumen.AsEnumerable()
                           where datos.Field<int>("IDUSUARIO") == idusuario
                           select datos;

            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                // RECUPERAMOS EL PRIMER ELEMENTO DEL CONJUNTO
                var row = consulta.First();
                ResumenTajamar resumen = new ResumenTajamar();

                resumen.IdUsuario = row.Field<int>("IDUSUARIO");
                resumen.Nombre = row.Field<string>("NOMBRE");
                resumen.Apellidos = row.Field<string>("APELLIDOS");
                resumen.Imagen = row.Field<string>("IMAGEN");
                resumen.Email = row.Field<string>("EMAIL");
                resumen.Curso = row.Field<string>("NombreCurso");
                resumen.Actividad = row.Field<string>("NombreActividad");
                resumen.FechaInscripcion = row.Field<DateTime>("fecha_inscripcion");
                resumen.Capitan = row.Field<bool>("quiere_ser_capitan");

                return resumen;
            }
        }
    }
}