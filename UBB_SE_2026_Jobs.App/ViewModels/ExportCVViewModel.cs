using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.ServiceProxies;

namespace UBB_SE_2026_Jobs.App.ViewModels;

public partial class ExportCVViewModel : DispatchableObservableObject
{
    private readonly CvExportProxy cvExport;
    private readonly SessionContext session;

    private string statusText = string.Empty;
    private bool isLoading;

    public ExportCVViewModel(CvExportProxy cvExport, SessionContext session)
    {
        this.cvExport = cvExport;
        this.session = session;
    }

    public string StatusText
    {
        get => statusText;
        set => SetProperty(ref statusText, value);
    }

    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    public async Task<string> GetPreviewHtmlAsync(CancellationToken cancellationToken = default)
    {
        var userId = ViewModelSupport.ResolveUserId(session);
        return await cvExport.GetHtmlAsync(userId, cancellationToken);
    }

    public async Task<byte[]> GetPdfBytesAsync(CancellationToken cancellationToken = default)
    {
        var userId = ViewModelSupport.ResolveUserId(session);
        return await cvExport.GetPdfBytesAsync(userId, cancellationToken);
    }
}
