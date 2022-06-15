using BlazingPizza.ComponentsLibrary.Map;

namespace BlazingPizza;

public class OrderWithStatus
{
  public static readonly TimeSpan PreparationDuration = TimeSpan.FromSeconds(10);

  public static readonly TimeSpan
    DeliveryDuration = TimeSpan.FromMinutes(1); // Unrealistic, but more interesting to watch

  public Order Order { get; set; }

  public string StatusText { get; set; }

  public bool IsDelivered => StatusText == "Delivered";

  public List<Marker> MapMarkers { get; set; }

  public static OrderWithStatus FromOrder(Order order)
  {
    // To simulate a real backend process, we fake status updates based on the amount
    // of time since the order was placed
    string statusText;
    List<Marker> mapMarkers;
    DateTime dispatchTime = order.CreatedTime.Add(PreparationDuration);

    if (DateTime.Now < dispatchTime)
    {
      statusText = "Preparing";
      mapMarkers = new List<Marker> { ToMapMarker("You", order.DeliveryLocation, true) };
    }
    else if (DateTime.Now < dispatchTime + DeliveryDuration)
    {
      statusText = "Out for delivery";

      LatLong startPosition = ComputeStartPosition(order);
      double proportionOfDeliveryCompleted = Math.Min(1,
        (DateTime.Now - dispatchTime).TotalMilliseconds / DeliveryDuration.TotalMilliseconds);
      LatLong driverPosition =
        LatLong.Interpolate(startPosition, order.DeliveryLocation, proportionOfDeliveryCompleted);
      mapMarkers = new List<Marker>
      {
        ToMapMarker("You", order.DeliveryLocation), ToMapMarker("Driver", driverPosition, true)
      };
    }
    else
    {
      statusText = "Delivered";
      mapMarkers = new List<Marker> { ToMapMarker("Delivery location", order.DeliveryLocation, true) };
    }

    return new OrderWithStatus { Order = order, StatusText = statusText, MapMarkers = mapMarkers };
  }

  private static LatLong ComputeStartPosition(Order order)
  {
    // Random but deterministic based on order ID
    var rng = new Random(order.OrderId);
    double distance = 0.01 + (rng.NextDouble() * 0.02);
    double angle = rng.NextDouble() * Math.PI * 2;
    (double, double) offset = (distance * Math.Cos(angle), distance * Math.Sin(angle));
    return new LatLong(order.DeliveryLocation.Latitude + offset.Item1, order.DeliveryLocation.Longitude + offset.Item2);
  }

  private static Marker ToMapMarker(string description, LatLong coords, bool showPopup = false)
    => new() { Description = description, X = coords.Longitude, Y = coords.Latitude, ShowPopup = showPopup };
}