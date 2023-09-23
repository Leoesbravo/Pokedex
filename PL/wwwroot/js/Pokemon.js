var typeIcons = {
    bug: "bug",
    dark: "dark",
    dragon: "dragon",
    electric: "electric",
    fire: "fire",
    fairy: "fairy",
    fighting: "fighting",
    flying: "flying",
    ghost: "ghost",
    grass: "grass",
    ground: "ground",
    ice: "ice",
    normal: "normal",
    poison: "poison",
    psychic: "psychic",
    rock: "rock",
    steel: "steel",
    water: "water"
};
function CargarInfo(url) {
    $("#overlay").show();

    $.ajax({
        url: "@Url.Action("GetPokemons", "Pokedex")",
        type: "GET",
        data: { "url": url },
        success: function (data) {
            $("#overlay").hide();

            // Actualiza el contenido del modal
            $("#ModalTitle").text(data.name);

            var modalContent = `
        <div class="row">
            <div class="col text-center">
                <img src="${data.sprites.front_default}" alt="Imagen del Pokémon">
            </div>
        </div>
        <div class="row">
            <div class="col text-center">
                <p>Numero: ${data.id}</p>
            </div>
        </div>
            <div class="row">
                <div class="col text-center">
                    <p><strong>Tipo</strong></p>
                </div>
            </div>
        <div class="row text-center">
                `;
            $.each(data.types.tipos, function (i, tipo) {
                var tipoNombre = tipo.nombre.toLowerCase(); // Convierte el nombre del tipo a minúsculas
                var iconClass = typeIcons[tipoNombre]; // Obtiene la clase CSS del objeto typeIcons
                modalContent += `<div class="icon ${iconClass}"><img src="/icons/${iconClass}.svg"/></div>`;
            });
            modalContent += `

        </div>

        <div class="row">
                <div class="col  text-center">
                <p><strong>Habilidades:</strong></p>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <ul>`;
            $.each(data.habilidad.habilidades, function (i, habilidad) {
                modalContent += `
                    <li>${habilidad.nombre}</li>`;
            });
            modalContent += `
                </ul>
            </div>
        </div>
        <div class="row">
            <div class="col text-center">
                <p><strong>Estadisticas:</strong></p>
            </div>
        </div>
    `;

            // Divide las stats en dos columnas
            $.each(data.stats.stats, function (i, stat) {
                if (i % 2 === 0) {
                    // Inicio de una nueva fila para las stats
                    modalContent += `</div></div><div class="row">`;
                }
                modalContent += `
            <div class="col">
                <p> ${stat.nombre}:</p>
                <div class="progress">
                    <div class="progress-bar" role="progressbar" style="width: ${stat.base_stat}%;" aria-valuenow="${stat.base_stat}" aria-valuemin="0" aria-valuemax="255">${stat.base_stat}</div>
                </div>
            </div>
        `;
            });

            $("#ModalBody").html(modalContent);
            $("#Modal").modal("show");

        }
    });
};
$(document).ready(function () {
    // Mostrar la pantalla de carga al cargar la página
    $("#overlay").show();

    // Texto que deseas mostrar y animar
    var loadingText = "Cargando...";
    var loadingElement = document.getElementById("loading-text");

    function animateLoadingText() {
        var index = 0;
        var animationInterval = setInterval(function () {
            loadingElement.textContent += loadingText[index];
            index++;
            if (index === loadingText.length) {
                clearInterval(animationInterval);
                // Reinicia la animación después de un breve retraso
                setTimeout(function () {
                    loadingElement.textContent = "";
                    index = 0;
                    animateLoadingText();
                }, 1000);
            }
        }, 100);
    }

    // Inicia la animación cuando se muestra el overlay
    animateLoadingText();

    // Realiza la solicitud AJAX después de iniciar la animación
    $.ajax({
        url: "@Url.Action("GetPokemons","Pokedex")",
        type: "GET",
        success: function (data) {
            // La solicitud AJAX se ha completado con éxito, oculta la pantalla de carga
            $("#overlay").hide();

            // Agrega los Pokémon al contenedor
            var pokemonContainer = $("#pokemonContainer");
            data.pokemons.forEach(function (pokemon) {
                var cardHtml = `
                        <div class="col">
                                    <div class="card bg-dark  border-warning  mb-3 border-5 rounded-pill" style="width: 18rem;">
                                        <img src="${pokemon.sprites.front_default}" class="card-img-top" alt="..."onclick="CargarInfo( ` + `'` + `${pokemon.url}` + `'` + `)" >
                                    <div class="card-body text-center">
                                    <h5 class="card-title card-header text-center bg-secondary bg-gradient rounded-pill">${pokemon.name}</h5>
                                    <p class="card-text">${pokemon.id}</p>
                                    <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/5/53/Pok%C3%A9_Ball_icon.svg/1200px-Pok%C3%A9_Ball_icon.svg.png" height="40" width="40" onclick="CargarInfo( ` + `'` + `${pokemon.url}` + `'` + `)"></a>
                                </div>
                            </div>
                        </div>
                    `;
                pokemonContainer.append(cardHtml);
            });
        },
        error: function (error) {
            // Manejar errores si es necesario
            console.error("Error en la solicitud AJAX: ", error);
            // Ocultar la pantalla de carga en caso de error
            $("#overlay").hide();
        }
    });
});
