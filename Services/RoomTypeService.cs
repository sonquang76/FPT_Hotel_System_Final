using BussinessObjects;
using Repositories;

namespace Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository repository;
        public RoomTypeService() { this.repository = new RoomTypeRepository(); }

        public List<Roomtype> GetRoomtypes()
        {
            return this.repository.GetRoomtypes();
        }
        public void AddRoomType(Roomtype roomType) => this.repository.AddRoomType(roomType);

        public void UpdateRoomType(Roomtype roomType) => this.repository.UpdateRoomType(roomType);

        public void DeleteRoomType(Roomtype roomType) => this.repository.DeleteRoomType(roomType);
    }
}
