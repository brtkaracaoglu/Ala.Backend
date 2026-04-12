namespace Ala.Backend.Application.Common.Responses
{
    public interface ISuccessDetails
    {
        int StatusCode { get; }
        string Detail { get; }
        object? DataObject { get; }
    }
}