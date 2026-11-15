namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Output port of the Rent Vehicle use case.
    /// </summary>
    public interface IRentVehicleOutputPort : IOutputPortStandard<RentVehicleOutput>, IOutputPortNotFound
    {
    }
}
