using Factoriada.Exceptions;
using Factoriada.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Factoriada.Services
{
    public class ApiDatabaseService
    {
        private static ApiDatabaseService _ServiceClientInstance;
        private readonly FirebaseClient _firebase;
        public static ApiDatabaseService ServiceClientInstance
        {
            get
            {
                if (_ServiceClientInstance == null)
                    _ServiceClientInstance = new ApiDatabaseService();

                return _ServiceClientInstance;
            }
        }

        private ApiDatabaseService()
        {
            _firebase = new FirebaseClient("https://factoriada-5ee06-default-rtdb.firebaseio.com/");

            Initialize();
        }
        private async void Initialize()
        {
            var result = (await _firebase
                .Child(nameof(Role))
                .OnceAsync<Role>())
                .Select(x => x.Object)
                .ToList();

            if (result.Count != 0)
                return;

            var id = Guid.NewGuid();

            await _firebase
               .Child(nameof(Role))
               .Child(id.ToString())
               .PutAsync(
                new Role()
                {
                    RoleId = id,
                    RoleTypeName = "Owner"
                });

            id = Guid.NewGuid();

            await _firebase
              .Child(nameof(Role))
              .Child(id.ToString())
              .PutAsync(
              new Role()
              {
                  RoleTypeName = "Chirias",
                  RoleId = id
              });

            id = Guid.NewGuid();

            await _firebase
              .Child(nameof(Role))
              .Child(id.ToString())
              .PutAsync(
              new Role()
              {
                  RoleTypeName = "Default",
                  RoleId = id
              });
        }

        public async Task<User> Login(string email)
        {
            var result = (await _firebase
                .Child(nameof(User))
                .OnceAsync<User>())
                .FirstOrDefault(x => x.Object.Email == email);

            return result?.Object;
        }

        public async Task Register(User user)
        {
            user.UserId = Guid.NewGuid();

            user.Role = (await _firebase
                .Child(nameof(Role))
                .OnceAsync<Role>())
                .FirstOrDefault(x => x.Object.RoleTypeName == "Default")
                ?.Object;

            await _firebase
                .Child(nameof(User))
                .Child(user.UserId.ToString())
                .PutAsync(user);
        }

        public async Task UpdateUser(User currentUser)
        {
            await _firebase
                .Child(nameof(User))
                .Child(currentUser.UserId.ToString())
                .PutAsync(currentUser);
        }

        public async Task SaveApartment(ApartmentDetail currentApartment)
        {
            await ChangeUserRoleTo(currentApartment.Owner, "Owner");

            currentApartment.Owner = await _firebase
                .Child(nameof(User))
                .Child(currentApartment.Owner.UserId.ToString())
                .OnceSingleAsync<User>();

            await _firebase
                .Child(nameof(ApartmentDetail))
                .Child(currentApartment.ApartmentDetailId.ToString)
                .PutAsync(currentApartment);
        }

        public async Task ChangeUserRoleTo(User user, string role)
        {
            var result = (await _firebase
                .Child(nameof(Role))
                .OnceAsync<Role>()).FirstOrDefault(x
                => x.Object.RoleTypeName == role)
                ?.Object;

            user.Role = result;

            await _firebase.Child(nameof(User))
                .Child(user.UserId.ToString())
                .PutAsync(user);
        }

        public async Task<ApartmentDetail> GetApartmentByToken(string result)
        {
            return (await _firebase
                .Child(nameof(ApartmentDetail))
                .OnceAsync<ApartmentDetail>())
                .FirstOrDefault(x => x.Object.Token == result)
                ?.Object;
        }

        public async Task JoinApartment(Apartment apartment)
        {
            await ChangeUserRoleTo(apartment.User, "Chirias");

            apartment.User = await _firebase
                .Child(nameof(User))
                .Child(apartment.User.UserId.ToString())
                .OnceSingleAsync<User>();

            await _firebase
                .Child(nameof(Apartment))
                .Child(apartment.ApartmentId.ToString())
                .PutAsync(apartment);
        }

        public async Task<ApartmentDetail> GetApartmentDetailByUserId(Guid userUserId)
        {
            var apartmentDetail = (await _firebase
                    .Child(nameof(ApartmentDetail))
                    .OnceAsync<ApartmentDetail>())
                .Select(x => x.Object)
                .FirstOrDefault(x => x.Owner.UserId == userUserId);

            if (apartmentDetail != null)
                return apartmentDetail;

            var apartment = (await _firebase
                    .Child(nameof(Apartment))
                    .OnceAsync<Apartment>())
                .Select(x => x.Object)
                .FirstOrDefault(x => x.User.UserId == userUserId);

            if (apartment != null)
                return await _firebase
                    .Child(nameof(ApartmentDetail))
                    .Child(apartment.ApartmentDetail.ToString())
                    .OnceSingleAsync<ApartmentDetail>();

            return new ApartmentDetail();
        }

        public async Task<List<Rule>> GetRulesByApartment(Guid apartmentId)
        {
            return (await _firebase
                    .Child(nameof(Rule))
                    .OnceAsync<Rule>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail == apartmentId)
                .OrderBy(x => x.InsertedDateTime)
                .ToList();
        }

        public async Task<ApartmentDetail> GetApartmentById(Guid apartmentId)
        {
            return await _firebase
                .Child(nameof(ApartmentDetail))
                .Child(apartmentId.ToString())
                .OnceSingleAsync<ApartmentDetail>();
        }

        public async Task UpdateRule(Rule currentRule)
        {
            await _firebase
                .Child(nameof(Rule))
                .Child(currentRule.RuleId.ToString())
                .PutAsync(currentRule);
        }

        public async Task DeleteRule(Rule currentRule)
        {
            await _firebase
                .Child(nameof(Rule))
                .Child(currentRule.RuleId.ToString())
                .DeleteAsync();
        }

        public async Task<List<Announce>> GetAnnouncesByApartmentId(Guid apartmentId)
        {
            return (await _firebase
                    .Child(nameof(Announce))
                    .OnceAsync<Announce>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetails == apartmentId)
                .OrderByDescending(x => x.InsertedDateTime)
                .ToList();
        }

        public async Task DeleteAnnounce(Announce currentAnnounce)
        {
            await _firebase
                .Child(nameof(Announce))
                .Child(currentAnnounce.AnnounceId.ToString())
                .DeleteAsync();
        }

        public async Task AddOrUpdateAnnounce(Announce currentAnnounce)
        {
            await _firebase
                .Child(nameof(Announce))
                .Child(currentAnnounce.AnnounceId.ToString())
                .PutAsync(currentAnnounce);
        }

        public async Task<List<BudgetHistory>> GetBugetHistoryByApartment(Guid apartmentDetailId)
        {
            return (await _firebase
                    .Child(nameof(BudgetHistory))
                    .OnceAsync<BudgetHistory>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail == apartmentDetailId)
                .OrderByDescending(x => x.InsertedDateTime)
                .ToList();
        }

        public async Task AddMoney(BudgetHistory money)
        {
            await _firebase
                .Child(nameof(BudgetHistory))
                .Child(money.BudgetHistoryId.ToString())
                .PutAsync(money);
        }

        public async Task UpdateApartmentDetail(ApartmentDetail currentApartment)
        {
            await _firebase
                .Child(nameof(ApartmentDetail))
                .Child(currentApartment.ApartmentDetailId.ToString())
                .PutAsync(currentApartment);
        }
        public async Task UpdateApartment(Apartment currentApartment)
        {
            await _firebase
                .Child(nameof(Apartment))
                .Child(currentApartment.ApartmentId.ToString())
                .PutAsync(currentApartment);
        }

        public async Task<List<Chat>> GetChatByApartment(Guid currentApartment)
        {
            var list = (await _firebase
                    .Child(nameof(Chat))
                    .OnceAsync<Chat>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentId == currentApartment)
                .ToList();

            list.Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));

            return list;
        }

        public async Task SendMessage(Chat chat)
        {
            await _firebase
                .Child(nameof(Chat))
                .Child(chat.ChatId.ToString())
                .PutAsync(chat);
        }

        public async Task AddOrUpdateBill(Bill bill)
        {
            await _firebase
                .Child(nameof(Bill))
                .Child(bill.BillId.ToString())
                .PutAsync(bill);
        }

        public async Task<List<Bill>> GetBills(Guid apartmentDetailApartmentDetailId)
        {
            return (await _firebase
                    .Child(nameof(Bill))
                    .OnceAsync<Bill>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail == apartmentDetailApartmentDetailId)
                .ToList();

        }

        public async Task DeleteBill(Bill selectedBill)
        {
            await _firebase
                .Child(nameof(Bill))
                .Child(selectedBill.BillId.ToString())
                .DeleteAsync();
        }

        public async Task<List<User>> GetUserListByApartment(Guid apartmentId)
        {
            var userList = new List<User>();

            var owner = (await _firebase
                    .Child(nameof(ApartmentDetail))
                    .OnceAsync<ApartmentDetail>())
                .Select(x => x.Object)
                .FirstOrDefault(x => x.ApartmentDetailId == apartmentId)
                ?.Owner;

            userList.Add(owner);

            var list = (await _firebase
                    .Child(nameof(Apartment))
                    .OnceAsync<Apartment>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail == apartmentId)
                .Select(x => x.User)
                .ToList();

            userList.AddRange(list);

            return userList;
        }

        public async Task<List<TimeAway>> GetTimeAwayListOfUser(Guid userUserId)
        {
           return (await _firebase
                    .Child(nameof(TimeAway))
                    .OnceAsync<TimeAway>())
                .Select(x => x.Object)
                .Where(x => x.User.UserId == userUserId)
                .ToList();
        }

        public async Task<List<TimeAway>> GetTimeAwayByUser(Guid userId)
        {
            return (await _firebase
                    .Child(nameof(TimeAway))
                    .OnceAsync<TimeAway>())
                .Select(x => x.Object)
                .Where(x => x.User.UserId == userId)
                .ToList();
        }

        public async Task DeleteTimeAway(TimeAway timeAway)
        {
            await _firebase
                .Child(nameof(TimeAway))
                .Child(timeAway.TimeAwayId.ToString())
                .DeleteAsync();
        }

        public async Task AddOrUpdateTimeAway(TimeAway timeAway)
        {
            await _firebase
                .Child(nameof(TimeAway))
                .Child(timeAway.TimeAwayId.ToString())
                .PutAsync(timeAway);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return (await _firebase
                    .Child(nameof(User))
                    .OnceAsync<User>())
                .Select(x => x.Object)
                .FirstOrDefault(x => x.Email == email);
        }

        public async Task<Apartment> GetApartmentByUserId(Guid userId)
        {
            return (await _firebase
                    .Child(nameof(Apartment))
                    .OnceAsync<Apartment>())
                .Select(x => x.Object)
                .FirstOrDefault(x => x.User.UserId == userId);
        }

        public async Task DeleteApartment(Apartment apartment)
        {
            await _firebase
                .Child(nameof(Apartment))
                .Child(apartment.ApartmentId.ToString())
                .DeleteAsync();
        }

        public async Task<List<Apartment>> GetApartmentsByApartmentDetail(ApartmentDetail apartment)
        {
            return (await _firebase
                    .Child(nameof(Apartment))
                    .OnceAsync<Apartment>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail == apartment.ApartmentDetailId)
                .ToList();
        }

        public async Task DeleteApartmentDetail(ApartmentDetail apartment)
        {
            await _firebase
                .Child(nameof(ApartmentDetail))
                .Child(apartment.ApartmentDetailId.ToString())
                .DeleteAsync();
        }

        public async Task<List<BuyList>> GetBuyListFromApartment(Guid apartmentId)
        {
            return (await _firebase
                    .Child(nameof(BuyList))
                    .OnceAsync<BuyList>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail == apartmentId)
                .ToList();
        }

        public async Task AddOrEditProduct(BuyList toBuy)
        {
            await _firebase
                .Child(nameof(BuyList))
                .Child(toBuy.BuyListId.ToString())
                .PutAsync(toBuy);
        }

        public async Task DeleteBuyList(BuyList selectedProduct)
        {
            await _firebase
                .Child(nameof(BuyList))
                .Child(selectedProduct.BuyListId.ToString())
                .DeleteAsync();
        }

        public async Task<List<Job>> GetJobsByApartment(Guid apartmentId)
        {
            return (await _firebase
                    .Child(nameof(Job))
                    .OnceAsync<Job>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail == apartmentId)
                .ToList();
        }

        public async Task AddOrUpdateJob(Job job)
        {
            await _firebase
                .Child(nameof(Job))
                .Child(job.JobId.ToString())
                .PutAsync(job);
        }

        public async Task DeleteJob(Job job)
        {
            await _firebase
                .Child(nameof(Job))
                .Child(job.JobId.ToString())
                .DeleteAsync();
        }

        public async Task<List<Reminder>> GetRemindersByApartment(Guid apartmentId)
        {
            return (await _firebase
                    .Child(nameof(Reminder))
                    .OnceAsync<Reminder>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail == apartmentId)
                .ToList();
        }

        public async Task DeleteReminder(Reminder currentReminder)
        {
            await _firebase
                .Child(nameof(Reminder))
                .Child(currentReminder.ReminderId.ToString())
                .DeleteAsync();
        }

        public async Task AddReminder(Reminder reminder)
        {
            await _firebase
                .Child(nameof(Reminder))
                .Child(reminder.ReminderId.ToString())
                .PutAsync(reminder);
        }

        public async Task<List<TimeAway>> GetTimeAwayByApartment(Guid apartmentDetail)
        {
            return (await _firebase
                    .Child(nameof(TimeAway))
                    .OnceAsync<TimeAway>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail == apartmentDetail)
                .ToList();
        }
    }
}
