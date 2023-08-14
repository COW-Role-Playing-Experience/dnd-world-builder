public class RoomTheme
{
    private int id;
    private int minWidth;
    private int maxWidth;
    private int minHeight;
    private int maxHeight;
    private int[] connectorIds;
    private int maxConnectors;
    private int minConnectors;

    public RoomTheme(int id, int minWidth, int maxWidth, int minHeight, int maxHeight, int minConnectors, int maxConnectors, int[] connectorIds)
    {
        this.id = id;
        this.minWidth = minWidth;
        this.maxWidth = maxWidth;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.minConnectors = minConnectors;
        this.maxConnectors = maxConnectors;
        this.connectorIds = connectorIds;
    }

    public int getId()
    {
        return this.id;
    }

    public int getMinWidth()
    {
        return this.minWidth;
    }

    public int getMaxWidth()
    {
        return this.maxWidth;
    }

    public int getMinHeight()
    {
        return this.minHeight;
    }

    public int getMaxHeight()
    {
        return this.maxHeight;
    }

    public int getMinConnector()
    {
        return this.minConnectors;
    }

    public int getMaxConnector()
    {
        return this.maxConnectors;
    }

    public int[] getConnectorIds()
    {
        return this.connectorIds;
    }
}
