namespace map_generator.DecorHandling;

public record MetaTile(int XPos, int YPos, DecorGroup DecorGroup)
{
    public int GetSize()
    {
        return DecorGroup.Width * DecorGroup.Height;
    }
}
