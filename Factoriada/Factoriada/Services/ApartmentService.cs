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
            currentApartment.Token = currentApartment.ApartmentDetailId.ToString().Substring(0,8);

            currentApartment.UnspentMoney = 0.0f;

            await ApiService.ServiceClientInstance.SaveApartment(currentApartment);
        }

        public async Task JoinApartment(User connectedUser, string result)
        {
            var apartmentDetail = await ApiService.ServiceClientInstance.GetApartmentByToken(result);

            if (apartmentDetail == null)
                throw new Exception("Codul nu apartine nici unui apartament.");

            var apartment = new Apartment()
            {
                ApartmentDetail = apartmentDetail,
                ApartmentId = Guid.NewGuid(),
                User = connectedUser
            };

            ApiService.ServiceClientInstance.JoinApartment(apartment);
        }
    }
}
