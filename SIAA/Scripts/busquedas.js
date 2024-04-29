function filtrarTabla() {
    // Obtener el valor de búsqueda
    var input = document.getElementById("myInput");
    var filtro = input.value.toUpperCase();

    // Obtener la tabla y las filas
    var tabla = document.getElementById("myTable");
    var filas = tabla.getElementsByTagName("tr");

    // Iterar sobre las filas y ocultar las que no coinciden con la búsqueda
    for (var i = 0; i < filas.length; i++) {
        var celdas = filas[i].getElementsByTagName("td");
        var mostrarFila = false;

        for (var j = 0; j < celdas.length; j++) {
            if (celdas[j]) {
                var textoCelda = celdas[j].innerText || celdas[j].textContent;
                if (textoCelda.toUpperCase().indexOf(filtro) > -1) {
                    mostrarFila = true;
                    break;
                }
            }
        }

        // Mostrar u ocultar la fila según la búsqueda
        filas[i].style.display = mostrarFila ? "" : "none";
    }
}

