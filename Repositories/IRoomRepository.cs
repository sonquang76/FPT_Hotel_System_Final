using BussinessObjects;

namespace Repositories
{
    public interface IRoomRepository
    {
        List<Room> GetRooOoccupancyReport();

        Room CreateRoom(Room addRoom);

        List<Room> SearchByRoomNumber(string roomNumber);

        Room UpdateRoom(Room room);

        bool DeleteRoom(int roomId);

        List<Room> FilterRooms(int? floor, string status);
        List<Room> GetRoomAvailiable();
        List<Room> FilterDateToChoose(DateTime startDate, DateTime endDate);
        List<Room> FilterRoomsbyFloor(int? floor);
        List<Room> FilterRoomsbyStatus(string status);
    }
}
