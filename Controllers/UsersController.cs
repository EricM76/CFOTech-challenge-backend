using AutoMapper;
using ChallengeCFOTech.Models;
using ChallengeCFOTech.Models.Dtos;
using ChallengeCFOTech.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChallengeCFOTech.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUser _userRepository;
        protected ResponseAPI _responseAPI;
        private readonly IMapper _mapper;

        public UsersController(IUser userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            this._responseAPI = new();
            _mapper = mapper;
        }

        // TRAER TODOS LOS USUARIOS
        [Authorize(Roles = "admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            var userList = _userRepository.GetUsers();
            var userListDto = new List<UserDto>();

            foreach (var list in userList)
            {
                userListDto.Add(_mapper.Map<UserDto>(list));
            }

            return Ok(userListDto);

        }

        //TRAER UN USUARIO POR ID
        [Authorize(Roles = "user")]
        [HttpGet("{userId}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(int userId)
        {

            var itemUser = _userRepository.GetUser(userId);

            if (itemUser == null)
            {
                return NotFound();
            }

            var itemUserDto = _mapper.Map<UserDto>(itemUser);

            return Ok(itemUserDto);

        }

        //REGISTRAR USUARIO
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody]UserRegisterDto userRegisterDto)
        {

            bool validateUserName = _userRepository.IsUniqueUser(userRegisterDto.UserName);
            if(!validateUserName)
            {
                _responseAPI.StatusCode = HttpStatusCode.BadRequest;
                _responseAPI.IsSuccess = false;
                _responseAPI.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_responseAPI);
            }

            var user = await _userRepository.Register(userRegisterDto);
            if(user == null)
            {
                _responseAPI.StatusCode = HttpStatusCode.BadRequest;
                _responseAPI.IsSuccess = false;
                _responseAPI.ErrorMessages.Add("Hubo un error en el registro");
                return BadRequest(_responseAPI);
            }

            _responseAPI.StatusCode = HttpStatusCode.OK;
            _responseAPI.IsSuccess = true;
            return Ok(_responseAPI);

        }

        //LOGIN DE USUARIO
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var responseLogin = await _userRepository.Login(userLoginDto);

            if (responseLogin.UserName == "" || string.IsNullOrEmpty(responseLogin.Token))
            {
                _responseAPI.StatusCode = HttpStatusCode.BadRequest;
                _responseAPI.IsSuccess = false;
                _responseAPI.ErrorMessages.Add("Credenciales inválidas");
                return BadRequest(_responseAPI);
            }

            _responseAPI.StatusCode = HttpStatusCode.OK;
            _responseAPI.IsSuccess = true;
            _responseAPI.Result = responseLogin;
            return Ok(_responseAPI);

        }
    }
}
