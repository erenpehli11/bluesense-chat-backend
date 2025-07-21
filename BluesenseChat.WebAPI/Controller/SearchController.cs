using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("messages")]
    public async Task<IActionResult> SearchMessages([FromQuery] SearchQueryDto query)
    {
        var result = await _searchService.SearchMessagesAsync(query);
        return Ok(result);
    }
}
