#nullable enable
using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CVAPP.UI.Web.MVC.Models;

public class CreateInstituteViewModel
{
    
    [StringLength(100)]
    public string? Name { get; set; }

    
    public InstituteType? Type { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }

    [Url]
    public string? Website { get; set; }

    [Phone] public string? TelephoneNumber { get; set; }

    public string? Fax { get; set; }

    [StringLength(30)]
    public string? BtwNumber { get; set; } 

    [StringLength(500)]
    public string? Comments { get; set; } 
    public string? ReturnUrl { get; set; } 
    public bool IsEmbedded { get; set; }

    
    public List<SelectListItem> InstituteTypes { get; set; } = new();
}