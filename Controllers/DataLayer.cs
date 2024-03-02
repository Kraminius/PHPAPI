using Microsoft.AspNetCore.Mvc;
using PeopleHelpPeople.Model;

namespace PeopleHelpPeople.Controllers
{
    public static class DataLayer
    {
        public static List<Match> Matches = new List<Match>
    {
        new Match { Id = 1, Name = "John Doe", Location = "New York" },
        new Match { Id = 2, Name = "Jane Doe", Location = "Copenhagen"}
    };



        public static IEnumerable<Match> GetAllMatches() => Matches;

        //New stuff

        private var Amanda = new Match { Id = 5, Name = "Amanda Young", Location = "New York" };

        public static List<Chat> Chats = new List<Chat>
        
    {
        new Chat { Id = 1, Updated = new DateTime (2015, 12, 31, 5, 10, 20), Requester = new Match { Id = 3, Name = "Amanda Young", Location = "Gideon Meatplant" },
            Provider = new Match { Id = 4, Name = "Micheal Meyers", Location = "Haddonfield"},
        Messages = new List<String> {{"Hello are you getting the stuff?"}, {"Is in the store now"}, {"Sounds good!"} } } ,
        new Chat { Id = 2, Updated = new DateTime (2024, 3, 1, 4, 20, 40), Requester = (Match)Matches.Where(m => m.Id == 1),
            Provider = (Match)Matches.Where(m => m.Id == 2),
         Messages = new List<String> { { "Im allergic to strawberries"}, {"I will keep in mind"}, {"Thank you, very much!"} } }
    };

        public static IEnumerable<Chat> GetChat(int id) => Chats.Where(c => c.Id == id);
        public static IEnumerable<DateTime> GetChatLastUpdate(int id)
        {
            yield return Chats.SingleOrDefault(c => c.Id == id).Updated;
        }
    }
}
