using MyLife.Core.API;

namespace MyLife.Core.Define
{
    public interface IFilePolicyProvider
    {
        FileSecurityPolicy GetPolicy();
    }
}