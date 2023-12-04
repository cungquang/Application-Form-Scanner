using BCHousing.AfsWebAppMvc.Entities;

namespace BCHousing.AfsWebAppMvc.Models
{
    public class FormsEditViewModel
    {
        public FormsEditViewModel() { }

        public FormsEditViewModel(string submissionId, string classifyType, IList<Form> listOfFormFields) : base()
        {
            SubmissionId = submissionId;
            ClassifyType = classifyType;
            ListOfFormFields = listOfFormFields;
        }

        public IList<Form> ListOfFormFields { get; set; }

        public string SubmissionId { get; set; }

        public string ClassifyType {  get; set; }

        public long NumberOfFields()
        {
            return ListOfFormFields.Count;
        }
    }
}
