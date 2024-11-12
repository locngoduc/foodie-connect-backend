namespace foodie_connect_backend.Shared.Dtos;

public class ScoreResponseDto
{
    public double AverageRating { get; set; } = 0;
    public int FiveStars { get; set; } = 0;
    public int FourStars { get; set; } = 0;
    public int ThreeStars { get; set; } = 0;
    public int TwoStars { get; set; } = 0;
    public int OneStar { get; set; } = 0;
}