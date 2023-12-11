using ChallengeCFOTech.Models.Dtos;
using ChallengeCFOTech.Models;
using ChallengeCFOTech.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiPeliculas.Data;
using ApiPeliculas.Utils;

namespace ChallengeCFOTech.Repositories
{
    public class CharacterRepository : ICharacter
    {

        public CharacterDto GetCharacter(int characterId)
        {
            throw new NotImplementedException();
        }

        public ICollection<CharacterDto> GetCharacters()
        {
            throw new NotImplementedException();
        }
    }

}
