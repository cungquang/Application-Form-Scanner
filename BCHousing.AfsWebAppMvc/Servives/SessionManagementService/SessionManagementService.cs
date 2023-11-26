using BCHousing.AfsWebAppMvc.Entities.Session;
using BCHousing.AfsWebAppMvc.Models;
using Microsoft.Extensions.Azure;

namespace BCHousing.AfsWebAppMvc.Servives.SessionManagementService
{
    public class SessionManagementService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionManagementService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetUploadViewData(UploadFileViewModel model)
        {

            _httpContextAccessor.HttpContext.Session.Set(SessionKey.UploadFileInfoKey, new SubmissionViewInputModel
            {
                DocumentType = model.DocumentType,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UploadFileName = model.UploadFile.FileName,
                UploadFileSize = model.UploadFile.Length
            });
        }

        public SubmissionViewInputModel GetUploadFileViewModel() 
        {
            return _httpContextAccessor.HttpContext.Session.Get<SubmissionViewInputModel>(SessionKey.UploadFileInfoKey) ?? new SubmissionViewInputModel();
        }

        public void ClearSsession()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
        }
    }
}
