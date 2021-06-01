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
    }
}
