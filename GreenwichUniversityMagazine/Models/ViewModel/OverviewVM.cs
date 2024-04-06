using System.Collections.Generic;

namespace GreenwichUniversityMagazine.Models.ViewModel
{
    public class OverviewVM
    {
        public List<dynamic> ChartDataList { get; set; }

        public OverviewVM()
        {
            ChartDataList = new List<dynamic>();
        }
    }
}
