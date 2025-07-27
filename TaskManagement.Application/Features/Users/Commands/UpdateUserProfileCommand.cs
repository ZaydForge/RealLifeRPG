using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Application.Dtos;
using TaskManagement.Application.Services;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Application.Features.Users.Commands
{
    public class UpdateUserProfileCommand : IRequest<string>
    {
        public int Id { get; }
        public UpdateUserProfileDto Request { get; }

        public UpdateUserProfileCommand(int id, UpdateUserProfileDto userDto)
        {
            Id = id;
            Request = userDto;
        }
    }

    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, string>
    {
        private readonly IUserProfileRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;
        private readonly IDistributedCache _cache;

        public UpdateUserProfileCommandHandler(
            IUserProfileRepository userRepo,
            IMapper mapper,
            IFileStorageService fileStorage,
            IDistributedCache cache)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _cache = cache;
        }

        public async Task<string> Handle(UpdateUserProfileCommand command, CancellationToken token)
        {
            var user = await _userRepo.GetUserByIdAsync(command.Id);
            if (user == null)
                throw new Exception("Profile not found");

            // Map simple fields (Bio, etc.)
            _mapper.Map(command.Request, user);

            // Handle profile picture upload
            if (command.Request.ProfilePicture != null)
            {
                var file = command.Request.ProfilePicture;

                // Set bucket and object name (e.g. "profiles/user-5.jpg")
                var bucketName = "profile-photo";
                var objectName = $"{Guid.NewGuid()}_{file.FileName}";

                using var stream = file.OpenReadStream();
                await _fileStorage.UploadFileAsync(bucketName, objectName, stream, file.ContentType);

                user.ProfilePictureUrl = objectName;
            }

            _userRepo.UpdateUser(user);
            await _userRepo.SaveChangesAsync();

            await _cache.RemoveAsync("users_list", token);
            await _cache.RemoveAsync("current_user", token);

            return "Profile updated successfully";
        }
    }
}
