using AqLife.Application.Abstractions.Commands;

namespace AqLife.Application.Corpus;

public record DeleteCorepusCommand(Guid UID) : IDeleteCommand;
