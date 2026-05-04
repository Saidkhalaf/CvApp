namespace cvapp.ui.web.mvc.Extensions;

public static class CvSections
{
    public static readonly (string Route, string Label)[] Sections = new[]
    {
        ("persoon-gegevens", "Persoonlijke informatie"),
        ("overzicht-skills", "Overzicht skills"),
        ("extra-expertises", "Extra expertises"),
        ("extra-technologieen", "Extra technologieën"),
        ("carriers", "Carrière"),
        ("certificaten", "Certificaten"),
        ("cursussen-trainingen", "Cursussen/Trainingen"),
        ("opleidingen", "Opleidingen"),
        ("projecten", "Projecten"),
        ("talen", "Talen")
    }; 
}