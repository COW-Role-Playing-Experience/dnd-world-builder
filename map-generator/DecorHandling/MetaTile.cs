namespace map_generator.DecorHandling;

public record MetaTile(int XPos, int YPos, DecorGroup Dg)
{
    private DecorGroup _decorGroup = Dg with { Elements = Dg.Elements.Select(d => d with { Decor = new Decor(d.Decor.ImageName) }).ToList() };

    public DecorGroup DecorGroup
    {
        get => _decorGroup;
    }

    public int GetSize()
    {
        return DecorGroup.Width * DecorGroup.Height;
    }
}
