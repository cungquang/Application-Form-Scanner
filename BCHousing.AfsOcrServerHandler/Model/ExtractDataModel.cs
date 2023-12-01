using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCHousing.AfsOcrServerHandler.Model
{
    public class ExtractDataModel
    {
        public string? Key { get; set; }
        public string? Content { get; set; }
        public float? Confidence { get; set; }
    }
}
