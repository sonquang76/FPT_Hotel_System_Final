using BussinessObjects;

namespace Services
{
    public interface IRoomTypeService
    {
        List<Roomtype> GetRoomtypes();
        void AddRoomType(Roomtype roomType);
        void UpdateRoomType(Roomtype roomType);
        void DeleteRoomType(Roomtype roomType);
    }
}
