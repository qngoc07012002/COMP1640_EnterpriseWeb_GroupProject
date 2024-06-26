﻿using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
namespace GreenwichUniversityMagazine.Repository
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        private dbContext _dbContext;
        
        public ArticleRepository(dbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Article> Search(string searchString)
        {
            return _dbContext.Articles
                .Include(a => a.User)  
                .Where(a => a.Title.Contains(searchString)
                         || a.User.Name.Contains(searchString)) 
                .ToList();
        }



        public List<Article> GetArticlesbyMagazine(int? id)
        {

            var query = _dbContext.Articles.Where(c => c.MagazinedId == id && c.Status == true).OrderByDescending(a => a.ArticleId);
            return query.ToList();
        }
        public List<Article> GetArticlesbyTerm(int? id)
        {
            var query = _dbContext.Articles.Where(c => c.Magazines.TermId == id && c.Status == true).OrderByDescending(a => a.ArticleId);
            return query.ToList();
        }
        public List<Article> GetArticlesbyFaculty(int? id)
        {
            var query = _dbContext.Articles.Where(c => c.Magazines.FacultyId == id && c.Status == true).OrderByDescending(a => a.ArticleId);
            return query.ToList();
        }

        public Article GetById(int id)
        {
            return _dbContext.Articles.FirstOrDefault(t => t.ArticleId == id);
        }


        public IEnumerable<int> GetArticleCountsByTerm(int range)
        {
            var query = (from term in _dbContext.Terms
                         join magazine in _dbContext.Magazines on term.Id equals magazine.TermId into magazineGroup
                         from magazine in magazineGroup.DefaultIfEmpty()
                         join article in _dbContext.Articles.Where(a => a.Status == true) on magazine.Id equals article.MagazinedId into articleGroup
                         group articleGroup by new { term.Name, term.EndDate } into g
                         orderby g.Key.EndDate
                         select g.SelectMany(x => x).Count()).ToList();


            List<int> results = query.GetRange(query.Count - range, range);
            return results;
        }

        //Query for Chart 2 Overview

        public async Task<IEnumerable<object>> GetMultipleLineChartData(int rangeSort)
        {
            var query = from trm in _dbContext.Terms
                        from fac in _dbContext.Faculties
                        join mag in _dbContext.Magazines on new { TermId = trm.Id, FacultyId = fac.Id } equals new { mag.TermId, mag.FacultyId } into magazineGroup
                        from mag in magazineGroup.DefaultIfEmpty()
                        join art in _dbContext.Articles.Where(a => a.Status == true) on mag.Id equals art.MagazinedId into articleGroup
                        from art in articleGroup.DefaultIfEmpty()
                        where _dbContext.Terms.OrderByDescending(t => t.EndDate).Take(rangeSort).Any(t => t.Id == trm.Id)
                        group new { trm, fac, art } by new { trm.Name, facName = fac.Name } into grouped
                        orderby grouped.Max(g => g.trm.EndDate) ascending, grouped.Key.Name, grouped.Key.facName
                        select new
                        {
                            TermName = grouped.Key.Name,
                            FacultyName = grouped.Key.facName,
                            ArticleCount = grouped.Count(g => g.art != null),
                            StudentCount = grouped.Select(g => g.art.UserId).Where(userId => userId != null).Distinct().Count()
                        };
            var results = await query.ToListAsync();
            int numberOfFaculties = _dbContext.Faculties.Count();
            List<object> listOfArrays = new List<object>();
            for (int i = 0; i < numberOfFaculties; i++)
            {
                string facultyName = "";
                int[] contributions = new int[results.Count() / numberOfFaculties];
                for (int j = i; j < results.Count(); j += numberOfFaculties)
                {
                    contributions[j / numberOfFaculties] = results[j].ArticleCount; // Corrected indexing
                    facultyName = results[j].FacultyName; // Move facultyName assignment outside the inner loop
                }
                listOfArrays.Add(new
                {
                    contributions = contributions,
                    facultyName = facultyName
                });
            }



            return listOfArrays;


        }


        //Query for Chart 3 OverView
        public async Task<IEnumerable<object>> GetGroupBarChart(int rangeSort)
        {
            var query = from trm in _dbContext.Terms
                        from fac in _dbContext.Faculties
                        join mag in _dbContext.Magazines on new { TermId = trm.Id, FacultyId = fac.Id } equals new { mag.TermId, mag.FacultyId } into magazineGroup
                        from mag in magazineGroup.DefaultIfEmpty()
                        join art in _dbContext.Articles.Where(a => a.Status == true) on mag.Id equals art.MagazinedId into articleGroup
                        from art in articleGroup.DefaultIfEmpty()
                        where _dbContext.Terms.OrderByDescending(t => t.EndDate).Take(rangeSort).Any(t => t.Id == trm.Id)
                        group new { trm, fac, art } by new { trm.Name, facName = fac.Name } into grouped
                        orderby grouped.Max(g => g.trm.EndDate) ascending, grouped.Key.Name, grouped.Key.facName
                        select new
                        {
                            TermName = grouped.Key.Name,
                            FacultyName = grouped.Key.facName,
                            ArticleCount = grouped.Count(g => g.art != null),
                            StudentCount = grouped.Select(g => g.art.UserId).Where(userId => userId != null).Distinct().Count()
                        };

            var results = await query.ToListAsync();
            int numberOfFaculties = _dbContext.Faculties.Count();
            List<object> listOfArrays = new List<object>();
            for (int i = 0; i < numberOfFaculties; i++)
            {
                string facultyName = "";
                int[] contributors = new int[results.Count() / numberOfFaculties];
                for (int j = i; j < results.Count(); j += numberOfFaculties)
                {
                    contributors[j / numberOfFaculties] = results[j].StudentCount; // Corrected indexing
                    facultyName = results[j].FacultyName; // Move facultyName assignment outside the inner loop
                }
                listOfArrays.Add(new
                {
                    contributors = contributors,
                    facultyName = facultyName
                });
            }



            return listOfArrays;


        }
        //Query for Chart 4 OverView
        public async Task<IEnumerable<object>> GetStackBarChartContributions(int rangeSort)
        {
            var query = from trm in _dbContext.Terms
                        from fac in _dbContext.Faculties
                        join mag in _dbContext.Magazines on new { TermId = trm.Id, FacultyId = fac.Id } equals new { mag.TermId, mag.FacultyId } into magazineGroup
                        from mag in magazineGroup.DefaultIfEmpty()
                        join art in _dbContext.Articles.Where(a => a.Status == true) on mag.Id equals art.MagazinedId into articleGroup
                        from art in articleGroup.DefaultIfEmpty()
                        where _dbContext.Terms.OrderByDescending(t => t.EndDate).Take(rangeSort).Any(t => t.Id == trm.Id)
                        group new { trm, fac, art } by new { trm.Name, facName = fac.Name } into grouped
                        orderby grouped.Max(g => g.trm.EndDate) ascending, grouped.Key.Name, grouped.Key.facName
                        select new
                        {
                            TermName = grouped.Key.Name,
                            FacultyName = grouped.Key.facName,
                            ArticleCount = grouped.Count(g => g.art != null),
                            StudentCount = grouped.Select(g => g.art.UserId).Where(userId => userId != null).Distinct().Count()
                        };

            var results = await query.ToListAsync();
            int numberOfFaculties = _dbContext.Faculties.Count();
            List<double[]> listOfContributions = new List<double[]>();
            //Sum of each Term
            List<double> sum = new List<double>();
            for (int i = 0; i < results.Count; i += numberOfFaculties)
            {
                double subTotal = 0.0;
                for (int j = 0; j < numberOfFaculties; j++)
                {
                    subTotal += results[j + i].ArticleCount;
                }
                sum.Add(subTotal);
            }
            //cal % each Faculty in term
            List<object> listObj = new List<object>();
            for (int i = 0; i < numberOfFaculties; i++)
            {
                double[] contributions = new double[results.Count / numberOfFaculties];
                int index = 0;
                for (int j = 0; j < results.Count; j += numberOfFaculties)
                {
                    if (sum[index] > 0)
                    {
                        contributions[index] = Math.Round(((Double)results[j + i].ArticleCount / sum[index]) * 100, 3);

                    }
                    else
                    {
                        contributions[index] = 0.0;
                    }
                    index++;
                }
                listObj.Add(new
                {
                    facultyName = results[i].FacultyName,
                    contributions,
                });
            }
            return listObj;
        }
        //Query for doughnutChart (Exception Article With Comment AND withoud Comment)
        public async Task<object> GetDoughnutChart(int rangeSort)
        {
            var query = from article in _dbContext.Articles
                        join comment in _dbContext.Comments on article.ArticleId equals comment.ArticleId into commentGroup
                        from comment in commentGroup.DefaultIfEmpty()
                        join magazine in _dbContext.Magazines on article.MagazinedId equals magazine.Id
                        join term in (from t in _dbContext.Terms
                                      orderby t.EndDate descending
                                      select new { t.Name, t.Id }).Take(rangeSort) on magazine.TermId equals term.Id
                        select new
                        {
                            article.ArticleId,
                            article.Title,
                            CommentId = comment != null ? comment.Id : 0
                        };

            var result = query.ToList();
            //Count number of articles witthout comments
            int articlesWithoutComments = 0;
            foreach (var item in result)
            {
                if (item.CommentId == 0)
                {
                    articlesWithoutComments++;
                }
            }
            int articlesWithComments = result.Select(article => article.ArticleId).Distinct().Count() - articlesWithoutComments;


            var chartData = new
            {
                labels = new[] { "Contributions With Comments", "Contributions Without Comments" },
                data = new[] { articlesWithComments, articlesWithoutComments }
            };

            return chartData;
        }

        public async Task<int> CountNumberOfUnapproved(int rangeSort)
        {
            var latestTermIds = await _dbContext.Terms
       .OrderByDescending(t => t.EndDate)
       .Select(t => t.Id)
       .Take(rangeSort)
       .ToListAsync();

            var query = from article in _dbContext.Articles.Where(a => a.Status == false)
                        join magazine in _dbContext.Magazines on article.MagazinedId equals magazine.Id
                        where latestTermIds.Contains(magazine.TermId)
                        select article;

            var totalUnapproved = await query.CountAsync();


            return totalUnapproved;

        }

        public async Task<object> GetPieChart(int rangeSort)
        {
            var query = from article in _dbContext.Articles
                        join comment in _dbContext.Comments on article.ArticleId equals comment.ArticleId into commentGroup
                        from comment in commentGroup.DefaultIfEmpty()
                        join magazine in _dbContext.Magazines on article.MagazinedId equals magazine.Id
                        join faculty in _dbContext.Faculties on magazine.FacultyId equals faculty.Id
                        join term in (from t in _dbContext.Terms
                                      orderby t.EndDate descending
                                      select new { t.Name, t.Id }).Take(rangeSort) on magazine.TermId equals term.Id
                        select new
                        {
                            article.ArticleId,
                            CommentId = comment != null ? comment.Id : 0,
                            Faculty=faculty.Name
                        };
            query = query.Where(item => item.CommentId == 0);
            query = query.OrderBy(item => item.Faculty);
            var groupedQuery = query.GroupBy(item => item.Faculty)
                        .Select(grouping => new
                        {
                            Faculty = grouping.Key,
                            CommentCount = grouping.Count()
                        });
            var result = groupedQuery.ToList();
            string[] labels = new string[result.Count()];
            int[] data= new int[result.Count()];
            for(int i= 0;i < result.Count();i++)
            {
                labels[i] = result[i].Faculty;
                data[i] = result[i].CommentCount;
            }

            var chartData = new
            {
                labels,
                data
            };

            return chartData;
        }

        }
}



    
