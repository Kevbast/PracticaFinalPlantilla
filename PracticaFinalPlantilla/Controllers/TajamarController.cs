using Microsoft.AspNetCore.Mvc;
using PracticaFinalPlantilla.Models;
using PracticaFinalPlantilla.Repositories;

namespace PracticaFinalPlantilla.Controllers
{
    public class TajamarController : Controller
    {
        RepositoryTajamar repo;

        public TajamarController()
        {
            this.repo = new RepositoryTajamar();
        }
        public IActionResult Index()
        {
            List<UsuarioTajamar> users = this.repo.GetUsuarios();
            return View(users);
        }

        //Detalles del USUARIO DE TAJAMAR
        public IActionResult DetailsUsuarioTajamar(int idusuario)
        {
            ResumenTajamar detailsuser = this.repo.DetailsUsuarioTajamar(idusuario);
            return View(detailsuser);
        }

    }
}
