using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Factoriada.Models;

namespace Factoriada.Services.Interfaces
{
    public interface IApartmentService
    {
        void TestAddress(Address apartmentAddress);
        Task SaveApartment(ApartmentDetail currentApartment);
        Task JoinApartment(User connectedUser, string result);
        Task<string> GetApartmentAddressByUser(Guid userUserId);
        Task<Guid> GetApartmentIdByUser(Guid userUserId);
        Task<List<Rule>> GetRulesByApartmentId(Guid apartmentId);
        Task AddRuleToApartment(Rule rule, Guid apartmentId);
        Task EditRuleFromApartment(Rule currentRule);
        Task DeleteRule(Rule currentRule);
        Task<List<Announce>> GetAnnouncesByApartmentId(Guid apartmentId);
        Task DeleteAnnounce(Announce currentAnnounce);
        Task EditAnnounceFromApartment(Announce currentAnnounce);
        Task AddAnnounceToApartment(Announce announce, Guid apartmentId);
        Task<ApartmentDetail> GetApartmentByUser(Guid userId);
        Task<List<BudgetHistory>> GetBudgetHistoryByApartmentId(Guid apartmentDetailId);
        Task AddMoneyToApartment(BudgetHistory money);
        Task UpdateApartment(ApartmentDetail currentApartment);
        Task<List<Chat>> GetChatByApartmentId(Guid currentApartment);
        Task SendMessage(Chat chat);
        Task<List<Reminder>> GetRemindersByApartmentId(Guid apartmentId);
        Task AddBill(Bill bill);
        Task<List<Bill>> GetBillsByApartment(Guid apartmentDetailApartmentDetailId);
        Task DeleteBill(Bill selectedBill);
        Task<List<BillPaidPersons>> GenerateBillPaidPersons(Bill selectedBill, Guid apartmentId);
        Task<List<TimeAway>> GetTimeAwayByUser(Guid userId);
        Task AddOrUpdateTimeAway(TimeAway timeAway);
        Task DeleteTimeAway(TimeAway timeAway);
        Task<List<User>> GetUsersByApartment(Guid apartmentId);
        Task RemoveUserFromApartment(User selectedUser);
        Task<User> GetUserByEmail(string email);
        Task DeleteApartment(ApartmentDetail apartment);
        Task<List<BuyList>> GetBuyListFromApartment(Guid apartmentId);
        Task AddOrUpdateProductToBuy(BuyList toBuy);
        Task DeleteToBuy(BuyList selectedProduct);
    }
}
