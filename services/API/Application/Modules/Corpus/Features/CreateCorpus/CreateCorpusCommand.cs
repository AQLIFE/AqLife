using AqLife.Application.Abstractions.Commands;

namespace AqLife.Application.Corpus;

public record CreateCorpusCommand(string Content) : ICreateCommand;
