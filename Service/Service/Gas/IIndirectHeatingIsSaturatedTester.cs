
namespace Service.Gas
{
    public interface IIndirectHeatingIsSaturatedTester
    {
        Task<bool> GasHeatingInletAndOutletTempsAreSimilarAndHotAsync();
    }
}