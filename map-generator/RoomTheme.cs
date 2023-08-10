public class RoomTheme
{
  private int id;
  private int minSize;
  private int maxSize;
  private int[] connectorIds;
  private int maxConnectors;
  private int minConnectors;

  public RoomTheme(int id, int minSize, int maxSize, int minConnectors, int maxConnectors, int[] connectorIds){
    this.id = id;
    this.minSize = minSize;
    this.maxSize = maxSize;
    this.minConnectors = minConnectors;
    this.maxConnectors = maxConnectors;
    this.connectorIds = connectorIds;
  }

  public int getId(){
    return this.id;
  }

  public int getMinSize(){
    return this.minSize;
  }

  public int getMaxSize(){
    return this.maxSize;
  }

  public int getMinConnector(){
    return this.minConnectors;
  }
  
  public int getMaxConnector(){
    return this.maxConnectors;
  }
  
  public int[] getConnectorIds(){
    return this.connectorIds;
  }
}
