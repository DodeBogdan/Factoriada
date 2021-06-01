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
        Task<string> GetApartmentByUser(Guid userUserId);
        Task<Guid> GetApartmentIdByUser(Guid userUserId);
        Task<List<Rule>> GetRulesByApartmentId(Guid apartmentId);
        Task AddRuleToApartment(Rule rule, Guid apartmentId);
        Task EditRuleFromApartment(Rule currentRule);
        Task DeleteRule(Rule currentRule);
        Task<List<Announce>> GetAnnouncesByApartmentId(Guid apartmentId);
        Task DeleteAnnounce(Announce currentAnnounce);
        Task EditAnnounceFromApartment(Announce currentAnnounce);
        Task AddAnnounceToApartment(Announce announce, Guid apartmentId);
    }
}
