using AutoMapper;
using CareerFlow.Application.Common;
using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using CareerFlow.Domain.Common;
using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CareerFlow.Application.Services;

public class UserService : ServiceBase, IUserService
{
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly JwtSettings _jwtSettings;

    public UserService(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<UserService> logger,
        IJwtTokenGenerator tokenGenerator,
        IOptions<JwtSettings> jwtSettings)
        : base(context, mapper, logger)
    {
        _tokenGenerator = tokenGenerator;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            throw new KeyNotFoundException($"Usuário com email {email} não encontrado.");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        // Verificar se email já existe
        if (await EmailExistsAsync(createUserDto.Email))
            throw new InvalidOperationException($"Email {createUserDto.Email} já está em uso.");

        var user = _mapper.Map<User>(createUserDto);
        user.PasswordHash = BCrypt.HashPassword(createUserDto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário criado com ID: {UserId}", user.Id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

        // Atualizar apenas propriedades fornecidas
        if (!string.IsNullOrEmpty(updateUserDto.Name))
            user.Name = updateUserDto.Name;

        if (!string.IsNullOrEmpty(updateUserDto.Phone))
            user.Phone = updateUserDto.Phone;

        if (!string.IsNullOrEmpty(updateUserDto.City))
            user.City = updateUserDto.City;

        if (!string.IsNullOrEmpty(updateUserDto.State))
            user.State = updateUserDto.State;

        // TODO: Lidar com upload de foto
        // if (updateUserDto.Photo != null)
        // {
        //     user.PhotoPath = await SaveFileAsync(updateUserDto.Photo);
        // }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário atualizado com ID: {UserId}", user.Id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário deletado com ID: {UserId}", id);
    }

    public async Task<AuthResponseDto> AuthenticateAsync(LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email ou senha inválidos.");

        var token = _tokenGenerator.GenerateToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto
        {
            Token = token,
            User = userDto,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes)
        };
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}