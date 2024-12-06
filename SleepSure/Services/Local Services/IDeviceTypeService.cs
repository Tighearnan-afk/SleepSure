namespace SleepSure.Services
{
    public interface IDeviceTypeService
    {
        public Task<List<Model.DeviceType>> GetTypesAsync();
    }
}
