namespace RiverLine.Api.Services.NileBerths;

public interface INileBerthService
{
    Task<Result<List<NileBerthDto>>> GetNileBerthsAsync();  
}