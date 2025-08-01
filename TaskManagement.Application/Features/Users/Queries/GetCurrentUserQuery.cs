﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;

namespace TaskManagement.Application.Features.Users.Queries
{
    public class GetCurrentUserQuery : IRequest<UserProfileDto>
    {

    }

    public class GetCurrentUserQueryHandler(
       IUserProfileRepository userRepo,
       IMapper mapper,
       IDistributedCache cache)
       : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
    {
        public readonly string _cacheKey = "current_user";
        public async Task<UserProfileDto> Handle(GetCurrentUserQuery query, CancellationToken token)
        {
            var cachedUser = await cache.GetStringAsync(_cacheKey);
            if (!string.IsNullOrEmpty(cachedUser))
            {
                var deserialized = JsonSerializer.Deserialize<UserProfileDto>(cachedUser, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null)
                {
                    throw new NotFoundException("User not found");
                }

                return deserialized;
            }
            var user = mapper.Map<UserProfileDto>(await userRepo.GetUserByIdAsync(1));

            var serialized = JsonSerializer.Serialize(user);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };

            await cache.SetStringAsync(_cacheKey, serialized, options);

            return user;
        }
    }
}
