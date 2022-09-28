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

using Grpc.Core;

namespace CyberCrypt.D1.Client.Credentials;

internal class D1CompositeCredentials : ChannelCredentials
{
    private readonly ChannelCredentials channelCredentials;
    private readonly CallCredentials callCredentials;

    public D1CompositeCredentials(ChannelCredentials channelCredentials, CallCredentials callCredentials)
    {
        this.channelCredentials = channelCredentials ?? throw new ArgumentNullException(nameof(channelCredentials));
        this.callCredentials = callCredentials ?? throw new ArgumentNullException(nameof(callCredentials));
    }

    public override void InternalPopulateConfiguration(ChannelCredentialsConfiguratorBase configurator, object state)
    {
        configurator.SetCompositeCredentials(state, channelCredentials, callCredentials);
    }
}