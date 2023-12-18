using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BCHousing.AfsWebAppMvc.Entities
{
    [PrimaryKey(nameof(submissionId), nameof(sequence))]
    public class Form
    {
        [ForeignKey("SubmissionLog")]
        public Guid submissionId { get; set; }
        public int? sequence {  get; set; }

        public string? field_name { get; set; }
        public string? field_value { get; set; }
    }

}
