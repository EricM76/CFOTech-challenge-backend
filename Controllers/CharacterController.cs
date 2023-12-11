using AutoMapper;
using ChallengeCFOTech.Models;
using ChallengeCFOTech.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChallengeCFOTech.Controllers
{
    [Route("api/characters")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly string _baseUrl;
        private readonly string _apikey;
        private readonly string _hash;
        private readonly ICharacter _characterRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public CharacterController(ICharacter characterRepository, IMapper mapper, IConfiguration configuration)
        {
            _characterRepository = characterRepository;
            _mapper = mapper;
            _configuration = configuration;

        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCharacters(int offset = 0, string nameStartsWith = "")
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            string url =
                $"{_configuration["ApiMarvel:urlBase"]}/characters" +
                $"?ts=1000&apikey={_configuration["ApiMarvel:apiKey"]}" +
                $"&hash={_configuration["ApiMarvel:hash"]}&offset={offset}";
             string optionals = nameStartsWith.GetType() == null || nameStartsWith == "" ? null : $"&nameStartsWith={nameStartsWith}";

            HttpResponseMessage response =
                client.GetAsync(url + optionals).Result;


            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return Conflict();
            }

            string content = response.Content.ReadAsStringAsync().Result;

            dynamic result = JsonConvert.DeserializeObject(content);

            var characterListDto = new List<CharacterDto>();

            foreach (var item in result.data.results)
            {
                CharacterDto character = new CharacterDto();
                character.Id = item.id;
                character.Name = item.name;
                character.Url_Image = item.thumbnail.path + "." + item.thumbnail.extension;

                characterListDto.Add(_mapper.Map<CharacterDto>(character));
            }

            return Ok(characterListDto);
        }

        [AllowAnonymous]
        [HttpGet("{characterId}", Name = "GetCharacter")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCharacter(int characterId)
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));


            HttpResponseMessage response = client.GetAsync($"{_configuration["ApiMarvel:urlBase"]}/characters/{characterId}?ts=1000&apikey={_configuration["ApiMarvel:apiKey"]}&hash={_configuration["ApiMarvel:hash"]}").Result;


            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }

            string content = response.Content.ReadAsStringAsync().Result;

            dynamic result = JsonConvert.DeserializeObject(content);


            Character character = new Character();
            character.Id = result.data.results[0].id;
            character.Name = result.data.results[0].name;
            character.Url_Image = result.data.results[0].thumbnail.path + "." + result.data.results[0].thumbnail.extension;
            character.Description = result.data.results[0].description;

            var comics = new List<string>();
            foreach (var item in result.data.results[0].comics.items)
            {
                comics.Add(item.name.ToString());
            }
            character.Comics = comics;

            var characterDetail = _mapper.Map<Character>(character);

            return Ok(characterDetail);
        }

    }
}
    
