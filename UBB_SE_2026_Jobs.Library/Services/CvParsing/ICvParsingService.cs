using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.CvParsing;

public interface ICvParsingService
{
    User ParseCvFile(string content, string fileType);
}
