using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos.Especificaciones
{
	public class Metadata
	{
		public int TotalPages { get; set; } // Total de páginas de tabla Productos
        public int PageSize { get; set; }
        public int TotalCount { get; set; } // Total de registros de productos
    }
}
