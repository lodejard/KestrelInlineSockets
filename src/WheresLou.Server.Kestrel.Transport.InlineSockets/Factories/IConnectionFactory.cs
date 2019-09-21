namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Factories
{
    public interface IConnectionFactory
    {
        IConnection Create(ConnectionContext context);
    }
}
