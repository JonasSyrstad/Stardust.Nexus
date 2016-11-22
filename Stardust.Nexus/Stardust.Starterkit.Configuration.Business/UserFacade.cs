using System.Collections.Generic;
using System.Linq;
using Stardust.Core.Security;
using Stardust.Nexus.Business.CahceManagement;
using Stardust.Nexus.Repository;
using Stardust.Particles;
using Stardust.Particles.Validator;

namespace Stardust.Nexus.Business
{
    public class UserFacade : IUserFacade
    {
        private readonly ICacheManagementService cacheManagement;

        private ConfigurationContext Repository;

        public UserFacade(IRepositoryFactory repository, ICacheManagementService cacheManagement)
        {
            this.cacheManagement = cacheManagement;
            Repository = repository.GetRepository();
        }

        public bool IsUserInRole(string username, string roleName)
        {
            return (from u in Repository.ConfigUsers
                    where u.NameId.ToLower().Equals(username.ToLower()) && u.AdministratorType == roleName.ParseAsEnum<AdministratorTypes>()
                    select u).Count() == 1;
        }

        public string[] GetUserRoles(string username)
        {
            var data = (from u in Repository.ConfigUsers where u.NameId.ToLower().Equals(username.ToLower()) select u).ToArray();
            return (from r in data select r.AdministratorType.ToString()).ToArray();
        }

        public string[] GetUsersInRole(string roleName)
        {
            return
                (from u in Repository.ConfigUsers
                 where u.AdministratorType == roleName.ParseAsEnum<AdministratorTypes>()
                 select u.NameId).ToArray();
        }

        public IEnumerable<IConfigUser> GetUsers()
        {
            return Repository.ConfigUsers.ToList();
        }

        public void CreateUser(IConfigUser newUser)
        {
            var user = Repository.ConfigUsers.Create();
            user.FirstName = newUser.FirstName;
            user.LastName = newUser.LastName;
            if (newUser.NameId.IsEmail()) user.Email = newUser.NameId;
            else
                user.Email = newUser.Email;
            user.NameId = newUser.NameId;
            user.AdministratorType = newUser.AdministratorType;
            user.SetAccessToken(UniqueIdGenerator.CreateNewId(20).Encrypt(KeySalt));
            Repository.SaveChanges();
        }

        protected static EncryptionKeyContainer KeySalt
        {
            get
            {
                return new EncryptionKeyContainer("makeItHarderTowrite");
            }
        }

        public void UpdateUser(ConfigUser model)
        {
            var user = GetUser(model.NameId);
            user.FirstName = model.FirstName;
            if (model.NameId.IsEmail()) user.Email = model.NameId;
            else
                user.Email = model.Email;
            user.LastName = model.LastName;
            if (!user.NameId.ToLower().Equals(ConfigReaderFactory.CurrentUser.NameId.ToLower()))
                user.AdministratorType = model.AdministratorType;
            if (user.AccessToken.IsNullOrWhiteSpace())
                user.SetAccessToken(UniqueIdGenerator.CreateNewId(20).Encrypt(KeySalt));
            cacheManagement.NotifyUserChange(model.NameId.ToLower());
            Repository.SaveChanges();
        }

        public IConfigUser GetUser(string id)
        {
            if (id.IsNullOrWhiteSpace()) return null;
            IConfigUser user;
            if (Repository.ConfigUsers.Count() == 0)
            {
                user = Repository.ConfigUsers.Create();
                user.FirstName = "";
                user.LastName = "";
                user.NameId = id;
                user.AdministratorType = AdministratorTypes.SystemAdmin;
                user.SetAccessToken(UniqueIdGenerator.CreateNewId(20).Encrypt(KeySalt));
                Repository.SaveChanges();
            }
            user = (from u in Repository.ConfigUsers
                    where u.NameId.ToLower().Equals(id.ToLower())
                    select u).Single();
            if (user.AccessToken.IsNullOrWhiteSpace())
            {
                user.SetAccessToken(UniqueIdGenerator.CreateNewId(20).Encrypt(KeySalt));
                cacheManagement.NotifyUserChange(id.ToLower());
                Repository.SaveChanges();
            }
            return user;
        }

        public void GenerateAccessToken(string id)
        {
            var user = GetUser(id);
            user.SetAccessToken(UniqueIdGenerator.CreateNewId(20).Encrypt(KeySalt));
            Repository.SaveChanges();
            cacheManagement.NotifyUserChange(id.ToLower());
        }

        public void DeleteUser(IConfigUser user)
        {
            var id = user.NameId;
            Repository.DeleteObject(user);
            Repository.SaveChanges();
            cacheManagement.NotifyUserChange(id.ToLower());
        }

        public void SendNotifications(ICollection<IConfigUser> administrators, List<IConfigUser> usersToRemove)
        {
            usersToRemove.AddRange(administrators);
            foreach (var user in usersToRemove)
            {
                cacheManagement.NotifyUserChange(user.Id);
            }
        }
    }
}