using BussinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class RoomDao
    {
        public RoomDao() { }

        //Báo cáo tình trạng phòng
        public static List<Room> GetRooOoccupancyReport()
        {
            using (ManagementHotelNewContext context = new ManagementHotelNewContext())
            {
                var rooms = context.Rooms
                    .Include(r => r.Bookings)
                    .Include(r => r.RoomType)
                    .ToList();

                var today = DateTime.Now;

                foreach (var room in rooms)
                {
                    // Nếu phòng đang bảo trì thì giữ nguyên, không tính toán lại
                    //if (room.Status == "Maintenance")
                    //    continue;
                    bool isCurrentlyOccupied = room.Bookings.Any(b =>
                                               b.BookingStatus == "Booked" &&
                                               today >= b.ExpectedCheckIn &&
                                               today < b.ExpectedCheckOut);


                    if (isCurrentlyOccupied)
                    {
                        room.Status = "Reserved";
                    }
                    else
                    {
                        room.Status = "Available";
                    }
                }
                return rooms;
            }
        }
        public static List<Room> GetRoomAvailiable()
        {
            using (ManagementHotelNewContext context = new ManagementHotelNewContext())
            {
                return context.Rooms
                    .Include(r => r.RoomType)
                    .Include(r => r.Bookings)
                    .Where(r => r.Status == "Available")
                    .ToList();
            }
        }
        //CRUD
        //C
        public static Room CreateRoom(Room addRoom)
        {
            using (var context = new ManagementHotelNewContext())
            {
                Room room = new Room()
                {
                    RoomNumber = addRoom.RoomNumber,
                    Status = addRoom.Status,
                    RoomTypeId = addRoom.RoomTypeId,
                    Floor = addRoom.Floor
                };
                context.Rooms.Add(room);
                context.SaveChanges();
                return room;
            }
        }
        //R
        public static List<Room> SearchByRoomNumber(string roomNumber)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var room = context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => r.RoomNumber.Contains(roomNumber))
                    .ToList();
                return room;
            }
        }
        //U
        public static Room UpdateRoom(Room Udroom)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var room = context.Rooms.Find(Udroom.RoomId);
                if (room == null) return null;
                bool exists = context.Rooms.Any(r =>
            r.RoomNumber == Udroom.RoomNumber &&
            r.RoomId != Udroom.RoomId);

                if (exists)
                {
                    throw new Exception("Room number already exists.");
                }

                room.RoomNumber = Udroom.RoomNumber;
                room.Status = Udroom.Status;
                room.RoomTypeId = Udroom.RoomTypeId;
                room.Floor = Udroom.Floor;

                context.SaveChanges();
                return room;
            }
        }
        //D
        public static bool DeleteRoom(int roomId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var room = context.Rooms.Find(roomId);
                if (room == null) return false;

                context.Rooms.Remove(room);

                return context.SaveChanges() > 0;
            }
        }
        public static List<Room> FilterRooms(int? floor, string status)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => (!floor.HasValue || r.Floor == floor) &&
                                (string.IsNullOrEmpty(status) || r.Status == status))
                    .ToList();
            }
        }
        public static List<Room> FilterDateToChoose(DateTime startDate, DateTime endDate)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var bookedRoomIds = context.Bookings
                                    .Where(b => b.ExpectedCheckIn < endDate
                                           && b.ExpectedCheckOut > startDate
                                           && b.BookingStatus != "Cancelled"
                                           && b.BookingStatus != "CheckedOut")
                                    .Select(b => b.RoomId)
                                    .Distinct()
                                    .ToList();

                var availableRooms = context.Rooms
                                     .Include(r => r.RoomType)
                                     .Where(r => !bookedRoomIds.Contains(r.RoomId)
                                                 && r.Status != "Maintenance")
                                     .ToList();

                foreach (var room in availableRooms)
                {
                    room.Status = "Available";
                }

                return availableRooms;
            }
        }
        public static List<Room> FilterRoomsbyFloor(int? floor)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => (!floor.HasValue || r.Floor == floor))
                    .ToList();
            }
        }

        public static List<Room> FilterRoomsbyStatus(string status)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => (string.IsNullOrEmpty(status) || r.Status == status))
                    .ToList();
            }
        }

    }
}
