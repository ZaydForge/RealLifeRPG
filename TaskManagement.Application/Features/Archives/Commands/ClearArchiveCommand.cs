using AutoMapper;
using MediatR;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Dtos;

namespace TaskManagement.Application.Features.Archives.Commands
{
    public class ClearArchiveCommand : IRequest<string>
    {
    }

    public class ClearArchiveCommandHandler(IArchiveRepository archiveRepo, IMapper mapper)
        : IRequestHandler<ClearArchiveCommand, string>
    {
        public async Task<string> Handle(ClearArchiveCommand command,
            CancellationToken cancellationToken)
        {
            var archives = await archiveRepo.GetAllAsync();
            if (archives.Any() || archives == null)
            {
                throw new NotFoundException("Archive is empty.");
            }
            foreach(var archive in archives)
            {
                archiveRepo.Delete(archive);
            }
            await archiveRepo.SaveChangesAsync();

            return "Archive is cleared!";
        }
    }
}


