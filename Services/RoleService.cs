using BussinessObjects;
using Repositories;

namespace Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository repository;
        public RoleService() { this.repository = new RoleRepository(); }
        public Role FindRoleById(string id)
        {
            return this.repository.FindRoleById(id);
        }

        public List<Role> GetRoles()
        {
            return this.repository.GetRoles();
        }
    }
}
