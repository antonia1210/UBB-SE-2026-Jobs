using System.Net;
using System.Net.Http.Headers;

namespace UBB_SE_2026_Jobs.App.Configuration;

public sealed class JwtForwardingHandler : DelegatingHandler
{
    private readonly SessionContext session;

    public JwtForwardingHandler(SessionContext session)
    {
        this.session = session;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var hadToken = !string.IsNullOrWhiteSpace(session.JwtToken);
        if (hadToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.JwtToken);
        }

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        // Only treat a 401 as an expired session (and bounce to login) when we actually
        // sent a token. Unauthenticated calls — e.g. the Register page loading companies,
        // or a failed login/register attempt — must NOT redirect to login.
        if (response.StatusCode == HttpStatusCode.Unauthorized && hadToken)
        {
            session.SignOut();
            await UIDispatcher.EnqueueAsync(() =>
            {
                if (UBB_SE_2026_Jobs.App.App.MainAppWindow is UBB_SE_2026_Jobs.App.MainWindow mainWindow)
                    mainWindow.ShowLogin();
            });
        }

        return response;
    }
}
