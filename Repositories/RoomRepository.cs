using BussinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class RoomRepository : IRoomRepository
    {
        public Room CreateRoom(Room addRoom)
        {
            return RoomDao.CreateRoom(addRoom);
        }

        public bool DeleteRoom(int roomId)
        {
            return RoomDao.DeleteRoom(roomId);
        }

        public List<Room> FilterDateToChoose(DateTime startDate, DateTime endDate)
        {
            return RoomDao.FilterDateToChoose(startDate, endDate);
        }

        public List<Room> FilterRooms(int? floor, string status)
        {
            return RoomDao.FilterRooms(floor, status);
        }

        public List<Room> FilterRoomsbyFloor(int? floor)
        {
            return RoomDao.FilterRoomsbyFloor(floor);
        }

        public List<Room> FilterRoomsbyStatus(string status)
        {
            return RoomDao.FilterRoomsbyStatus(status);
        }

        public List<Room> GetRoomAvailiable()
        {
            return RoomDao.GetRoomAvailiable();
        }

        public List<Room> GetRooOoccupancyReport()
        {
            return RoomDao.GetRooOoccupancyReport();
        }

        public List<Room> SearchByRoomNumber(string roomNumber)
        {
            return RoomDao.SearchByRoomNumber(roomNumber);
        }

        public Room UpdateRoom(Room room)
        {
            return RoomDao.UpdateRoom(room);
        }
    }
}
