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
    openTab('registro');
});

document.addEventListener("DOMContentLoaded", function () {
    var formRegistro = document.getElementById("formRegistro");

    if (formRegistro) { // Verificar si el formulario existe
        formRegistro.addEventListener("submit", function (event) {
            var correoInput = document.getElementById("correoInput");
            var correoError = document.getElementById("correoError");
            var correoValue = correoInput.value.trim();

            // Expresión regular para verificar que el correo termine con "@uabc.edu.mx"
            var regex = /@uabc\.edu\.mx$/;

            if (!regex.test(correoValue)) {
                correoError.innerText = "El correo debe terminar con @uabc.edu.mx";
                event.preventDefault(); // Evitar que el formulario se envíe si la validación falla
            } else {
                correoError.innerText = ""; // Limpiar el mensaje de error si la validación es exitosa
            }
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    var formRegistro = document.getElementById("formRegistro");

    if (formRegistro) {
        formRegistro.addEventListener("submit", function (event) {
            validarCampo("idInput", "idError", "Es necesario insertar un id", event);                  
        });
    }
});

function validarCampo(inputId, errorId, errorMessage, event) {
    var input = document.getElementById(inputId);
    var error = document.getElementById(errorId);
    var inputValue = input.value.trim();

    // Expresión regular para verificar que el campo no contiene números
    var regex = /^[^\d]+$/;

    if (!inputValue) {
        error.innerText = "Este campo no puede estar vacío";
        event.preventDefault(); // Evita que el formulario se envíe si la validación falla
    } else if (!regex.test(inputValue)) {
        error.innerText = errorMessage;
        event.preventDefault(); // Evita que el formulario se envíe si la validación falla
    } else {
        error.innerText = ""; // Limpia el mensaje de error si la validación es exitosa
    }
}
function validarFormulario() {
    // Obtener el valor del campo
    var idAsesor = document.getElementById("IdAsesoria").value;

    // Verificar si el campo está vacío
    if (idAsesor.trim() === "") {
        // Mostrar un mensaje de error (puedes personalizar según tus necesidades)
        alert("Por favor, ingrese la matrícula.");
        // Devolver false para evitar que el formulario se envíe
        return false;
    }

    // Si llegamos aquí, el formulario se enviará
    return true;
}
