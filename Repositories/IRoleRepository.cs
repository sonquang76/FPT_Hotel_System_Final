using BussinessObjects;

namespace Repositories
{
    public interface IRoleRepository
    {
        List<Role> GetRoles();
        Role FindRoleById(string id);

    }
}
