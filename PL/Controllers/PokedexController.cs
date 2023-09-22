using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using ML;

namespace PL.Controllers
{
    public class PokedexController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public PokedexController(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public ActionResult GetAll()
        {
            return View();
        }
        public JsonResult GetPokemons(string? url)
        {
            Pokemon pokemon = new Pokemon();
            if (url != null)
            {
                using (var client = new HttpClient())
                {
                    string urlApi = _configuration["urlApi"];//Colocar la url
                    client.BaseAddress = new Uri(urlApi);

                    var responseTask = client.GetAsync(url);
                    responseTask.Wait();

                    var result = responseTask.Result;
                        
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        dynamic resultJSON = JObject.Parse(readTask.Result.ToString());
                        readTask.Wait();

                        pokemon.Habilidad = new Habilidad();
                        pokemon.Habilidad.Habilidades = new List<object>();

                        pokemon.id = resultJSON.id;
                        pokemon.Name = resultJSON.name;
                        foreach(dynamic ability in resultJSON.abilities)
                        {
                            ML.Habilidad habilidad = new Habilidad();
                            habilidad.Nombre = ability.ability.name;
                            pokemon.Habilidad.Habilidades.Add(habilidad);                          
                        }
                        pokemon.sprites = new Sprites();
                        var sprite = resultJSON.sprites;
                        pokemon.sprites.front_default = resultJSON.sprites.other.home.front_default;
                        pokemon.stats = new Stat();
                        pokemon.stats.Stats = new List<object>();
                        foreach (dynamic stats in resultJSON.stats)
                        {
                            ML.Stat stat = new Stat();
                            stat.Nombre = stats.stat.name;
                            stat.base_stat = stats.base_stat;
                            pokemon.stats.Stats.Add(stat);
                        }
                        pokemon.types = new Typee();
                        pokemon.types.Tipos = new List<object>();
                        foreach(dynamic type in resultJSON.types)
                        {
                            ML.Typee tipo = new Typee();
                            tipo.Nombre = type.type.name;
                            pokemon.types.Tipos.Add(tipo);
                        }
                    }
                }
                return Json(pokemon);
            }
            else
            {
                using (var client = new HttpClient())
                {
                    string urlApi = _configuration["urlApi"];//Colocar la url
                    client.BaseAddress = new Uri(urlApi);

                    var responseTask = client.GetAsync("pokemon/?limit=21&offset=0");
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        dynamic resultJSON = JObject.Parse(readTask.Result.ToString());
                        readTask.Wait();
                        pokemon.Pokemons = new List<object>();

                        foreach (var resultItem in resultJSON.results)
                        {
                            Pokemon pokemonItem = new Pokemon();
                            pokemonItem.Name = resultItem.name;
                            pokemonItem.url = resultItem.url;

                            Pokemon pokemonsub = new Pokemon();
                            pokemonsub = ObtenerCadaUno(pokemonItem.url);

                            pokemonItem.id = pokemonsub.id;
                            pokemonItem.sprites = new Sprites();
                            pokemonItem.sprites.front_default = pokemonsub.sprites.front_default;
                            pokemonItem.types = new Typee();
                            pokemonItem.types.Tipos = pokemonsub.types.Tipos;

                            pokemon.Pokemons.Add(pokemonItem);
                        }
                    }
                }
                return Json(pokemon);
            }
        }
        public Pokemon ObtenerCadaUno(string url)
        {
            Pokemon pokemonObtener = new Pokemon();

            using (var client = new HttpClient())
            {
                string urlApi = _configuration["urlApi"];
                client.BaseAddress = new Uri(urlApi);

                var responseTask = client.GetAsync(url);
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    dynamic resultJSON = JObject.Parse(readTask.Result.ToString());
                    readTask.Wait();

                    var listPoke = new List<dynamic>();
                    listPoke.Add(resultJSON);
                    foreach (var resultItem in listPoke.ToList())
                    {
                        pokemonObtener.id = resultItem.id;

                        pokemonObtener.types = new Typee();
                        pokemonObtener.types.Tipos = new List<object>();
                        foreach (var Tipo in resultItem.types)
                        {
                            Typee type = new Typee();
                            type.slot = Tipo.slot;

                            type.type = new Type2();
                            type.type.Name = Tipo.type.name;
                            pokemonObtener.types.Tipos.Add(type);
                        }

                        pokemonObtener.sprites = new Sprites();
                        pokemonObtener.sprites.front_default = resultItem.sprites.other.home.front_default;

                    }
                }
            }
            return pokemonObtener;
        }
        public ActionResult Detalles(string url)
        {
            Pokemon pokemon = new Pokemon();

            if (url != null)
            {
                pokemon = Caracteristicas(url);
                return View(pokemon);
            }
            else
            {

                return View(pokemon);
            }
        }

        public Pokemon Caracteristicas(string url)
        {
            Pokemon pokemonObtener = new Pokemon();

            using (var client = new HttpClient())
            {
                string urlApi = _configuration["urlApi"];
                client.BaseAddress = new Uri(urlApi);

                var responseTask = client.GetAsync(url);
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    dynamic resultJSON = JObject.Parse(readTask.Result.ToString());
                    readTask.Wait();

                    var listPoke = new List<dynamic>();
                    listPoke.Add(resultJSON);

                    foreach (var resultItem in listPoke.ToList())
                    {
                        pokemonObtener.sprites = new Sprites();
                        pokemonObtener.sprites.front_default = resultItem.sprites.other.home.front_default;
                        pokemonObtener.Name = resultItem.name;
                        pokemonObtener.base_experience = resultItem.base_experience;

                        pokemonObtener.stats = new Stat();
                        pokemonObtener.stats.stat = new Stat2();
                        pokemonObtener.stats.Stats = new List<object>();

                        //Listado de stats
                        foreach (var stat in resultItem.stats)
                        {
                            Stat stat1 = new Stat();
                            stat1.base_stat = stat.base_stat;

                            dynamic objeto = stat;

                            stat1.stat = new Stat2();
                            stat1.stat.name = objeto.stat.name;
                            pokemonObtener.stats.Stats.Add(stat1);

                        }
                        //Listado de tipos
                        pokemonObtener.types = new Typee();
                        pokemonObtener.types.Tipos = new List<object>();
                        foreach (var Tipo in resultItem.types)
                        {
                            Typee type = new Typee();
                            type.slot = Tipo.slot;

                            dynamic objeto = Tipo;

                            type.type = new Type2();
                            type.type.Name = objeto.type.name;
                            type.type.url = objeto.type.url;
                            pokemonObtener.types.Tipos.Add(type);
                        }

                    }
                }
            }
            return pokemonObtener;
        }
        [HttpPost]
        public ActionResult GetAll(int Id)
        {
            Id = Id - 1;
            Pokemon pokemon = new Pokemon();
            using (var client = new HttpClient())
            {
                string urlApi = _configuration["urlApi"];//Colocar la url
                client.BaseAddress = new Uri(urlApi);

                var responseTask = client.GetAsync("pokemon/?limit=1&offset=" + Id);
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    dynamic resultJSON = JObject.Parse(readTask.Result.ToString());
                    readTask.Wait();
                    pokemon.Pokemons = new List<object>();

                    foreach (var resultItem in resultJSON.results)
                    {
                        Pokemon pokemonItem = new Pokemon();
                        pokemonItem.Name = resultItem.name;
                        pokemonItem.url = resultItem.url;

                        Pokemon pokemonsub = new Pokemon();
                        pokemonsub = ObtenerCadaUno(pokemonItem.url);

                        pokemonItem.id = pokemonsub.id;
                        pokemonItem.sprites = new Sprites();
                        pokemonItem.sprites.front_default = pokemonsub.sprites.front_default;
                        pokemonItem.types = new Typee();
                        pokemonItem.types.Tipos = pokemonsub.types.Tipos;

                        pokemon.Pokemons.Add(pokemonItem);
                    }
                }
            }
            return View(pokemon);
        }
    }
}
