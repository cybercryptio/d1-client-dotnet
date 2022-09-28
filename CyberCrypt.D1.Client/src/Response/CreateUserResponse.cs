// Copyright 2022 CYBERCRYPT
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace CyberCrypt.D1.Client.Response;

/// <summary>
/// Response from <see cref="ServiceClients.ID1Authn.CreateUser"/> or <see cref="ServiceClients.ID1Authn.CreateUserAsync"/>.
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
