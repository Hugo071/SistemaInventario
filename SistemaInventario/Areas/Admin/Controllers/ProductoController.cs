using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        // Unidad de trabajo para trabajar en la BD
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnviroment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        // Método Upsert GET
        public async Task<IActionResult> Upsert(int? id)
        {

            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Marca"),
                PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Producto")
            };
            if(id == null)
            {
                //Crear un producto nuevo
                return View(productoVM);
            }
            else
            {
                //Actualizar un producto existente
                productoVM.Producto = await _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());
                if(productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }

        // Generar una región
        #region API

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnviroment.WebRootPath;
                if (productoVM.Producto.Id == 0)
                {
                    // Crear un nuevo producto
                    string upload = webRootPath + DS.ImagenRuta;

                    // Crear un id único en el sistema
                    string fileName = Guid.NewGuid().ToString();

                    // Creamos una variable para conocer la extensión del archivo
                    string extension = Path.GetExtension(files[0].FileName);

                    // Habilitar el filestream para crear el archivo de imagen en tiempo real
                    using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    await _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    // Actualizar el producto
                    var objProducto = await _unidadTrabajo.Producto.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking: false);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + DS.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        // Borrar la imagen anterior 
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        // Verificar que la imagen exista
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        // Crear la nueva imagen
                        using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        productoVM.Producto.ImagenUrl = fileName + extension;
                    }
                    // SI no eligen imagen
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }
                TempData[DS.Exitosa] = "Producto registrado";
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            TempData[DS.Error] = "Error al registrar el producto";
            productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Categoria");
            productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Marca");
            productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Producto");
            return View(productoVM);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            return Json(new {data = todos});
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string serie, int id=0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Producto.ObtenerTodos();
            if(id == 0)
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim());
            }
            else 
            {
				valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim() && b.Id != id);
			}
            if(valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });
		}

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productoDB = await _unidadTrabajo.Producto.Obtener(id);
            if (productoDB == null)
            {
                return Json(new { success = false, message = "Error al borrar el registro en la Base de Datos" });
            }
            // Borrar la imagen del producto eliminado
            string upload = _webHostEnviroment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload, productoDB.ImagenUrl);
            if(System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }
            _unidadTrabajo.Producto.Remover(productoDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto eliminado con éxito" });
        }
        #endregion

        // Clic derecho en Index --> agregar vista -> Vista de Razor -> plantilla layout.cshtml
    }
}
