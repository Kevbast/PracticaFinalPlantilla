using Microsoft.Data.SqlClient;
using PracticaFinalPlantilla.Models;
using System.Data;

namespace PracticaFinalPlantilla.Repositories
{
    public class RepositoryPlantilla
    {
        private SqlConnection cn;
        private SqlCommand com;
        private DataTable tablaPlantilla;

        public RepositoryPlantilla()
        {
            string connectionString = @"Data Source=LOCALHOST\DEVELOPER;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Encrypt=True;Trust Server Certificate=True";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;

            string sql = "select * from PLANTILLA";
            //CREAMOS EL ADAPTADOR PUENTE ENTRE SQL SERVER Y LINQ
            SqlDataAdapter ad = new SqlDataAdapter(sql, this.cn);//es lo mismo que connectionstring aqui
            this.tablaPlantilla = new DataTable();
            ad.Fill(this.tablaPlantilla);
        }

        public List<Plantilla> GetPlantilla()
        {
            var consulta = from datos in this.tablaPlantilla.AsEnumerable()
                           select datos;
            List<Plantilla> empleadosp = new List<Plantilla>();

            foreach (var row in consulta)
            {
                Plantilla p = new Plantilla();
                p.HospitalCod = row.Field<int>("HOSPITAL_COD");//Mirar bien las variables
                p.SalaCod = row.Field<int>("SALA_COD");
                p.EmpleadoNo = row.Field<int>("EMPLEADO_NO");
                p.Apellido = row.Field<string>("APELLIDO");
                p.Funcion = row.Field<string>("FUNCION");
                p.Turno = row.Field<string>("T");
                p.Salario = row.Field<int>("SALARIO");

                empleadosp.Add(p);//añadimos
            }
            return empleadosp;

        }

        //ENCONTRAMOS AL EMPLEADO DE LA PLANTILLA POR SU EMPNO
        public Plantilla FindEmpleadoPlantilla(int empno)
        {
            //FILTRAMOS NUESTRA CONSULTA
            var consulta = from datos in this.tablaPlantilla.AsEnumerable()
                           where datos.Field<int>("EMPLEADO_NO") == empno
                           select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                //DEVUELVE SOLA UNA FILA,PERO LINQ SIEMPRE DEVUELVE UN CONJUNTO
                //DENTRO DE ESTE CONJUTO TENEMOS MÉTODOS LAMBDA PARA HACER COSITAS QUE YA IREMOS VIENDO
                //POR EJEMPLO PODRIAMOS CONTAR,PODRIAMOS SABER EL MAXIMO O RECUPERAR EL PRIMER ELEMENTO DEL CONJUNTO
                var row = consulta.First();
                Plantilla p = new Plantilla();
                p.HospitalCod = row.Field<int>("HOSPITAL_COD");
                p.SalaCod = row.Field<int>("SALA_COD");
                p.EmpleadoNo = row.Field<int>("EMPLEADO_NO");
                p.Apellido = row.Field<string>("APELLIDO");
                p.Funcion = row.Field<string>("FUNCION");
                p.Turno = row.Field<string>("T");
                p.Salario = row.Field<int>("SALARIO");

                return p;
            }
        }

        //PASAMOS CON LA FUNCION INSERT/UPDATE
        public async Task InsertUpdateEmpleadoPlantilla
           (int HospitalCod,int SalaCod,int EmpleadoNo, string Apellido,string Funcion, string Turno,int Salario)

        {
            string sql = "SP_PLANTILLA_UPSERT";

            this.com.Parameters.AddWithValue("@HospitalCod", HospitalCod);
            this.com.Parameters.AddWithValue("@SalaCod", SalaCod);
            this.com.Parameters.AddWithValue("@EmpleadoNo", EmpleadoNo);
            this.com.Parameters.AddWithValue("@Apellido", Apellido);
            this.com.Parameters.AddWithValue("@Funcion", Funcion);
            this.com.Parameters.AddWithValue("@Turno", Turno);
            this.com.Parameters.AddWithValue("@Salario", Salario);

            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();

            await this.com.ExecuteNonQueryAsync();

            await this.cn.CloseAsync();

            this.com.Parameters.Clear();//LIMPIAMOS PARAMETROS SIEMPRE
        }



        //FUNCIÓN DELETE POR EMPNO
        public async Task DeleteEmpleadoPlantilla(int empno)
        {
            string sql = "DELETE from PLANTILLA where EMPLEADO_NO=@empno";//REVISAR SIEMPRE LA CONSULTA
            this.com.Parameters.AddWithValue("@empno", empno);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            //LIMPIAMOS PARAMETROS SIEMPRE
            this.com.Parameters.Clear();
        }

        //AHORA PASAMOS A LAS FUNCIONES QUE QUEDAN PARA EL APARTADO DE BUSCAR

        public List<string> GetFunciones()
        {
            var consulta = (from datos in tablaPlantilla.AsEnumerable()
                            select datos.Field<string>("FUNCION")).Distinct();//se aplica en la consulta el filtro
            //AHORA MISMO YA TENEMOS LO QUE NECESITAMOS,UN CONJUNTO DE STRING
            //LA NORMA SUELE SER DEVOLVER LA COLECCIÓN GENERICA List<t>
            return consulta.ToList();
        }

        //SEGUIMOS CON LAS DEMÁS FUNCIONES
        public ResumenPlantilla GetEmpleadosPlantillaFuncion(string funcion)//DEVOLVEMOS UN RESUMEN DE LOS EMPLEADOS
        {
            //Consulta 
            var consulta = from datos in this.tablaPlantilla.AsEnumerable()
                           where datos.Field<string>("FUNCION") == funcion
                           select datos;



            if (consulta.Count() == 0)
            {
                //HAREMOS QUE DEVUELVA DATOS NEUTROS
                //Creamos nuestro resumen empleados
                ResumenPlantilla model = new ResumenPlantilla();
                model.Personas = 0;
                model.MaximoSalario = 0;
                model.MediaSalarial = 0;
                model.Empleadosp = null;//los empleados de la consulta
                return model;
            }
            else
            {
                //QUIERO ORDENAR LAS PERSONAS POR SU SALARIO

                //Filtramos por filas,puesto q es lo que nos da la consulta
                consulta.OrderBy(p => p.Field<int>("SALARIO"));

                int personas = consulta.Count();
                int maximo = consulta.Max(x => x.Field<int>("SALARIO"));//SE LO TENEMOS QUE INDICAR CON LAMBDA
                double media = consulta.Average(x => x.Field<int>("SALARIO"));
                //cogemos una coleecion de empleado
                List<Plantilla> empleadosplantilla = new List<Plantilla>();

                foreach (var row in consulta)
                {
                    Plantilla p = new Plantilla();
                    p.HospitalCod = row.Field<int>("HOSPITAL_COD");
                    p.SalaCod = row.Field<int>("SALA_COD");
                    p.EmpleadoNo = row.Field<int>("EMPLEADO_NO");
                    p.Apellido = row.Field<string>("APELLIDO");
                    p.Funcion = row.Field<string>("FUNCION");
                    p.Turno = row.Field<string>("T");
                    p.Salario = row.Field<int>("SALARIO");

                    empleadosplantilla.Add(p);
                }
                //Creamos nuestro resumen empleados
                ResumenPlantilla model = new ResumenPlantilla();
                model.Personas = personas;
                model.MaximoSalario = maximo;
                model.MediaSalarial = media;
                model.Empleadosp = empleadosplantilla;//los empleados de la consulta
                return model;

            }

        }




    }
}
