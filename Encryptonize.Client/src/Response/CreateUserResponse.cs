// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

/// <summary>
/// Response from <see cref="IEncryptonizeClient.CreateUser"/> or <see cref="IEncryptonizeClient.CreateUserAsync"/>.
/// </summary>
public class CreateUserResponse {
    /// <summary>
    /// Gets the user ID.
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// Gets the password.
    /// </summary>
    public string Password { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserResponse"/>.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="password">The password.</param>
    public CreateUserResponse(string userId, string password) {
        UserId = userId;
        Password = password;
    }
}
