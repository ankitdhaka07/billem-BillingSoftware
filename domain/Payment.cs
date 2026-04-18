using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain;

public class Payment
{
    public Guid Id { get; set; }
    public double AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public Guid? CustomerId { get; set; }
}
