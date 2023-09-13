namespace map_generator.DecorHandling;

/**A data class for storing multiple DecorPositions*/
public record DecorGroup(string Name, int Width, int Height, IList<DecorPosition> Elements);