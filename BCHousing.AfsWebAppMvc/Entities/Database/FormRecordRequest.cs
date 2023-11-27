namespace BCHousing.AfsWebAppMvc.Entities.Database
{
    public class FormRecordRequest
    {
        public List<FormData>? FormDatas {  get; set; }
    }

    public class FormData { 
        public string? Key { get; set; }
        public string? Content { get; set; }
        public float? Confidence { get; set; }
    
    }
}
