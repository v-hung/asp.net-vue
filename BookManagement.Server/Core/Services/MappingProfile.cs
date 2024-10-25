using AutoMapper;
using BookManagement.Server.Core.Dto;
using BookManagement.Server.Core.Models;

namespace BookManagement.Server.Core.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
