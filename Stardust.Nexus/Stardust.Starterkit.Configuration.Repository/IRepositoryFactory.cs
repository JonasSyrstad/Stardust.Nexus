namespace Stardust.Nexus.Repository
{
    public interface IRepositoryFactory
    {
        ConfigurationContext GetRepository();
    }
}