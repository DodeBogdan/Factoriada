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
            _firebase = new FirebaseClient("https://factoriada-40e0f-default-rtdb.firebaseio.com/");

            Initialize();
        }

        private async void Initialize()
        {
            var result = (await _firebase
                .Child("User")
                .OnceAsync<User>()).FirstOrDefault(x => x.Object.Email == "admin");

            if (result != null)
                return;

            var id = Guid.NewGuid();

            await _firebase
                 .Child("Role")
                 .Child(id.ToString())
                 .PutAsync(
                 new Role()
                 {
                     RoleId = id,
                     RoleTypeName = "Admin"
                 });

            id = Guid.NewGuid();

            await _firebase
               .Child("Role")
               .Child(id.ToString())
               .PutAsync(
                new Role()
                {
                    RoleId = id,
                    RoleTypeName = "Owner"
                });

            id = Guid.NewGuid();

            await _firebase
              .Child("Role")
              .Child(id.ToString())
              .PutAsync(
              new Role()
              {
                  RoleTypeName = "User",
                  RoleId = id
              });

            id = Guid.NewGuid();

            await _firebase
              .Child("Role")
              .Child(id.ToString())
              .PutAsync(
              new Role()
              {
                  RoleTypeName = "Default",
                  RoleId = id
              });

            var role = (await _firebase
                   .Child("Role")
                   .OnceAsync<Role>()).FirstOrDefault(x => x.Object.RoleTypeName == "Admin")
                ?.Object;

            if (role == null)
                return;

            id = Guid.NewGuid();

            await _firebase
               .Child("User")
               .Child(id.ToString())
               .PutAsync(
               new User()
               {
                   UserId = id,
                   Email = "admin",
                   Password = "admin",
                   Role = role
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
                .Child("Role")
                .OnceAsync<Role>())
                .FirstOrDefault(x => x.Object.RoleTypeName == "Default")
                ?.Object;

            await _firebase
                .Child("User")
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
                .Child("User")
                .Child(currentApartment.Owner.UserId.ToString())
                .OnceSingleAsync<User>();

            await _firebase
                .Child("ApartmentDetail")
                .Child(currentApartment.ApartmentDetailId.ToString)
                .PutAsync(currentApartment);
        }

        public async Task ChangeUserRoleTo(User user, string role)
        {
            var result = (await _firebase
                .Child("Role")
                .OnceAsync<Role>()).FirstOrDefault(x
                => x.Object.RoleTypeName == role)
                ?.Object;

            user.Role = result;

            await _firebase.Child("User")
                .Child(user.UserId.ToString())
                .PutAsync(user);
        }

        public async Task<ApartmentDetail> GetApartmentByToken(string result)
        {
            return (await _firebase
                .Child("ApartmentDetail")
                .OnceAsync<ApartmentDetail>())
                .FirstOrDefault(x => x.Object.Token == result)
                ?.Object;
        }

        public async void JoinApartment(Apartment apartment)
        {
            await ChangeUserRoleTo(apartment.User, "User");

            apartment.User = await _firebase
                .Child("User")
                .Child(apartment.User.UserId.ToString())
                .OnceSingleAsync<User>();

            await _firebase
                .Child("Apartment")
                .Child(apartment.ApartmentId.ToString())
                .PutAsync(apartment);
        }

        public async Task<ApartmentDetail> GetApartmentDetailByUserId(Guid userUserId)
        {
            var apartmentDetail = (await _firebase
                .Child(nameof(ApartmentDetail))
                .OnceAsync<ApartmentDetail>()).FirstOrDefault(x => x.Object.Owner.UserId == userUserId);

            if (apartmentDetail != null)
                return apartmentDetail.Object;

            var apartment = (await _firebase
                .Child(nameof(Apartment))
                .OnceAsync<Apartment>()).FirstOrDefault(x => x.Object.User.UserId == userUserId);

            if (apartment != null)
                return await _firebase
                    .Child(nameof(ApartmentDetail))
                    .Child(apartment.Object.ApartmentDetail.ApartmentDetailId.ToString())
                    .OnceSingleAsync<ApartmentDetail>();

            return new ApartmentDetail();
        }

        public async Task<List<Rule>> GetRulesByApartment(Guid apartmentId)
        {
            return (await _firebase
                    .Child(nameof(Rule))
                    .OnceAsync<Rule>())
                .Select(x => x.Object)
                .Where(x => x.ApartmentDetail.ApartmentDetailId == apartmentId)
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
                .Where(x => x.ApartmentDetails.ApartmentDetailId == apartmentId)
                .ToList();
        }

        public async Task DeleteAnnounce(Announce currentAnnounce)
        {
            await _firebase
                .Child(nameof(Announce))
                .Child(currentAnnounce.AnnounceId.ToString())
                .DeleteAsync();
        }

        public async Task UpdateAnnounce(Announce currentAnnounce)
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
                .Where(x => x.ApartmentDetail.ApartmentDetailId == apartmentDetailId)
                .ToList();
        }

        public async Task AddMoney(BudgetHistory money)
        {
            await _firebase
                .Child(nameof(BudgetHistory))
                .Child(money.BudgetHistoryId.ToString())
                .PutAsync(money);
        }

        public async Task UpdateApartment(ApartmentDetail currentApartment)
        {
            await _firebase
                .Child(nameof(ApartmentDetail))
                .Child(currentApartment.ApartmentDetailId.ToString())
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

        public async Task AddBill(Bill bill)
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
                .Where(x => x.ApartmentDetail.ApartmentDetailId == apartmentDetailApartmentDetailId)
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
                .Where(x => x.ApartmentDetail.ApartmentDetailId == apartmentId)
                .Select(x => x.User)
                .ToList();

            userList.AddRange(list);

            return userList;
        }

        public async Task<List<TimeAway>> GetTimeAwayOfUserByInterval(Guid userUserId)
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
                .Where(x => x.ApartmentDetail.ApartmentDetailId == apartment.ApartmentDetailId)
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
                .Where(x => x.ApartmentDetail.ApartmentDetailId == apartmentId)
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
    }
}
