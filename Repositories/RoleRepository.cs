using BussinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public RoleRepository() { }
        public Role FindRoleById(string id)
        {
            return RoleDao.FindRoleById(id);
        }

        public List<Role> GetRoles()
        {
            return RoleDao.GetRoles();
        }
    }
}
