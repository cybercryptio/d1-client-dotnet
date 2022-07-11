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