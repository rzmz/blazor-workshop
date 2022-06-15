namespace BlazingPizza;

public class OrderState
{
  public bool ShowConfiguringDialog { get; private set; }
  public Pizza ConfiguringPizza { get; private set; }
  public Order Order { get; private set; } = new Order();
  
  public void ShowConfigurePizzaDialog(PizzaSpecial special)
  {
    ConfiguringPizza = new Pizza
    {
      Special = special,
      SpecialId = special.Id,
      Size = Pizza.DefaultSize,
      Toppings = new List<PizzaTopping>()
    };

    ShowConfiguringDialog = true;
  }

  public void CancelConfigurePizzaDialog()
  {
    ConfiguringPizza = null;
    ShowConfiguringDialog = false;
  }

  public void ConfirmConfigurePizzaDialog()
  {
    Order.Pizzas.Add(ConfiguringPizza);
    ConfiguringPizza = null;
    ShowConfiguringDialog = false;
  }

  public void ResetOrder()
  {
    Order = new Order();
  }

  public void RemoveConfiguredPizza(Pizza pizza)
  {
    Order.Pizzas.Remove(pizza);
  }
}