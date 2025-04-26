namespace CoreNotify.API.LemonSqueezy;

public class Request
{
	public Data data { get; set; }
	public Meta meta { get; set; }
}

public class Data
{
	public string id { get; set; }
	public string type { get; set; }
	public Links links { get; set; }
	public Attributes attributes { get; set; }
	public Relationships relationships { get; set; }
}

public class Links
{
	public string self { get; set; }
	public string related { get; set; }
}

public class Attributes
{
	public int tax { get; set; }
	public Urls urls { get; set; }
	public int total { get; set; }
	public string status { get; set; }
	public int tax_usd { get; set; }
	public string currency { get; set; }
	public bool refunded { get; set; }
	public int store_id { get; set; }
	public int subtotal { get; set; }
	public string tax_name { get; set; }
	public int tax_rate { get; set; }
	public int setup_fee { get; set; }
	public bool test_mode { get; set; }
	public int total_usd { get; set; }
	public string user_name { get; set; }
	public DateTime created_at { get; set; }
	public string identifier { get; set; }
	public DateTime updated_at { get; set; }
	public string user_email { get; set; }
	public int customer_id { get; set; }
	public object refunded_at { get; set; }
	public int order_number { get; set; }
	public int subtotal_usd { get; set; }
	public string currency_rate { get; set; }
	public int setup_fee_usd { get; set; }
	public string tax_formatted { get; set; }
	public bool tax_inclusive { get; set; }
	public int discount_total { get; set; }
	public int refunded_amount { get; set; }
	public string total_formatted { get; set; }
	public First_Order_Item first_order_item { get; set; }
	public string status_formatted { get; set; }
	public int discount_total_usd { get; set; }
	public string subtotal_formatted { get; set; }
	public int refunded_amount_usd { get; set; }
	public string setup_fee_formatted { get; set; }
	public string discount_total_formatted { get; set; }
	public string refunded_amount_formatted { get; set; }
}

public class Urls
{
	public string receipt { get; set; }
}

public class First_Order_Item
{
	public int id { get; set; }
	public int price { get; set; }
	public int order_id { get; set; }
	public int price_id { get; set; }
	public int quantity { get; set; }
	public bool test_mode { get; set; }
	public DateTime created_at { get; set; }
	public int product_id { get; set; }
	public DateTime updated_at { get; set; }
	public int variant_id { get; set; }
	public string product_name { get; set; }
	public string variant_name { get; set; }
}

public class Relationships
{
	public Store store { get; set; }
	public Customer customer { get; set; }
	public OrderItems orderitems { get; set; }
	public LicenseKeys licensekeys { get; set; }
	public Subscriptions subscriptions { get; set; }
	public DiscountRedemptions discountredemptions { get; set; }
}

public class Store
{
	public Links links { get; set; }
}

public class Customer
{
	public Links links { get; set; }
}

public class OrderItems
{
	public Links links { get; set; }
}

public class LicenseKeys
{
	public Links links { get; set; }
}

public class Subscriptions
{
	public Links links { get; set; }
}

public class DiscountRedemptions
{
	public Links links { get; set; }
}

public class Meta
{
	public bool test_mode { get; set; }
	public string event_name { get; set; }
	public string webhook_id { get; set; }
}
