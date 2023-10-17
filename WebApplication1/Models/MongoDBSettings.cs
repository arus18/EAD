//MongoDB Settings

namespace WebApplication1.Models;

public class MongoDBSettings {

    public string ConnectionURI { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
    
    public string UserCollectionName { get; set; } = null!; 
    
    public string ReservationCollectionName { get; set; } = null!;
    
    public string ScheduleCollectionName { get; set; } = null!;
    
    public string TrainDetailCollectionName { get; set; } = null!;
    
    public string StationCollectionName { get; set; } = null!;

}