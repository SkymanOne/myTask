namespace myTask.Domain.Models
{
    public class SubTask
    {
        public string Title { get; set; }
        public bool Completed { get; set; } = false;

        public SubTask(string title)
        {
            Title = title;
        }
    }
}