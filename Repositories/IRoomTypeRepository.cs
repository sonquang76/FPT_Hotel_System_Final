using BussinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface IRoomTypeRepository
    {
        List<Roomtype> GetRoomtypes();
        void AddRoomType(Roomtype roomType);
        void UpdateRoomType(Roomtype roomType);
        void DeleteRoomType(Roomtype roomType);
    }
}