using BussinessObjects;
using Repositories;

namespace Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository repository;
        public RoomService()
        {
            this.repository = new RoomRepository();
        }
        public Room CreateRoom(Room addRoom)
        {
            return this.repository.CreateRoom(addRoom);
        }

        public bool DeleteRoom(int roomId)
        {
            return this.repository.DeleteRoom(roomId);
        }

        public List<Room> FilterDateToChoose(DateTime startDate, DateTime endDate)
        {
            return this.repository.FilterDateToChoose(startDate, endDate);
        }

        public List<Room> FilterRooms(int? floor, string status)
        {
            return this.repository.FilterRooms(floor, status);
        }

        public List<Room> FilterRoomsbyFloor(int? floor)
        {
            return this.repository.FilterRoomsbyFloor(floor);  
        }

        public List<Room> FilterRoomsbyStatus(string status)
        {
            return this.repository.FilterRoomsbyStatus(status);
        }

        public List<Room> GetRoomAvailiable()
        {
            return this.repository.GetRoomAvailiable();
        }

        public List<Room> GetRooOoccupancyReport()
        {
            return this.repository.GetRooOoccupancyReport();
        }

        public List<Room> SearchByRoomNumber(string roomNumber)
        {
            return this.repository.SearchByRoomNumber(roomNumber);
        }

        public Room UpdateRoom(Room room)
        {
            return this.repository.UpdateRoom(room);
        }
    }
}
