namespace TaskerBC.Models
{
    public class TaskerItem
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public bool IsComplete { get; set; }
    }
}
