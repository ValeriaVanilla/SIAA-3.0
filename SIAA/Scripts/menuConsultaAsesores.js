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
    openTab('consulta');
});

$(document).ready(function () {
    // Evento keyup para el campo de búsqueda
    $("#myInput").on("keyup", function () {
        var searchText = $(this).val().toLowerCase();

        // Filtra las filas de la tabla
        $(".searchable-table tr").each(function () {
            var rowText = $(this).text().toLowerCase();
            $(this).toggle(rowText.indexOf(searchText) > -1);
        });
    });
});

