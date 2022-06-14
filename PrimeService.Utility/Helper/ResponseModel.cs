namespace PrimeService.Utility.Helper;

// public class ResponseModel<T>
// {
//     public string Message { get; set; }
//     public Data<T> Data { get; set; }
// }

public class ResponseData<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
}

