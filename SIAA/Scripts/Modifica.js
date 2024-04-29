


function mostrarModal(url) {
    // Cargar el contenido de edición usando Ajax
    $.get(url, function (data) {
        // Colocar el contenido en el modal
        $("#modalContent").html(data);

        // Mostrar el modal
        $("#myModal").modal("show");

    });
}

$(document).ready(function () {
    // Captura el clic en el enlace de modificación de asesoría
    $('.modificar-asesoria').on('click', function (e) {
        e.preventDefault(); // Evita que el enlace se comporte como un enlace normal

        var idAsesoria = $(this).data('id');

        // Obtén la URL de acción desde el atributo data-modifica-url del modal
        var modificaUrl = $("#myModal").data("modifica-url");

        // Llama a la función para cargar la vista de modificación de asesoría
        mostrarModal(modificaUrl + '/' + idAsesoria);

    });



    // Captura el clic en el enlace de modificación de asesor
    $('.modificar-asesor').on('click', function (e) {
        e.preventDefault(); // Evita que el enlace se comporte como un enlace normal

        var idAsesor = $(this).data('id');

        // Llama a la función para cargar la vista de modificación de asesor
        mostrarModal('@Url.Action("ModificaAsesor", "asesor")/' + idAsesor);
    });

});

