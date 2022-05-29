using System.Net;

namespace RestoreCord.Utilities;
public class ProxyConfiguration
{
    public Dictionary<Uri, NetworkCredential> _credentialList = new();
    public ProxyConfiguration()
    {

    }
}
public class ProxyGenerator : IWebProxy
{
    private readonly Uri proxyUri;
    private readonly NetworkCredential credentials;
    private ProxyConfiguration _configuration;
    public ProxyGenerator()
    {
        (Uri?, NetworkCredential?) creds = GetRandomProxy();
        if (creds.Item1 is not null && creds.Item2 is not null)
        {
            credentials = creds.Item2;
            proxyUri = creds.Item1;
        }
    }
    public Uri GetProxy(Uri destination) => proxyUri;
    public bool IsBypassed(Uri host) => false;
    private (Uri?, NetworkCredential?) GetRandomProxy()
    {
        if (_configuration._credentialList.Count == 0)
            return (null, null);
        int randy = new Random().Next(0, _configuration._credentialList.Count);
        KeyValuePair<Uri, NetworkCredential> item = _configuration._credentialList.ElementAt(randy);
        return (item.Key, item.Value);
    }
    public ICredentials? Credentials { get => credentials; set => throw new NotImplementedException(); }
    public async Task LoadProxyListAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            string[] fileText = await File.ReadAllLinesAsync(path, cancellationToken);
            _configuration._credentialList.Clear();
            foreach(string line in fileText)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string[] proxy = line.Split(':');
                if (proxy.Length != 5)
                    continue;
                _configuration._credentialList.Add(new Uri($"{proxy[0]}://{proxy[1]}:{proxy[2]}"), new NetworkCredential
                {
                    UserName = proxy[3],
                    Password = proxy[4]
                });
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("task cancelled");
        }
        catch { throw; }
    }
}