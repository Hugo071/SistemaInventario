using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using SistemaInventario.AccesoDatos.Data;
using Microsoft.EntityFrameworkCore;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsuarioController : Controller
    {
        // Crear los accesos a la Unidad de Trabajo y a la Base de Datos
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly ApplicationDbContext _db;

        // Inyección de dependencias
        public UsuarioController(IUnidadTrabajo unidadTrabajo, ApplicationDbContext db)
        {
            _unidadTrabajo = unidadTrabajo;
            _db = db;
        }

        public IActionResult Index() 
        {
            return View();
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarioLista = await _unidadTrabajo.UsuarioAplicacion.ObtenerTodos();
            var userRole = await _db.UserRoles.ToListAsync();
            var roles = await _db.Roles.ToListAsync();

            foreach(var usuario in usuarioLista)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == usuario.Id).RoleId;
                usuario.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return Json(new { data = usuarioLista });
        }

        [HttpPost]
        public async Task<IActionResult> BloquearDesbloquear([FromBody]string id)
        {
            var usuario = await _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(u => u.Id == id);
            if(usuario == null)
            {
                return Json(new {success = false, message="Error de Usuario"});
            }
            if(usuario.LockoutEnd != null && usuario.LockoutEnd > DateTime.Now)
            {
                // Usuario Bloqueado, hay que desbloquear
                usuario.LockoutEnd = DateTime.Now;
            }
            else
            {
                // Usuario Desbloqueado, hay que bloquearlo
                usuario.LockoutEnd= DateTime.Now.AddYears(1000);
            }
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Operacion exitosa" });
        }
        #endregion
    }
}
