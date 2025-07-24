using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Dtos;
using TaskManagement.Domain.Entities;
using TaskManagement.Dtos;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Application.Features.Users.Commands
{
    public class UpdateUserProfileCommand(int id, UpdateUserProfileDto userDto) : IRequest<string>
    {
        public int Id { get; } = id;
        public UpdateUserProfileDto Request { get; } = userDto;
    }

    public class UpdateUserProfileCommandHandler(
        IUserProfileRepository userRepo,
        IMapper mapper,
        IDistributedCache cache) : IRequestHandler<UpdateUserProfileCommand, string>
    {
        public async Task<string> Handle(UpdateUserProfileCommand command, CancellationToken token)
        {
            var user = await userRepo.GetUserByIdAsync(command.Id);
            if (user == null)
            {
                throw new Exception("Profile not found");
            }

            mapper.Map(command.Request, user);
            userRepo.UpdateUser(user);
            await userRepo.SaveChangesAsync();

            await cache.RemoveAsync("users_list", token);
            await cache.RemoveAsync($"user_{user.Id}", token);

            return "Profile updated successfully";
        }
    }
}
