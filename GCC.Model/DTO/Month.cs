namespace GlobalCalendar.MVC.DTO
{
    public class Month
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Month(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
