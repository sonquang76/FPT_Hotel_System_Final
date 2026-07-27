using BussinessObjects;

namespace DataAccessLayer
{
    public class RoleDao
    {
        public RoleDao() { }
        public static List<Role> GetRoles()
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Roles.ToList();
            }
        }

        public static Role FindRoleById(string id)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Roles.Find(id);
            }
        }
    }
}
