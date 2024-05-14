using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
	public class UsuarioAplicacion:IdentityUser
	{
		[Required(ErrorMessage = "El Nombre es requerido...")]
		[MaxLength(80, ErrorMessage = "Máximo 80 caracteres")]
		public string Nombres { get; set; }
		[Required(ErrorMessage = "Los Apellidos son requeridos...")]
		[MaxLength(80, ErrorMessage = "Máximo 80 caracteres")]
		public string Apellidos { get; set; }
		[Required(ErrorMessage = "La Dirección es requerida...")]
		[MaxLength(200, ErrorMessage = "Máximo 200 caracteres")]
		public string Direccion { get; set; }
		[Required(ErrorMessage = "La Ciudad es requerida...")]
		[MaxLength(60, ErrorMessage = "Máximo 60 caracteres")]
		public string Ciudad { get; set; }
		[Required(ErrorMessage = "El País es requerido...")]
		[MaxLength(60, ErrorMessage = "Máximo 60 caracteres")]
		public string Pais { get; set; }

		[NotMapped]
        public string Role { get; set; }
    }
}
