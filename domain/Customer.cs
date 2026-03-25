using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain;

public class Customer
{
    public Guid Id { get; set; }
    [Required] public string Name { get; set; }
    public string? GstNo { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }

}