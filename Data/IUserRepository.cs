using DotnetApi.Models;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToRemove);

        public IEnumerable<User> GetUsersEF();

        public User GetSingleUserEF(int userId);
        public UserSalary GetSingleUserSalaryEF(int userId);
        public UserJobInfo GetSingleUserJobInfoEF(int userId);
    }
}