using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities;

namespace AqLife.Application.Tag;

public record UpdateTagCommand(Guid UID, string TagName, string? AliasName)
    : IUpdateCommand, IRequireValidEntity<TagEntity>;
