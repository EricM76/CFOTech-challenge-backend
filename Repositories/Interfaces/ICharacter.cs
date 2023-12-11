using ChallengeCFOTech.Models.Dtos;
using ChallengeCFOTech.Models;

namespace ChallengeCFOTech.Repositories.Interfaces
{
    public interface ICharacter
    {
        ICollection<CharacterDto> GetCharacters();
        CharacterDto GetCharacter(int characterId);
    }
}
