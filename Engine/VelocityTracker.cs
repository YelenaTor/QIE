namespace Quartermaster.Engine;

/// <summary>
/// Sale velocity from price history — sliding window calculation.
/// </summary>
public class VelocityTracker
{
    /// <summary>
    /// Calculate average sales per day over the given window.
    /// </summary>
    public Task<float> GetVelocityAsync(uint itemId, uint worldId, int windowDays = 7)
    {
        throw new NotImplementedException();
    }
}
