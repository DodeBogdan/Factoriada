using System;
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
            var apartment = await ApiDatabaseService.ServiceClientInstance.GetApartmentByUserId(userUserId);

            return "Oras: " + apartment.ApartmentAddress.City + ", Strada: " + apartment.ApartmentAddress.Street +
                   ", Numar: " + apartment.ApartmentAddress.Number + ", Bloc" + apartment.ApartmentAddress.Building +
                   ", Scara: " + apartment.ApartmentAddress.Staircase + ", Etaj: " + apartment.ApartmentAddress.Floor +
                   ", Apartament: " + apartment.ApartmentAddress.Apartment;
        }

        public async Task<Guid> GetApartmentIdByUser(Guid userUserId)
        {
            var result = await ApiDatabaseService.ServiceClientInstance.GetApartmentByUserId(userUserId);

            if (result != null)
                return result.ApartmentDetailId;
            return new Guid();
        }

        #region ApartmentBudget
        public async Task<ApartmentDetail> GetApartmentByUser(Guid userId)
        {
            return await ApiDatabaseService.ServiceClientInstance.GetApartmentByUserId(userId);
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

        #endregion


    }
}
