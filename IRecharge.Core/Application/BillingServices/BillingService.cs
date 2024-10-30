using IRecharge.Core.Application.BillingServices.BillDto;
using IRecharge.Core.Application.Interface;
using IRecharge.Core.Utilities;
using IRecharge.Domain;
using IRecharge.Infrastructure;
using IRecharge.Infrastructure.Data;
using Microsoft.Extensions.Logging;


namespace IRecharge.Core.Application.BillingServices
{
    public class BillingService : IBillingService
    {

        private readonly ServiceBusHelper _serviceBusHelper;
        private readonly ILogger<BillingService> _logger;
        private readonly AppDBContext _appDBContext;

        public BillingService(ServiceBusHelper serviceBusHelper, ILogger<BillingService> logger, AppDBContext appDBContext)
        {
            _serviceBusHelper = serviceBusHelper;
            _logger = logger;
            _appDBContext = appDBContext;
        }

        public async Task<ApiResponse<Bill>> CreateBill(CreateBillRequest request)
        {
            ApiResponse<Bill> response = new ApiResponse<Bill>();
            try
            {
                // Validate amount
                if(request.Amount <= 0)
                {
                    _logger.LogInformation($"Invalid amount");
                    return response.Fail("Invalid amount");
                }

                Bill bill = new Bill()
                {
                    Status = "Pending",
                    Amount = request.Amount,
                    UserId = request.UserId,
                };

                await _appDBContext.Bills.AddAsync(bill);
                int saveChanges = await _appDBContext.SaveChangesAsync();

                if (saveChanges <= 0)
                    return response.Fail($"Unable to create bill");

                // Publish "bill-created" event
                var billCreatedEvent = new BillCreatedEvent { BillId = bill.Id, Amount = bill.Amount, UserId = bill.UserId };
                await _serviceBusHelper.PublishEventAsync("bill-created", billCreatedEvent);

                return response.Success("success", bill);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateBill {ex.Message}", ex);
                throw;
            }
          
        }

    }

}
