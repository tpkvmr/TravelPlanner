using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPlanner.Core.Models.Requests;

namespace TravelPlanner.Application.DTOs
{
    public class TripBudgetDto
    {
        public decimal TotalBudget { get; set; }
        public List<DailyBudgetDto> DailyBudgets { get; set; }
    }

}
