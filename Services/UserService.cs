using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using WisVestAPI.Constants;
using WisVestAPI.Models;
 
namespace WisVestAPI.Services
{
    public class UserService
    {
        private readonly string _filePath;
        private readonly PasswordHasher<User> _passwordHasher;
 
            public UserService(IConfiguration configuration)
    {
        _filePath = configuration["UserFilePath"]
            ?? throw new InvalidOperationException(ResponseMessages.UserFilePathNotConfigured);
        _passwordHasher = new PasswordHasher<User>();
    }

    public List<User> GetAllUsers()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new List<User>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException(string.Format(ResponseMessages.FileAccessDenied, _filePath), ex);
        }
        catch (IOException ex)
        {
            throw new IOException(string.Format(ResponseMessages.FileReadError, _filePath), ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException(string.Format(ResponseMessages.InvalidJson, _filePath), ex);
        }
    }

    public void SaveAllUsers(List<User> users)
    {
        try
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException(string.Format(ResponseMessages.FileAccessDenied, _filePath), ex);
        }
        catch (IOException ex)
        {
            throw new IOException(string.Format(ResponseMessages.FileWriteError, _filePath), ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException(ResponseMessages.JsonSerializationError, ex);
        }
    }

    public bool UserExists(string email)
    {
        try
        {
            var users = GetAllUsers();
            return users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(string.Format(ResponseMessages.UserExistsCheckFailed, email), ex);
        }
    }

    public User? GetUserByEmail(string email)
    {
        try
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(string.Format(ResponseMessages.UserRetrievalFailed, email), ex);
        }
    }

    public void AddUser(User user)
    {
        try
        {
            var users = GetAllUsers();
            user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            SaveAllUsers(users);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(string.Format(ResponseMessages.UserAddFailed, user.Email), ex);
        }
    }

    public void UpdateUser(User user)
    {
        try
        {
            var users = GetAllUsers();
            var existingUser = users.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser == null)
            {
                throw new KeyNotFoundException(string.Format(ResponseMessages.UserNotFound, user.Id));
            }

            existingUser.PasswordHash = user.PasswordHash;
            SaveAllUsers(users);
            Console.WriteLine("User updated successfully.");
        }
        catch (KeyNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(string.Format(ResponseMessages.UserUpdateFailed, user.Id), ex);
        }
    }

    public bool ValidateUserLogin(string email, string password)
    {
        try
        {
            var user = GetUserByEmail(email);

            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                return result == PasswordVerificationResult.Success;
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(string.Format(ResponseMessages.UserValidationFailed, email), ex);
        }
    }
}

}