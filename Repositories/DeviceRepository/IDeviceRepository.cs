using WebApplication3.Models;

namespace WebApplication3.Repositories.DeviceRepository
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetAll();
        Task<Device?> GetById(int id);
        Task<int> Create(Device device);
        Task<int> Update(Device device);
        Task<int> Delete(int id);
    }
}
