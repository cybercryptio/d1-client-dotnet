// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class CreateUserResponse {
    public string UserId { get; private set; }
    public string Password { get; private set; }

    public CreateUserResponse(string UserId, string Password) {
        this.UserId = UserId;
        this.Password = Password;
    }
}
