using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using MediatR;

namespace AqLife.Application.Business.File.Handler
{
    public class CreateFileHandler(FileWriter fileWriter) : IRequestHandler<CreateFileCommand, IEnumerable<Guid>>
    {
        public async Task<IEnumerable<Guid>> Handle(CreateFileCommand command, CancellationToken ct)
        => await fileWriter.WriteAsync(command.GetFiles(), ct);
    }
}
