using StargazingApp.Tests.Stubs;

namespace StargazingApp.Tests;

/// <summary>
/// Tests for SevenTimerService.GetVisibilityRating().
/// Covers all rating bands and the atmosphere-poor downgrade logic.
/// </summary>
public class SevenTimerServiceTests
{
    private readonly SevenTimerService _service = new();

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Builds a SevenTimerEntry with default "ideal" values.
    /// Override only the fields relevant to each test.
    /// </summary>
    private static SevenTimerEntry Good(int cloud = 1, int seeing = 1, int transparency = 1)
        => new() { CloudCover = cloud, Seeing = seeing, Transparency = transparency };

    // =========================================================================
    // 🟢 Excellent
    // =========================================================================

    [Fact]
    public void GetVisibilityRating_PerfectConditions_ReturnsExcellent()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 1, seeing: 1, transparency: 1));
        Assert.StartsWith("🟢 Excellent", result);
    }

    [Fact]
    public void GetVisibilityRating_CloudCover2Seeing3Transparency3_ReturnsExcellent()
    {
        // Boundary: cloud ≤ 2, seeing ≤ 3, transparency ≤ 3 => Excellent
        var result = _service.GetVisibilityRating(Good(cloud: 2, seeing: 3, transparency: 3));
        Assert.StartsWith("🟢 Excellent", result);
    }

    // =========================================================================
    // 🟡 Fair
    // =========================================================================

    [Fact]
    public void GetVisibilityRating_CloudCover3GoodAtmosphere_ReturnsFair()
    {
        // cloud == 3, atmosphere not poor => Fair
        var result = _service.GetVisibilityRating(Good(cloud: 3, seeing: 2, transparency: 2));
        Assert.StartsWith("🟡 Fair", result);
    }

    [Fact]
    public void GetVisibilityRating_CloudCover4GoodAtmosphere_ReturnsFair()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 4, seeing: 2, transparency: 2));
        Assert.StartsWith("🟡 Fair", result);
    }

    [Fact]
    public void GetVisibilityRating_PoorSeeingLowCloud_ReturnsFair()
    {
        // Seeing >= 5 makes atmosphere poor; cloud < 3 => Fair (not Poor)
        var result = _service.GetVisibilityRating(Good(cloud: 2, seeing: 5, transparency: 2));
        Assert.StartsWith("🟡 Fair", result);
    }

    [Fact]
    public void GetVisibilityRating_PoorTransparencyLowCloud_ReturnsFair()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 1, seeing: 2, transparency: 5));
        Assert.StartsWith("🟡 Fair", result);
    }

    // =========================================================================
    // 🟠 Poor
    // =========================================================================

    [Fact]
    public void GetVisibilityRating_CloudCover5_ReturnsPoor()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 5, seeing: 1, transparency: 1));
        Assert.StartsWith("🟠 Poor", result);
    }

    [Fact]
    public void GetVisibilityRating_CloudCover6_ReturnsPoor()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 6, seeing: 1, transparency: 1));
        Assert.StartsWith("🟠 Poor", result);
    }

    [Fact]
    public void GetVisibilityRating_CloudCover3WithPoorSeeing_ReturnsPoor()
    {
        // cloud >= 3 AND atmosphere poor => Poor
        var result = _service.GetVisibilityRating(Good(cloud: 3, seeing: 5, transparency: 2));
        Assert.StartsWith("🟠 Poor", result);
    }

    [Fact]
    public void GetVisibilityRating_CloudCover3WithPoorTransparency_ReturnsPoor()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 3, seeing: 2, transparency: 5));
        Assert.StartsWith("🟠 Poor", result);
    }

    // =========================================================================
    // 🔴 Too cloudy
    // =========================================================================

    [Fact]
    public void GetVisibilityRating_CloudCover7_ReturnsTooMuchCloud()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 7, seeing: 1, transparency: 1));
        Assert.StartsWith("🔴 Too cloudy", result);
    }

    [Fact]
    public void GetVisibilityRating_CloudCover9_ReturnsTooMuchCloud()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 9, seeing: 8, transparency: 8));
        Assert.StartsWith("🔴 Too cloudy", result);
    }

    [Fact]
    public void GetVisibilityRating_MaxCloudAndAtmosphere_ReturnsTooMuchCloud()
    {
        // Cloud cover takes priority over everything else
        var result = _service.GetVisibilityRating(Good(cloud: 8, seeing: 8, transparency: 8));
        Assert.StartsWith("🔴 Too cloudy", result);
    }

    // =========================================================================
    // Output format — rating string contains detail lines
    // =========================================================================

    [Fact]
    public void GetVisibilityRating_AlwaysContainsCloudCoverLine()
    {
        var result = _service.GetVisibilityRating(Good());
        Assert.Contains("☁️ Cloud cover:", result);
    }

    [Fact]
    public void GetVisibilityRating_AlwaysContainsSeeingLine()
    {
        var result = _service.GetVisibilityRating(Good());
        Assert.Contains("👁️ Seeing:", result);
    }

    [Fact]
    public void GetVisibilityRating_AlwaysContainsTransparencyLine()
    {
        var result = _service.GetVisibilityRating(Good());
        Assert.Contains("🔭 Transparency:", result);
    }

    [Fact]
    public void GetVisibilityRating_CloudCover1_ShowsCorrectPercentage()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 1));
        Assert.Contains("0–6%", result);
    }

    [Fact]
    public void GetVisibilityRating_Seeing1_ShowsExcellent()
    {
        var result = _service.GetVisibilityRating(Good(seeing: 1));
        Assert.Contains("Seeing: Excellent", result);
    }

    [Fact]
    public void GetVisibilityRating_Transparency8_ShowsTerrible()
    {
        var result = _service.GetVisibilityRating(Good(cloud: 7, seeing: 1, transparency: 8));
        Assert.Contains("Transparency: Terrible", result);
    }
}
