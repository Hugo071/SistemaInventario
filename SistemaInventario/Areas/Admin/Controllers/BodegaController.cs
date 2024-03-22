using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BodegaController : Controller
    {
        // Unidad de trabajo para trabajar en la BD
        private readonly IUnidadTrabajo _unidadTrabajo;
        public BodegaController(IUnidadTrabajo unidadTrabajo)
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
            Bodega bodega = new Bodega();
            if (id == null)
            {
                // Creamos un nuevo registro
                bodega.Estado = true;
                return View(bodega);
            }
            bodega = await _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());
            if(bodega == null)
            {
                return NotFound();
            }
            return View(bodega);
        }

        // Tag's 
        [HttpPost]
        [ValidateAntiForgeryToken] // Seguridad
        public async Task<IActionResult> Upsert(Bodega bodega)
        {
            if(ModelState.IsValid)
            {
                if(bodega.Id == 0)
                {
                    await _unidadTrabajo.Bodega.Agregar(bodega);
                    TempData[DS.Exitosa] = "La Bodega se creó con éxito";
                }
                else
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
					TempData[DS.Exitosa] = "La Bodega se actualizó con éxito";
				}
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
			TempData[DS.Error] = "Error al grabar la Bodega";
			return View(bodega);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bodegaDB = await _unidadTrabajo.Bodega.Obtener(id);
            if(bodegaDB == null)
            {
                return Json(new { success = false, message = "Error al borrar el registro en la Base de Datos" });
            }
            _unidadTrabajo.Bodega.Remover(bodegaDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Bodega eliminada con éxito" });
        }


        // Generar una región
        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new {data = todos});
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id=0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Bodega.ObtenerTodos();
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
