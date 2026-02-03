using Microsoft.AspNetCore.Mvc;
using PracticaFinalPlantilla.Models;
using PracticaFinalPlantilla.Repositories;

namespace PracticaFinalPlantilla.Controllers
{
    public class PlantillaController : Controller
    {
        RepositoryPlantilla repo;

        public PlantillaController()
        {
            this.repo = new RepositoryPlantilla();
        }

        //-------------CRUD DE LA PLANTILLA------------------
        public IActionResult Index()
        {
            List<Plantilla> empleadosp = this.repo.GetPlantilla();
            return View(empleadosp);
        }
        //Detalles del Empleado
        public IActionResult DetailsEmpleadoPlantilla(int empno)
        {
            Plantilla emp = this.repo.FindEmpleadoPlantilla(empno);
            return View(emp);
        }

        //CREATE EMPLEADO PLANTILLA
        public async Task<IActionResult> CreateEmpleadoPlantilla()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmpleadoPlantilla(Plantilla p)
        {
            await this.repo.InsertUpdateEmpleadoPlantilla(p.HospitalCod,p.SalaCod,p.EmpleadoNo,p.Apellido,p.Funcion,p.Turno,p.Salario);
            return RedirectToAction("Index");
        }

        //UPDATE_EMPLEADO_PLANTILLA
        public IActionResult EditEmpleadoPlantilla(int empno)
        {
            Plantilla emp = this.repo.FindEmpleadoPlantilla(empno);
            return View(emp);
        }
        [HttpPost]
        public async Task<IActionResult> EditEmpleadoPlantilla(Plantilla p)
        {
            await this.repo.InsertUpdateEmpleadoPlantilla(p.HospitalCod, p.SalaCod, p.EmpleadoNo, p.Apellido, p.Funcion, p.Turno, p.Salario);

            return RedirectToAction("Index");
        }

        //UN DELETE SIMPLE POR EMPNO

        public async Task<IActionResult> DeleteEmpPlantilla(int empno)
        {
            await this.repo.DeleteEmpleadoPlantilla(empno);
            return RedirectToAction("Index");//lo mandamos para que actualice
        }

        //----------------------BUSCADOR POR FUNCION EMPLEADOS PLANTILLA------------------------------
        public IActionResult DatosEmpleadosPlantilla()
        {
            List<string> funciones = this.repo.GetFunciones();
            //cargamos con viewdata los oficios
            ViewData["FUNCIONES"] = funciones;
            return View();
        }
        [HttpPost]
        public IActionResult DatosEmpleadosPlantilla(string funcion)
        {
            List<string> funciones = this.repo.GetFunciones();
            //cargamos con viewdata los oficios
            ViewData["FUNCIONES"] = funciones;

            ResumenPlantilla model = this.repo.GetEmpleadosPlantillaFuncion(funcion);

            return View(model);//recogemos el model con todo lo que necesitamos 

        }


    }
}
