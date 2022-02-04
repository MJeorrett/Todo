using System.ComponentModel.DataAnnotations;

namespace Todo.WebApi.ViewModels;

public class LoginViewModel
{
    [Required]
    public string UserName { get; init; } = "";

    [Required]
    public string Password { get; init; } = "";

    public string? ReturnUrl { get; init; }
}
