﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Factoriada.Exceptions;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;

namespace Factoriada.Services
{
    internal class ApartmentService : IApartmentService
    {
        #region CreateApartment
        public void TestAddress(Address apartmentAddress)
        {
            if (apartmentAddress.Country == null) throw new AddressException("Tara trebuie sa fie completat.");

            if (apartmentAddress.Country.Length < 3 || apartmentAddress.Country.Length > 50)
                throw new AddressException("Tara trebuie sa aiba minim 3 caractere sau maxim 50.");

            if (apartmentAddress.City == null) throw new AddressException("Orasul trebuie sa fie completat.");

            if (apartmentAddress.City.Length < 3 || apartmentAddress.City.Length > 50)
                throw new AddressException("Orasul trebuie sa aiba minim 3 caractere sau maxim 50.");

            if (apartmentAddress.Street == null) throw new AddressException("Strada trebuie sa fie completat.");

            if (apartmentAddress.Street.Length < 3 || apartmentAddress.Street.Length > 50)
                throw new AddressException("Strada trebuie sa aiba minim 3 caractere sau maxim 50.");
        }

        public async Task SaveApartment(ApartmentDetail currentApartment)
        {
            TestAddress(currentApartment.ApartmentAddress);

            if (currentApartment.ApartmentName.Length < 3 || currentApartment.ApartmentName.Length > 50)
                throw new AddressException("Numele apartamentului trebuie sa aiba minim 3 caractere sau maxim 50.");

            currentApartment.Owner = ActiveUser.User;
            currentApartment.ApartmentDetailId = Guid.NewGuid();
            currentApartment.Token = currentApartment.ApartmentDetailId.ToString().Substring(0, 8);

            currentApartment.UnspentMoney = 0.0f;

            await ApiDatabaseService.ServiceClientInstance.SaveApartment(currentApartment);
        }
        #endregion


        public async Task JoinApartment(User connectedUser, string result)
        {

            if (connectedUser.Role.RoleTypeName != "Default")
                throw new UserException("Acest user este deja intr-un apartament.");

            var apartmentDetail = await ApiDatabaseService.ServiceClientInstance.GetApartmentByToken(result);

            if (apartmentDetail == null)
                throw new Exception("Codul nu apartine nici unui apartament.");

            var apartment = new Apartment()
            {
                ApartmentDetail = apartmentDetail,
                ApartmentId = Guid.NewGuid(),
                User = connectedUser
            };

            ApiDatabaseService.ServiceClientInstance.JoinApartment(apartment);
        }

        #region ApartmentRules
        public async Task<List<Rule>> GetRulesByApartmentId(Guid apartmentId)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetRulesByApartment(apartmentId);
        }

        public async Task AddRuleToApartment(Rule rule, Guid apartmentId)
        {
            rule.ApartmentDetail = await ApiDatabaseService.ServiceClientInstance.GetApartmentById(apartmentId);

            await ApiDatabaseService.ServiceClientInstance.UpdateRule(rule);
        }

        public async Task EditRuleFromApartment(Rule currentRule)
        {
            await ApiDatabaseService.ServiceClientInstance.UpdateRule(currentRule);
        }

        public async Task DeleteRule(Rule currentRule)
        {
            await ApiDatabaseService.ServiceClientInstance.DeleteRule(currentRule);
        }
        #endregion

