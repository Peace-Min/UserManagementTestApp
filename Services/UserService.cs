using System;
using System.Collections.Generic;
using System.Linq;
using UserManagementTestApp.Data;
using UserManagementTestApp.Models;

namespace UserManagementTestApp.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;

        public UserService()
        {
            _repository = new UserRepository();
        }

        public List<User> GetUsers()
        {
            return _repository.LoadAll();
        }

        public User GetUserById(string id)
        {
            return GetUsers().FirstOrDefault(u => u.Id == id);
        }

        public bool Exists(string id)
        {
            return GetUsers().Any(u => u.Id == id);
        }

        public void AddUser(User newUser)
        {
            var users = GetUsers();
            if (users.Any(u => u.Id == newUser.Id))
            {
                throw new Exception("이미 존재하는 ID입니다.");
            }

            users.Add(newUser);
            _repository.SaveAll(users);
        }

        public void UpdateUser(User updatedUser)
        {
            var users = GetUsers();
            var existingUser = users.FirstOrDefault(u => u.Id == updatedUser.Id);

            if (existingUser != null)
            {
                existingUser.Name = updatedUser.Name;
                existingUser.EmployeeNumber = updatedUser.EmployeeNumber;
                existingUser.Department = updatedUser.Department;
                existingUser.Password = updatedUser.Password;

                _repository.SaveAll(users);
            }
        }

        public void DeleteUser(string id)
        {
            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                users.Remove(user);
                _repository.SaveAll(users);
            }
        }
    }
}
