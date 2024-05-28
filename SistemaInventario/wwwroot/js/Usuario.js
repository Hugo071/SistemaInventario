let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $("#tblDatos").DataTable({
        "ajax": {
            "url": "/Admin/Usuario/ObtenerTodos"
        },
        "columns": [
            { "data": "email" },
            { "data": "nombres" },
            { "data": "apellidos" },
            { "data": "phoneNumber" },
            { "data": "role" },
            {
                "data":
                {
                   id:"id",lockoutEnd:"lockoutEnd"
                },
                "render": function (data) {
                    let hoy = new Date().getTime();
                    let bloqueo = new Date(data.lockoutEnd).getTime();
                    if (bloqueo > hoy) {
                        // usuario bloqueado
                        return `
                        <div class="text-center">
                            <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-danger text-white" style="cursor:pointer", width:200px>
                                <i class="bi bi-unlock-fill"></i>
                                Desbloquear
                            </a>
                        </div>
                    `;
                    }
                    else {
                        // usuario desbloqueado
                        return `
                        <div class="text-center">
                            <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-success text-white" style="cursor:pointer", width:200px>
                                <i class="bi bi-lock-fill"></i>
                                Bloquear
                            </a>
                        </div>
                    `;
                    }
                },
            }
        ],

        "language": {
            "decimal": "",
            "emptyTable": "No hay datos disponibles en la tabla",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ entradas totales",
            "infoEmpty": "Mostrando de 0 a 0 en 0 entradas",
            "infoFiltered": "(Filtrado de MAX entradas totales)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "",
            "search": "Buscar:",
            "zeroRecords": "No se encontraron registros coincidentes",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            },
            "aria": {
                "orderable": "Ordenar por esta columna",
                "orderableReverse": "Ordena al revés por esta columna"
            }
        }
        
    });
}

function BloquearDesbloquear(id) {    
        $.ajax({
            type: "POST",
            url: '/Admin/Usuario/BloquearDesbloquear',
            data: JSON.stringify(id),
            contentType: "application/json",
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    datatable.ajax.reload();
                }
                else {
                    toastr.error(data.message);
                }
            }
        });
}