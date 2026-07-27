using BussinessObjects;

namespace DataAccessLayer
{
    public class RoomTypeDao
    {
        public RoomTypeDao() { }
        public static List<Roomtype> GetRoomtypes()
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Roomtypes.ToList();
            }
        }
        // Thêm mới loại phòng
        public static void AddRoomType(Roomtype roomType)
        {
            using (var context = new ManagementHotelNewContext())
            {
                context.Roomtypes.Add(roomType);
                context.SaveChanges();
            }
        }

        // Cập nhật loại phòng
        public static void UpdateRoomType(Roomtype roomType)
        {
            using (var context = new ManagementHotelNewContext())
            {
                context.Roomtypes.Update(roomType);
                context.SaveChanges();
            }
        }

        // Xóa loại phòng
        public static void DeleteRoomType(Roomtype roomType)
        {
            using (var context = new ManagementHotelNewContext())
            {
                context.Roomtypes.Remove(roomType);
                context.SaveChanges();
            }
        }
    }
}
