using FundooNotes.Models.DTOs;

namespace FundooNotes.BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDTO> RegisterUser(UserRegisterDTO dto);

        Task<ResponseDTO> LoginUser(UserLoginDTO dto);

        Task<ResponseDTO> ForgotPassword(string email);

        Task<ResponseDTO> ResetPassword(ResetPasswordDTO dto);
    }
}
