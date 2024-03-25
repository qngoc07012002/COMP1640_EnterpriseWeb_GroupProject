using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace GreenwichUniversityMagazine.Areas.Manager.Controllers
{
    [Area("Manager")]
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
        //End Point For Chart 1 Line
        public IActionResult GetDataCountTotalContributions()
        {
            var listTerms = _unitOfWork.TermRepository.GetAll()
                        .OrderBy(term => term.EndDate)
                        .Select(term => term.Name) 
                        .ToArray();

            var totalContributions = _unitOfWork.ArticleRepository.GetArticleCountsByTerm();
            
            var data = new
            {
                listContributions = totalContributions,
                listTerms = listTerms
            };

            return Ok(data);
        }

        public async Task<IActionResult> GetMultipleLineChartData()
        {
            var chartData = await _unitOfWork.ArticleRepository.GetMultipleLineChartData();
            var listTerms = _unitOfWork.TermRepository.GetAll()
                         .OrderBy(term => term.EndDate)
                         .Select(term => term.Name)
                         .ToArray();
            var responseData = new
            {
                ListTerms = listTerms,
                ChartData = chartData
            };
            return Ok(responseData);
        }
        public async Task<IActionResult> GetGroupBarChart()
        {
            var chartData = await _unitOfWork.ArticleRepository.GetGroupBarChart();
            var listTerms = _unitOfWork.TermRepository.GetAll()
                         .OrderBy(term => term.EndDate)
                         .Select(term => term.Name)
                         .ToArray();
            var responseData = new
            {
                ListTerms = listTerms,
                ChartData = chartData
            };
            return Ok(responseData);
        }
        public async Task<IActionResult> GetStackBarChartContributions()
        {
            var chartData = await _unitOfWork.ArticleRepository.GetStackBarChartContributions();
            var listTerms = _unitOfWork.TermRepository.GetAll()
                         .OrderBy(term => term.EndDate)
                         .Select(term => term.Name)
                         .ToArray();
            var responseData = new
            {
                ListTerms = listTerms,
                ChartData = chartData
            };
            return Ok(responseData);
        }
        public IActionResult OverView()
        {
            return View("~/Areas/Manager/Views/DashBoard/Overview.cshtml");  
        }

    }
}

