using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Models;
public interface IOrderable
{
    public int Order { get; set; }
}

public enum OrderDirection { Up, Down }