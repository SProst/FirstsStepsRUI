﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstsStepsRUI.Models;

namespace FirstsStepsRUI.Repositories
{
    public class StubUserRepository : IUserRepository
    {
        private readonly List<User> _users;
        private readonly List<Credential> _credentials;

        public StubUserRepository()
        {
            _users = new List<User>
            {
                new User(1, "Admin", false, UserGroup.Admin),
                new User(2, "Generic", true, UserGroup.Worker)
            };

            _credentials = new List<Credential>
            {
                new Credential(_users.First(e => e.Id == 1), "1234"),
                new Credential(_users.First(e => e.Id == 2), "abcd")
            };
        }

        public async Task<User> Login(string userName, string unsecurePassword)
        {
            User result;
            if (userName.IsInvalid())
                throw new ArgumentException("userName");
            if (unsecurePassword.IsInvalid())
                throw new ArgumentException("unsecurePassword");
            // TODO real call to DB
            result = _users.Where(e => e.Code == userName)
                    .Join(_credentials.Where(e => e.Password == unsecurePassword), user => user.Id,credential => credential.User.Id,(user, credential) => user)
                    .FirstOrDefault();
            if (result == null)
                throw new Exception("Not found.");

            return await Task.FromResult(result);
        }

        public async Task<IList<Menu>> GetMenuByUser(User user)
        {
            List<Menu> result;
            if (user == null)
                return new List<Menu> {new Menu(MenuOption.Login)};
            // TODO real call to DB
            result = user.Group == UserGroup.Admin
                ? new List<Menu>
                {
                    new Menu(MenuOption.Login),
                    new Menu(MenuOption.User),
                    new Menu(MenuOption.Placeholder)
                }
                : new List<Menu>
                {
                    new Menu(MenuOption.User),
                    new Menu(MenuOption.Placeholder)
                };

            return await Task.FromResult(result);
        }

        public async Task<bool> Submit(User user)
        {
            bool submitted;
            if (user.Blocked)
                throw new Exception("User blocked.");
            var userDb = _users.FirstOrDefault(e => e.Id == user.Id);
            if (userDb != null)
            {
                userDb.Group = user.Group;
                submitted = true;
            }
            else
                submitted = false;

            return submitted;
        }
    }
}