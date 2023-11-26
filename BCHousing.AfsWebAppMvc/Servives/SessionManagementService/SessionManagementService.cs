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

        public void SetSubmissionViewInputModel(UploadFileViewModel model, bool IsUploadSuccess)
        {
            var newSubmission = GetSubmissionViewInputModel();
            newSubmission.FirstName.Add(model.FirstName);
            newSubmission.LastName.Add(model.LastName);
            newSubmission.DocumentType.Add(model.DocumentType);
            newSubmission.UploadFileName.Add(model.UploadFile.FileName);
            newSubmission.UploadFileSize.Add(model.UploadFile.Length);
            newSubmission.IsSuccess.Add(IsUploadSuccess);
            _httpContextAccessor.HttpContext.Session.Set(SessionKey.UploadFileInfoKey, newSubmission);
        }

        public SubmissionViewInputModel GetSubmissionViewInputModel() 
        {
            return _httpContextAccessor.HttpContext.Session.Get<SubmissionViewInputModel>(SessionKey.UploadFileInfoKey) ?? new SubmissionViewInputModel();
        }

        public void ClearSsession()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
        }
    }
}
