using IRecharge.Core.Application.BillingServices.BillDto;
using IRecharge.Core.Utilities;
using IRecharge.Domain;

namespace IRecharge.Core.Application.Interface
{
    public interface IBillingService
    {
        Task<ApiResponse<Bill>> CreateBill(CreateBillRequest request);
    }
}