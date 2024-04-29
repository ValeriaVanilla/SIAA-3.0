function openTab(tabName) {
    // Oculta todos los contenidos
    document.querySelectorAll('.tab-pane').forEach(content => {
        content.classList.remove('active');
    });

    // Quita la clase 'active' de todas las pestañas
    document.querySelectorAll('.nav-tabs a').forEach(tab => {
        tab.classList.remove('active');
    });

    // Muestra el contenido de la pestaña seleccionada
    document.getElementById(tabName).classList.add('active');

    // Marca la pestaña como activa
    document.querySelector(`.nav-tabs a[href="#${tabName}"]`).classList.add('active');
}

document.addEventListener('DOMContentLoaded', function () {
    // Llama a openTab con el nombre de la pestaña de registro al cargar la página
    openTab('registroA');
});


document.addEventListener('DOMContentLoaded', function () {
    // Llama a openTab con el nombre de la pestaña de registro al cargar la página
    openTab('registroA');
});
$(document).ready(function () {
    $("#Id_AsesorDropDown").change(function () {
        var selectedId = $(this).val();

        // Construir la URL explícitamente
        var url = '@Url.Action("ObtenerNombreAsesor", "asesorias")' + '?id=' + selectedId;

        // Realiza una solicitud Ajax para obtener el nombre del asesor
        $.ajax({
            url: url,
            type: 'GET',
            success: function (result) {
                $("#NombreEditor").val(result); // Actualiza el valor del editor de texto
            },
            error: function (xhr, status, error) {
                console.error("Error en la solicitud Ajax:", error);
                // Maneja errores si es necesario
            }
        });
    });
});