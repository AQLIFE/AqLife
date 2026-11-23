using AqLife.Application.Abstractions.Commands;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Application.Tag;

public record CreateTagCommand(
    [Required(ErrorMessage = "TagName is required")] string Name,
    string? AliasName) : ICreateCommand;
