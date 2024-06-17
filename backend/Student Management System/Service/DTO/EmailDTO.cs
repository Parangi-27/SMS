namespace Service.DTO
{
    public class EmailDTO
    {
        public List<string> Email { get; set; }
        public List<string>? CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
