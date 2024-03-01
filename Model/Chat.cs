namespace PeopleHelpPeople.Model
{
    public class Chat
    {
        public int Id { get; set; }
        public DateTime Updated { get; set; }
        public Match Requester { get; set; }
        public Match Provider { get; set; }
        public List<string> Messages { get; set; }
    }
}
