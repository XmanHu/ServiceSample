
namespace CalcService
{
    using System.ServiceModel;

    [ServiceContract]
    public interface ICalcService
    {
        [OperationContract]
        double Add(double a, double b);

    }
}
