using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriaController : Controller
    {
        // Unidad de trabajo para trabajar en la BD
        private readonly IUnidadTrabajo _unidadTrabajo;
        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        // Método Upsert GET
        public async Task<IActionResult> Upsert(int? id)
        {
            Categoria categoria = new Categoria();
            if (id == null)
            {
                // Creamos un nuevo registro
                categoria.Estado = true;
                return View(categoria);
            }
            categoria = await _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if(categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // Tag's 
        [HttpPost]
        [ValidateAntiForgeryToken] // Seguridad
        public async Task<IActionResult> Upsert(Categoria categoria)
        {
            if(ModelState.IsValid)
            {
                if(categoria.Id == 0)
                {
                    await _unidadTrabajo.Categoria.Agregar(categoria);
                    TempData[DS.Exitosa] = "La Categoria se creó con éxito";
                }
                else
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
					TempData[DS.Exitosa] = "La Categoria se actualizó con éxito";
				}
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
			TempData[DS.Error] = "Error al grabar la Categoria";
			return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriaDB = await _unidadTrabajo.Categoria.Obtener(id);
            if(categoriaDB == null)
            {
                return Json(new { success = false, message = "Error al borrar el registro en la Base de Datos" });
            }
            _unidadTrabajo.Categoria.Remover(categoriaDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria eliminada con éxito" });
        }


        // Generar una región
        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new {data = todos});
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id=0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Categoria.ObtenerTodos();
            if(id == 0)
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else 
            {
				valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
			}
            if(valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });
		}
        #endregion

        // Clic derecho en Index --> agregar vista -> Vista de Razor -> plantilla layout.cshtml
    }
}
