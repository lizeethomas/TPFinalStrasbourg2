using TPFinalStrasbourg.Repositories;
using TPFinalStrasbourg.Services;
using TPFinalStrasbourg.DTOs;
using TPFinalStrasbourg.Models;
using System.Security.Claims;

namespace TPFinalStrasbourg.Services

{
    public class AdService
    {
        private readonly AdRepository _adRepo;
        private readonly UserRepository _userRepo;
        private readonly CategoryRepository _catRepo;
        private readonly IHttpContextAccessor _httpContext;
        private string userRole;
        private string userName;
        private User user;

        public AdService(AdRepository adRepo, UserRepository userRepo, CategoryRepository catRepo, IHttpContextAccessor httpContext)
        {
            _adRepo = adRepo;
            _httpContext = httpContext;
            _userRepo = userRepo;
            _catRepo = catRepo;
        }

        public void setUser()
        {
            this.userRole = _httpContext.HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Role).Value;
            this.userName = _httpContext.HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Name).Value;
            this.user = _userRepo.SearchOne(u => u.Name == this.userName);
        }

        public AdResponseDTO Create(AdRequestDTO req)
        {
            this.setUser();
            AdResponseDTO response = new AdResponseDTO();

            Ad ad = new Ad
            {
                Title = req.Title,
                Description = req.Description,
                Status = this.userRole == "admin" ? "Validée" : "En attente",
                Date = DateTime.Now,
                CategoryId = req.CategoryId,
                User = this.user
            };

            if (_adRepo.Save(ad))
            {
                response.Description = ad.Description;
                response.Title = ad.Title;
                response.Status = ad.Status;
                response.Category = _catRepo.FindById(req.CategoryId).Name;
                response.Date = ad.Date;

                return response;
            }
            return null;
        }

        public CommentResponseDTO AddComment(int adId, string text)
        {
            this.setUser();
            Ad ad = _adRepo.FindById(adId);
            Comment comment = new()
            {
                Text = text,
                Status = this.userRole == "admin" ? "Validée" : "En attente",
                Date = DateTime.Now,
                UserId = this.user.Id
            };
            ad.Comments.Add(comment);
            _adRepo.Update();
            CommentResponseDTO commentResponse = new()
            {
                Text = comment.Text,
                Status = comment.Status,
                Date = comment.Date
            };
            return commentResponse;

        }

        public bool AddPicture(int adId, string url)
        {
            Ad ad = _adRepo.FindById(adId);
            ad.Images.Add(new Image()
            {
                Url = url,
            });
            return _adRepo.Update();
        }

        public List<AdResponseDTO> DisplayAll()
        {
            List<AdResponseDTO> responses = new List<AdResponseDTO>();

            List<Ad> ads = _adRepo.FindAll();

            foreach(Ad ad in ads)
            {
                AdResponseDTO response = new AdResponseDTO
                {
                    Title = ad.Title,
                    Description = ad.Description,
                    Date = ad.Date,
                    Status = ad.Status,
                    Category = ad.Category.Name
                };
                responses.Add(response);
            }
            return responses;
        }




        public List<AdResponseDTO> SearchByKeyword(string word)
        {
            List<AdResponseDTO> adResponses = new();
            List<Ad> ads = _adRepo.SearchAll(a => a.Title.Contains(word) || a.Description.Contains(word));
            ads.ForEach(a =>
            {
                AdResponseDTO adResponse = new()
                {
                    Title = a.Title,
                    Description = a.Description,
                    Date = a.Date,
                    Status = a.Status,
                    Category = a.Category.Name
                };
                adResponses.Add(adResponse);
            });
            return adResponses;
        }
    }
}