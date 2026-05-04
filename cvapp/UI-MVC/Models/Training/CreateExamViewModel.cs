#nullable enable
using cvapp.BL.Domain.enums;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace CVAPP.UI.Web.MVC.Models.Training;

public class CreateExamViewModel
{
    public string? Name { get; set; } 
    public ExamResult? Result { get; set; }

    public string? ReturnUrl { get; set; } 
    public bool IsEmbedded { get; set; }
    public long? SelectedInstituteId { get; set; }

    public List<SelectListItem> ExamResults { get; set; } = [];
}


