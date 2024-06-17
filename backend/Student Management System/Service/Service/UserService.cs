using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repository.Interface;
using Service.Interface;

namespace Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserService(IUserRepo userRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            
        }
        public int GetLoggedInUserId()
        {
            var loggedInUserId = 0;
            var loggedInUserClaim = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == "Id");

            if (loggedInUserClaim != null && int.TryParse(loggedInUserClaim.Value, out int result))
            {
                loggedInUserId = result;
            }

            return loggedInUserId;
        }
    }

}
