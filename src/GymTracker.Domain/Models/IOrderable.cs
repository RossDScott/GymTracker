namespace GymTracker.Domain.Models;
public interface IOrderable
{
    public int Order { get; set; }
}

public enum OrderDirection { Up, Down }