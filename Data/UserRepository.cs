using DotnetApi.Models;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {  
        DataContextEF _entityFramework;
        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        // public bool AddEntity<T>(T entityToAdd)
        {
            if(entityToAdd != null){
            _entityFramework.Add(entityToAdd);
            // return true;
            }
            // return false;
        }
        public void RemoveEntity<T>(T entityToRemove)
        // public bool AddEntity<T>(T entityToAdd)
        {
            if(entityToRemove != null){
            _entityFramework.Remove(entityToRemove);
            // return true;
            }
            // return false;
        }

        public IEnumerable<User> GetUsersEF()
        {
                IEnumerable<User> users = _entityFramework.Users.ToList<User>();
                return users;
        }

        public User GetSingleUserEF(int userId)
        {
                User? user = _entityFramework.Users
                    .Where(u => u.UserId == userId)
                    .FirstOrDefault<User>();
                if(user != null)
                {
                    return user;
                }

                throw new Exception("Failed to Get User");
        
        }
        public UserSalary GetSingleUserSalaryEF(int userId)
        {
                UserSalary? userSalary = _entityFramework.UserSalary
                    .Where(u => u.UserId == userId)
                    .FirstOrDefault<UserSalary>();
                if(userSalary != null)
                {
                    return userSalary;
                }

                throw new Exception("Failed to Get User");
        
        }
        public UserJobInfo GetSingleUserJobInfoEF(int userId)
        {
                UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
                    .Where(u => u.UserId == userId)
                    .FirstOrDefault<UserJobInfo>();
                if(userJobInfo != null)
                {
                    return userJobInfo;
                }

                throw new Exception("Failed to Get User");
        
        }
    }
}