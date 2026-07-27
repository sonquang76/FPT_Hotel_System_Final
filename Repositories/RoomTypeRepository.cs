using BussinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        public RoomTypeRepository() { }
        public List<Roomtype> GetRoomtypes()
        {
            return RoomTypeDao.GetRoomtypes();
        }
        public void AddRoomType(Roomtype roomType) => RoomTypeDao.AddRoomType(roomType);

        public void UpdateRoomType(Roomtype roomType) => RoomTypeDao.UpdateRoomType(roomType);

        public void DeleteRoomType(Roomtype roomType) => RoomTypeDao.DeleteRoomType(roomType);
    }
}
