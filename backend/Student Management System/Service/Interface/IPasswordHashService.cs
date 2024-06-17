namespace Service.Interface
{
    public interface IPasswordHashService
    {
        public string GeneratePasswordHash(string password);
    }
}
