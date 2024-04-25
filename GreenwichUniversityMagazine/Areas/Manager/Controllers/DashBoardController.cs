using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Authentication;


namespace GreenwichUniversityMagazine.Areas.Manager.Controllers
{
    [Area("Manager")]
    [ManagerAuthentication()]
    public class DashBoardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashBoardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View("~/Areas/Manager/Views/DashBoard/Index.cshtml");
        }
        public async Task<IActionResult> Overview(int rangeSort = 2)
        {
            int numberOfTerms = _unitOfWork.TermRepository.GetAll().Count();

            if (rangeSort == -1 || rangeSort > numberOfTerms)
            {
                rangeSort = numberOfTerms;
            }
            //RangeSort= 1,2,4,6,8,12 Terms
            OverviewVM overviewVM = new OverviewVM();
            var listTerms = _unitOfWork.TermRepository.GetAll()
                       .OrderBy(term => term.EndDate)
                       .Select(term => term.Name)
                       .ToArray();
            //get last number of elements in array
            var newListTerms = listTerms.Skip(Math.Max(0, listTerms.Length - rangeSort));
            // Chart 1: 1 Line
            var totalContributions = _unitOfWork.ArticleRepository.GetArticleCountsByTerm(rangeSort);
            overviewVM.ChartDataList.Add(new
            {
                listTerms = newListTerms,
                totalContributions

            });
            //End Of Chart 1
            //Chart 2: Multiple Line
            var multipleLineChartData = await _unitOfWork.ArticleRepository.GetMultipleLineChartData(rangeSort);
            overviewVM.ChartDataList.Add(new
            {
                listTerms = newListTerms,
                multipleLineChartData

            });
            //End Of Chart 2
            //Chart 3
            var groupBarChartData = await _unitOfWork.ArticleRepository.GetGroupBarChart(rangeSort);
            overviewVM.ChartDataList.Add(new
            {
                listTerms = newListTerms,
                groupBarChartData

            });
            //End Of Chart3
            //Chart 4
            var StackBarChartData = await _unitOfWork.ArticleRepository.GetStackBarChartContributions(rangeSort);
            overviewVM.ChartDataList.Add(new
            {
                listTerms = newListTerms,
                StackBarChartData

            });

            //End Of Chart 4
            //Chart 5 Doughnut Chart
            var DoughnutChartData = await _unitOfWork.ArticleRepository.GetDoughnutChart(rangeSort);
            overviewVM.ChartDataList.Add(new
            {
                DoughnutChartData
            });
            //End Of Chart 5
            //Chart 6 Exceptions by Faculties
            var PieChartData = await _unitOfWork.ArticleRepository.GetPieChart(rangeSort);
            overviewVM.ChartDataList.Add(new
            {
                PieChartData
            });
         
            //End of chart 6
            //Widget Data
            //Sum Contributions
            int sumContributions = 0;
            foreach (var item in totalContributions)
            {
                sumContributions += item;
            }
            //Sum Comments
            int sumComments = await _unitOfWork.CommentRepository.CountNumberOfComment(rangeSort);
            //Sum Contributors
            int sumContributors = groupBarChartData
       .Select(item => item?.GetType().GetProperty("contributors")?.GetValue(item) as int[])
       .Where(contributorsArray => contributorsArray != null)
       .SelectMany(contributorsArray => contributorsArray)
       .Sum();
            //Sum of Student
            int sumStudents = _unitOfWork.UserRepository.GetNumberOfStudents();
            //Sum of Faculty
            int sumFaculty = _unitOfWork.FacultyRepository.GetAll().Count();
            //Sum of Unapproved Articles 
            int sumUnapproved = await _unitOfWork.ArticleRepository.CountNumberOfUnapproved(rangeSort);
            //Sum of Magazines
            int sumMagazines = await _unitOfWork.MagazineRepository.CountNumberOfMagazine(rangeSort);
            //Sum of Exceptions
            dynamic dynamicChartData = PieChartData;

            int[] data = dynamicChartData.data;

            int sumExceptions = data.Sum();
            overviewVM.ChartDataList.Add(new
            {
                sumContributions,
                sumComments,
                sumContributors,
                sumStudents,
                sumFaculty,
                sumUnapproved,
                sumMagazines,
                sumExceptions
            });
            //End Of Widget Data


            //return Ok(overviewVM.ChartDataList);
            return View("~/areas/manager/views/dashboard/overview.cshtml", overviewVM);


        }
    }
}

