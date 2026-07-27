using BussinessObjects;

namespace Services
{
    public interface IRoleService
    {
        List<Role> GetRoles();
        Role FindRoleById(string id);
    }
}
