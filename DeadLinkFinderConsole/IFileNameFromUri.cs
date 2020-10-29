using System;

namespace DeadLinkFinderConsole
{
    public interface IFileNameFromUri
    {
        string ConvertToWindowsFileName(Uri urlText);
    }
}