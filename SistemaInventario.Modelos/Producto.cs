using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="El número de serie es requerido")]
        [MaxLength(30, ErrorMessage ="Máximo 30 caracteres")]
        public string NumeroSerie { get; set; }
        [Required(ErrorMessage = "La descripción es requerida")]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Precio requerido")]
        public double Precio { get; set; }
        [Required(ErrorMessage = "El costo es requerido")]
        public double Costo { get; set; }
        public string ImagenUrl { get; set; }
        [Required(ErrorMessage = "El estado es requerido")]
        public bool Estado { get; set; }

        // Relación con la tabla Categoria
        // Mismo tipo de dato de la clave primaria de la tabla a la que se hará relación
        [Required(ErrorMessage = "La categoria es requerida")]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "La marca es requerida")]
        public int MarcaId { get; set; }
        [ForeignKey("MarcaId")]
        public Marca Marca { get; set; }

        // Recursividad del padre

        public int? PadreId { get; set; }

        public virtual Producto Padre { get; set; }
    }
}
