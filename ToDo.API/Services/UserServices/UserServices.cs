using Microsoft.AspNetCore.Identity;
using ToDo.API.Dtos.UserDtos;
using ToDo.API.Dtos.UserDtos.Login;
using ToDo.API.Mapping;
using ToDo.API.Services.EmailServices;
using ToDo.API.Services.TokenServices;
using ToDo.Data.Entities;
using ToDo.Data.Repositories;

namespace ToDo.API.Services.UserServices
{
    public class UserServices : IUserServices
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserServices> _logger;

        // cooldown window for resending
        private static readonly TimeSpan ResendCooldown = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan CodeExpiry = TimeSpan.FromMinutes(15);

        public UserServices(IGenericRepository<User> userRepository, ITokenService tokenService, IEmailService emailService, ILogger<UserServices> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<CreateUserResponseDto> CreateUser(CreateUserRequestDto dto, string? profileImagePath)
        {
            // check duplicate email
            var exists = await _userRepository.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted);
            if (exists)
                throw new InvalidOperationException("Email already registered.");

            var entity = dto.ToEntity();

            // password hashing
            var hasher = new PasswordHasher<User>();
            entity.Password = hasher.HashPassword(entity, dto.Password);

            // generate numeric code and store hashed version
            var code = TokenHelpers.CreateNumericCode(6); // e.g. "034591"
            entity.EmailVerificationCodeHash = TokenHelpers.Hash(code);
            entity.EmailVerificationCodeExpires = DateTime.UtcNow.Add(CodeExpiry);
            entity.LastVerificationSentAt = DateTime.UtcNow;

            var result = await _userRepository.AddAsync(entity);
            await _userRepository.SaveChangesAsync(); // persist to get Id

            // send code by email; if sending fails, remove created user (roll back)
            var subject = "Confirm your email";
            var body = $"Hi {result.Username},<br/><br/>Your verification code is <b>{code}</b>. It expires in {CodeExpiry.TotalMinutes} minutes.<br/><br/>If you didn't request this, ignore this email.";
            try
            {
                await _emailService.SendEmailAsync(result.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", result.Email);
                // rollback (permanent delete) to avoid orphan unverified account:
                await _userRepository.DeleteAsync(result, softDelete: false);
                await _userRepository.SaveChangesAsync();
                throw new InvalidOperationException("Failed to send confirmation email. Please try again later.");
            }

            return result.ToCreateUserResponseDto();
        }

        public async Task<VerifyEmailResponseDto> VerifyEmailAsync(VerifyEmailRequestDto dto)
        {
            var user = (await _userRepository.GetOneByFilter(u => u.Email == dto.Email, includeProperties: ""))
                       ?? throw new InvalidOperationException("Invalid email or code.");

            if (user.IsEmailConfirmed)
                return new VerifyEmailResponseDto(true, "Email already confirmed.");

            if (user.EmailVerificationCodeExpires == null || user.EmailVerificationCodeExpires < DateTime.UtcNow)
                return new VerifyEmailResponseDto(false, "Verification code expired. Request a new one.");

            if (!TokenHelpers.VerifyHash(dto.Code, user.EmailVerificationCodeHash))
                return new VerifyEmailResponseDto(false, "Invalid verification code.");

            // success
            user.IsEmailConfirmed = true;
            user.EmailVerificationCodeHash = null;
            user.EmailVerificationCodeExpires = null;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return new VerifyEmailResponseDto(true, "Email successfully verified.");
        }

        public async Task<ResendVerificationResponseDto> ResendVerificationAsync(ResendVerificationRequestDto dto)
        {
            var user = (await _userRepository.GetOneByFilter(u => u.Email == dto.Email, includeProperties: ""));


            if (user.IsEmailConfirmed)
                return new ResendVerificationResponseDto(false, "Email already confirmed.");

            // rate limit
            if (user.LastVerificationSentAt != null && user.LastVerificationSentAt > DateTime.UtcNow.Subtract(ResendCooldown))
                return new ResendVerificationResponseDto(false, $"Please wait {ResendCooldown.TotalSeconds} seconds before requesting again.");

            // regenerate code
            var code = TokenHelpers.CreateNumericCode(6);
            user.EmailVerificationCodeHash = TokenHelpers.Hash(code);
            user.EmailVerificationCodeExpires = DateTime.UtcNow.Add(CodeExpiry);
            user.LastVerificationSentAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            var subject = "Your new verification code";
            var body = $"Hi {user.Username},<br/><br/>Your verification code is <b>{code}</b>. It expires in {CodeExpiry.TotalMinutes} minutes.";

            try
            {
                await _emailService.SendEmailAsync(user.Email, subject, body);
                return new ResendVerificationResponseDto(true, "Verification code sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed sending verification email on resend to {Email}", user.Email);
                return new ResendVerificationResponseDto(false, "Failed to send verification email. Try again later.");
            }
        }
    

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            // Find user by email (ensure case normalization if needed)
            var user = await _userRepository.GetOneByFilter(u => u.Email == dto.Email && !u.IsDeleted, includeProperties: "UserRoles,UserRoles.Role");
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials."); // don't reveal which piece failed

            // verify password
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid credentials.");

            // optional: require email confirmation
            if (!user.IsEmailConfirmed)
                throw new UnauthorizedAccessException("Email is not confirmed.");

            // collect roles (from UserRoles relationship)
            var roles = user.UserRoles?.Select(ur => ur.Role?.Name).Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>();

            // create JWT
            var token = _tokenService.CreateToken(user, roles);
            var expiresAt = _tokenService.GetExpiry();

            return new LoginResponseDto(token, expiresAt, user.Id, user.Username);
        }

    }
}