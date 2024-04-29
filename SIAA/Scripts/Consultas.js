


function Buscar() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("myInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("tabla");
    tr = table.getElementsByTagName("tr");

    for (i = 0; i < tr.length; i++) {
        var matriculaCol = tr[i].getElementsByTagName("td")[0]; // Índice 0 para la columna de matrícula
        var nombreCol = tr[i].getElementsByTagName("td")[1]; // Índice 1 para la columna de nombre

        if (matriculaCol || nombreCol) {
            var matriculaValue = matriculaCol.textContent || matriculaCol.innerText;
            var nombreValue = nombreCol.textContent || nombreCol.innerText;

            if (matriculaValue.toUpperCase().indexOf(filter) > -1 || nombreValue.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}
