using System.ComponentModel.DataAnnotations;
using AssetManager.Library.Classes.Asset;
using AssetManager.Library.Enums;

namespace AssetManager.Server.Pages.ProdViews;

public class AssetFormModel
{
    public int AssetId { get; set; }

    public AssetType Type { get; set; }

    [Required] public string? Name { get; set; }

    [Required] public string? Summary { get; set; }

    [Required] public string? Author { get; set; }

    [Required] [Url] public string? Url { get; set; }

    public string? ThumbnailUrl { get; set; }
    public string? Doi { get; set; }
    public string? Publisher { get; set; }
    public DateTime PublishDate { get; set; }
    public string? Isbn { get; set; }
    public DateTime DateRecorded { get; set; }
    public DateTime Created { get; set; }

    public bool IsActive { get; set; }
    public bool IsOfficial { get; set; }
    public bool IsInternal { get; set; }

    public General GetAsAsset() => new General(AssetId, Name!, Summary!, Author!, Url!, string.IsNullOrEmpty(ThumbnailUrl) ?  "/images/asset_icon.png" : ThumbnailUrl, Doi ?? "", Created, IsActive, IsOfficial, IsInternal);
    public Book GetAsBook() => new Book(AssetId, Name!, Summary!, Author!, Url!, string.IsNullOrEmpty(ThumbnailUrl) ? "/images/asset_icon_book.png" : ThumbnailUrl, Doi ?? "", Created, IsActive, IsOfficial, IsInternal, Isbn!, Publisher!, PublishDate);
    public Cohort GetAsCohort() => new Cohort(AssetId, Name!, Summary!, Author!, Url!, string.IsNullOrEmpty(ThumbnailUrl) ?  "/images/asset_icon_cohort.png" : ThumbnailUrl, Doi ?? "", Created, IsActive, IsOfficial, IsInternal, DateRecorded);
}