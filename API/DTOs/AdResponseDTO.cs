﻿using TPFinalStrasbourg.Models;

namespace TPFinalStrasbourg.DTOs
{

    public class AdResponseDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }

}