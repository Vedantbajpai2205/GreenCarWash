public interface IAuthService
    {
        Task<ResponseDto> Register(RegisterDto dto);
        Task<ResponseDto> Login(LoginDto dto);
    }
 