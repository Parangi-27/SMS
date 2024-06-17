namespace Service.DTO
{
    public class ValidationDTO
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public bool IsValid => Errors == null || Errors.Count == 0;
        public ValidationDTO()
        {
            Errors = new List<string>();
        }
    }
}
