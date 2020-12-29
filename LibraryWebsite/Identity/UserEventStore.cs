using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LibraryWebsite.Identity
{
    public sealed class UserEventStore : 
        UserStoreBase<
            ApplicationUser?,
            IdentityRole?, 
            string, 
            IdentityUserClaim<string>, 
            IdentityUserRole<string>?,
            IdentityUserLogin<string>, 
            IdentityUserToken<string>, 
            IdentityRoleClaim<string>>
    {
        private readonly UsersRolesMemoryStore _store;

        public UserEventStore(IdentityErrorDescriber describer, UsersRolesMemoryStore store) : base(describer)
        {
            _store = store;
        }

        public override IQueryable<ApplicationUser> Users => _store.Users.AsQueryable();

        public override Task<IdentityResult> CreateAsync(ApplicationUser? user, CancellationToken cancellationToken = new CancellationToken())
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            _store.Users.Add(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> UpdateAsync(ApplicationUser? user, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> DeleteAsync(ApplicationUser? user, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var id = ConvertIdFromString(userId);
            return Task.FromResult(_store.Users.FirstOrDefault(x=>x.Id == id));
        }

        public override Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(_store.Users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName));
        }

        protected override Task<ApplicationUser?> FindUserAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<IdentityUserLogin<string>> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<IdentityUserLogin<string>> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<IList<Claim>> GetClaimsAsync(ApplicationUser? user, CancellationToken cancellationToken = new CancellationToken())
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = _store.UserClaims.Where(uc => uc.UserId.Equals(user.Id)).Select(c => c.ToClaim()).ToList();
            return Task.FromResult<IList<Claim>>(claims);
        }

        public override Task AddClaimsAsync(ApplicationUser? user, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override Task ReplaceClaimAsync(ApplicationUser? user, Claim claim, Claim newClaim,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override Task RemoveClaimsAsync(ApplicationUser? user, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override Task<IList<ApplicationUser?>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override Task<IdentityUserToken<string>> FindTokenAsync(ApplicationUser? user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task AddUserTokenAsync(IdentityUserToken<string> token)
        {
            throw new NotImplementedException();
        }

        protected override Task RemoveUserTokenAsync(IdentityUserToken<string> token)
        {
            throw new NotImplementedException();
        }

        public override Task AddLoginAsync(ApplicationUser? user, UserLoginInfo login,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override Task RemoveLoginAsync(ApplicationUser? user, string loginProvider, string providerKey,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser? user, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> IsInRoleAsync(ApplicationUser? user, string normalizedRoleName,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }
            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var userRole = await FindUserRoleAsync(user.Id, role.Id, cancellationToken);
                return userRole != null;
            }
            return false;
        }

        protected override Task<IdentityRole?> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_store.Roles.SingleOrDefault(r => r.NormalizedName == normalizedRoleName));
        }

        protected override Task<IdentityUserRole<string>?> FindUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_store.UserRoles.FirstOrDefault(x => x.UserId == userId && x.RoleId == roleId));
        }

        public override async Task<IList<ApplicationUser?>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);

            if (role != null)
            {
                var query = from userrole in _store.UserRoles
                    join user in Users on userrole.UserId equals user.Id
                    where userrole.RoleId.Equals(role.Id)
                    select user;

                return query.Cast<ApplicationUser?>().ToList();
            }
            
            return new List<ApplicationUser?>();
        }

        public override async Task AddToRoleAsync(ApplicationUser? user, string normalizedRoleName,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }
            var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (roleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role {0} not found", normalizedRoleName));
            }
            _store.UserRoles.Add(CreateUserRole(user, roleEntity)!);
        }

        public override Task RemoveFromRoleAsync(ApplicationUser? user, string normalizedRoleName,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override Task<IList<string>> GetRolesAsync(ApplicationUser? user, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var userId = user.Id;
            var query = from userRole in _store.UserRoles
                join role in _store.Roles on userRole.RoleId equals role.Id
                where userRole.UserId.Equals(userId)
                select role.Name;

            var roles = query.ToList();
            return Task.FromResult<IList<string>>(roles);
        }
    }
}
