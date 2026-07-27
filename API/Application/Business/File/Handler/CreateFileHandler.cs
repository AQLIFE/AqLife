using MediatR;
using MyLife.Application.Business.File.Service;
using MyLife.Domain.Command;

namespace MyLife.Application.Business.File.Handler
{
    public class CreateFileHandler(FileWriter fileWriter) : IRequestHandler<CreateFileCommand, IEnumerable<Guid>>
    {
        public async Task<IEnumerable<Guid>> Handle(CreateFileCommand command, CancellationToken ct)
        => await fileWriter.WriteAsync(command.GetFiles(), ct);
    }
}
