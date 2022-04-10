using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Dependencies
{
    public interface IFormFileTools
    {
        Task<string> GetFormData(string formId, string username);
        Task<byte[]> GetImageData(string imageId, string formId, string username);
        Task<string> GetLexiconData(string lexiconId, string formId, string username);
        void SaveAndUnzipFormFiles(string formId, string username, byte[] zipData);

        void DeleteFormData(string formId, string username);
    }
}
