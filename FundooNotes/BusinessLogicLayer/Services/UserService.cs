using FundooNotes.BusinessLogicLayer.Interfaces;
using FundooNotes.DataLogicLayer.Interfaces;
using FundooNotes.Models.DTOs;
using FundooNotes.Models.Entities;
using MailKit.Net.Smtp;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FundooNotes.BusinessLogicLayer.Services
{
    public class UserService(IUserRepository userRepository, IConfiguration _configuration) : IUserService
    {
        // User Login Service
        public async Task<ResponseDTO> LoginUser(UserLoginDTO dto)
        {
            var user = await userRepository.GetUserByEmail(dto.Email);

            if (user == null)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "Email not found",
                    Data = null
                };
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.HashPassword);

            if (!isValid)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "Invalid password",
                    Data = null
                };
            }

            var token = GenerateJwtToken(user);

            return new ResponseDTO
            {
                Success = true,
                Message = "Login successful",
                Data = new { Token = token }
            };
        }

        // Token Generation Logic
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // User Register Service
        public async Task<ResponseDTO> RegisterUser(UserRegisterDTO dto)
        {
            // Step 1 - Check if email already exists
            var exists = await userRepository.GetUserByEmail(dto.Email);

            if (exists != null)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "Email already exists",
                    Data = null
                };
            }

            // Step 2 - Check passwords match
            if (dto.Password != dto.ConfirmPassword)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "Passwords do not match",
                    Data = null
                };
            }

            // Step 3 - Create and save user
            await userRepository.RegisterUser(new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            });

            // Step 4 - Return success response
            return new ResponseDTO
            {
                Success = true,
                Message = "User registered successfully",
                Data = null
            };
        }

        public async Task<ResponseDTO> ForgotPassword(string email)
        {
            var exists = await userRepository.GetUserByEmail(email);

            if (exists == null)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "Email not Found",
                    Data = null
                };
            }

            var token = Guid.NewGuid().ToString();

            exists.ResetToken = token;

            exists.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await userRepository.UpdateUser(exists);

            await SendResetEmail(email, token);

            return new ResponseDTO
            {
                Success = true,
                Message = "Password reset email sent",
                Data = null
            };

        }

        private async Task SendResetEmail(string email, string token)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("FundooNotes", _configuration["EmailSettings:Username"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Password Reset Request";
            message.Body = new TextPart("plain")
            {
                Text = $"Reset your password using this token: {token}\n" +
               $"This token expires in 1 hour."
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["EmailSettings:Host"],
                int.Parse(_configuration["EmailSettings:Port"]),
                false);
            await client.AuthenticateAsync(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task<ResponseDTO> ResetPassword(ResetPasswordDTO dto)
        {
            // Step 1 - Find user by token
            var user = await userRepository.GetUserByResetToken(dto.Token);
            if (user == null)
                return new ResponseDTO { Success = false, Message = "Invalid token" };

            // Step 2 - Check token expiry
            if (user.ResetTokenExpiry < DateTime.UtcNow)
                return new ResponseDTO { Success = false, Message = "Token expired" };

            // Step 3 - Check passwords match
            if (dto.NewPassword != dto.ConfirmPassword)
                return new ResponseDTO { Success = false, Message = "Passwords do not match" };

            // Step 4 - Hash new password
            user.HashPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            // Step 5 - Clear token + expiry
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            // Step 6 - Save to DB
            await userRepository.UpdateUser(user);

            return new ResponseDTO { Success = true, Message = "Password reset successful" };
        }
    }
}
