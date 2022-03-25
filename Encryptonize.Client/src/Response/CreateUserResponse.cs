// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class CreateUserResponse {
    private string userId;
    public string UserId { get { return userId; } }
    private string password;
    public string Password { get { return password; } }

    public CreateUserResponse(string userId, string password) {
        this.userId = userId;
        this.password = password;
    }
}
