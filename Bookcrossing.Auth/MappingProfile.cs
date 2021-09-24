using AutoMapper;
using Bookcrossing.Auth.Contracts.Models;
using Bookcrossing.Auth.Data.Entities;

namespace Bookcrossing.Auth
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, User>();
        }
    }
}
