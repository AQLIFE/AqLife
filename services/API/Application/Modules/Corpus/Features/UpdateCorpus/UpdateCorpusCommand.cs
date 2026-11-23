using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities;

namespace AqLife.Application.Corpus;

public record UpdateCorpusCommand(Guid UID, string Content)
    : IUpdateCommand, IRequireValidEntity<CorpusEntity>;
