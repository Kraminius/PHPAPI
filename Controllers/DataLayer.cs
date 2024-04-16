using Microsoft.AspNetCore.Mvc;
using PHPAPI.Model;

namespace PHPAPI.Controllers
{
    public static class DataLayer
    {
        public static List<Match> Matches = new List<Match>
    {
        new Match { Id = 1, Name = "John Doe", Location = "New York" },
        new Match { Id = 2, Name = "Jane Doe", Location = "Copenhagen"}
    };

        public static IEnumerable<Match> GetAllMatches() => Matches;
    }
}
