using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities;

namespace AqLife.Application.Tag;

public record DeleteTagCommand(Guid UID)
    : IDeleteCommand, IRequireValidEntity<TagEntity>;
