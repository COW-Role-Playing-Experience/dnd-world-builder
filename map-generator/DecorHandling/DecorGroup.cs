namespace map_generator.DecorHandling;

/**A data class for storing multiple DecorPositions*/
public record DecorGroup(String Name, int Width, int Height, List<DecorPosition> Elements);