        #region ApartmentAnnounces
        public async Task<List<Announce>> GetAnnouncesByApartmentId(Guid apartmentId)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetAnnouncesByApartmentId(apartmentId);
        }

        public async Task DeleteAnnounce(Announce currentAnnounce)
        {
            await ApiDatabaseService.ServiceClientInstance.DeleteAnnounce(currentAnnounce);
        }

        public async Task EditAnnounceFromApartment(Announce currentAnnounce)
        {
            await ApiDatabaseService.ServiceClientInstance.UpdateAnnounce(currentAnnounce);
        }

        public async Task AddAnnounceToApartment(Announce announce, Guid apartmentId)
        {
            announce.ApartmentDetails = await ApiDatabaseService.ServiceClientInstance.GetApartmentById(apartmentId);

            await ApiDatabaseService.ServiceClientInstance.UpdateAnnounce(announce);
        }
        #endregion

        public async Task<string> GetApartmentAddressByUser(Guid userUserId)
        {
            var apartment = await ApiDatabaseService.ServiceClientInstance.GetApartmentDetailByUserId(userUserId);

            return "Oras: " + apartment.ApartmentAddress.City + ", Strada: " + apartment.ApartmentAddress.Street +
                   ", Numar: " + apartment.ApartmentAddress.Number + ", Bloc" + apartment.ApartmentAddress.Building +
                   ", Scara: " + apartment.ApartmentAddress.Staircase + ", Etaj: " + apartment.ApartmentAddress.Floor +
                   ", Apartament: " + apartment.ApartmentAddress.Apartment;
        }

        public async Task<Guid> GetApartmentIdByUser(Guid userUserId)
        {
            var result = await ApiDatabaseService.ServiceClientInstance.GetApartmentDetailByUserId(userUserId);

            if (result != null)
                return result.ApartmentDetailId;
            return new Guid();
        }

        #region ApartmentBudget
        public async Task<ApartmentDetail> GetApartmentByUser(Guid userId)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetApartmentDetailByUserId(userId);
        }

        public async Task<List<BudgetHistory>> GetBudgetHistoryByApartmentId(Guid apartmentDetailId)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetBugetHistoryByApartment(apartmentDetailId);
        }

        public async Task AddMoneyToApartment(BudgetHistory money)
        {
            await ApiDatabaseService.ServiceClientInstance.AddMoney(money);
        }

        public async Task UpdateApartment(ApartmentDetail currentApartment)
        {
            await ApiDatabaseService.ServiceClientInstance.UpdateApartment(currentApartment);
        }
        #endregion

        #region ApartmentChat   
        public async Task<List<Chat>> GetChatByApartmentId(Guid currentApartment)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetChatByApartment(currentApartment);
        }

        public async Task SendMessage(Chat chat)
        {
            await ApiDatabaseService.ServiceClientInstance.SendMessage(chat);
        }

        public Task<List<Reminder>> GetRemindersByApartmentId(Guid apartmentId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Bill

        public async Task AddBill(Bill bill)
        {
            TestBill(bill);

            await ApiDatabaseService.ServiceClientInstance.AddBill(bill);

        }

        public async Task<List<Bill>> GetBillsByApartment(Guid apartmentDetailApartmentDetailId)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetBills(apartmentDetailApartmentDetailId);
        }

        public async Task DeleteBill(Bill selectedBill)
        {
            await ApiDatabaseService.ServiceClientInstance.DeleteBill(selectedBill);
        }

        public async Task<List<BillPaidPersons>> GenerateBillPaidPersons(Bill selectedBill, Guid apartmentId)
        {
            var userList = await ApiDatabaseService.ServiceClientInstance.GetUserListByApartment(apartmentId);
            var totalPrice = selectedBill.BillPrice / userList.Count;
            var daysOnMouth = (selectedBill.DueDate - selectedBill.StartDate).Days;
            float pricePerDay;
            if (totalPrice != 0)
                pricePerDay = totalPrice / daysOnMouth;
            else
            {
                pricePerDay = 0;
            }

            var billPaidPersonList = new List<BillPaidPersons>();

            foreach (var user in userList)
            {
                var billPaidPersons = new BillPaidPersons
                {
                    BillUserPaid = user,
                    BillPaidId = Guid.NewGuid(),
                    BillPaidBillId = selectedBill.BillId
                };

                var daysOff = await ApiDatabaseService.ServiceClientInstance.GetTimeAwayOfUserByInterval(user.UserId);

                billPaidPersons.MoneyPaid = pricePerDay * daysOnMouth;

                billPaidPersonList.Add(billPaidPersons);
            }

            return billPaidPersonList;
        }
        
        private void TestBill(Bill bill)
        {
            if (bill.StartDate >= DateTime.Now)
                throw new InvalidBillException("Data de inceput nu poate fii mai noua decat data curenta.");

            if (bill.StartDate >= bill.DueDate)
                throw new InvalidBillException("Data de inceput nu poate fii mai mare decat cea de final.");

            if (bill.StartDate.AddMonths(1).AddDays(10) < bill.DueDate)
                throw new InvalidBillException(
                    "Timpul dintre cele 2 dati nu poate fii mai lung decat o luna si 10 zile.");

            if (bill.DueDate >= bill.DateOfIssue)
                throw new InvalidBillException("Data scadenta nu poate fii mai mica decat data de final.");

            if (bill.DueDate.AddMonths(1) < bill.DateOfIssue)
                throw new InvalidBillException("Data scadenta nu poate fii mai tarzie de o luna dupa data de final.");
        }

        #endregion

        #region TimeAway
        public async Task<List<TimeAway>> GetTimeAwayByUser(Guid userId)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetTimeAwayByUser(userId);
        }

        public async Task AddOrUpdateTimeAway(TimeAway timeAway)
        {
            await TestTimeAway(timeAway);

            await ApiDatabaseService.ServiceClientInstance.AddOrUpdateTimeAway(timeAway);
        }

        public async Task DeleteTimeAway(TimeAway timeAway)
        {
            await ApiDatabaseService.ServiceClientInstance.DeleteTimeAway(timeAway);
        }

        private async Task TestTimeAway(TimeAway timeAway)
        {
            var list = await ApiDatabaseService.ServiceClientInstance.GetTimeAwayByUser(timeAway.User.UserId);
            foreach (var away in list)
            {
                if(timeAway.TimeAwayId == away.TimeAwayId)
                    continue;

                if (timeAway.LeaveFrom >= away.LeaveFrom && timeAway.LeaveFrom <= away.LeaveTo)
                    throw new Exception("Data de plecare deja a fost aleasa odata.");
                
                if (timeAway.LeaveTo >= away.LeaveFrom && timeAway.LeaveTo <= away.LeaveTo)
                    throw new Exception("Data de plecare deja a fost aleasa odata.");

                if(timeAway.LeaveFrom < away.LeaveFrom && timeAway.LeaveTo > away.LeaveFrom)
                    throw new Exception("Data de plecare deja a fost aleasa odata.");
            }
        }
        #endregion

        #region SeePersonFromApartment

        public async Task<List<User>> GetUsersByApartment(Guid apartmentId)
        {
           return await ApiDatabaseService.ServiceClientInstance.GetUserListByApartment(apartmentId);
        }

        public async Task RemoveUserFromApartment(User selectedUser)
        {
            var apartment = await ApiDatabaseService.ServiceClientInstance.GetApartmentByUserId(selectedUser.UserId);
            await ApiDatabaseService.ServiceClientInstance.DeleteApartment(apartment);
            await ApiDatabaseService.ServiceClientInstance.ChangeUserRoleTo(selectedUser, "Default");
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var result = await ApiDatabaseService.ServiceClientInstance.GetUserByEmail(email);

            if (result == null)
                throw new UserException("Nu exista nici un user cu acest email.");

            return result;
        }

        public async Task DeleteApartment(ApartmentDetail apartment)
        {
            var users = await ApiDatabaseService.ServiceClientInstance.GetUserListByApartment(apartment.ApartmentDetailId);

            foreach (var user in users)
            {
                if (user.Role.RoleTypeName != "Owner")
                {
                    await RemoveUserFromApartment(user);
                }
            }

            await ApiDatabaseService.ServiceClientInstance.DeleteApartmentDetail(apartment);
            await ApiDatabaseService.ServiceClientInstance.ChangeUserRoleTo(ActiveUser.User, "Default");

        }

        public async Task<List<BuyList>> GetBuyListFromApartment(Guid apartmentId)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetBuyListFromApartment(apartmentId);
        }

        public async Task AddOrUpdateProductToBuy(BuyList toBuy)
        {
            await ApiDatabaseService.ServiceClientInstance.AddOrEditProduct(toBuy);
        }

        public async Task DeleteToBuy(BuyList selectedProduct)
        {
            await ApiDatabaseService.ServiceClientInstance.DeleteBuyList(selectedProduct);
        }

        public async Task<List<Job>> GetJobsByApartment(Guid apartmentDetailId)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetJobsByApartment(apartmentDetailId);
        }

        public async Task AddOrUpdateJob(Job job)
        {
            await ApiDatabaseService.ServiceClientInstance.AddOrUpdateJob(job);
        }

        public async Task DeleteJob(Job selectedJob)
        {
            await ApiDatabaseService.ServiceClientInstance.DeleteJob(selectedJob);
        }

        #endregion

    }
}
