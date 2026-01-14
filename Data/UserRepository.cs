using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using UserManagementTestApp.Helpers;
using UserManagementTestApp.Models;

namespace UserManagementTestApp.Data
{
    public class UserRepository
    {
        private const string UsersFileName = "users.json";
        private readonly string _usersFilePath;

        public UserRepository()
        {
            _usersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UsersFileName);
        }

        public List<User> LoadAll()
        {
            if (!File.Exists(_usersFilePath))
            {
                return new List<User>();
            }

            try
            {
                string json = File.ReadAllText(_usersFilePath);
                var serializer = new JavaScriptSerializer();
                var users = serializer.Deserialize<List<User>>(json);

                if (users != null)
                {
                    foreach (var user in users)
                    {
                        user.Id = CryptoHelper.Decrypt(user.Id);
                        user.Password = CryptoHelper.Decrypt(user.Password);
                    }
                }

                return users ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        public void SaveAll(List<User> users)
        {
            try
            {
                var usersToSave = new List<User>();

                foreach (var u in users)
                {
                    usersToSave.Add(new User
                    {
                        Id = CryptoHelper.Encrypt(u.Id),
                        Name = u.Name,
                        EmployeeNumber = u.EmployeeNumber,
                        Department = u.Department,
                        Password = CryptoHelper.Encrypt(u.Password) 
                    });
                }

                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(usersToSave);
                File.WriteAllText(_usersFilePath, json);
            }
            catch (IOException ex)
            {
                throw new Exception("파일에 접근할 수 없습니다.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("파일 저장 권한이 없습니다.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("저장 중 오류 발생.", ex);
            }
        }
    }
}
